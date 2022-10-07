using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Passingwind.Abp.ElsaModule.Services
{
    public class EmptyUserLookupService : IUserLookupService
    {
        public Task<UserLookupResultItem> FindByUserNameAsync(string userName, CancellationToken cancellationToken = default)
        {
            return Task.FromResult<UserLookupResultItem>(default);
        }

        public Task<List<UserLookupResultItem>> GetListAsync(string filter = null, CancellationToken cancellationToken = default)
        {
            return Task.FromResult<List<UserLookupResultItem>>(Enumerable.Empty<UserLookupResultItem>().ToList());
        }

        public Task<List<UserLookupResultItem>> GetListByRoleNameAsync(string roleName, CancellationToken cancellationToken = default)
        {
            return Task.FromResult<List<UserLookupResultItem>>(Enumerable.Empty<UserLookupResultItem>().ToList());
        }
    }
}
