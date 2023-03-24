using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Elsa.Persistence.Specifications;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Guids;
using Volo.Abp.Linq;
using Volo.Abp.MultiTenancy;
using Volo.Abp.Timing;
using Volo.Abp.Uow;

namespace Passingwind.Abp.ElsaModule.Stores;

[UnitOfWork]
public abstract class Store<TModel, TEntity, TKey> : ITransientDependency where TModel : Elsa.Models.IEntity where TEntity : class, IEntity<TKey>
{
    // private static readonly SemaphoreSlim _semaphore = new(1);

    protected Guid StoreId { get; }

    public IAbpLazyServiceProvider LazyServiceProvider { get; set; }

    protected IClock Clock => LazyServiceProvider.LazyGetRequiredService<IClock>();

    protected IGuidGenerator GuidGenerator => LazyServiceProvider.LazyGetService<IGuidGenerator>(SimpleGuidGenerator.Instance);

    protected ILoggerFactory LoggerFactory => LazyServiceProvider.LazyGetRequiredService<ILoggerFactory>();

    protected ICurrentTenant CurrentTenant => LazyServiceProvider.LazyGetRequiredService<ICurrentTenant>();

    protected IAsyncQueryableExecuter AsyncExecuter => LazyServiceProvider.LazyGetRequiredService<IAsyncQueryableExecuter>();

    protected ILogger Logger => LazyServiceProvider.LazyGetService<ILogger>(provider => LoggerFactory?.CreateLogger(GetType()) ?? NullLogger.Instance);

    protected IUnitOfWorkManager UnitOfWork => LazyServiceProvider.LazyGetRequiredService<IUnitOfWorkManager>();

    protected IRepository<TEntity, TKey> Repository => LazyServiceProvider.LazyGetRequiredService<IRepository<TEntity, TKey>>();

    protected IStoreMapper StoreMapper => LazyServiceProvider.LazyGetRequiredService<IStoreMapper>();


    protected Store()
    {
        StoreId = Guid.NewGuid();
    }

    public virtual async Task AddAsync(TModel model, CancellationToken cancellationToken = default)
    {
        Logger.LogDebug($"Invoke {typeof(TModel).Name} [AddAsync] with id '{model.Id}' ");

        using var uow = UnitOfWork.Begin(requiresNew: true);

        var entity = await MapToEntityAsync(model);
        await Repository.InsertAsync(entity, true, cancellationToken);

        await uow.SaveChangesAsync();
    }

    public virtual async Task AddManyAsync(IEnumerable<TModel> models, CancellationToken cancellationToken = default)
    {
        Logger.LogDebug($"Invoke {typeof(TModel).Name} [AddManyAsync] ");

        if (models.Any() == false)
            return;

        using var uow = UnitOfWork.Begin(requiresNew: true);

        var entities = new List<TEntity>();

        foreach (var item in models)
        {
            entities.Add(await MapToEntityAsync(item));
        }

        await Repository.InsertManyAsync(entities, true, cancellationToken);

        await uow.SaveChangesAsync();
    }

    public virtual async Task<int> CountAsync(ISpecification<TModel> specification, CancellationToken cancellationToken = default)
    {
        var expression = await MapSpecificationAsync(specification);
        return await Repository.CountAsync(expression, cancellationToken);
    }

    public virtual async Task DeleteAsync(TModel model, CancellationToken cancellationToken = default)
    {
        Logger.LogDebug($"Invoke {typeof(TModel).Name} [DeleteAsync] with id '{model.Id}'");

        using var uow = UnitOfWork.Begin(requiresNew: true);

        var id = (TKey)Convert.ChangeType(model.Id, typeof(TKey));
        await Repository.DeleteAsync(x => x.Id.Equals(id), true, cancellationToken);

        await uow.SaveChangesAsync();
    }

    public virtual async Task<int> DeleteManyAsync(ISpecification<TModel> specification, CancellationToken cancellationToken = default)
    {
        Logger.LogDebug($"Invoke {typeof(TModel).Name} [DeleteManyAsync] ");

        using var uow = UnitOfWork.Begin(requiresNew: true);

        var expression = await MapSpecificationAsync(specification);
        var count = await Repository.CountAsync(expression, cancellationToken);
        await Repository.DeleteAsync(expression, true, cancellationToken);

        await uow.SaveChangesAsync();

        return count;
    }

    public virtual async Task<TModel> FindAsync(ISpecification<TModel> specification, CancellationToken cancellationToken = default)
    {
        var expression = await MapSpecificationAsync(specification);
        var entity = await Repository.FindAsync(expression, true, cancellationToken);

        if (entity == null)
        {
            Logger.LogWarning("The {0} entity was not found.", typeof(TModel));
            return default;
        }

        return await MapToModelAsync(entity);
    }

