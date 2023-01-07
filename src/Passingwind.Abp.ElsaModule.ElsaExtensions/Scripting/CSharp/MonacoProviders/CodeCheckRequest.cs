using System.ComponentModel.DataAnnotations;

namespace Passingwind.Abp.ElsaModule.Scripting.CSharp.MonacoProviders;

public class CodeCheckRequest
{
    [Required]
    public virtual string Code { get; set; }
}
