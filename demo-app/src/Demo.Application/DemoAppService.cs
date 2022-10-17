using System;
using System.Collections.Generic;
using System.Text;
using Demo.Localization;
using Volo.Abp.Application.Services;

namespace Demo;

/* Inherit your application services from this class.
 */
public abstract class DemoAppService : ApplicationService
{
    protected DemoAppService()
    {
        LocalizationResource = typeof(DemoResource);
    }
}
