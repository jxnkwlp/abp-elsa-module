using System;
using Elsa.Models;
using Volo.Abp.Application.Dtos;

namespace Passingwind.Abp.ElsaModule.Common
{
    public class WorkflowInstanceFaultDto : EntityDto<Guid>
    {
        public Guid? FaultedActivityId { get; set; }
        public bool Resuming { get; set; }
        public object ActivityInput { get; set; }
        public string Message { get; set; }
        public SimpleException Exception { get; set; }
    }
}
