using Microsoft.EntityFrameworkCore;
using Passingwind.Abp.ElsaModule.Common;
using Passingwind.Abp.ElsaModule.Groups;
using Passingwind.Abp.ElsaModule.Teams;
using Volo.Abp.Data;
using Volo.Abp.EntityFrameworkCore;

namespace Passingwind.Abp.ElsaModule.EntityFrameworkCore;

[ConnectionStringName(ElsaModuleDbProperties.ConnectionStringName)]
public interface IElsaModuleDbContext : IEfCoreDbContext
{
    DbSet<Bookmark> Bookmarks { get; }
    DbSet<Trigger> Triggers { get; }
    DbSet<WorkflowDefinition> WorkflowDefinitions { get; }
    DbSet<WorkflowDefinitionVersion> WorkflowDefinitionVersions { get; }
    DbSet<WorkflowExecutionLog> WorkflowExecutionLogs { get; }
    DbSet<WorkflowInstance> WorkflowInstances { get; }
    DbSet<GlobalVariable> GlobalVariables { get; }
    DbSet<WorkflowTeam> WorkflowTeams { get; }
    DbSet<WorkflowGroup> WorkflowGroups { get; }
}
