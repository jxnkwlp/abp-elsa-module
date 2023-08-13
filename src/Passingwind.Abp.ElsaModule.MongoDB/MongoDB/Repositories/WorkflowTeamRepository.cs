using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using Passingwind.Abp.ElsaModule.Teams;
using Volo.Abp.Domain.Repositories.MongoDB;
using Volo.Abp.MongoDB;

namespace Passingwind.Abp.ElsaModule.MongoDB.Repositories;

public class WorkflowTeamRepository : MongoDbRepository<IElsaModuleMongoDbContext, WorkflowTeam, Guid>, IWorkflowTeamRepository
{
    public WorkflowTeamRepository(IMongoDbContextProvider<IElsaModuleMongoDbContext> dbContextProvider) : base(dbContextProvider)
    {
    }

    public async Task UpdateRoleNameAsync(string oldName, string roleName, CancellationToken cancellationToken = default)
    {
        var query = await GetMongoQueryableAsync(cancellationToken);

        // TODO: performance optimization

        var list = await query.Where(x => x.RoleScopes.Any(r => r.RoleName == oldName)).As<IMongoQueryable<WorkflowTeam>>().ToListAsync();

        foreach (var item in list)
        {
            foreach (var scope in item.RoleScopes)
            {
                if (scope.RoleName == oldName)
                {
                    scope.RoleName = roleName;
                }
            }
        }
    }
     
}
