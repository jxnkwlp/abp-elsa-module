using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using Passingwind.Abp.ElsaModule.Common;
using Passingwind.Abp.ElsaModule.Designer;
using Passingwind.Abp.ElsaModule.WorkflowDefinitions;
using Passingwind.Abp.ElsaModule.WorkflowInstances;
using Volo.Abp.Application;
using Volo.Abp.Application.Dtos;
using Volo.Abp.AutoMapper;
using Volo.Abp.Json;
using Volo.Abp.Json.SystemTextJson;
using Volo.Abp.Modularity;

namespace Passingwind.Abp.ElsaModule;

[DependsOn(
    typeof(ElsaModuleDomainModule),
    typeof(ElsaModuleApplicationContractsModule),
    typeof(AbpDddApplicationModule),
    typeof(AbpAutoMapperModule),
    typeof(AbpJsonModule)
    )]
public partial class ElsaModuleApplicationModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.AddAutoMapperObjectMapper<ElsaModuleApplicationModule>();

        Configure<AbpAutoMapperOptions>(options =>
        {
            options.AddMaps<ElsaModuleApplicationModule>(validate: true);
        });

        Configure<AbpSystemTextJsonSerializerOptions>(options =>
        {
            options.UnsupportedTypes.AddIfNotContains(typeof(ActivityTypeDescriptorListResultDto));
            options.UnsupportedTypes.AddIfNotContains(typeof(WorkflowInstanceDto));
            options.UnsupportedTypes.AddIfNotContains(typeof(PagedResultDto<WorkflowInstanceBasicDto>));
            options.UnsupportedTypes.AddIfNotContains(typeof(ListResultDto<WorkflowExecutionLogDto>));
            options.UnsupportedTypes.AddIfNotContains(typeof(WorkflowDefinitionVersionCreateOrUpdateDto));
            options.UnsupportedTypes.AddIfNotContains(typeof(WorkflowDefinitionVersionDto));
        });

    }
}