    public virtual async Task<IEnumerable<TModel>> FindManyAsync(ISpecification<TModel> specification, IOrderBy<TModel> orderBy = null, IPaging paging = null, CancellationToken cancellationToken = default)
    {
        var filter = await MapSpecificationAsync(specification).ConfigureAwait(false);

        var query = await Repository.WithDetailsAsync();

        query = query.Where(filter);

        // TODO orderBy  
        //if (orderBy != null)
        //{
        //var orderByExp = orderBy.OrderByExpression.ConvertType<TModel, TEntity>();
        //query = (orderBy.SortDirection == SortDirection.Ascending) ? query.OrderBy(orderByExp) : query.OrderByDescending(orderByExp);
        //}
        //else
        //{
        query = query.OrderByDescending(x => x.Id);
        //}

        if (paging != null)
            query = query.Skip(paging.Skip).Take(paging.Take);

        var list = await AsyncExecuter.ToListAsync(query).ConfigureAwait(false);

        var result = new List<TModel>();

        foreach (var item in list)
        {
            result.Add(await MapToModelAsync(item));
        }

        return result;
    }

    private static readonly object _saveKey = new object();

    public virtual async Task SaveAsync(TModel model, CancellationToken cancellationToken = default)
    {
        if (cancellationToken.IsCancellationRequested)
            return;

        using (var uow = UnitOfWork.Begin(requiresNew: true))
        {
            if (cancellationToken.IsCancellationRequested)
                return;

            Logger.LogDebug($"Invoke {typeof(TModel).Name} [SaveAsync] ");

            try
            {
                Logger.LogDebug($"Saving [{typeof(TModel).Name}] with id '{model.Id}' ... ");

                TKey id = ConvertKey(model.Id);

                var entity = await Repository.FindAsync(id, true, cancellationToken);

                if (entity == default)
                {
                    entity = await MapToEntityAsync(model);
                    await Repository.InsertAsync(entity, true, cancellationToken);
                }
                else
                {
                    entity = await MapToEntityAsync(model, entity);
                    await Repository.UpdateAsync(entity, true, cancellationToken);
                }

                await uow.SaveChangesAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                if (ex is TaskCanceledException ex2)
                {
                    Logger.LogWarning(ex2, $"Task canceled when saving '{typeof(TModel).Name}' with id '{model.Id}'.");
                }
                else
                {
                    Logger.LogError(ex, $"Saving '{typeof(TModel).Name}' with id '{model.Id}' failed.");
                    throw;
                }
            }
        }
    }

    public virtual async Task UpdateAsync(TModel model, CancellationToken cancellationToken = default)
    {
        Logger.LogDebug($"Invoke {typeof(TModel).Name} [UpdateAsync] with id '{model.Id}'");

        using var uow = UnitOfWork.Begin(requiresNew: true);

        TKey id = ConvertKey(model.Id);

        var entity = await Repository.GetAsync(x => x.Id.Equals(id), true, cancellationToken);

        entity = await MapToEntityAsync(model, entity);

        await Repository.UpdateAsync(entity, true, cancellationToken);

        await uow.SaveChangesAsync();
    }

    protected virtual async Task<Expression<Func<TEntity, bool>>> MapSpecificationAsync(ISpecification<TModel> specification)
    {
        // TODO auto mapper

        if (specification is OrSpecification<TModel> orSpecification)
        {
            var leftField = orSpecification.GetType().GetFields(System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).First(x => x.Name == "_left");
            var rightField = orSpecification.GetType().GetFields(System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).First(x => x.Name == "_right");

            var left = leftField.GetValue(specification) as ISpecification<TModel>;
            var right = rightField.GetValue(specification) as ISpecification<TModel>;

            return (await MapSpecificationAsync(left)).Or(await MapSpecificationAsync(right));
        }
        else if (specification is AndSpecification<TModel> andSpecification)
        {
            var left = andSpecification.Left;
            var right = andSpecification.Right;

            return (await MapSpecificationAsync(left)).And(await MapSpecificationAsync(right));
        }
        else if (specification is NotSpecification<TModel> notSpecification)
        {
            Expression<Func<TEntity, bool>> expression = null;

            if (_notSpecificationCache.ContainsKey(notSpecification.GetType()))
            {
                expression = await MapSpecificationAsync(_notSpecificationCache[notSpecification.GetType()]);
            }
            else
            {
                // TODO
                var sp = notSpecification.GetType().GetField("_specification", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static).GetValue(notSpecification) as ISpecification<TModel>;

                _notSpecificationCache[notSpecification.GetType()] = sp;

                expression = await MapSpecificationAsync(sp);
            }

            return Expression.Lambda<Func<TEntity, bool>>(Expression.Not(expression.Body), new ParameterExpression[1]
                         {
                            expression.Parameters.Single()
                         });
        }
        else if (specification.GetType().Name.StartsWith("IdentitySpecification`1"))
        {
            return (_) => true;
        }
        else
            throw new NotSupportedException($"{specification.GetType().FullName} of {typeof(TModel).FullName} is not supported.");
    }

    private static readonly Dictionary<Type, ISpecification<TModel>> _notSpecificationCache = new Dictionary<Type, ISpecification<TModel>>();

    protected abstract TKey ConvertKey(string value);

    protected abstract Task<TEntity> MapToEntityAsync(TModel model);

    protected abstract Task<TEntity> MapToEntityAsync(TModel model, TEntity entity);

    protected abstract Task<TModel> MapToModelAsync(TEntity entity);
}
