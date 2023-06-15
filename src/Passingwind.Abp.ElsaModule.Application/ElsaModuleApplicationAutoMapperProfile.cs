using System.Linq;
using AutoMapper;
using Elsa.Metadata;
using Passingwind.Abp.ElsaModule.Common;
using Passingwind.Abp.ElsaModule.GlobalVariables;
using Passingwind.Abp.ElsaModule.Workflow;
using Passingwind.Abp.ElsaModule.WorkflowDefinitions;
using Passingwind.Abp.ElsaModule.WorkflowGroups;
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

        CreateMap<WorkflowInstance, WorkflowInstanceDto>()
            .ForMember(x => x.Variables, x => x.MapFrom(f => f.GetVariables()))
            .ForMember(x => x.Metadata, x => x.MapFrom(f => f.GetMetadata()))
            ;
        CreateMap<WorkflowInstance, WorkflowInstanceBasicDto>();
        CreateMap<WorkflowInstanceActivityData, WorkflowInstanceActivityDataDto>();
        CreateMap<WorkflowInstanceScheduledActivity, WorkflowInstanceScheduledActivityDto>();
        CreateMap<WorkflowInstanceBlockingActivity, WorkflowInstanceBlockingActivityDto>();
        CreateMap<WorkflowInstanceActivityScope, WorkflowInstanceActivityScopeDto>();
        CreateMap<WorkflowInstanceActivityData, WorkflowInstanceActivityDataDto>();
        CreateMap<WorkflowInstanceFault, WorkflowInstanceFaultBasicDto>();
        CreateMap<WorkflowInstanceFault, WorkflowInstanceFaultDto>();

        CreateMap<WorkflowExecutionLog, WorkflowExecutionLogDto>();

        CreateMap<GlobalVariable, GlobalVariableDto>();

        CreateMap<WorkflowGroup, WorkflowGroupBasicDto>();
        CreateMap<WorkflowGroup, WorkflowGroupDto>()
            .ForMember(x => x.UserIds, x => x.MapFrom(f => f.Users.Select(u => u.UserId)))
            .Ignore(x => x.WorkflowIds);
    }
}
