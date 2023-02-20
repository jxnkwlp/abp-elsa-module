using System.Collections.Generic;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Passingwind.Abp.ElsaModule.CSharp;
using Passingwind.Abp.ElsaModule.Services;
using Volo.Abp.Caching;
using Volo.Abp.Domain;
using Volo.Abp.Json.SystemTextJson;
using Volo.Abp.Modularity;

namespace Passingwind.Abp.ElsaModule;

[DependsOn(
    typeof(AbpDddDomainModule),
    typeof(ElsaModuleDomainSharedModule),
    typeof(AbpCachingModule)
)]
public class ElsaModuleDomainModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        PostConfigure<AbpSystemTextJsonSerializerOptions>(options =>
        {
            options.JsonSerializerOptions.Converters.AddIfNotContains(new SystemTextJsonTypeJsonConverter());
        });

        context.Services.AddMediatR(typeof(ElsaModuleDomainModule));

        context.Services.AddTransient<IWorkflowCSharpEditorService, NullMonacoEditorService>();
    }

    public override void PostConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.Replace<Elsa.Services.IIdGenerator, AbpElsaIdGenerator>(ServiceLifetime.Singleton);
        context.Services.Replace<Elsa.Services.ITenantAccessor, AbpElsaTenantAccessor>(ServiceLifetime.Singleton);
        context.Services.Replace<Elsa.Services.IWorkflowFactory, NewWorkflowFactory>(ServiceLifetime.Transient);
    }
}
