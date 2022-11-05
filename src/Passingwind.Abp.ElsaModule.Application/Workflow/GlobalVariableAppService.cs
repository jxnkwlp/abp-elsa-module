using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Passingwind.Abp.ElsaModule.Common;
using Passingwind.Abp.ElsaModule.GlobalVariables;
using Volo.Abp;
using Volo.Abp.Application.Dtos;

namespace Passingwind.Abp.ElsaModule.Workflow
{
    [Authorize]
    public class GlobalVariableAppService : ElsaModuleAppService, IGlobalVariableAppService
    {
        private readonly IGlobalVariableRepository _globalVariableRepository;

        public GlobalVariableAppService(IGlobalVariableRepository globalVariableRepository)
        {
            _globalVariableRepository = globalVariableRepository;
        }

        public async Task<GlobalVariableDto> CreateAsync(GlobalVariableCreateOrUpdateDto input)
        {
            var exists = await _globalVariableRepository.FindAsync(input.Key);

            if (exists != null)
                throw new UserFriendlyException($"The key '{input.Key}' is exists.");

            var entity = new GlobalVariable()
            {
                Key = input.Key,
                Value = input.Value,
                IsSecret = input.IsSecret,
            };

            await _globalVariableRepository.InsertAsync(entity);

            return ObjectMapper.Map<GlobalVariable, GlobalVariableDto>(entity);
        }

        public async Task DeleteAsync(Guid id)
        {
            await _globalVariableRepository.DeleteAsync(id);
        }

        public async Task<GlobalVariableDto> GetAsync(Guid id)
        {
            var entity = await _globalVariableRepository.GetAsync(id);

            var dto = ObjectMapper.Map<GlobalVariable, GlobalVariableDto>(entity);

            if (dto.IsSecret)
                dto.Value = null;

            return dto;
        }

        public async Task<GlobalVariableDto> GetByKeyAsync(string key)
        {
            var entity = await _globalVariableRepository.GetAsync(key);

            var dto = ObjectMapper.Map<GlobalVariable, GlobalVariableDto>(entity);

            if (dto.IsSecret)
                dto.Value = null;

            return dto;
        }

        public async Task<PagedResultDto<GlobalVariableDto>> GetListAsync(GlobalVariableListRequestDto input)
        {
            var count = await _globalVariableRepository.CountAsync(input.Filter);
            var list = await _globalVariableRepository.GetPagedListAsync(input.SkipCount, input.MaxResultCount, input.Filter, nameof(GlobalVariableDto.Key));

            var result = ObjectMapper.Map<List<GlobalVariable>, List<GlobalVariableDto>>(list);

            foreach (var item in result)
            {
                if (item.IsSecret)
                    item.Value = null;
            }

            return new PagedResultDto<GlobalVariableDto>(count, result);
        }

        public async Task<GlobalVariableDto> UpdateAsync(Guid id, GlobalVariableCreateOrUpdateDto input)
        {
            var entity = await _globalVariableRepository.GetAsync(id);

            entity.Value = input.Value;
            entity.IsSecret = input.IsSecret;

            await _globalVariableRepository.UpdateAsync(entity);

            return ObjectMapper.Map<GlobalVariable, GlobalVariableDto>(entity);
        }
    }
}
