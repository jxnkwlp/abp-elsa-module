using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Passingwind.Abp.ElsaModule.WorkflowGroups;

public class WorkflowGroupCreateOrUpdateDto
{
    [Required]
    [MaxLength(64)]
    public string Name { get; set; }

    public string Description { get; set; }

    public Guid RoleId { get; set; }

    public List<Guid> UserIds { get; set; }

    public List<Guid> WorkflowIds { get; set; }
}
