using System.ComponentModel.DataAnnotations;

namespace Passingwind.Abp.ElsaModule.Teams;

public class WorkflowTeamRoleScopeValueDto
{
    [Required]
    [MaxLength(64)]
    public string ProviderName { get; set; }

    [Required]
    [MaxLength(64)]
    public string ProviderValue { get; set; }
}
