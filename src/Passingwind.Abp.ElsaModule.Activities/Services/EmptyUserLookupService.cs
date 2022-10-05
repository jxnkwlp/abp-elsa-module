using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Passingwind.Abp.ElsaModule.Services
{
    public class EmptyUserLookupService : IUserLookupService
    {
        public Task<List<UserLookupResultItem>> SearchAsync(string filter = null, CancellationToken cancellationToken = default)
        {
            return Task.FromResult<List<UserLookupResultItem>>(Enumerable.Empty<UserLookupResultItem>().ToList());
        }
    }
}
