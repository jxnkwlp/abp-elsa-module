using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Identity;

namespace Passingwind.Abp.ElsaModule.Services
{
    public class UserLookupService : IUserLookupService
    {
        private readonly IIdentityUserRepository _identityUserRepository;

        public UserLookupService(IIdentityUserRepository identityUserRepository)
        {
            _identityUserRepository = identityUserRepository;
        }

        public async Task<List<UserLookupResultItem>> SearchAsync(string filter = null, CancellationToken cancellationToken = default)
        {
            var list = await _identityUserRepository.GetListAsync();
            return list.Select(x => new UserLookupResultItem
            {
                UserName = x.UserName,
                DisplayName = x.UserName,
            }).ToList();
        }
    }

    public class RoleLookupService : IRoleLookupService
    {
        private readonly IIdentityRoleRepository _identityRoleRepository;

        public RoleLookupService(IIdentityRoleRepository identityRoleRepository)
        {
            _identityRoleRepository = identityRoleRepository;
        }

        public async Task<List<RoleLookupResultItem>> SearchAsync(string filter = null, CancellationToken cancellationToken = default)
        {
            var list = await _identityRoleRepository.GetListAsync();
            return list.Select(x => new RoleLookupResultItem
            {
                Id = x.Id.ToString(),
                Name = x.Name
            }).ToList();
        }
    }
}
