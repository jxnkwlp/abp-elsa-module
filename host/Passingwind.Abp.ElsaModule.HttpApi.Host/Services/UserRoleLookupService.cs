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

        public async Task<UserLookupResultItem> FindByUserNameAsync(string userName, CancellationToken cancellationToken = default)
        {
            var item = await _identityUserRepository.FindByNormalizedUserNameAsync(userName);

            if (item == null)
                return null;

            return new UserLookupResultItem
            {
                UserName = item.UserName,
                DisplayName = item.UserName,
                Email = item.Email,
            };
        }

        public async Task<List<UserLookupResultItem>> GetListAsync(string filter = null, CancellationToken cancellationToken = default)
        {
            var list = await _identityUserRepository.GetListAsync();
            return list.Select(x => new UserLookupResultItem
            {
                UserName = x.UserName,
                DisplayName = x.UserName,
                Email = x.Email,
            }).ToList();
        }

        public async Task<List<UserLookupResultItem>> GetListByRoleNameAsync(string roleName, CancellationToken cancellationToken = default)
        {
            var users = await _identityUserRepository.GetListByNormalizedRoleNameAsync(roleName);

            return users.Select(x => new UserLookupResultItem
            {
                UserName = x.UserName,
                DisplayName = x.UserName,
                Email = x.Email,
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

        public async Task<List<RoleLookupResultItem>> GetListAsync(string filter = null, CancellationToken cancellationToken = default)
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
