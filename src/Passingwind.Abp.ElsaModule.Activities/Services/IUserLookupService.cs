using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Passingwind.Abp.ElsaModule.Services
{
    public interface IUserLookupService
    {
        Task<List<UserLookupResultItem>> SearchAsync(string filter = null, CancellationToken cancellationToken = default);
    }
}
