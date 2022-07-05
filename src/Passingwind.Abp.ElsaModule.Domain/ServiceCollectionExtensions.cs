using Elsa.Options;
using Microsoft.Extensions.DependencyInjection;
using Passingwind.Abp.ElsaModule.Stores;

namespace Passingwind.Abp.ElsaModule
{
    public static class ServiceCollectionExtensions
    {
        public static ElsaOptionsBuilder UseStore(this ElsaOptionsBuilder builder)
        {
            return builder
                       .UseTriggerStore(sp => sp.GetRequiredService<TriggerStore>())
                       .UseBookmarkStore(sp => sp.GetRequiredService<BookmarkStore>())
                       .UseWorkflowDefinitionStore(sp => sp.GetRequiredService<WorkflowDefinitionStore>())
                       .UseWorkflowExecutionLogStore(sp => sp.GetRequiredService<WorkflowExecutionLogStore>())
                       .UseWorkflowInstanceStore(sp => sp.GetRequiredService<WorkflowInstanceStore>())
                       ;
        }

    }
}
