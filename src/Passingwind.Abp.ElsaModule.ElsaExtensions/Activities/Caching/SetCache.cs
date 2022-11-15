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
        DisplayName = "Set Cache",
        Outcomes = new[] { OutcomeNames.Done }
    )]
    public class SetCache : Activity
    {
        [ActivityInput(
            Label = "Key",
            Hint = "",
            SupportedSyntaxes = new[] { SyntaxNames.Literal, SyntaxNames.JavaScript, SyntaxNames.Liquid },
            IsDesignerCritical = true)]
        [Required]
        public string Key { get; set; }

        [ActivityInput(
            Label = "Value",
            Hint = "",
            SupportedSyntaxes = new[] { SyntaxNames.Literal, SyntaxNames.JavaScript, SyntaxNames.Liquid })]
        public string Value { get; set; }

        [ActivityInput(
            Label = "Expiration",
            Hint = "How many seconds before the value expires",
            SupportedSyntaxes = new[] { SyntaxNames.Literal, SyntaxNames.JavaScript })]
        public double? Expiration { get; set; }

        private readonly IDistributedCache _distributedCache;

        public SetCache(IDistributedCache distributedCache)
        {
            _distributedCache = distributedCache;
        }

        protected override async ValueTask<IActivityExecutionResult> OnExecuteAsync(ActivityExecutionContext context)
        {
            return await ExecuteAsync();
        }

        private async ValueTask<IActivityExecutionResult> ExecuteAsync()
        {
            if (string.IsNullOrEmpty(Key))
                throw new ArgumentNullException(nameof(Key));

            var options = new DistributedCacheEntryOptions();

            if (Expiration.HasValue)
            {
                options.AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(Expiration.Value);
            }

            await _distributedCache.SetStringAsync(Key, Value ?? string.Empty, options);

            return Done();
        }

    }
}
