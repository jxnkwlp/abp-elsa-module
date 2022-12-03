using System;
using Volo.Abp.Application.Services;

namespace Demo.ApiKeys;

public interface IApiKeyAppService : ICrudAppService<ApiKeyDto, Guid, ApiKeyListRequestDto, ApiKeyCreateOrUpdateDto, ApiKeyCreateOrUpdateDto>
{

}
