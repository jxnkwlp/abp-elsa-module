using System;
using System.Collections.Generic;

namespace Passingwind.Abp.ElsaModule.WorkflowGroups;
public class WorkflowGroupUpdateUsersRequestDto
{
    public List<Guid> UserIds { get; set; }
}
