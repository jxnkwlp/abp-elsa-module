using MongoDB.Driver;
using Passingwind.Abp.ElsaModule.Common;
using Passingwind.Abp.ElsaModule.WorkflowGroups;
using Volo.Abp.Data;
using Volo.Abp.MongoDB;

namespace Passingwind.Abp.ElsaModule.MongoDB;

[ConnectionStringName(ElsaModuleDbProperties.ConnectionStringName)]
public interface IElsaModuleMongoDbContext : IAbpMongoDbContext
{
    IMongoCollection<Bookmark> Bookmarks { get; }
    IMongoCollection<Trigger> Triggers { get; }
    IMongoCollection<WorkflowDefinition> WorkflowDefinitions { get; }
    IMongoCollection<WorkflowDefinitionVersion> WorkflowDefinitionVersions { get; }
    IMongoCollection<WorkflowExecutionLog> WorkflowExecutionLogs { get; }
    IMongoCollection<WorkflowInstance> WorkflowInstances { get; }
    IMongoCollection<WorkflowGroup> WorkflowGroups { get; }
}
