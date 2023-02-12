using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Passingwind.Abp.ElsaModule.Services;
using Volo.Abp.Identity;

namespace Passingwind.WorkflowApp.Web.Services;

public class UserAndRoleLookupService : IUserLookupService, IRoleLookupService
{
    private readonly IIdentityUserRepository _identityUserRepository;
    private readonly IIdentityRoleRepository _identityRoleRepository;

    public UserAndRoleLookupService(IIdentityUserRepository identityUserRepository, IIdentityRoleRepository identityRoleRepository)
    {
        _identityUserRepository = identityUserRepository;
        _identityRoleRepository = identityRoleRepository;
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

    async Task<List<RoleLookupResultItem>> IRoleLookupService.GetListAsync(string filter, CancellationToken cancellationToken)
    {
        var list = await _identityRoleRepository.GetListAsync();
        return list.Select(x => new RoleLookupResultItem
        {
            Id = x.Id.ToString(),
            Name = x.Name
        }).ToList();
    }
}
