using Passingwind.WorkflowApp.Localization;
using Volo.Abp.AspNetCore.Mvc;

namespace Passingwind.WorkflowApp.Controllers;

/* Inherit your controllers from this class.
 */
public abstract class WorkflowAppController : AbpControllerBase
{
    protected WorkflowAppController()
    {
        LocalizationResource = typeof(WorkflowAppResource);
    }
}
