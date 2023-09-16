using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;

namespace Passingwind.Abp.ElsaModule.Common;

public interface IWorkflowImporter : ITransientDependency
{
    Task<WorkflowDefinition> ImportAsync(string jsonContent, bool overwrite = false, CancellationToken cancellationToken = default);
}
