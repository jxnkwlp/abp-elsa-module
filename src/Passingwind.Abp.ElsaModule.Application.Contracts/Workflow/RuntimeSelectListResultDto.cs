using System.Collections.Generic;
using Elsa.Design;

namespace Passingwind.Abp.ElsaModule.Workflow;

public class RuntimeSelectListResultDto : SelectList
{
    public RuntimeSelectListResultDto()
    {
    }

    public RuntimeSelectListResultDto(ICollection<SelectListItem> items, bool isFlagsEnum = false) : base(items, isFlagsEnum)
    {
    }
}
