using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Passingwind.Abp.ElsaModule.Common;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace Passingwind.Abp.ElsaModule.EntityFrameworkCore.Repositories;

public class WorkflowDefinitionRepository : EfCoreRepository<IElsaModuleDbContext, WorkflowDefinition, Guid>, IWorkflowDefinitionRepository
{
    public WorkflowDefinitionRepository(IDbContextProvider<IElsaModuleDbContext> dbContextProvider) : base(dbContextProvider)
    {
    }

    public async Task<long> CountAsync(string name = null, bool? isSingleton = null, int? publishedVersion = null, string channel = null, string tag = null, IEnumerable<Guid> filterIds = null, CancellationToken cancellationToken = default)
    {
        var dbset = await GetDbSetAsync();

        return await dbset
              .WhereIf(!string.IsNullOrEmpty(name), x => x.Name.Contains(name) || x.DisplayName.Contains(name))
              .WhereIf(!string.IsNullOrEmpty(channel), x => x.Channel.Contains(name))
              .WhereIf(!string.IsNullOrEmpty(tag), x => x.Channel.Contains(tag))
              .WhereIf(filterIds?.Any() == true, x => filterIds.Contains(x.Id))
              .WhereIf(isSingleton.HasValue, x => x.IsSingleton == isSingleton)
              .WhereIf(publishedVersion.HasValue, x => x.PublishedVersion == publishedVersion)
              .LongCountAsync();
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

    public async Task<List<WorkflowDefinition>> GetListAsync(string name = null, bool? isSingleton = null, int? publishedVersion = null, string channel = null, string tag = null, IEnumerable<Guid> filterIds = null, CancellationToken cancellationToken = default)
    {
        var dbset = await GetDbSetAsync();

        return await dbset
              .WhereIf(!string.IsNullOrEmpty(name), x => x.Name.Contains(name) || x.DisplayName.Contains(name))
              .WhereIf(!string.IsNullOrEmpty(channel), x => x.Channel.Contains(name))
              .WhereIf(!string.IsNullOrEmpty(tag), x => x.Channel.Contains(tag))
              .WhereIf(filterIds?.Any() == true, x => filterIds.Contains(x.Id))
              .WhereIf(isSingleton.HasValue, x => x.IsSingleton == isSingleton)
              .WhereIf(publishedVersion.HasValue, x => x.PublishedVersion == publishedVersion)
              .ToListAsync(cancellationToken);
    }

    public async Task<List<WorkflowDefinition>> GetPagedListAsync(int skipCount, int maxResultCount, string name = null, bool? isSingleton = null, int? publishedVersion = null, string channel = null, string tag = null, IEnumerable<Guid> filterIds = null, string ordering = null, CancellationToken cancellationToken = default)
    {
        var dbset = await GetDbSetAsync();

        return await dbset
              .WhereIf(!string.IsNullOrEmpty(name), x => x.Name.Contains(name) || x.DisplayName.Contains(name))
              .WhereIf(!string.IsNullOrEmpty(channel), x => x.Channel.Contains(name))
              .WhereIf(!string.IsNullOrEmpty(tag), x => x.Channel.Contains(tag))
              .WhereIf(filterIds?.Any() == true, x => filterIds.Contains(x.Id))
              .WhereIf(isSingleton.HasValue, x => x.IsSingleton == isSingleton)
              .WhereIf(publishedVersion.HasValue, x => x.PublishedVersion == publishedVersion)
              .OrderBy(ordering ?? $"{nameof(WorkflowDefinition.CreationTime)} desc")
              .PageBy(skipCount, maxResultCount)
              .ToListAsync(cancellationToken);
    }
}
