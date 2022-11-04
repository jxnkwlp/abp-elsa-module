using System;
using Volo.Abp.Application.Dtos;

namespace Passingwind.Abp.ElsaModule.GlobalVariables
{
    public class GlobalVariableDto : AuditedEntityDto<Guid>
    {
        public string Key { get; set; }
        public string Value { get; set; }
        public bool IsSecret { get; set; }
    }
}
