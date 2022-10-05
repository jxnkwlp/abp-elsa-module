using Elsa;
using Elsa.Attributes;
using Elsa.Services;

namespace Passingwind.Abp.ElsaModule.Activities
{
    [Activity(
        Category = "Abp",
        DisplayName = "Send Email V2",
        Description = "",
        Outcomes = new[] { OutcomeNames.Done }
    )]
    public class SendEmailV2 //: Activity
    {

    }
}
