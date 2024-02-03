using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Elsa.Design;
using Elsa.Metadata;
using Elsa.Scripting.JavaScript.Services;
using Elsa.Services;
using Elsa.Services.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using Passingwind.Abp.ElsaModule.Common;
using Passingwind.Abp.ElsaModule.CSharp;
using Passingwind.Abp.ElsaModule.Permissions;
using Passingwind.Abp.ElsaModule.Stores;
using Volo.Abp.Auditing;
using Volo.Abp.Content;

namespace Passingwind.Abp.ElsaModule.Workflow;

[DisableAuditing]
[Authorize(Policy = ElsaModulePermissions.Definitions.CreateOrUpdateOrPublish)]
public class DesignerAppService : ElsaModuleAppService, IDesignerAppService
{
    private readonly IActivityTypeService _activityTypeService;
    private readonly ITypeScriptDefinitionService _typeScriptDefinitionService;
    private readonly IStoreMapper _storeMapper;
    private readonly IWorkflowDefinitionVersionRepository _workflowDefinitionVersionRepository;
    private readonly IWorkflowDefinitionRepository _workflowDefinitionRepository;
    private readonly IWorkflowCSharpEditorService _workflowCSharpEditorService;

    public DesignerAppService(
        IActivityTypeService activityTypeService,
        ITypeScriptDefinitionService typeScriptDefinitionService,
        IStoreMapper storeMapper,
        IWorkflowDefinitionVersionRepository workflowDefinitionVersionRepository,
        IWorkflowDefinitionRepository workflowDefinitionRepository,
        IWorkflowCSharpEditorService workflowCSharpEditorService)
    {
        _activityTypeService = activityTypeService;
        _typeScriptDefinitionService = typeScriptDefinitionService;
        _storeMapper = storeMapper;
        _workflowDefinitionVersionRepository = workflowDefinitionVersionRepository;
        _workflowDefinitionRepository = workflowDefinitionRepository;
        _workflowCSharpEditorService = workflowCSharpEditorService;
    }

    public async Task<ActivityTypeDescriptorListResultDto> GetActivityTypesAsync()
    {
        var activityTypes = await _activityTypeService.GetActivityTypesAsync();

        var tasks = activityTypes.Select(async x => await DescribeActivityAsync(x)).ToArray();

        var descriptors = await Task.WhenAll(tasks);

        var categories = descriptors.Select(x => x.Category).Distinct().OrderBy(x => x).ToArray();

        return new ActivityTypeDescriptorListResultDto()
        {
            Categories = categories,
            Items = ObjectMapper.Map<List<ActivityDescriptor>, List<ActivityTypeDescriptorDto>>(descriptors.ToList())
        };

        async Task<ActivityDescriptor> DescribeActivityAsync(ActivityType activityType, CancellationToken cancellationToken = default)
        {
            var activityDescriptor = await _activityTypeService.DescribeActivityType(activityType, cancellationToken);

            // Filter out any non-browsable properties.
            activityDescriptor.InputProperties = activityDescriptor.InputProperties.Where(x => x.IsBrowsable is true or null).ToArray();
            activityDescriptor.OutputProperties = activityDescriptor.OutputProperties.Where(x => x.IsBrowsable is true or null).ToArray();

            return activityDescriptor;
        }
    }

    public async Task<RuntimeSelectListResultDto> GetRuntimeSelectListItems(RuntimeSelectListContextDto input)
    {
        var type = Type.GetType(input.ProviderTypeName);

        if (type == null)
            return new RuntimeSelectListResultDto();

        var provider = LazyServiceProvider.LazyGetService(type);

        if (provider == null && type == null)
        {
            return new RuntimeSelectListResultDto();
        }

        var context = input.Context;

        if (provider is IRuntimeSelectListProvider newProvider)
        {
            var selectList = await newProvider.GetSelectListAsync(context, default);

            return new RuntimeSelectListResultDto(selectList.Items, selectList.IsFlagsEnum);
        }

#pragma warning disable CS0618
        var items = await ((IRuntimeSelectListItemsProvider)provider).GetItemsAsync(context, default);
#pragma warning restore CS0618

        return new RuntimeSelectListResultDto(items.ToArray());
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

        try
        {
            var result = await _workflowCSharpEditorService.GetCompletionAsync(workflowDefinition, input.Id, input.Text, input.Position);

            return new WorkflowDesignerCSharpLanguageCompletionProviderResultDto
            {
                Items = result?.Items ?? new List<WorkflowCSharpEditorCompletionItem>(),
            };
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Get csharp language completions failed.");
            return new WorkflowDesignerCSharpLanguageCompletionProviderResultDto();
        }
    }

