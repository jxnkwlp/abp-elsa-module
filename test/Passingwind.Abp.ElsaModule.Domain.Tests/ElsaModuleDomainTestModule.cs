using Passingwind.Abp.ElsaModule.EntityFrameworkCore;
using Volo.Abp.Modularity;

namespace Passingwind.Abp.ElsaModule;

/* Domain tests are configured to use the EF Core provider.
 * You can switch to MongoDB, however your domain tests should be
 * database independent anyway.
 */
[DependsOn(
    typeof(ElsaModuleEntityFrameworkCoreTestModule)
    )]
public class ElsaModuleDomainTestModule : AbpModule
{
}
