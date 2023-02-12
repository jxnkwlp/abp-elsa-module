using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Passingwind.WorkflowApp.Controllers;
using Volo.Abp.Application.Dtos;

namespace Passingwind.WorkflowApp.ApiKeys;

[Route("api/secrets/apikeys")]
public class ApiKeyController : WorkflowAppController, IApiKeyAppService
{
    private readonly IApiKeyAppService _service;

    public ApiKeyController(IApiKeyAppService service)
    {
        _service = service;
    }

    [HttpPost]
    public Task<ApiKeyDto> CreateAsync(ApiKeyCreateOrUpdateDto input)
    {
        return _service.CreateAsync(input);
    }

    [HttpDelete("{id}")]
    public Task DeleteAsync(Guid id)
    {
        return _service.DeleteAsync(id);
    }

    [HttpGet("{id}")]
    public Task<ApiKeyDto> GetAsync(Guid id)
    {
        return _service.GetAsync(id);
    }

    [HttpGet]
    public Task<PagedResultDto<ApiKeyDto>> GetListAsync(ApiKeyListRequestDto input)
    {
        return _service.GetListAsync(input);
    }

    [HttpPut("{id}")]
    public Task<ApiKeyDto> UpdateAsync(Guid id, ApiKeyCreateOrUpdateDto input)
    {
        return _service.UpdateAsync(id, input);
    }
}
