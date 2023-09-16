using System.ComponentModel.DataAnnotations;
using Volo.Abp.Application.Dtos;

namespace Passingwind.Abp.ElsaModule.Groups;

public class WorkflowGroupCreateOrUpdateDto : ExtensibleEntityDto
{
    [Required]
    [MaxLength(64)]
    public virtual string Name { get; set; }

    [MaxLength(128)]
    public virtual string Description { get; set; }

}
