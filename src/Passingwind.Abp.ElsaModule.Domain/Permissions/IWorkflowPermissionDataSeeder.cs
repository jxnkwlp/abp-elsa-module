using System;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;

namespace Passingwind.Abp.ElsaModule.Permissions;

public interface IWorkflowPermissionDataSeeder : ITransientDependency
{
    Task SendAsync(Guid? tenantId = null);
}
