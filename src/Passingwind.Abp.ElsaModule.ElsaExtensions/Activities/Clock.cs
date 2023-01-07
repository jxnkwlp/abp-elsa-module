using System;
using Elsa;
using Elsa.ActivityResults;
using Elsa.Attributes;
using Elsa.Services;
using Elsa.Services.Models;
using Volo.Abp.Timing;

namespace Passingwind.Abp.ElsaModule.Activities;

[Activity(
    Category = "Abp",
    DisplayName = "Clock",
    Outcomes = new[] { OutcomeNames.Done }
)]
public class Clock : Activity
{
    [ActivityOutput]
    public ClockOutputModel Output { get; set; }

    private readonly IClock _clock;

    public Clock(IClock clock)
    {
        _clock = clock;
    }

    protected override IActivityExecutionResult OnExecute(ActivityExecutionContext context)
    {
        return Execute();
    }

    private IActivityExecutionResult Execute()
    {
        Output = new ClockOutputModel
        {
            Now = _clock.Now,
            Kind = _clock.Kind,
        };

        return Done();
    }

}

public class ClockOutputModel
{
    public DateTime Now { get; set; }
    public DateTimeKind Kind { get; set; }
}
