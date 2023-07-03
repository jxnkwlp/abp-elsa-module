using System.ComponentModel.DataAnnotations;
using Volo.Abp.Content;

namespace Passingwind.Abp.ElsaModule.WorkflowDefinitions;

public class WorkflowDefinitionImportRequestDto
{
    [Required]
    public IRemoteStreamContent File { get; set; }

    public bool Overwrite { get; set; }
}
