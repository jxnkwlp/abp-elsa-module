using Volo.Abp.Domain.Services;

namespace Passingwind.Abp.ElsaModule.Groups;

public class WorkflowGroupManager : DomainService
{
    private readonly IWorkflowGroupRepository _workflowGroupRepository;

    public WorkflowGroupManager(IWorkflowGroupRepository workflowGroupRepository)
    {
        _workflowGroupRepository = workflowGroupRepository;
    }
}
