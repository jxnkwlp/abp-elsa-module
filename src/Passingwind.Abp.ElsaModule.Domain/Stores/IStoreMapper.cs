using Passingwind.Abp.ElsaModule.Common;
using Volo.Abp.DependencyInjection;
using ActivityDefinition = Elsa.Models.ActivityDefinition;
using BookmarkModel = Elsa.Models.Bookmark;
using ConnectionDefinitionModel = Elsa.Models.ConnectionDefinition;
using TriggerModel = Elsa.Models.Trigger;
using WorkflowDefinitionModel = Elsa.Models.WorkflowDefinition;
using WorkflowExecutionLogRecordModel = Elsa.Models.WorkflowExecutionLogRecord;
using WorkflowInstanceModel = Elsa.Models.WorkflowInstance;

namespace Passingwind.Abp.ElsaModule.Stores;

public interface IStoreMapper : ITransientDependency
{
    Activity MapToEntity(ActivityDefinition model);
    ActivityConnection MapToEntity(ConnectionDefinitionModel model);
    ActivityDefinition MapToModel(Activity entity);
    Bookmark MapToEntity(BookmarkModel model);
    Bookmark MapToEntity(BookmarkModel model, Bookmark entity);
    BookmarkModel MapToModel(Bookmark entity);
    ConnectionDefinitionModel MapToModel(ActivityConnection entity);
    Trigger MapToEntity(TriggerModel model);
    Trigger MapToEntity(TriggerModel model, Trigger entity);
    TriggerModel MapToModel(Trigger entity);
    WorkflowDefinitionModel MapToModel(WorkflowDefinitionVersion entity, WorkflowDefinition definition);
    WorkflowDefinitionVersion MapToEntity(WorkflowDefinitionModel model);
    WorkflowDefinitionVersion MapToEntity(WorkflowDefinitionModel model, WorkflowDefinitionVersion entity);
    WorkflowExecutionLog MapToEntity(WorkflowExecutionLogRecordModel model);
    WorkflowExecutionLog MapToEntity(WorkflowExecutionLogRecordModel model, WorkflowExecutionLog entity);
    WorkflowExecutionLogRecordModel MapToModel(WorkflowExecutionLog entity);
    WorkflowInstance MapToEntity(WorkflowInstanceModel model);
    WorkflowInstance MapToEntity(WorkflowInstanceModel model, WorkflowInstance entity);
    WorkflowInstanceModel MapToModel(WorkflowInstance entity);
}
