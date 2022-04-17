using Volo.Abp.Application.Dtos;

namespace Passingwind.Abp.ElsaModule.WorkflowDefinitions
{
    public class WorkflowDefinitionListRequestDto : PagedResultRequestDto
    {
        public string Filter { get; set; }
    }
}
