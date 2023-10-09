using System.Linq;
using AutoMapper;
using Elsa.Metadata;
using Passingwind.Abp.ElsaModule.Common;
using Passingwind.Abp.ElsaModule.GlobalCodes;
using Passingwind.Abp.ElsaModule.GlobalVariables;
using Passingwind.Abp.ElsaModule.Groups;
using Passingwind.Abp.ElsaModule.Teams;
using Passingwind.Abp.ElsaModule.Workflow;
using Passingwind.Abp.ElsaModule.WorkflowDefinitions;
using Passingwind.Abp.ElsaModule.WorkflowExecutionLog;
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

        CreateMap<WorkflowDefinition, WorkflowDefinitionDto>();
        CreateMap<WorkflowDefinition, WorkflowDefinitionBasicDto>();
        CreateMap<WorkflowDefinitionVersion, WorkflowDefinitionVersionListItemDto>();
        CreateMap<WorkflowDefinitionVersion, WorkflowDefinitionVersionDto>()
            .Ignore(x => x.Definition);

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

        CreateMap<Common.WorkflowExecutionLog, WorkflowExecutionLogDto>();

        CreateMap<GlobalVariable, GlobalVariableDto>();

        CreateMap<WorkflowTeam, WorkflowTeamBasicDto>();
        CreateMap<WorkflowTeam, WorkflowTeamDto>()
            .ForMember(x => x.UserIds, x => x.MapFrom(f => f.Users.Select(u => u.UserId)));

        CreateMap<WorkflowTeamRoleScope, WorkflowTeamRoleScopeDto>();
        CreateMap<WorkflowTeamRoleScopeValue, WorkflowTeamRoleScopeValueDto>();

        CreateMap<WorkflowGroup, WorkflowGroupDto>();

        CreateMap<GlobalCode, GlobalCodeDto>()
            .Ignore(x => x.Content);
    }
}
