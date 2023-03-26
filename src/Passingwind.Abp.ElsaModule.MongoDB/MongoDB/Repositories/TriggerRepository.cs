using System;
using Passingwind.Abp.ElsaModule.Common;
using Volo.Abp.Domain.Repositories.MongoDB;
using Volo.Abp.MongoDB;

namespace Passingwind.Abp.ElsaModule.MongoDB.Repositories;

public class TriggerRepository : MongoDbRepository<IElsaModuleMongoDbContext, Trigger, Guid>, ITriggerRepository
{
    public TriggerRepository(IMongoDbContextProvider<IElsaModuleMongoDbContext> dbContextProvider) : base(dbContextProvider)
    {
    }
}
