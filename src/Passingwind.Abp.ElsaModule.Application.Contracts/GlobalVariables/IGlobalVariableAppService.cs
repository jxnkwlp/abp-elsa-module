using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace Passingwind.Abp.ElsaModule.GlobalVariables;

public interface IGlobalVariableAppService : ICrudAppService<GlobalVariableDto, Guid, GlobalVariableListRequestDto, GlobalVariableCreateOrUpdateDto, GlobalVariableCreateOrUpdateDto>, IApplicationService
{
    Task<GlobalVariableDto> GetByKeyAsync(string key);
}
