using System.ComponentModel.DataAnnotations;
using Volo.Abp.Application.Dtos;

namespace Passingwind.Abp.ElsaModule.GlobalCodes;

public class GlobalCodeCreateOrUpdateDto : ExtensibleEntityDto
{
    [Required]
    [MaxLength(32)]
    public virtual string Name { get; set; }
    [MaxLength(256)]
    public virtual string Description { get; set; }

    public virtual GlobalCodeLanguage Language { get; set; }

    public virtual GlobalCodeType Type { get; set; }

    public bool Publish { get; set; }

    public string Content { get; set; }
}
