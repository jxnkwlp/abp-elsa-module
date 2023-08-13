using System;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Passingwind.Abp.ElsaModule.Teams;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace Passingwind.Abp.ElsaModule.EntityFrameworkCore.Repositories;

public class WorkflowTeamRepository : EfCoreRepository<ElsaModuleDbContext, WorkflowTeam, Guid>, IWorkflowTeamRepository
{
    public WorkflowTeamRepository(IDbContextProvider<ElsaModuleDbContext> dbContextProvider) : base(dbContextProvider)
    {
    }

    public async Task UpdateRoleNameAsync(string oldName, string roleName, CancellationToken cancellationToken = default)
    {
        var dbcontext = await GetDbContextAsync();

        var dbset = dbcontext.Set<WorkflowTeamRoleScope>();

        await dbset.Where(x => x.RoleName == oldName).ExecuteUpdateAsync(s => s.SetProperty(x => x.RoleName, roleName));
    } 
}
