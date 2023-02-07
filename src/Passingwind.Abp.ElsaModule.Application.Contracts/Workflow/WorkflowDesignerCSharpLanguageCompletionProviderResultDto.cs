using System.Collections.Generic;
using Passingwind.Abp.ElsaModule.Monacos.Providers;

namespace Passingwind.Abp.ElsaModule.Workflow;
public class WorkflowDesignerCSharpLanguageCompletionProviderResultDto
{
    public List<MonacoCompletionItem> Items { get; set; }
}
