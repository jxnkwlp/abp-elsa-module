using System.Threading;
using System.Threading.Tasks;
using Elsa;
using Elsa.Models;
using Elsa.Services;
using Elsa.Services.Models;
using NodaTime.Extensions;
using Volo.Abp.Guids;
using Volo.Abp.Timing;
using Volo.Abp.Tracing;
using WorkflowInstance = Elsa.Models.WorkflowInstance;

namespace Passingwind.Abp.ElsaModule.Services;

public class NewWorkflowFactory : IWorkflowFactory
{
    private readonly IGuidGenerator _guidGenerator;
    private readonly IClock _clock;
    private readonly ICorrelationIdProvider _correlationIdProvider;

    public NewWorkflowFactory(IGuidGenerator guidGenerator, IClock clock, ICorrelationIdProvider correlationIdProvider)
    {
        _guidGenerator = guidGenerator;
        _clock = clock;
        _correlationIdProvider = correlationIdProvider;
    }

    public Task<Elsa.Models.WorkflowInstance> InstantiateAsync(IWorkflowBlueprint workflowBlueprint, string correlationId = null, string contextId = null, string tenantId = null, CancellationToken cancellationToken = default)
    {
        var now = _clock.Now;
        var workflowInstanceModel = new WorkflowInstance
        {
            Id = _guidGenerator.Create().ToString(),
            TenantId = tenantId ?? workflowBlueprint.TenantId,
            DefinitionId = workflowBlueprint.Id,
            Version = workflowBlueprint.Version,
            DefinitionVersionId = workflowBlueprint.VersionId,
            WorkflowStatus = WorkflowStatus.Idle,
            CorrelationId = correlationId ?? _correlationIdProvider.Get(),
            ContextId = contextId,
            CreatedAt = now.ToInstant(),
            Variables = new Variables(workflowBlueprint.Variables),
            ContextType = workflowBlueprint.ContextOptions?.ContextType?.GetContextTypeName(),

            // set default name
            Name = $"{workflowBlueprint.Name}-{now.ToLocalTime():yyyyMMddHHmmss}",
        };

        return Task.FromResult(workflowInstanceModel);
    }
}
