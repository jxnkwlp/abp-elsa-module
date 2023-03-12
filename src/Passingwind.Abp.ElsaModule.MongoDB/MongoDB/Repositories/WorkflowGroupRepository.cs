using System;
using Passingwind.Abp.ElsaModule.WorkflowGroups;
using Volo.Abp.Domain.Repositories.MongoDB;
using Volo.Abp.MongoDB;

namespace Passingwind.Abp.ElsaModule.MongoDB.Repositories;

public class WorkflowGroupRepository : MongoDbRepository<IElsaModuleMongoDbContext, WorkflowGroup, Guid>, IWorkflowGroupRepository
{
    public WorkflowGroupRepository(IMongoDbContextProvider<IElsaModuleMongoDbContext> dbContextProvider) : base(dbContextProvider)
    {
    }
}
