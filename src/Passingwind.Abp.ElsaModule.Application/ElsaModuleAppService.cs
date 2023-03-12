using System;
using System.Threading.Tasks;
using Passingwind.Abp.ElsaModule.Common;
using Passingwind.Abp.ElsaModule.Localization;
using Volo.Abp.Application.Services;

namespace Passingwind.Abp.ElsaModule;

public abstract class ElsaModuleAppService : ApplicationService
{
    protected ElsaModuleAppService()
    {
        LocalizationResource = typeof(ElsaModuleResource);
        ObjectMapperContext = typeof(ElsaModuleApplicationModule);
    }

    protected virtual async Task CheckWorkflowPermissionAsync(Guid workflowId, string name)
    {
        var repository = LazyServiceProvider.LazyGetRequiredService<IWorkflowDefinitionRepository>();

        var definition = await repository.GetAsync(workflowId, false);

        await AuthorizationService.CheckAsync(definition, name);
    }

    protected virtual async Task CheckWorkflowPermissionAsync(WorkflowInstance workflow, string name)
    {
        var repository = LazyServiceProvider.LazyGetRequiredService<IWorkflowDefinitionRepository>();

        var definition = await repository.GetAsync(workflow.WorkflowDefinitionId, false);

        await AuthorizationService.CheckAsync(definition, name);
    }

    protected virtual async Task CheckWorkflowPermissionAsync(WorkflowDefinition definition, string name)
    {
        await AuthorizationService.CheckAsync(definition, name);
    }
}
