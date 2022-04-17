using System;
using Passingwind.Abp.ElsaModule.Common;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace Passingwind.Abp.ElsaModule.EntityFrameworkCore.Repositories
{
    public class WorkflowDefinitionRepository : EfCoreRepository<IElsaModuleDbContext, WorkflowDefinition, Guid>, IWorkflowDefinitionRepository
    {
        public WorkflowDefinitionRepository(IDbContextProvider<IElsaModuleDbContext> dbContextProvider) : base(dbContextProvider)
        {
        }
    }
}
