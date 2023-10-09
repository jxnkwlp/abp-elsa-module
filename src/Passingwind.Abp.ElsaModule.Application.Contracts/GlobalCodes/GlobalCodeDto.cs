using System;
using Volo.Abp.Application.Dtos;

namespace Passingwind.Abp.ElsaModule.GlobalCodes;

public class GlobalCodeDto : ExtensibleAuditedEntityDto<Guid>
{
    public virtual string Name { get; set; }
    public virtual string Description { get; set; }
    public virtual GlobalCodeLanguage Language { get; set; }
    public virtual GlobalCodeType Type { get; set; }
    public virtual int LatestVersion { get; set; }
    public virtual int PublishedVersion { get; set; }
    public virtual Guid? TenantId { get; set; }
    public string Content { get; set; }

    public string LanguageDescription => Language.ToString();
    public string TypeDescription => Type.ToString();
}
