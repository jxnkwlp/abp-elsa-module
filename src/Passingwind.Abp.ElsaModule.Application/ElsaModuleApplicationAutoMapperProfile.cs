using AutoMapper;
using Elsa.Metadata;
using Newtonsoft.Json.Linq;
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

        //CreateMap<WorkflowDefinitionCreateOrUpdateDto, WorkflowDefinition>()
        //    .Ignore(x => x.TenantId)
        //    .Ignore(x => x.LatestVersion)
        //    .Ignore(x => x.PublishedVersion)
        //    .Ignore(x => x.Id)
        //    .Ignore(x => x.ContextOptions)
        //    .Ignore(x => x.CustomAttributes)
        //    .Ignore(x => x.Variables)
        //    .Ignore(x => x.ConcurrencyStamp)
        //    .Ignore(x => x.ExtraProperties)
        //    .IgnoreFullAuditedObjectProperties();

        //CreateMap<WorkflowDefinitionVersionCreateOrUpdateDto, WorkflowDefinitionVersion>()
        //    .Ignore(x => x.TenantId)
        //    //.Ignore(x => x.Definition)
        //    .Ignore(x => x.DefinitionId)
        //    .Ignore(x => x.Id)
        //    .Ignore(x => x.IsLatest)
        //    //.Ignore(x => x.IsPublished)
        //    .Ignore(x => x.Version)
        //    .IgnoreFullAuditedObjectProperties();

        CreateMap<WorkflowDefinitionVersion, WorkflowDefinitionVersionListItemDto>();
        CreateMap<WorkflowDefinition, WorkflowDefinitionDto>();
        CreateMap<WorkflowDefinitionVersion, WorkflowDefinitionVersionDto>()
            .Ignore(x => x.Definition);

        //CreateMap<ActivityCreateOrUpdateDto, Activity>()
        //    .Ignore(x => x.WorkflowDefinitionVersionId)
        //    .IgnoreAuditedObjectProperties();

        //CreateMap<ActivityConnectionDto, ActivityConnection>()
        //    .Ignore(x => x.WorkflowDefinitionVersionId)
        //    .IgnoreCreationAuditedObjectProperties();

        //CreateMap<ActivityConnectionCreateDto, ActivityConnection>()
        //    .Ignore(x => x.WorkflowDefinitionVersionId)
        //    .IgnoreCreationAuditedObjectProperties();

        CreateMap<Activity, ActivityDto>();
        CreateMap<ActivityConnection, ActivityConnectionDto>();

        CreateMap<ActivityDescriptor, ActivityTypeDescriptorDto>();

        CreateMap<WorkflowInstance, WorkflowInstanceDto>();
        CreateMap<WorkflowInstance, WorkflowInstanceBasicDto>();

        CreateMap<WorkflowExecutionLog, WorkflowExecutionLogDto>()
            .ForMember(x => x.Data, x => x.MapFrom(s => string.IsNullOrEmpty(s.Data) ? default : JObject.Parse(s.Data)));

    }
}
