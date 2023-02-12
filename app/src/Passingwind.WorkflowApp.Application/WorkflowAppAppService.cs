using Passingwind.WorkflowApp.Localization;
using Volo.Abp.Application.Services;

namespace Passingwind.WorkflowApp;

/* Inherit your application services from this class.
 */
public abstract class WorkflowAppAppService : ApplicationService
{
    protected WorkflowAppAppService()
    {
        LocalizationResource = typeof(WorkflowAppResource);
    }
}
