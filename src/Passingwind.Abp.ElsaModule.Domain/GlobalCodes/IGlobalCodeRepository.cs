using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace Passingwind.Abp.ElsaModule.GlobalCodes;

public interface IGlobalCodeRepository : IRepository<GlobalCode, Guid>
{
    Task<long> GetCountAsync(string filter = null, CancellationToken cancellationToken = default);

    Task<List<GlobalCode>> GetListAsync(string filter = null, bool includeDetails = false, CancellationToken cancellationToken = default);

    Task<List<GlobalCode>> GetPagedListAsync(int skipCount, int maxResultCount, string filter = null, string sorting = null, bool includeDetails = false, CancellationToken cancellationToken = default);

    Task<bool> IsNameExistsAsync(string name, Guid[] excludeIds = null, CancellationToken cancellationToken = default);
    Task<GlobalCode> GetByNameAsync(string name, CancellationToken cancellationToken = default);
    Task<GlobalCode> FindByNameAsync(string name, CancellationToken cancellationToken = default);
}
