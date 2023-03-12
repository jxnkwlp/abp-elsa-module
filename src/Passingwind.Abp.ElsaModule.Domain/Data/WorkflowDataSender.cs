using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Passingwind.Abp.ElsaModule.Permissions;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Guids;
using Volo.Abp.Identity;
using Volo.Abp.MultiTenancy;
using Volo.Abp.PermissionManagement;

namespace Passingwind.Abp.ElsaModule.Data;

public class WorkflowDataSender : IWorkflowDataSender
{
    protected ICurrentTenant CurrentTenant { get; }
    protected IdentityRoleManager RoleManager { get; }
    protected IGuidGenerator GuidGenerator { get; }
    protected IPermissionManager PermissionManager { get; }

    public WorkflowDataSender(ICurrentTenant currentTenant, IdentityRoleManager roleManager, IGuidGenerator guidGenerator, IPermissionManager permissionManager)
    {
        CurrentTenant = currentTenant;
        RoleManager = roleManager;
        GuidGenerator = guidGenerator;
        PermissionManager = permissionManager;
    }

    public async Task SendAsync(Guid? tenantId = null, CancellationToken cancellationToken = default)
    {
        const string workflowAdministratorRoleName = "Workflow Administrator";
        const string workflowDeveloperRoleName = "Workflow Developer";
        const string workflowViewerRoleName = "Workflow Viewer";

        using (CurrentTenant.Change(tenantId))
        {
            if (!await RoleManager.RoleExistsAsync(workflowAdministratorRoleName))
            {
                (await RoleManager.CreateAsync(new IdentityRole(GuidGenerator.Create(), workflowAdministratorRoleName) { IsStatic = true })).CheckErrors();

                // // permissions
                foreach (var name in ElsaModulePermissions.GetAll())
                {
                    await PermissionManager.SetAsync(name, RolePermissionValueProvider.ProviderName, workflowAdministratorRoleName, true);
                }
            }

            if (!await RoleManager.RoleExistsAsync(workflowDeveloperRoleName))
            {
                (await RoleManager.CreateAsync(new IdentityRole(GuidGenerator.Create(), workflowDeveloperRoleName) { IsStatic = true })).CheckErrors();

                // permissions
                foreach (var name in ElsaModulePermissions.GetAll())
                {
                    if (name.EndsWith(".Delete") || name.Contains("GlobalVariables"))
                        continue;

                    await PermissionManager.SetAsync(name, RolePermissionValueProvider.ProviderName, workflowDeveloperRoleName, true);
                }
            }

            if (!await RoleManager.RoleExistsAsync(workflowViewerRoleName))
            {
                (await RoleManager.CreateAsync(new IdentityRole(GuidGenerator.Create(), workflowViewerRoleName) { IsStatic = true })).CheckErrors();

                // permissions
                await PermissionManager.SetAsync(ElsaModulePermissions.Workflow.Default, RolePermissionValueProvider.ProviderName, workflowViewerRoleName, true);
                await PermissionManager.SetAsync(ElsaModulePermissions.Instances.Default, RolePermissionValueProvider.ProviderName, workflowViewerRoleName, true);
                await PermissionManager.SetAsync(ElsaModulePermissions.Instances.Statistic, RolePermissionValueProvider.ProviderName, workflowViewerRoleName, true);
            }
        }
    }
}
