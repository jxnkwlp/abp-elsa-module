using System;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace Passingwind.Abp.ElsaModule.GlobalCodes;

public interface IGlobalCodeContentRepository : IRepository<GlobalCodeContent, Guid>
{
    Task<GlobalCodeContent> FindByVersionAsync(Guid codeId, int version, CancellationToken cancellationToken = default);
    Task<bool> IsVersionExistsAsync(Guid codeId, int version, CancellationToken cancellationToken = default);
    Task UpdateContentAsync(Guid codeId, int version, string content, CancellationToken cancellationToken = default);
}
