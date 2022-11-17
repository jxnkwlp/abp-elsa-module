using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Elsa;
using Elsa.ActivityResults;
using Elsa.Attributes;
using Elsa.Expressions;
using Elsa.Services;
using Elsa.Services.Models;
using Microsoft.Extensions.Caching.Distributed;

namespace Passingwind.Abp.ElsaModule.Activities.Caching
{
    [Activity(
        Category = "Abp",
        DisplayName = "Remove Cache",
        Outcomes = new[] { OutcomeNames.Done }
    )]
    public class RemoveCache : Activity
    {
        [ActivityInput(
            Label = "Key",
            Hint = "",
            SupportedSyntaxes = new[] { SyntaxNames.Literal, SyntaxNames.JavaScript, SyntaxNames.Liquid },
            IsDesignerCritical = true)]
        [Required]
        public string Key { get; set; }


        private readonly IDistributedCache _distributedCache;

        public RemoveCache(IDistributedCache distributedCache)
        {
            _distributedCache = distributedCache;
        }

        public override async ValueTask<IActivityExecutionResult> ExecuteAsync(ActivityExecutionContext context)
        {
            if (string.IsNullOrEmpty(Key))
                throw new ArgumentNullException(nameof(Key));

            await _distributedCache.RemoveAsync(Key, context.CancellationToken);

            return Done();
        }
    }
}