    public async Task<WorkflowDesignerCSharpLanguageHoverProviderResultDto> CSharpLanguageHoverProviderAsync(Guid id, WorkflowDesignerCSharpLanguageHoverProviderRequestDto input)
    {
        var version = await _workflowDefinitionVersionRepository.GetLatestAsync(id);
        var definition = await _workflowDefinitionRepository.GetAsync(version.DefinitionId);

        var workflowDefinition = _storeMapper.MapToModel(version, definition);

        try
        {
            var result = await _workflowCSharpEditorService.GetHoverInfoAsync(workflowDefinition, input.Id, input.Text, input.Position);

            if (result == null)
            {
                return null;
            }

            return new WorkflowDesignerCSharpLanguageHoverProviderResultDto
            {
                Information = result.Information,
                OffsetFrom = result.OffsetFrom,
                OffsetTo = result.OffsetTo,
            };
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Get csharp language hover info failed.");
            return null;
        }
    }

    public async Task<WorkflowDesignerCSharpLanguageSignatureProviderResultDto> CSharpLanguageSignatureProviderAsync(Guid id, WorkflowDesignerCSharpLanguageSignatureProviderRequestDto input)
    {
        var version = await _workflowDefinitionVersionRepository.GetLatestAsync(id);
        var definition = await _workflowDefinitionRepository.GetAsync(version.DefinitionId);

        var workflowDefinition = _storeMapper.MapToModel(version, definition);

        try
        {
            var result = await _workflowCSharpEditorService.GetSignaturesAsync(workflowDefinition, input.Id, input.Text, input.Position);

            if (result == null)
                return null;

            return new WorkflowDesignerCSharpLanguageSignatureProviderResultDto
            {
                Signatures = result.Signatures,
                ActiveParameter = result.ActiveParameter,
                ActiveSignature = result.ActiveSignature,
            };
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Get csharp language signature failed.");
            return null;
        }
    }

    public async Task<WorkflowDesignerCSharpLanguageAnalysisResultDto> CSharpLanguageCodeAnalysisAsync(Guid id, WorkflowDesignerCSharpLanguageAnalysisRequestDto input)
    {
        var version = await _workflowDefinitionVersionRepository.GetLatestAsync(id);
        var definition = await _workflowDefinitionRepository.GetAsync(version.DefinitionId);

        var workflowDefinition = _storeMapper.MapToModel(version, definition);

        try
        {
            var result = await _workflowCSharpEditorService.GetCodeAnalysisAsync(workflowDefinition, input.Id, input.Text);

            return new WorkflowDesignerCSharpLanguageAnalysisResultDto
            {
                Items = result?.Items ?? new List<WorkflowCSharpEditorCodeAnalysis>(),
            };
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Get csharp language analysis failed.");
            return new WorkflowDesignerCSharpLanguageAnalysisResultDto();
        }
    }

    public async Task<WorkflowDesignerCSharpLanguageFormatterResult> CSharpLanguageCodeFormatterAsync(WorkflowDesignerCSharpLanguageFormatterRequestDto input)
    {
        var result = await _workflowCSharpEditorService.CodeFormatterAsync(input.Id, input.Text);

        return new WorkflowDesignerCSharpLanguageFormatterResult()
        {
            Success = result != null,
            Code = result?.Text,
        };
    }
}
