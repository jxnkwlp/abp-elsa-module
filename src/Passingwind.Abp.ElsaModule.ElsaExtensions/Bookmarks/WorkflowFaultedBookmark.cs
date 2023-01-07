using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Elsa.Services;
using Passingwind.Abp.ElsaModule.Activities.Workflows;

namespace Passingwind.Abp.ElsaModule.Bookmarks;

public record WorkflowFaultedBookmark : IBookmark
{
    public WorkflowFaultedBookmark(string definitionName, int? definitionVersion = null)
    {
        DefinitionName = definitionName;
        DefinitionVersion = definitionVersion;
    }

    public string DefinitionName { get; set; }
    public int? DefinitionVersion { get; set; }
}

public class WorkflowFaultedBookmarkProvider : BookmarkProvider<WorkflowFaultedBookmark, WorkflowFaulted>
{
    public override async ValueTask<IEnumerable<BookmarkResult>> GetBookmarksAsync(BookmarkProviderContext<WorkflowFaulted> context, CancellationToken cancellationToken)
    {
        var name = await context.ReadActivityPropertyAsync(x => x.DefinitionName);
        var version = await context.ReadActivityPropertyAsync(x => x.DefinitionVersion);

        var bookmark = new WorkflowFaultedBookmark(name, version);

        return new[] { Result(bookmark) };
    }
}
