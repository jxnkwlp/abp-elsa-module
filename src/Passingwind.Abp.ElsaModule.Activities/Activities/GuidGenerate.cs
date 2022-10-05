using System;
using Elsa;
using Elsa.ActivityResults;
using Elsa.Attributes;
using Elsa.Services;
using Volo.Abp.Guids;

namespace Passingwind.Abp.ElsaModule.Activities
{
    [Action(
        Category = "Abp",
        DisplayName = "Guid Generate",
        Outcomes = new[] { OutcomeNames.Done }
    )]
    public class GuidGenerate : Activity
    {
        [ActivityOutput(Hint = "The result of GUID.")]
        public string Output { get; set; }

        private readonly IGuidGenerator _guidGenerator;

        public GuidGenerate(IGuidGenerator guidGenerator)
        {
            _guidGenerator = guidGenerator;
        }

        protected override IActivityExecutionResult OnExecute()
        {
            Output = _guidGenerator.Create().ToString();

            return Done();
        }
    }
}
