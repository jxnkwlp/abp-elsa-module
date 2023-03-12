using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Passingwind.Abp.ElsaModule.CSharp;
using Passingwind.Abp.ElsaModule.Permissions;
using Passingwind.Abp.ElsaModule.Services;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Caching;
using Volo.Abp.Domain;
using Volo.Abp.Identity;
using Volo.Abp.Modularity;
using Volo.Abp.PermissionManagement;

namespace Passingwind.Abp.ElsaModule;

[DependsOn(
    typeof(AbpDddDomainModule),
    typeof(ElsaModuleDomainSharedModule),
    typeof(AbpIdentityDomainModule),
    typeof(AbpPermissionManagementDomainModule),
    typeof(AbpCachingModule)
)]
public class ElsaModuleDomainModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.AddMediatR(typeof(ElsaModuleDomainModule));

        context.Services.AddTransient<IWorkflowCSharpEditorService, NullMonacoEditorService>();

        Configure<PermissionManagementOptions>(options =>
        {
            options.IsDynamicPermissionStoreEnabled = true;
            options.ManagementProviders.Add<WorkflowGroupPermissionManagementProvider>();
            options.ManagementProviders.Add<WorkflowUserOwnerPermissionManagementProvider>();
        });

        Configure<AbpPermissionOptions>(options =>
        {
            options.ValueProviders.Add<WorkflowGroupPermissionValueProvider>();
            options.ValueProviders.Add<WorkflowUserOwnerPermissionValueProvider>();
        });
    }

    public override void PostConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.Replace<Elsa.Services.IIdGenerator, AbpElsaIdGenerator>(ServiceLifetime.Singleton);
        context.Services.Replace<Elsa.Services.ITenantAccessor, AbpElsaTenantAccessor>(ServiceLifetime.Transient);
        context.Services.Replace<Elsa.Services.IWorkflowFactory, NewWorkflowFactory>(ServiceLifetime.Transient);
    } 
}
