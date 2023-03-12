using System;
using Volo.Abp.Domain.Repositories;

namespace Passingwind.Abp.ElsaModule.WorkflowGroups;

public interface IWorkflowGroupRepository : IRepository<WorkflowGroup, Guid>
{
}
