using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Elsa;
using Elsa.ActivityResults;
using Elsa.Attributes;
using Elsa.Expressions;
using Elsa.Services;
using Elsa.Services.Models;
using Volo.Abp.Caching;

namespace Passingwind.Abp.ElsaModule.Activities.Caching;

[Activity(
    Category = "Caching",
    DisplayName = "Get Cache",
    Outcomes = new[] { OutcomeNames.Done }
)]
public class GetCache : Activity
{
    [ActivityInput(
        Label = "Key",
        Hint = "",
        SupportedSyntaxes = new[] { SyntaxNames.Literal, SyntaxNames.JavaScript, SyntaxNames.Liquid },
        IsDesignerCritical = true)]
    [Required]
    public string Key { get; set; }

    [ActivityOutput]
    public string Value { get; set; }

    private readonly IDistributedCache<CacheActivityCacheItem> _distributedCache;

    public GetCache(IDistributedCache<CacheActivityCacheItem> distributedCache)
    {
        _distributedCache = distributedCache;
    }

    protected override async ValueTask<IActivityExecutionResult> OnExecuteAsync(ActivityExecutionContext context)
    {
        if (string.IsNullOrEmpty(Key))
            throw new ArgumentNullException(nameof(Key));

        var result = await _distributedCache.GetAsync(Key, token: context.CancellationToken);

        this.Value = result.Value;

        return Done();
    }
}
