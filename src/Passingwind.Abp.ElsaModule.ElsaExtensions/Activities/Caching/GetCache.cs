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

        //[ActivityOutput]
        //public string Output => Value;

        private readonly IDistributedCache _distributedCache;

        public GetCache(IDistributedCache distributedCache)
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

            Value = await _distributedCache.GetStringAsync(Key);

            return Done();
        }

    }
}
