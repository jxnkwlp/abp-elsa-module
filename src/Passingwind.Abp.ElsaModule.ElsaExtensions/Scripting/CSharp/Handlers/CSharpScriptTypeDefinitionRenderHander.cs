using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using Elsa.Models;
using Elsa.Options;
using Elsa.Services.Models;
using MediatR;
using NodaTime;
using Passingwind.Abp.ElsaModule.Scripting.CSharp.Messages;

namespace Passingwind.Abp.ElsaModule.Scripting.CSharp.Handlers;

public class CSharpScriptTypeDefinitionRenderHander : INotificationHandler<CSharpTypeDefinitionNotification>
{
    public Task Handle(CSharpTypeDefinitionNotification notification, CancellationToken cancellationToken)
    {
        var source = notification.DefinitionSource;
        var reference = notification.Reference;

        // methods
        source.Append(@"
// properties
public static object Input => throw new System.NotImplementedException();
public static string DefinitionId => throw new System.NotImplementedException();
public static int DefinitionVersion => throw new System.NotImplementedException();
public static string CorrelationId => throw new System.NotImplementedException();
public static ").Append(typeof(CultureInfo).FullName).Append(@" CurrentCulture => throw new System.NotImplementedException();
public static ").Append(typeof(WorkflowInstance).FullName).Append(@" WorkflowInstance => throw new System.NotImplementedException();
public static ").Append(typeof(ActivityExecutionContext).FullName).Append(@" ActivityExecutionContext => throw new System.NotImplementedException();
public static ").Append(typeof(WorkflowExecutionContext).FullName).AppendLine(@" WorkflowExecutionContext => throw new System.NotImplementedException();
public static object WorkflowContext => throw new System.NotImplementedException(); 

// Core functions
public static object? GetService(Type type) => throw new System.NotImplementedException(); 
public static object GetRequiredService(Type type) => throw new System.NotImplementedException(); 

// methods
public static void AddJournalData(string key, object? value) => throw new System.NotImplementedException(); 
public static object GetWorkflowContext() => throw new System.NotImplementedException(); 
public static object GetTransientVariable(string name) => throw new System.NotImplementedException(); 
public static void SetTransientVariable(string name, object value) => throw new System.NotImplementedException(); 
public static object GetVariable(string name) => throw new System.NotImplementedException(); 
public static void SetVariable(string name, object value) => throw new System.NotImplementedException(); 
public static void PurgeVariables() => throw new System.NotImplementedException(); 
public static string JsonEncode(object value) => throw new System.NotImplementedException(); 
public static dynamic JsonDecode(string value) => throw new System.NotImplementedException();
public static string GetWorkflowDefinitionIdByName(string name) => throw new System.NotImplementedException(); 
public static string GetWorkflowDefinitionIdByTag(string tag) => throw new System.NotImplementedException(); 
public static void AddEntry(string eventName, string message, object? data) => throw new System.NotImplementedException(); 

// activities
public static IDictionary<string, object> GetActivity(string name) => throw new System.NotImplementedException(); 
public static IDictionary<string, Dictionary<string, object>> ActivitityData => throw new System.NotImplementedException(); 

");

        // TODO Workflow variables.
        // ... 

        // assemblies
        reference.Assemblies.Add(typeof(WorkflowInstance).Assembly);
        reference.Assemblies.Add(typeof(ElsaOptions).Assembly);

        // imports
        reference.Imports.Add("Elsa.Models");
        reference.Imports.Add("Elsa.Services.Models");

        // NodaTime  
        reference.Assemblies.Add(typeof(Instant).Assembly);
        reference.Imports.Add("NodaTime");

        return Task.CompletedTask;
    }
}
