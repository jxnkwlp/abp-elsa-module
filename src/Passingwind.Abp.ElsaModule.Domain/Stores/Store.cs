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
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Linq;
using Volo.Abp.Uow;

namespace Passingwind.Abp.ElsaModule.Stores
{
    [UnitOfWork]
    public abstract class Store<TModel, TEntity, TKey> where TModel : Elsa.Models.IEntity where TEntity : class, IEntity<TKey>
    {
        private readonly SemaphoreSlim _semaphore = new(1);

        protected ILogger Logger { get; set; }
        //protected IAbpLazyServiceProvider LazyServiceProvider { get; }
        protected IRepository<TEntity, TKey> Repository { get; }
        protected IAsyncQueryableExecuter AsyncQueryableExecuter { get; }

        protected Store(ILoggerFactory loggerFactory, IRepository<TEntity, TKey> repository, IAsyncQueryableExecuter asyncQueryableExecuter)
        {
            Logger = loggerFactory.CreateLogger(GetType());
            Repository = repository;
            AsyncQueryableExecuter = asyncQueryableExecuter;
        }

        public virtual async Task AddAsync(TModel model, CancellationToken cancellationToken = default)
        {
            Logger.LogDebug($"Add [{model.GetType()}] {model.Id} ... ");

            var entity = await MapToEntityAsync(model);
            await Repository.InsertAsync(entity, true, cancellationToken);
        }

        public virtual async Task AddManyAsync(IEnumerable<TModel> models, CancellationToken cancellationToken = default)
        {
            Logger.LogDebug($"AddMany [{models.GetType()}] ... ");

            var entities = new List<TEntity>();

            foreach (var item in models)
            {
                entities.Add(await MapToEntityAsync(item));
            }

            await Repository.InsertManyAsync(entities, true, cancellationToken);
        }

        public virtual async Task<int> CountAsync(ISpecification<TModel> specification, CancellationToken cancellationToken = default)
        {
            var expression = await MapSpecificationAsync(specification);
            return await Repository.CountAsync(expression, cancellationToken);
        }

        public virtual async Task DeleteAsync(TModel model, CancellationToken cancellationToken = default)
        {
            Logger.LogDebug($"Delete [{model.GetType()}] {model.Id} ... ");

            var id = (TKey)Convert.ChangeType(model.Id, typeof(TKey));
            await Repository.DeleteAsync(x => x.Id.Equals(id), true, cancellationToken);
        }

        public virtual async Task<int> DeleteManyAsync(ISpecification<TModel> specification, CancellationToken cancellationToken = default)
        {
            var expression = await MapSpecificationAsync(specification);
            var count = await Repository.CountAsync(expression, cancellationToken);
            await Repository.DeleteAsync(expression, true, cancellationToken);

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

        [UnitOfWork]
        public virtual async Task<IEnumerable<TModel>> FindManyAsync(ISpecification<TModel> specification, IOrderBy<TModel> orderBy = null, IPaging paging = null, CancellationToken cancellationToken = default)
        {
            var filter = await MapSpecificationAsync(specification).ConfigureAwait(false);

            var query = await Repository.WithDetailsAsync();

            query = query.Where(filter);

            // TODO orderBy

            if (paging != null)
                query = query.OrderByDescending(x => x.Id).Skip(paging.Skip).Take(paging.Take);

            var list = await AsyncQueryableExecuter.ToListAsync(query, cancellationToken).ConfigureAwait(false);

            var result = new List<TModel>();

            foreach (var item in list)
            {
                result.Add(await MapToModelAsync(item));
            }

            return result;
        }

        public virtual async Task SaveAsync(TModel model, CancellationToken cancellationToken = default)
        {
            await _semaphore.WaitAsync(cancellationToken);

            Logger.LogDebug($"Save [{model.GetType()}] {model.Id} ... ");

            try
            {
                TKey id = ConvertKey(model.Id);

                var entity = await Repository.FindAsync(id);

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
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, $"Saving '{typeof(TModel)}' data failed.");
                throw;
            }
            finally
            {
                _semaphore.Release();
            }
        }

        public virtual async Task UpdateAsync(TModel model, CancellationToken cancellationToken = default)
        {
            Logger.LogDebug($"Update [{model.GetType()}] {model.Id} ... ");

            TKey id = ConvertKey(model.Id);

            var entity = await Repository.GetAsync(x => x.Id.Equals(id));

            entity = await MapToEntityAsync(model, entity);

            await Repository.UpdateAsync(entity);
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

}
