using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Passingwind.Abp.ElsaModule.Services;

public class EmptyRoleLookupService : IRoleLookupService
{
    public Task<List<RoleLookupResultItem>> GetListAsync(string filter = null, CancellationToken cancellationToken = default)
    {
        return Task.FromResult<List<RoleLookupResultItem>>(Enumerable.Empty<RoleLookupResultItem>().ToList());
    }
}
