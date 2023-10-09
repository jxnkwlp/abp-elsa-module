using Volo.Abp.Application.Dtos;

namespace Passingwind.Abp.ElsaModule.GlobalCodes;

public class GlobalCodeListRequestDto : PagedAndSortedResultRequestDto
{
    public string Filter { get; set; }
}
