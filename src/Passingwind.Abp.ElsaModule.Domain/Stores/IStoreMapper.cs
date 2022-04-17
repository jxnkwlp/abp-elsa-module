using Elsa.Models;
using Passingwind.Abp.ElsaModule.Common;
using Volo.Abp.DependencyInjection;

namespace Passingwind.Abp.ElsaModule.Stores
{
    public interface IStoreMapper : ITransientDependency
    {
        Activity MapToEntity(ActivityDefinition model);

        Common.Bookmark MapToEntity(Elsa.Models.Bookmark model);

        Common.Bookmark MapToEntity(Elsa.Models.Bookmark model, Common.Bookmark entity);

        ActivityConnection MapToEntity(ConnectionDefinition model);

        Common.Trigger MapToEntity(Elsa.Models.Trigger model);

        Common.Trigger MapToEntity(Elsa.Models.Trigger model, Common.Trigger entity);

        WorkflowExecutionLog MapToEntity(WorkflowExecutionLogRecord model);

        WorkflowExecutionLog MapToEntity(WorkflowExecutionLogRecord model, WorkflowExecutionLog entity);

        Common.WorkflowInstance MapToEntity(Elsa.Models.WorkflowInstance model);

        Common.WorkflowInstance MapToEntity(Elsa.Models.WorkflowInstance model, Common.WorkflowInstance entity);

        WorkflowDefinitionVersion MapToEntity(Elsa.Models.WorkflowDefinition model);

        WorkflowDefinitionVersion MapToEntity(Elsa.Models.WorkflowDefinition model, WorkflowDefinitionVersion entity);

        ActivityDefinition MapToModel(Activity entity);

        ConnectionDefinition MapToModel(ActivityConnection entity);

        Elsa.Models.Bookmark MapToModel(Common.Bookmark entity);

        Elsa.Models.Trigger MapToModel(Common.Trigger entity);

        Elsa.Models.WorkflowDefinition MapToModel(WorkflowDefinitionVersion entity);

        WorkflowExecutionLogRecord MapToModel(WorkflowExecutionLog entity);

        Elsa.Models.WorkflowInstance MapToModel(Common.WorkflowInstance entity);
    }
}
