using System;
using Elsa.Models;
using Volo.Abp.Application.Dtos;

namespace Passingwind.Abp.ElsaModule.WorkflowDefinitions;

public class WorkflowDefinitionListRequestDto : PagedAndSortedResultRequestDto
{
    public string Filter { get; set; }

    public bool IsSingleton { get; set; }

    public bool DeleteCompletedInstances { get; set; }

    public string Channel { get; set; }

    public string Tag { get; set; }

    public Guid? GroupId { get; set; }

    public WorkflowPersistenceBehavior? PersistenceBehavior { get; set; }
}
