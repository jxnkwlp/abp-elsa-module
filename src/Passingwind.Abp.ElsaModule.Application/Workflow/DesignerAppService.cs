using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Elsa.Design;
using Elsa.Metadata;
using Elsa.Options;
using Elsa.Scripting.JavaScript.Services;
using Elsa.Services;
using Microsoft.AspNetCore.Authorization;
using Passingwind.Abp.ElsaModule.Common;
using Passingwind.Abp.ElsaModule.Stores;
using Volo.Abp.Content;

namespace Passingwind.Abp.ElsaModule.Workflow;

[Authorize]
public class DesignerAppService : ElsaModuleAppService, IDesignerAppService
{
    private readonly IActivityTypeService _activityTypeService;
    private readonly ITypeScriptDefinitionService _typeScriptDefinitionService;
    private readonly IStoreMapper _storeMapper;
    private readonly IWorkflowDefinitionVersionRepository _workflowDefinitionVersionRepository;
    private readonly IWorkflowDefinitionRepository _workflowDefinitionRepository;
    private readonly ElsaOptions _elsaOptions;

    public DesignerAppService(IActivityTypeService activityTypeService, ITypeScriptDefinitionService typeScriptDefinitionService, IStoreMapper storeMapper, IWorkflowDefinitionVersionRepository workflowDefinitionVersionRepository, IWorkflowDefinitionRepository workflowDefinitionRepository, ElsaOptions elsaOptions)
    {
        _activityTypeService = activityTypeService;
        _typeScriptDefinitionService = typeScriptDefinitionService;
        _storeMapper = storeMapper;
        _workflowDefinitionVersionRepository = workflowDefinitionVersionRepository;
        _workflowDefinitionRepository = workflowDefinitionRepository;
        _elsaOptions = elsaOptions;
    }

    public async Task<ActivityTypeDescriptorListResultDto> GetActivityTypesAsync()
    {
        var activityTypes = await _activityTypeService.GetActivityTypesAsync();

        var tasks = activityTypes.Select(async x => await _activityTypeService.DescribeActivityType(x)).ToArray();

        var descriptors = await Task.WhenAll(tasks);

        var categories = descriptors.Select(x => x.Category).Distinct().OrderBy(x => x).ToArray();

        return new ActivityTypeDescriptorListResultDto()
        {
            Categories = categories,
            Items = ObjectMapper.Map<List<ActivityDescriptor>, List<ActivityTypeDescriptorDto>>(descriptors.ToList())
        };
    }

    public async Task<RuntimeSelectListResultDto> GetRuntimeSelectListItems(RuntimeSelectListContextDto input)
    {
        // LazyServiceProvider.LazyGetRequiredService()
        var type = Type.GetType(input.ProviderTypeName)!;
        var provider = LazyServiceProvider.LazyGetRequiredService(type);
        var context = input.Context;

        if (provider is IRuntimeSelectListProvider newProvider)
        {
            var selectList = await newProvider.GetSelectListAsync(context, default);

            return new RuntimeSelectListResultDto
            {
                SelectList = selectList,
            };
        }
#pragma warning disable CS0618
        var items = await ((IRuntimeSelectListItemsProvider)provider).GetItemsAsync(context, default);
#pragma warning restore CS0618

        return new RuntimeSelectListResultDto
        {
            SelectList = new SelectList(items.ToArray())
        };
    }

    public async Task<IRemoteStreamContent> GetScriptTypeDefinitionAsync(Guid id)
    {
        var version = await _workflowDefinitionVersionRepository.GetLatestAsync(id);
        var definition = await _workflowDefinitionRepository.GetAsync(version.DefinitionId);

        var workflowDefinition = _storeMapper.MapToModel(version, definition);

        var typeDefinitions = await _typeScriptDefinitionService.GenerateTypeScriptDefinitionsAsync(workflowDefinition);

        var fileName = $"workflow-definition-{id}.d.ts";

        var data = Encoding.UTF8.GetBytes(typeDefinitions);

        return new RemoteStreamContent(new MemoryStream(data), fileName, "application/x-typescript");
    }

}
