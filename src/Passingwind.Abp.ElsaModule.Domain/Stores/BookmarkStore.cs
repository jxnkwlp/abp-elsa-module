using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Elsa.Persistence;
using Elsa.Persistence.Specifications;
using Elsa.Persistence.Specifications.Bookmarks;
using Microsoft.Extensions.Logging;
using Passingwind.Abp.ElsaModule.Common;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Linq;
using BookmarkModel = Elsa.Models.Bookmark;

namespace Passingwind.Abp.ElsaModule.Stores
{
    public class BookmarkStore : Store<BookmarkModel, Bookmark, Guid>, IBookmarkStore
    {
        private readonly IStoreMapper _storeMapper;

        public BookmarkStore(ILoggerFactory loggerFactory, IRepository<Bookmark, Guid> repository, IAsyncQueryableExecuter asyncQueryableExecuter, IStoreMapper storeMapper) : base(loggerFactory, repository, asyncQueryableExecuter)
        {
            _storeMapper = storeMapper;
        }

        protected override Task<Bookmark> MapToEntityAsync(BookmarkModel model)
        {
            return Task.FromResult(_storeMapper.MapToEntity(model));
        }

        protected override Task<Bookmark> MapToEntityAsync(BookmarkModel model, Bookmark entity)
        {
            return Task.FromResult(_storeMapper.MapToEntity(model, entity));
        }

        protected override Task<BookmarkModel> MapToModelAsync(Bookmark entity)
        {
            return Task.FromResult(_storeMapper.MapToModel(entity));
        }

        protected override async Task<Expression<Func<Bookmark, bool>>> MapSpecificationAsync(ISpecification<BookmarkModel> specification)
        {
            switch (specification)
            {
                case BookmarkHashSpecification hashSpecification:
                    {
                        var tenantId = hashSpecification.TenantId.ToGuid();
                        return x => x.TenantId == tenantId && x.ActivityType == hashSpecification.ActivityType && x.Hash == hashSpecification.Hash;
                    }

                case BookmarkIdsSpecification idsSpecification:
                    {
                        var ids = idsSpecification.Ids.ToList().ConvertAll(Guid.Parse);
                        return x => ids.Contains(x.Id);
                    }

                case BookmarkSpecification bookmarkSpecification:
                    {
                        var tenantId = bookmarkSpecification.TenantId.ToGuid();
                        Expression<Func<Bookmark, bool>> expression = (x) => x.ActivityType == bookmarkSpecification.ActivityType && x.TenantId == tenantId;

                        if (!string.IsNullOrEmpty(bookmarkSpecification.CorrelationId))
                            expression = expression.And(x => x.CorrelationId == bookmarkSpecification.CorrelationId);

                        return expression;
                    }

                case BookmarkTypeAndWorkflowInstanceSpecification bookmarkTypeAndWorkflowInstanceSpecification:
                    {
                        return x => x.ModelType == bookmarkTypeAndWorkflowInstanceSpecification.ModelType && x.WorkflowInstanceId == Guid.Parse(bookmarkTypeAndWorkflowInstanceSpecification.WorkflowInstanceId);
                    }

                case BookmarkTypeSpecification bookmarkTypeSpecification:
                    {
                        var tenantId = bookmarkTypeSpecification.TenantId.ToGuid();
                        return x => x.ModelType == bookmarkTypeSpecification.ModelType && x.TenantId == tenantId;
                    }

                case CorrelationIdSpecification correlationIdSpecification:
                    {
                        return x => x.CorrelationId == correlationIdSpecification.CorrelationId;
                    }

                case WorkflowInstanceIdSpecification workflowInstanceIdSpecification:
                    {
                        return x => x.WorkflowInstanceId == Guid.Parse(workflowInstanceIdSpecification.WorkflowInstanceId);
                    }

                case WorkflowInstanceIdsSpecification workflowInstanceIdsSpecification:
                    {
                        var ids = workflowInstanceIdsSpecification.WorkflowInstanceIds.ToList().ConvertAll(Guid.Parse);
                        return x => ids.Contains(x.WorkflowInstanceId);
                    }

                default:
                    return await base.MapSpecificationAsync(specification);
            }
        }

        protected override Guid ConvertKey(string value)
        {
            return Guid.Parse(value);
        }
    }
}
