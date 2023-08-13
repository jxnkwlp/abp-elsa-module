using System;
using System.Collections.Generic;

namespace Passingwind.Abp.ElsaModule.Teams;

[Obsolete]
public class WorkflowTeamSetWorkflowRequestDto
{
    public List<Guid> WorkflowIds { get; set; }
}
