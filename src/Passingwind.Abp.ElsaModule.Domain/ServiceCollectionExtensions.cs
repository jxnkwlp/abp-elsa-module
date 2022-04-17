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
                       .UseWorkflowBookmarkTriggerStore(sp => sp.GetRequiredService<TriggerStore>())
                       .UseWorkflowTriggerStore(sp => sp.GetRequiredService<BookmarkStore>())
                       .UseWorkflowDefinitionStore(sp => sp.GetRequiredService<WorkflowDefinitionStore>())
                       .UseWorkflowExecutionLogStore(sp => sp.GetRequiredService<WorkflowExecutionLogStore>())
                       .UseWorkflowInstanceStore(sp => sp.GetRequiredService<WorkflowInstanceStore>())
                       ;
        }

    }
}
