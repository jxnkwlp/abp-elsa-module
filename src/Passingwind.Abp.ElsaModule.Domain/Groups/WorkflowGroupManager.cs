using System;
using System.Linq;
using System.Data;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Domain.Services;
using Volo.Abp.Uow;

namespace Passingwind.Abp.ElsaModule.Groups;

public class WorkflowGroupManager : DomainService
{
    private readonly IWorkflowGroupRepository _workflowGroupRepository;

    public WorkflowGroupManager(IWorkflowGroupRepository workflowGroupRepository)
    {
        _workflowGroupRepository = workflowGroupRepository;
    }
}
