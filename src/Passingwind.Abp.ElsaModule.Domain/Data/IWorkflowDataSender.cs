using System;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;

namespace Passingwind.Abp.ElsaModule.Data;

public interface IWorkflowDataSender : ITransientDependency
{
    Task SendAsync(Guid? tenantId = null, CancellationToken cancellationToken = default);
}
