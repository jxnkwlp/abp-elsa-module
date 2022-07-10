using System;
using System.Collections.Generic;
using Volo.Abp.Domain.Entities;

namespace Passingwind.Abp.ElsaModule.Common;

public class ActivityConnection : Entity
{
    public ActivityConnection(Guid sourceId, Guid targetId, string outcome, Dictionary<string, object> attributes)
    {
        SourceId = sourceId;
        TargetId = targetId;
        Outcome = outcome;
        Attributes = attributes ?? new Dictionary<string, object>();
    }

    public Guid WorkflowDefinitionVersionId { get; set; }
    public Guid SourceId { get; set; }
    public Guid TargetId { get; set; }
    public string Outcome { get; set; }
    public Dictionary<string, object> Attributes { get; set; }

    public override object[] GetKeys()
    {
        return new object[] { WorkflowDefinitionVersionId, SourceId, TargetId, Outcome };
    }
}
