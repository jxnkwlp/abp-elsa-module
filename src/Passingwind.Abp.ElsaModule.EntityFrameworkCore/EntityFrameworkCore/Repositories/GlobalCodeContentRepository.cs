using System;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Passingwind.Abp.ElsaModule.GlobalCodes;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace Passingwind.Abp.ElsaModule.EntityFrameworkCore.Repositories;

public class GlobalCodeContentRepository : EfCoreRepository<ElsaModuleDbContext, GlobalCodeContent, Guid>, IGlobalCodeContentRepository
{
    public GlobalCodeContentRepository(IDbContextProvider<ElsaModuleDbContext> dbContextProvider) : base(dbContextProvider)
    {
    }

    public async Task<GlobalCodeContent> FindByVersionAsync(Guid codeId, int version, CancellationToken cancellationToken = default)
    {
        var dbset = await GetDbSetAsync();

        return await dbset.FirstOrDefaultAsync(x => x.GlobalCodeId == codeId && x.Version == version, cancellationToken: cancellationToken);
    }

    public async Task<bool> IsVersionExistsAsync(Guid codeId, int version, CancellationToken cancellationToken = default)
    {
        var dbset = await GetDbSetAsync();

        return await dbset.Where(x => x.GlobalCodeId == codeId && x.Version == version).AnyAsync(cancellationToken: cancellationToken);
    }

    public async Task UpdateContentAsync(Guid codeId, int version, string content, CancellationToken cancellationToken = default)
    {
        var dbset = await GetDbSetAsync();

        if (await dbset.AnyAsync(x => x.GlobalCodeId == codeId && x.Version == version, cancellationToken: cancellationToken))
        {
            await dbset.Where(x => x.GlobalCodeId == codeId && x.Version == version).ExecuteUpdateAsync(x => x.SetProperty(p => p.Content, p => content), cancellationToken: cancellationToken);
        }
        else
        {
            await dbset.AddAsync(new GlobalCodeContent(GuidGenerator.Create(), codeId, version, content, CurrentTenant.Id), cancellationToken);
        }
    }
}
