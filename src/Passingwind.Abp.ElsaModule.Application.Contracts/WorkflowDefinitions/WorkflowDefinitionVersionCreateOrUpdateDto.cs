using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Passingwind.Abp.ElsaModule.Workflow;

namespace Passingwind.Abp.ElsaModule.WorkflowDefinitions
{
    public class WorkflowDefinitionVersionCreateOrUpdateDto
    {
        [Required]
        public WorkflowDefinitionCreateOrUpdateDto Definition { get; set; }

        public List<ActivityCreateOrUpdateDto> Activities { get; set; }

        public List<ActivityConnectionCreateDto> Connections { get; set; }

        public bool IsPublished { get; set; }
    }
}
