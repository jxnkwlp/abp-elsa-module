using System.Collections.Generic;
using Volo.Abp.Application.Dtos;

namespace Passingwind.Abp.ElsaModule.Scripting.CSharp.MonacoProviders;

public class TabCompletionResult : ListResultDto<TabCompletionResultItem>
{
    public TabCompletionResult(IReadOnlyList<TabCompletionResultItem> items) : base(items)
    {
    }
}

public class TabCompletionResultItem
{
    public virtual string Suggestion { get; set; }

    public virtual string Description { get; set; }
}
