using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Elsa.Models;
using Volo.Abp.Domain.Repositories;

namespace Passingwind.Abp.ElsaModule.Common;

public interface IWorkflowDefinitionRepository : IRepository<WorkflowDefinition, Guid>
{
    Task<Guid[]> GetIdsByNamesAsync(IEnumerable<string> names, CancellationToken cancellationToken = default);

    Task<Guid> GetIdByNameAsync(string name, CancellationToken cancellationToken = default);

    Task<Guid[]> GetIdsByTagsAsync(IEnumerable<string> tags, CancellationToken cancellationToken = default);

    Task<Guid> GetIdByTagAsync(string tags, CancellationToken cancellationToken = default);

    Task<long> CountAsync(string name = null, bool? isSingleton = null, bool? deleteCompletedInstances = null, int? publishedVersion = null, string channel = null, string tag = null, Guid? groupId = null, WorkflowPersistenceBehavior? workflowPersistenceBehavior = null, IEnumerable<Guid> filterIds = null, CancellationToken cancellationToken = default);

    Task<List<WorkflowDefinition>> GetListAsync(string name = null, bool? isSingleton = null, bool? deleteCompletedInstances = null, int? publishedVersion = null, string channel = null, string tag = null, Guid? groupId = null, WorkflowPersistenceBehavior? workflowPersistenceBehavior = null, IEnumerable<Guid> filterIds = null, CancellationToken cancellationToken = default);

    Task<List<WorkflowDefinition>> GetPagedListAsync(int skipCount, int maxResultCount, string name = null, bool? isSingleton = null, bool? deleteCompletedInstances = null, int? publishedVersion = null, string channel = null, string tag = null, Guid? groupId = null, WorkflowPersistenceBehavior? workflowPersistenceBehavior = null, IEnumerable<Guid> filterIds = null, string ordering = null, CancellationToken cancellationToken = default);

    Task UpdateGroupNameAsync(Guid groupId, string groupName, CancellationToken cancellationToken = default);
    Task RemoveGroupAsync(Guid groupId, CancellationToken cancellationToken = default);
}
