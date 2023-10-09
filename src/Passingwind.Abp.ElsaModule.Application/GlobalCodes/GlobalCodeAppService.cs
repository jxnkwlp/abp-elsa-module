using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Entities;
using Volo.Abp.ObjectExtending;

namespace Passingwind.Abp.ElsaModule.GlobalCodes;

/// <summary>
///  Application service for GlobalCode
/// </summary>
public class GlobalCodeAppService : ElsaModuleAppService, IGlobalCodeAppService
{
    private readonly GlobalCodeManager _globalCodeManager;
    private readonly IGlobalCodeRepository _globalCodeRepository;
    private readonly IGlobalCodeContentRepository _globalCodeContentRepository;
    private readonly IGlobalCodeVersionRepository _globalCodeVersionRepository;

    public GlobalCodeAppService(GlobalCodeManager globalCodeManager, IGlobalCodeRepository globalCodeRepository, IGlobalCodeContentRepository globalCodeContentRepository, IGlobalCodeVersionRepository globalCodeVersionRepository)
    {
        _globalCodeManager = globalCodeManager;
        _globalCodeRepository = globalCodeRepository;
        _globalCodeContentRepository = globalCodeContentRepository;
        _globalCodeVersionRepository = globalCodeVersionRepository;
    }

    /// <inheritdoc/>
    public virtual async Task<PagedResultDto<GlobalCodeDto>> GetListAsync(GlobalCodeListRequestDto input)
    {
        var count = await _globalCodeRepository.GetCountAsync(filter: input.Filter);
        var list = await _globalCodeRepository.GetPagedListAsync(input.SkipCount, input.MaxResultCount, filter: input.Filter, sorting: input.Sorting);

        return new PagedResultDto<GlobalCodeDto>()
        {
            Items = ObjectMapper.Map<List<GlobalCode>, List<GlobalCodeDto>>(list),
            TotalCount = count,
        };
    }

    public virtual async Task<ListResultDto<int>> GetVersionsAsync(Guid id)
    {
        var entity = await _globalCodeRepository.GetAsync(id);
        var versions = await _globalCodeVersionRepository.GetListAsync(id);

        return new ListResultDto<int>(versions.ConvertAll(x => x.Version));
    }

    /// <inheritdoc/>
    public virtual async Task<GlobalCodeDto> GetAsync(Guid id)
    {
        var entity = await _globalCodeRepository.GetAsync(id);

        var contentEntity = await _globalCodeContentRepository.FindByVersionAsync(id, entity.LatestVersion);

        var dto = ObjectMapper.Map<GlobalCode, GlobalCodeDto>(entity);

        dto.Content = contentEntity?.Content;

        return dto;
    }

    /// <inheritdoc/>
    public virtual async Task<GlobalCodeDto> GetByVersionAsync(Guid id, int version)
    {
        var entity = await _globalCodeRepository.GetAsync(id);

        var contentEntity = await _globalCodeContentRepository.FindByVersionAsync(id, version);

        if (contentEntity == null)
            throw new EntityNotFoundException();

        var dto = ObjectMapper.Map<GlobalCode, GlobalCodeDto>(entity);

        dto.PublishedVersion = version;
        dto.Content = contentEntity?.Content;

        return dto;
    }

    /// <inheritdoc/>
    public virtual async Task<GlobalCodeDto> CreateAsync(GlobalCodeCreateOrUpdateDto input)
    {
        if (await _globalCodeRepository.IsNameExistsAsync(input.Name))
        {
            throw new BusinessException(ElsaModuleErrorCodes.GlobalCodeNameExists);
        }

        var entity = new GlobalCode(GuidGenerator.Create(), input.Name, input.Type, CurrentTenant.Id)
        {
            Description = input.Description,
            Language = input.Language,
        };

        entity.SetLatestVersion(1);

        entity.SetPublishedVersion(1);

        input.MapExtraPropertiesTo(entity);

        await _globalCodeRepository.InsertAsync(entity);

        await _globalCodeManager.UpdateContentAsync(entity, input.Content);

        return ObjectMapper.Map<GlobalCode, GlobalCodeDto>(entity);
    }

    /// <inheritdoc/>
    public virtual async Task<GlobalCodeDto> UpdateAsync(Guid id, GlobalCodeCreateOrUpdateDto input)
    {
        var entity = await _globalCodeRepository.GetAsync(id);

        if (await _globalCodeRepository.IsNameExistsAsync(input.Name, new[] { id }))
        {
            throw new BusinessException(ElsaModuleErrorCodes.GlobalCodeNameExists);
        }

        entity.SetName(input.Name);
        entity.Description = input.Description;
        entity.Language = input.Language;
        entity.Type = input.Type;

        var version = entity.LatestVersion;

        if (input.Publish)
        {
            if (entity.LatestVersion == entity.PublishedVersion)
            {
                entity.SetLatestVersion(version + 1);
                entity.SetPublishedVersion(version + 1);
            }
            else
            {
                entity.SetPublishedVersion(version);
            }
        }
        else if (entity.LatestVersion == entity.PublishedVersion)
        {
            entity.SetLatestVersion(version + 1);
        }

        input.MapExtraPropertiesTo(entity);

        await _globalCodeRepository.UpdateAsync(entity);

        await _globalCodeManager.UpdateContentAsync(entity, input.Content);

        return ObjectMapper.Map<GlobalCode, GlobalCodeDto>(entity);
    }

    /// <inheritdoc/>
    public virtual async Task DeleteAsync(Guid id)
    {
        await _globalCodeManager.DeleteAsync(id);
    }

    public async Task DeleteByVersionAsync(Guid id, int version)
    {
        var entity = await _globalCodeRepository.GetAsync(id);

        await _globalCodeContentRepository.DeleteDirectAsync(x => x.GlobalCodeId == id && x.Version == version);
        await _globalCodeVersionRepository.DeleteDirectAsync(x => x.GlobalCodeId == id && x.Version == version);
    }
}
