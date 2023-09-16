using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp;
using Volo.Abp.Application.Dtos;

namespace Passingwind.Abp.ElsaModule.GlobalVariables;

[RemoteService]
[Route("api/elsa/workflow/global-variables")]
public class GlobalVariableController : ElsaModuleController, IGlobalVariableAppService
{
    private readonly IGlobalVariableAppService _service;

    public GlobalVariableController(IGlobalVariableAppService service)
    {
        _service = service;
    }

    [HttpGet("{id}")]
    public virtual Task<GlobalVariableDto> GetAsync(Guid id)
    {
        return _service.GetAsync(id);
    }

    [HttpGet]
    public virtual Task<PagedResultDto<GlobalVariableDto>> GetListAsync(GlobalVariableListRequestDto input)
    {
        return _service.GetListAsync(input);
    }

    [HttpPost]
    public virtual Task<GlobalVariableDto> CreateAsync(GlobalVariableCreateOrUpdateDto input)
    {
        return _service.CreateAsync(input);
    }

    [HttpPut("{id}")]
    public virtual Task<GlobalVariableDto> UpdateAsync(Guid id, GlobalVariableCreateOrUpdateDto input)
    {
        return _service.UpdateAsync(id, input);
    }

    [HttpDelete("{id}")]
    public virtual Task DeleteAsync(Guid id)
    {
        return _service.DeleteAsync(id);
    }

    [HttpGet("by-key/{key}")]
    public virtual Task<GlobalVariableDto> GetByKeyAsync(string key)
    {
        return _service.GetByKeyAsync(key);
    }
}
