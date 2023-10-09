using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace Passingwind.Abp.ElsaModule.GlobalCodes;

/// <summary>
///  Application service contracts for GlobalCode
/// </summary>
public interface IGlobalCodeAppService : IApplicationService
{
    /// <summary>
    ///  Get GlobalCode list by paged params
    /// </summary>
    /// <param name="input"></param>
    Task<PagedResultDto<GlobalCodeDto>> GetListAsync(GlobalCodeListRequestDto input);

    /// <summary>
    ///  Get an GlobalCode
    /// </summary>
    /// <param name="id"></param>
    Task<GlobalCodeDto> GetAsync(Guid id);

    Task<GlobalCodeDto> GetByVersionAsync(Guid id, int version);

    Task<ListResultDto<int>> GetVersionsAsync(Guid id);

    Task DeleteByVersionAsync(Guid id, int version);

    /// <summary>
    ///  Create GlobalCode
    /// </summary>
    /// <param name="input"></param>
    Task<GlobalCodeDto> CreateAsync(GlobalCodeCreateOrUpdateDto input);

    /// <summary>
    ///  Update GlobalCode by id
    /// </summary>
    /// <param name="id"></param>
    /// <param name="input"></param>
    Task<GlobalCodeDto> UpdateAsync(Guid id, GlobalCodeCreateOrUpdateDto input);

    /// <summary>
    ///  Delete GlobalCode by id
    /// </summary>
    /// <param name="id"></param>
    Task DeleteAsync(Guid id);
}
