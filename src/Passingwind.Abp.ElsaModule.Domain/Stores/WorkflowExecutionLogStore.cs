using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Elsa.Persistence;
using Elsa.Persistence.Specifications;
using Elsa.Persistence.Specifications.WorkflowExecutionLogRecords;
using Passingwind.Abp.ElsaModule.Common;
using WorkflowExecutionLogRecordModel = Elsa.Models.WorkflowExecutionLogRecord;

namespace Passingwind.Abp.ElsaModule.Stores;

public class WorkflowExecutionLogStore : Store<WorkflowExecutionLogRecordModel, WorkflowExecutionLog, Guid>, IWorkflowExecutionLogStore
{
    protected override Task<WorkflowExecutionLog> MapToEntityAsync(WorkflowExecutionLogRecordModel model)
    {
        return Task.FromResult(StoreMapper.MapToEntity(model));
    }

    protected override Task<WorkflowExecutionLog> MapToEntityAsync(WorkflowExecutionLogRecordModel model, WorkflowExecutionLog entity)
    {
        return Task.FromResult(StoreMapper.MapToEntity(model));
    }

    protected override Task<WorkflowExecutionLogRecordModel> MapToModelAsync(WorkflowExecutionLog entity)
    {
        return Task.FromResult(StoreMapper.MapToModel(entity));
    }

    protected override async Task<Expression<Func<WorkflowExecutionLog, bool>>> MapSpecificationAsync(ISpecification<WorkflowExecutionLogRecordModel> specification)
    {
        return specification switch
        {
            ActivityIdSpecification activityIdSpecification => x => x.ActivityId == Guid.Parse(activityIdSpecification.ActivityId),
            ActivityTypeSpecification activityTypeSpecification => x => x.ActivityType == activityTypeSpecification.ActivityType,
            WorkflowInstanceIdSpecification workflowInstanceIdSpecification => x => x.WorkflowInstanceId == Guid.Parse(workflowInstanceIdSpecification.WorkflowInstanceId),
            _ => await base.MapSpecificationAsync(specification)
        };
    }

    protected override Guid ConvertKey(string value)
    {
        return Guid.Parse(value);
    }
}
