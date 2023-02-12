using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace Passingwind.WorkflowApp.ApiKeys;

public interface IApiKeyRepository : IRepository<ApiKey, Guid>
{
    Task<long> GetCountAsync(Guid userId, CancellationToken cancellationToken = default);

    Task<List<ApiKey>> GetListAsync(Guid userId, bool includeDetails = false, CancellationToken cancellationToken = default);

    Task<List<ApiKey>> GetPagedListAsync(int skipCount, int maxResultCount, Guid userId, string sorting, bool includeDetails = false, CancellationToken cancellationToken = default);

}
