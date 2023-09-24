using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp;
using Volo.Abp.Application.Dtos;

namespace Passingwind.Abp.ElsaModule.GlobalCodes;

[Area(ElsaModuleRemoteServiceConsts.RemoteServiceName)]
[RemoteService(Name = ElsaModuleRemoteServiceConsts.RemoteServiceName)]
[Route("api/elsa/workflow/global-codes")]
public class GlobalCodeController : ElsaModuleController, IGlobalCodeAppService
{
    private readonly IGlobalCodeAppService _service;

    public GlobalCodeController(IGlobalCodeAppService service)
    {
        _service = service;
    }

    /// <inheritdoc/>
    [HttpGet]
    public virtual Task<PagedResultDto<GlobalCodeDto>> GetListAsync(GlobalCodeListRequestDto input)
    {
        return _service.GetListAsync(input);
    }

    /// <inheritdoc/>
    [HttpGet("{id}")]
    public virtual Task<GlobalCodeDto> GetAsync(Guid id)
    {
        return _service.GetAsync(id);
    }

    /// <inheritdoc/>
    [HttpGet("{id}/versions/{version}")]
    public virtual Task<GlobalCodeDto> GetByVersionAsync(Guid id, int version)
    {
        return _service.GetByVersionAsync(id, version);
    }

    /// <inheritdoc/>
    [HttpGet("{id}/versions")]
    public virtual Task<ListResultDto<int>> GetVersionsAsync(Guid id)
    {
        return _service.GetVersionsAsync(id);
    }

    /// <inheritdoc/>
    [HttpPost]
    public virtual Task<GlobalCodeDto> CreateAsync(GlobalCodeCreateOrUpdateDto input)
    {
        return _service.CreateAsync(input);
    }

    /// <inheritdoc/>
    [HttpPut("{id}")]
    public virtual Task<GlobalCodeDto> UpdateAsync(Guid id, GlobalCodeCreateOrUpdateDto input)
    {
        return _service.UpdateAsync(id, input);
    }

    /// <inheritdoc/>
    [HttpDelete("{id}")]
    public virtual Task DeleteAsync(Guid id)
    {
        return _service.DeleteAsync(id);
    }

    /// <inheritdoc/>
    [HttpDelete("{id}/versions/{version}")]
    public virtual Task DeleteByVersionAsync(Guid id, int version)
    {
        return _service.DeleteByVersionAsync(id, version);
    }
}
