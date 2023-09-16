using System;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace Passingwind.Abp.ElsaModule.Teams;

public interface IWorkflowTeamRepository : IRepository<WorkflowTeam, Guid>
{
    Task UpdateRoleNameAsync(string oldName, string roleName, CancellationToken cancellationToken = default);
}
