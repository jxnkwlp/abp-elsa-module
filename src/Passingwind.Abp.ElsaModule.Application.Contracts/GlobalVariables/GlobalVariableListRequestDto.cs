using Volo.Abp.Application.Dtos;

namespace Passingwind.Abp.ElsaModule.GlobalVariables
{
    public class GlobalVariableListRequestDto : PagedResultRequestDto
    {
        public string Filter { get; set; }
    }
}
