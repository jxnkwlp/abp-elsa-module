using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Elsa.Persistence;
using Elsa.Persistence.Specifications;
using Elsa.Persistence.Specifications.WorkflowExecutionLogRecords;
using Microsoft.Extensions.Logging;
using Passingwind.Abp.ElsaModule.Common;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Linq;
using WorkflowExecutionLogRecordModel = Elsa.Models.WorkflowExecutionLogRecord;

namespace Passingwind.Abp.ElsaModule.Stores
{
    public class WorkflowExecutionLogStore : Store<WorkflowExecutionLogRecordModel, WorkflowExecutionLog, long>, IWorkflowExecutionLogStore
    {
        private readonly IStoreMapper _storeMapper;

        public WorkflowExecutionLogStore(ILoggerFactory loggerFactory, IRepository<WorkflowExecutionLog, long> repository, IAsyncQueryableExecuter asyncQueryableExecuter, IStoreMapper storeMapper) : base(loggerFactory, repository, asyncQueryableExecuter)
        {
            _storeMapper = storeMapper;
        }

        protected override Task<WorkflowExecutionLog> MapToEntityAsync(WorkflowExecutionLogRecordModel model)
        {
            return Task.FromResult(_storeMapper.MapToEntity(model));
        }

        protected override Task<WorkflowExecutionLog> MapToEntityAsync(WorkflowExecutionLogRecordModel model, WorkflowExecutionLog entity)
        {
            return Task.FromResult(_storeMapper.MapToEntity(model));
        }

        protected override Task<WorkflowExecutionLogRecordModel> MapToModelAsync(WorkflowExecutionLog entity)
        {
            return Task.FromResult(_storeMapper.MapToModel(entity));
        }

        protected override Expression<Func<WorkflowExecutionLog, bool>> MapSpecification(ISpecification<WorkflowExecutionLogRecordModel> specification)
        {
            if (specification is ActivityIdSpecification activityIdSpecification)
            {
                return x => x.ActivityId == long.Parse(activityIdSpecification.ActivityId);
            }
            else if (specification is ActivityTypeSpecification activityTypeSpecification)
            {
                return x => x.ActivityType == activityTypeSpecification.ActivityType;
            }
            else if (specification is WorkflowInstanceIdSpecification workflowInstanceIdSpecification)
            {
                return x => x.WorkflowInstanceId == Guid.Parse(workflowInstanceIdSpecification.WorkflowInstanceId);
            }
            else
                throw new NotSupportedException(specification.GetType().Name);
        }

        protected override long ConvertKey(string value)
        {
            return long.Parse(value);
        }
    }
}
