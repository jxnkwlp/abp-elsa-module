using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Passingwind.Abp.ElsaModule.Teams;

public class WorkflowTeamCreateOrUpdateDto
{
    [Required]
    [MaxLength(64)]
    public string Name { get; set; }

    public string Description { get; set; }

    public List<Guid> UserIds { get; set; }
}
