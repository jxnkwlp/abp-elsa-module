using System.Collections.Generic;

namespace Passingwind.WorkflowApp.Web.Controllers;

public class AccountResultDto
{
    public bool EnableLocalLogin { get; set; }

    public IEnumerable<ExternalProviderDto> ExternalProviders { get; set; }

}
