using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Passingwind.Abp.ElsaModule.Common;
using Passingwind.Abp.ElsaModule.Localization;
using Passingwind.Abp.ElsaModule.Permissions;
using Volo.Abp.Application.Services;

namespace Passingwind.Abp.ElsaModule;

public abstract class ElsaModuleAppService : ApplicationService
{
    protected IWorkflowPermissionProvider WorkflowPermissionProvider => LazyServiceProvider.GetRequiredService<IWorkflowPermissionProvider>();

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

    protected async Task<IEnumerable<Guid>> FilterWorkflowsAsync(IEnumerable<Guid> sources = null)
    {
        var grantedResult = await WorkflowPermissionProvider.GetGrantsAsync();

        var filterIds = sources?.ToList();

        if (!grantedResult.AllGranted)
        {
            if (sources?.Any() == true)
            {
                filterIds = grantedResult.WorkflowIds.Intersect(sources).ToList();
            }
            else
            {
                filterIds = grantedResult.WorkflowIds.ToList();
            }
        }

        return filterIds;
    }
}
