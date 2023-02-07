using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Passingwind.Abp.ElsaModule.Monacos.Providers;

namespace Passingwind.Abp.ElsaModule.Workflow;
public class WorkflowDesignerCSharpLanguageAnalysisResultDto
{
    public List<MonacoCodeAnalysisItem> Items { get; set; }
}
