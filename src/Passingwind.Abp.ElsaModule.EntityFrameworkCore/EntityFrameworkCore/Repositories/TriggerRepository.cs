using System;
using Passingwind.Abp.ElsaModule.Common;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace Passingwind.Abp.ElsaModule.EntityFrameworkCore.Repositories
{
    public class TriggerRepository : EfCoreRepository<IElsaModuleDbContext, Trigger, Guid>, ITriggerRepository
    {
        public TriggerRepository(IDbContextProvider<IElsaModuleDbContext> dbContextProvider) : base(dbContextProvider)
        {
        }

    }
}
