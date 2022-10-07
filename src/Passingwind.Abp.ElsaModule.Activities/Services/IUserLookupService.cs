using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Passingwind.Abp.ElsaModule.Services
{
    public interface IUserLookupService
    {
        Task<List<UserLookupResultItem>> GetListAsync(string filter = null, CancellationToken cancellationToken = default);

        Task<UserLookupResultItem> FindByUserNameAsync(string userName, CancellationToken cancellationToken = default);

        Task<List<UserLookupResultItem>> GetListByRoleNameAsync(string roleName, CancellationToken cancellationToken = default);
    }
}
