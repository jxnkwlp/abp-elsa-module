using System;
using Volo.Abp.Application.Dtos;

namespace Demo.ApiKeys;

public class ApiKeyDto : AuditedEntityDto<Guid>
{
    public string Name { get; set; }
    public DateTime? ExpirationTime { get; set; }
    public string Secret { get; set; }
}
