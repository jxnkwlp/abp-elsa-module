using Microsoft.EntityFrameworkCore;
using Passingwind.Abp.ElsaModule.Common;
using Passingwind.Abp.ElsaModule.GlobalCodes;
using Passingwind.Abp.ElsaModule.Groups;
using Passingwind.Abp.ElsaModule.Teams;
using Volo.Abp.Data;
using Volo.Abp.EntityFrameworkCore;

namespace Passingwind.Abp.ElsaModule.EntityFrameworkCore;

[ConnectionStringName(ElsaModuleDbProperties.ConnectionStringName)]
public class ElsaModuleDbContext : AbpDbContext<ElsaModuleDbContext>, IElsaModuleDbContext
{
    public DbSet<Bookmark> Bookmarks { get; set; }
    public DbSet<Trigger> Triggers { get; set; }
    public DbSet<WorkflowDefinition> WorkflowDefinitions { get; set; }
    public DbSet<WorkflowDefinitionVersion> WorkflowDefinitionVersions { get; set; }
    public DbSet<WorkflowExecutionLog> WorkflowExecutionLogs { get; set; }
    public DbSet<WorkflowInstance> WorkflowInstances { get; set; }
    public DbSet<GlobalVariable> GlobalVariables { get; set; }
    public DbSet<WorkflowTeam> WorkflowTeams { get; set; }
    public DbSet<WorkflowGroup> WorkflowGroups { get; set; }
    public DbSet<GlobalCode> GlobalCodes { get; set; }
    public DbSet<GlobalCodeVersion> GlobalCodeVersions { get; set; }
    public DbSet<GlobalCodeContent> GlobalCodeContents { get; }

    public ElsaModuleDbContext(DbContextOptions<ElsaModuleDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ConfigureElsaModule();
    }
}
