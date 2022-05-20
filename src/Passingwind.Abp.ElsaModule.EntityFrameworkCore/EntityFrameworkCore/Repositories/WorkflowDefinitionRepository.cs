using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
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

        public async Task<Guid> GetIdByNameAsync(string name, CancellationToken cancellationToken = default)
        {
            var dbset = await GetDbSetAsync();

            return await dbset.Where(x => x.Name == name).Select(x => x.Id).FirstOrDefaultAsync();
        }

        public async Task<Guid> GetIdByTagAsync(string tags, CancellationToken cancellationToken = default)
        {
            var dbset = await GetDbSetAsync();

            return await dbset.Where(x => x.Tag == tags).Select(x => x.Id).FirstOrDefaultAsync();
        }

        public async Task<Guid[]> GetIdsByNamesAsync(IEnumerable<string> names, CancellationToken cancellationToken = default)
        {
            var dbset = await GetDbSetAsync();

            return await dbset.Where(x => names.Contains(x.Name)).Select(x => x.Id).ToArrayAsync();
        }

        public async Task<Guid[]> GetIdsByTagsAsync(IEnumerable<string> tags, CancellationToken cancellationToken = default)
        {
            var dbset = await GetDbSetAsync();

            return await dbset.Where(x => tags.Contains(x.Tag)).Select(x => x.Id).ToArrayAsync();
        }

    }
}
