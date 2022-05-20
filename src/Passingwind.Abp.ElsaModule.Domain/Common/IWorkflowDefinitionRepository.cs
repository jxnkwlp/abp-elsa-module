using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace Passingwind.Abp.ElsaModule.Common
{
    public interface IWorkflowDefinitionRepository : IRepository<WorkflowDefinition, Guid>
    {
        Task<Guid[]> GetIdsByNamesAsync(IEnumerable<string> names, CancellationToken cancellationToken = default);
        Task<Guid> GetIdByNameAsync(string name, CancellationToken cancellationToken = default);
        Task<Guid[]> GetIdsByTagsAsync(IEnumerable<string> tags, CancellationToken cancellationToken = default);
        Task<Guid> GetIdByTagAsync(string tags, CancellationToken cancellationToken = default);
    }
}
