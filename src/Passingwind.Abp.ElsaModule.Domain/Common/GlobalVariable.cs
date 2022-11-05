using System;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace Passingwind.Abp.ElsaModule.Common;

public class GlobalVariable : FullAuditedAggregateRoot<Guid>, IMultiTenant
{
    public Guid? TenantId { get; set; }
    public string Key { get; set; }
    // TODO
    public GlobalVariableValueType Type { get; set; }
    public bool IsSecret { get; set; }
    public string Value { get; set; }

    // Limit the variable who can use.
    // public string[] Scopes { get; set; }
}

public enum GlobalVariableValueType
{
    // TODO
}