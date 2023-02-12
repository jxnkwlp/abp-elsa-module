using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Users;

namespace Passingwind.WorkflowApp.ApiKeys;

[Authorize]
public class ApiKeyAppService : WorkflowAppAppService, IApiKeyAppService
{
    private readonly IApiKeyRepository _repository;
    private readonly ApiKeyDomainService _domainService;

    public ApiKeyAppService(IApiKeyRepository repository, ApiKeyDomainService domainService)
    {
        _repository = repository;
        _domainService = domainService;
    }

    public async Task<ApiKeyDto> CreateAsync(ApiKeyCreateOrUpdateDto input)
    {
        var entity = new ApiKey
        {
            UserId = CurrentUser.GetId(),
            Name = input.Name,
            ExpirationTime = input.ExpirationTime,
        };

        entity.Secret = await _domainService.GenerateSecretAsync(entity);

        await _repository.InsertAsync(entity);

        return ObjectMapper.Map<ApiKey, ApiKeyDto>(entity);
    }

    public async Task DeleteAsync(Guid id)
    {
        var userId = CurrentUser.GetId();
        await _repository.DeleteAsync(x => x.Id == id && x.UserId == userId);
    }

    public Task<ApiKeyDto> GetAsync(Guid id)
    {
        //var entity = await _repository.GetAsync(id);

        //if (entity.UserId != CurrentUser.GetId())
        //    throw new EntityNotFoundException();

        //entity.Secret = string.Empty;

        //return ObjectMapper.Map<ApiKey, ApiKeyDto>(entity);

        throw new NotSupportedException();
    }

    public async Task<PagedResultDto<ApiKeyDto>> GetListAsync(ApiKeyListRequestDto input)
    {
        var userId = CurrentUser.GetId();
        var count = await _repository.GetCountAsync(userId);
        var list = await _repository.GetPagedListAsync(input.SkipCount, input.MaxResultCount, userId, nameof(ApiKey.CreationTime) + " desc");

        var result = ObjectMapper.Map<List<ApiKey>, List<ApiKeyDto>>(list);
        result.ForEach(x =>
        {
            x.Secret = null;
        });

        return new PagedResultDto<ApiKeyDto>(count, result);
    }

    public Task<ApiKeyDto> UpdateAsync(Guid id, ApiKeyCreateOrUpdateDto input)
    {
        throw new NotSupportedException();
    }
}
