using System;
using System.ComponentModel.DataAnnotations;
using Volo.Abp.Timing;

namespace Demo.ApiKeys;

public class ApiKeyCreateOrUpdateDto
{
    [Required]
    [MaxLength(32)]
    public string Name { get; set; }
    [DisableDateTimeNormalization]
    public DateTime? ExpirationTime { get; set; }
}
