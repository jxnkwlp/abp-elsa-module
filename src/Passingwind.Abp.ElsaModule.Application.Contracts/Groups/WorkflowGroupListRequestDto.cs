using Volo.Abp.Application.Dtos;

namespace Passingwind.Abp.ElsaModule.Groups;

public class WorkflowGroupListRequestDto : PagedResultRequestDto
{
    public string Filter { get; set; }
}
