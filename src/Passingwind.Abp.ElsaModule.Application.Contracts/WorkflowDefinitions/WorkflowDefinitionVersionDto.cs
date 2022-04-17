using System;
using System.Collections.Generic;
using Passingwind.Abp.ElsaModule.Workflow;
using Volo.Abp.Application.Dtos;

namespace Passingwind.Abp.ElsaModule.WorkflowDefinitions
{
    public class WorkflowDefinitionVersionDto : AuditedEntityDto<Guid>
    {
        public WorkflowDefinitionDto Definition { get; set; }

        public int Version { get; set; } = 1;

        public bool IsLatest { get; set; }

        public bool IsPublished { get; set; }

        public List<ActivityDto> Activities { get; set; }

        public List<ActivityConnectionDto> Connections { get; set; }
    }
}
