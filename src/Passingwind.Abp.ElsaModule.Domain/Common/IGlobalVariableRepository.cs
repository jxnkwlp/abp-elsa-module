using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace Passingwind.Abp.ElsaModule.Common
{
    public interface IGlobalVariableRepository : IRepository<GlobalVariable, Guid>
    {
        Task<long> CountAsync(string filter, CancellationToken cancellationToken = default);

        Task<List<GlobalVariable>> GetListAsync(string filter, bool includeDetails = false, CancellationToken cancellationToken = default);

        Task<List<GlobalVariable>> GetPagedListAsync(int skipCount, int maxResultCount, string filter, string sorting, bool includeDetails = false, CancellationToken cancellationToken = default);

        Task<GlobalVariable> GetAsync(string key, CancellationToken cancellationToken = default);
        Task<GlobalVariable> FindAsync(string key, CancellationToken cancellationToken = default);
    }

}
