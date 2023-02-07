using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Elsa;
using Elsa.Models;
using Elsa.Providers.WorkflowStorage;
using Elsa.Services;
using Elsa.Services.Models;
using Elsa.Services.WorkflowStorage;
using MediatR;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Passingwind.Abp.ElsaModule.Scripting.CSharp.Messages;
using Volo.Abp.Json;

namespace Passingwind.Abp.ElsaModule.Scripting.CSharp.Handlers;

public class ConfigureCSharpEngine : INotificationHandler<CSharpExpressionEvaluationNotification>
{
    private readonly IOptions<CSharpScriptOptions> _options;
    private readonly IJsonSerializer _jsonSerializer;
    private readonly IActivityTypeService _activityTypeService;
    private readonly IWorkflowStorageService _workflowStorageService;

    public ConfigureCSharpEngine(IOptions<CSharpScriptOptions> options, IJsonSerializer jsonSerializer, IActivityTypeService activityTypeService, IWorkflowStorageService workflowStorageService)
    {
        _options = options;
        _jsonSerializer = jsonSerializer;
        _activityTypeService = activityTypeService;
        _workflowStorageService = workflowStorageService;
    }

    public async Task Handle(CSharpExpressionEvaluationNotification notification, CancellationToken cancellationToken)
    {
        await ConfigEngineGlobalAsync(notification.Context.EvaluationGlobal, notification.ActivityExecutionContext, cancellationToken);
    }

    private async Task ConfigEngineGlobalAsync(CSharpEvaluationGlobal global, ActivityExecutionContext context, CancellationToken cancellationToken)
    {
        // Global variables.
        global.Context.Input = context.Input;
        global.Context.DefinitionId = context.WorkflowExecutionContext.WorkflowInstance.DefinitionId;
        global.Context.DefinitionVersion = context.WorkflowExecutionContext.WorkflowInstance.Version;
        global.Context.CorrelationId = context.WorkflowExecutionContext.WorkflowInstance.CorrelationId;
        global.Context.CurrentCulture = CultureInfo.InvariantCulture;
        global.Context.ActivityExecutionContext = context;
        global.Context.WorkflowExecutionContext = context.WorkflowExecutionContext;
        global.Context.WorkflowInstance = context.WorkflowExecutionContext.WorkflowInstance;
        global.Context.WorkflowContext = context.GetWorkflowContext();

        // Global functions.
        global.Context.GetTransientVariable = (Func<string, object>)((string name) => context.GetTransientVariable(name));
        global.Context.SetTransientVariable = (Action<string, object>)((string name, object value) => context.SetTransientVariable(name, value));
        global.Context.GetVariable = (Func<string, object>)((string name) => context.GetVariable(name));
        global.Context.SetVariable = (Action<string, object>)((string name, object value) => context.SetVariable(name, value));
        global.Context.HasVariable = (Func<string, bool>)((string name) => context.HasVariable(name));
        global.Context.PurgeVariables = (Action)(() => context.PurgeVariables());
        //global.Context.JsonEncode = (Func<object, string>)((object source) => _jsonSerializer.Serialize(source));
        //global.Context.JsonDecode = (Func<string, object>)((string json) => _jsonSerializer.Deserialize<object>(json));
        global.Context.JsonEncode = (Func<object, string>)((object source) => JsonConvert.SerializeObject(source));
        //global.Context.JsonDecode = (Func<string, object>)((string json) => JsonConvert.DeserializeObject<object>(json));
        global.Context.JsonDecode = (Func<string, dynamic>)((string json) => JsonConvert.DeserializeObject<dynamic>(json));
        global.Context.JsonDecodeToType = (Func<string, object, dynamic>)((string json, dynamic obj) => JsonConvert.DeserializeAnonymousType<dynamic>(json, obj));
        global.Context.GetWorkflowDefinitionIdByName = (Func<string, string>)(name => GetWorkflowDefinitionIdByName(context, name));
        global.Context.getWorkflowDefinitionIdByTag = (Func<string, string>)(tag => GetWorkflowDefinitionIdByTag(context, tag));
        global.Context.GetActivity = (Func<string, object>)(idOrName => GetActivityModel(context, idOrName));
        global.Context.GetActivityId = (Func<string, string>)(activityName => GetActivityId(context, activityName));

        // Workflow variables.
        var variables = context.WorkflowExecutionContext.GetMergedVariables();

        foreach (var variable in variables.Data)
            global.AddVariable(variable.Key, variable.Value);

        // activity output
        await AddActivityOutputAsync(global, context, cancellationToken);

    }

    private async Task AddActivityOutputAsync(CSharpEvaluationGlobal global, ActivityExecutionContext activityExecutionContext, CancellationToken cancellationToken)
    {
        var workflowExecutionContext = activityExecutionContext.WorkflowExecutionContext;
        var workflowInstance = activityExecutionContext.WorkflowInstance;
        var workflowBlueprint = workflowExecutionContext.WorkflowBlueprint;
        var activities = new Dictionary<string, Dictionary<string, object>>();

        foreach (var activity in workflowBlueprint.Activities.Where(x => !string.IsNullOrWhiteSpace(x.Name)))
        {
            var activityType = await _activityTypeService.GetActivityTypeAsync(activity.Type, cancellationToken);
            var activityDescriptor = await _activityTypeService.DescribeActivityType(activityType, cancellationToken);
            var outputProperties = activityDescriptor.OutputProperties.Where(x => x.IsBrowsable is true or null);
            var storageProviderLookup = activity.PropertyStorageProviders;
            var activityModel = new Dictionary<string, object>();
            var storageContext = new WorkflowStorageContext(workflowInstance, activity.Id);

            foreach (var property in outputProperties)
            {
                var propertyName = property.Name;
                var storageProviderName = storageProviderLookup.GetItem(propertyName) ?? property.DefaultWorkflowStorageProvider;

                activityModel[propertyName] = (Func<object>)(() => _workflowStorageService.LoadAsync(storageProviderName, storageContext, propertyName, cancellationToken).Result);
            }

            activities[activity.Name!] = activityModel;
        }

        global.Context.Activities = activities;

    }


    private string GetWorkflowDefinitionIdByTag(ActivityExecutionContext activityExecutionContext, string tag)
    {
        var workflowRegistry = activityExecutionContext.GetService<IWorkflowRegistry>();
        var workflowBlueprint = workflowRegistry.FindByTagAsync(tag, VersionOptions.Published).Result;
        return workflowBlueprint?.Id;
    }

    private string GetWorkflowDefinitionIdByName(ActivityExecutionContext activityExecutionContext, string name)
    {
        var workflowRegistry = activityExecutionContext.GetService<IWorkflowRegistry>();
        var workflowBlueprint = workflowRegistry.FindByNameAsync(name, VersionOptions.Published).Result;
        return workflowBlueprint?.Id;
    }

    private string GetActivityId(ActivityExecutionContext context, string activityName)
    {
        var workflowExecutionContext = context.WorkflowExecutionContext;
        var activity = workflowExecutionContext.GetActivityBlueprintByName(activityName);
        return activity?.Id;
    }

    private object GetActivityModel(ActivityExecutionContext context, string idOrName)
    {
        var workflowExecutionContext = context.WorkflowExecutionContext;
        var activity = workflowExecutionContext.GetActivityBlueprintByName(idOrName) ?? workflowExecutionContext.GetActivityBlueprintById(idOrName);
        return activity == null ? null : workflowExecutionContext.WorkflowInstance.ActivityData[activity.Id];
    }
}
