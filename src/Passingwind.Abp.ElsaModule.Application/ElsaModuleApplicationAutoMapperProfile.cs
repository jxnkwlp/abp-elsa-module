using AutoMapper;
using Elsa.Metadata;
using Passingwind.Abp.ElsaModule.Common;
using Passingwind.Abp.ElsaModule.Designer;
using Passingwind.Abp.ElsaModule.Workflow;
using Passingwind.Abp.ElsaModule.WorkflowDefinitions;
using Passingwind.Abp.ElsaModule.WorkflowInstances;
using Volo.Abp.AutoMapper;

namespace Passingwind.Abp.ElsaModule;

public class ElsaModuleApplicationAutoMapperProfile : Profile
{
    public ElsaModuleApplicationAutoMapperProfile()
    {
        /* You can configure your AutoMapper mapping configuration here.
         * Alternatively, you can split your mapping configurations
         * into multiple profile classes for a better organization. */

        CreateMap<WorkflowDefinitionCreateOrUpdateDto, WorkflowDefinition>()
            .Ignore(x => x.TenantId)
            .Ignore(x => x.LatestVersion)
            .Ignore(x => x.PublishedVersion)
            .Ignore(x => x.Id)
            .Ignore(x => x.ContextOptions)
            .Ignore(x => x.CustomAttributes)
            .Ignore(x => x.Variables)
            .IgnoreFullAuditedObjectProperties();

        CreateMap<WorkflowDefinitionVersionCreateOrUpdateDto, WorkflowDefinitionVersion>()
            .Ignore(x => x.TenantId)
            .Ignore(x => x.Definition)
            .Ignore(x => x.DefinitionId)
            .Ignore(x => x.Id)
            .Ignore(x => x.IsLatest)
            //.Ignore(x => x.IsPublished)
            .Ignore(x => x.Version)
            .IgnoreFullAuditedObjectProperties();

        CreateMap<WorkflowDefinitionVersion, WorkflowDefinitionVersionListItemDto>();
        CreateMap<WorkflowDefinition, WorkflowDefinitionDto>();
        CreateMap<WorkflowDefinitionVersion, WorkflowDefinitionVersionDto>();

        CreateMap<ActivityCreateOrUpdateDto, Activity>()
            .Ignore(x => x.DefinitionVersionId)
            .IgnoreAuditedObjectProperties();

        CreateMap<ActivityConnectionDto, ActivityConnection>()
            .Ignore(x => x.DefinitionVersionId)
            .IgnoreCreationAuditedObjectProperties();

        CreateMap<ActivityConnectionCreateDto, ActivityConnection>()
            .Ignore(x => x.DefinitionVersionId)
            .IgnoreCreationAuditedObjectProperties();

        CreateMap<Activity, ActivityDto>();
        CreateMap<ActivityConnection, ActivityConnectionDto>();

        CreateMap<ActivityDescriptor, ActivityTypeDescriptorDto>();

        CreateMap<WorkflowInstance, WorkflowInstanceDto>();
        CreateMap<WorkflowInstance, WorkflowInstanceBasicDto>();

        CreateMap<WorkflowExecutionLog, WorkflowExecutionLogDto>();

    }
}
