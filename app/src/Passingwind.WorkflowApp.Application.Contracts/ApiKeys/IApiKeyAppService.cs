using System;
using Volo.Abp.Application.Services;

namespace Passingwind.WorkflowApp.ApiKeys;

public interface IApiKeyAppService : ICrudAppService<ApiKeyDto, Guid, ApiKeyListRequestDto, ApiKeyCreateOrUpdateDto, ApiKeyCreateOrUpdateDto>, IApplicationService
{

}
