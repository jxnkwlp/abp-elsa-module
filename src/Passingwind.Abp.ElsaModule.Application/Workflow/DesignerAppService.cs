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
using Passingwind.Abp.ElsaModule.Monacos.Providers;
using Passingwind.Abp.ElsaModule.Stores;
using Volo.Abp.Auditing;
using Volo.Abp.Content;

namespace Passingwind.Abp.ElsaModule.Workflow;

[DisableAuditing]
[Authorize]
public class DesignerAppService : ElsaModuleAppService, IDesignerAppService
{
    private readonly IActivityTypeService _activityTypeService;
    private readonly ITypeScriptDefinitionService _typeScriptDefinitionService;
    private readonly IStoreMapper _storeMapper;
    private readonly IWorkflowDefinitionVersionRepository _workflowDefinitionVersionRepository;
    private readonly IWorkflowDefinitionRepository _workflowDefinitionRepository;
    private readonly ElsaOptions _elsaOptions;

    private readonly IMonacoCodeAnalysisProvider _monacoCodeAnalysisProvider;
    private readonly IMonacoCompletionProvider _monacoCompletionProvider;
    private readonly IMonacoHoverInfoProvider _monacoHoverInfoProvider;
    private readonly IMonacoSignatureProvider _monacoSignatureProvider;
    private readonly IMonacoCodeFormatterProvider _monacoCodeFormatterProvider;

    public DesignerAppService(IActivityTypeService activityTypeService,
                              ITypeScriptDefinitionService typeScriptDefinitionService,
                              IStoreMapper storeMapper,
                              IWorkflowDefinitionVersionRepository workflowDefinitionVersionRepository,
                              IWorkflowDefinitionRepository workflowDefinitionRepository,
                              ElsaOptions elsaOptions,
                              IMonacoCodeAnalysisProvider monacoCodeAnalysisProvider,
                              IMonacoCompletionProvider monacoCompletionProvider,
                              IMonacoHoverInfoProvider monacoHoverInfoProvider,
                              IMonacoSignatureProvider monacoSignatureProvider,
                              IMonacoCodeFormatterProvider monacoCodeFormatterProvider)
    {
        _activityTypeService = activityTypeService;
        _typeScriptDefinitionService = typeScriptDefinitionService;
        _storeMapper = storeMapper;
        _workflowDefinitionVersionRepository = workflowDefinitionVersionRepository;
        _workflowDefinitionRepository = workflowDefinitionRepository;
        _elsaOptions = elsaOptions;
        _monacoCodeAnalysisProvider = monacoCodeAnalysisProvider;
        _monacoCompletionProvider = monacoCompletionProvider;
        _monacoHoverInfoProvider = monacoHoverInfoProvider;
        _monacoSignatureProvider = monacoSignatureProvider;
        _monacoCodeFormatterProvider = monacoCodeFormatterProvider;
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

    public async Task<IRemoteStreamContent> GetJavaScriptTypeDefinitionAsync(Guid id)
    {
        var version = await _workflowDefinitionVersionRepository.GetLatestAsync(id);
        var definition = await _workflowDefinitionRepository.GetAsync(version.DefinitionId);

        var workflowDefinition = _storeMapper.MapToModel(version, definition);

        var typeDefinitions = await _typeScriptDefinitionService.GenerateTypeScriptDefinitionsAsync(workflowDefinition);

        var fileName = $"workflow-definition-{id}.d.ts";

        var data = Encoding.UTF8.GetBytes(typeDefinitions);

        return new RemoteStreamContent(new MemoryStream(data), fileName, "application/x-typescript");
    }

    public async Task<WorkflowDesignerCSharpLanguageCompletionProviderResultDto> CSharpLanguageCompletionProviderAsync(Guid id, WorkflowDesignerCSharpLanguageCompletionProviderRequestDto input)
    {
        var version = await _workflowDefinitionVersionRepository.GetLatestAsync(id);
        var definition = await _workflowDefinitionRepository.GetAsync(version.DefinitionId);

        var workflowDefinition = _storeMapper.MapToModel(version, definition);

        var result = await _monacoCompletionProvider.HandleAsync(new MonacoCompletionRequest(workflowDefinition, input.SessionId ?? workflowDefinition.Id, input.Code, input.Position));

        return new WorkflowDesignerCSharpLanguageCompletionProviderResultDto
        {
            Items = result.Items,
        };
    }

    public async Task<WorkflowDesignerCSharpLanguageHoverProviderResultDto> CSharpLanguageHoverProviderAsync(Guid id, WorkflowDesignerCSharpLanguageHoverProviderRequestDto input)
    {
        var version = await _workflowDefinitionVersionRepository.GetLatestAsync(id);
        var definition = await _workflowDefinitionRepository.GetAsync(version.DefinitionId);

        var workflowDefinition = _storeMapper.MapToModel(version, definition);

        var result = await _monacoHoverInfoProvider.HandleAsync(new MonacoHoverInfoRequest(workflowDefinition, input.SessionId ?? workflowDefinition.Id, input.Code, input.Position));

        if (result == null)
            return null;

        return new WorkflowDesignerCSharpLanguageHoverProviderResultDto
        {
            Information = result.Information,
            OffsetFrom = result.OffsetFrom,
            OffsetTo = result.OffsetTo,
        };
    }

    public async Task<WorkflowDesignerCSharpLanguageSignatureProviderResultDto> CSharpLanguageSignatureProviderAsync(Guid id, WorkflowDesignerCSharpLanguageSignatureProviderRequestDto input)
    {
        var version = await _workflowDefinitionVersionRepository.GetLatestAsync(id);
        var definition = await _workflowDefinitionRepository.GetAsync(version.DefinitionId);

        var workflowDefinition = _storeMapper.MapToModel(version, definition);

        var result = await _monacoSignatureProvider.HandleAsync(new MonacoSignatureRequest(workflowDefinition, input.SessionId ?? workflowDefinition.Id, input.Code, input.Position));

        if (result == null)
            return null;

        return new WorkflowDesignerCSharpLanguageSignatureProviderResultDto
        {
            Signatures = result.Signatures,
            ActiveParameter = result.ActiveParameter,
            ActiveSignature = result.ActiveSignature,
        };
    }

    public async Task<WorkflowDesignerCSharpLanguageAnalysisResultDto> CSharpLanguageCodeAnalysisAsync(Guid id, WorkflowDesignerCSharpLanguageAnalysisRequestDto input)
    {
        var version = await _workflowDefinitionVersionRepository.GetLatestAsync(id);
        var definition = await _workflowDefinitionRepository.GetAsync(version.DefinitionId);

        var workflowDefinition = _storeMapper.MapToModel(version, definition);

        var result = await _monacoCodeAnalysisProvider.HandleAsync(new MonacoCodeAnalysisRequest(workflowDefinition, input.SessionId ?? workflowDefinition.Id, input.Code));

        return new WorkflowDesignerCSharpLanguageAnalysisResultDto
        {
            Items = result.Items,
        };
    }

    public async Task<WorkflowDesignerCSharpLanguageFormatterResult> CSharpLanguageCodeFormatterAsync(WorkflowDesignerCSharpLanguageFormatterRequestDto input)
    {
        var result = await _monacoCodeFormatterProvider.HandleAsync(new MonacoCodeFormatterRequest { Code = input.Code });

        return new WorkflowDesignerCSharpLanguageFormatterResult()
        {
            Code = result.Code,
        };
    }
}
