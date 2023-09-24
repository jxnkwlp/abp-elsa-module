using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace Passingwind.Abp.ElsaModule.GlobalCodes;

public interface IGlobalCodeVersionRepository : IRepository<GlobalCodeVersion, Guid>
{
    Task<List<GlobalCodeVersion>> GetListAsync(Guid codeId, CancellationToken cancellationToken = default);
}
