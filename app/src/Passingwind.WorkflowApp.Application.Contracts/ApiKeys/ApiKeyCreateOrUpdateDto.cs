﻿using System;
using System.ComponentModel.DataAnnotations;
using Volo.Abp.Timing;

namespace Passingwind.WorkflowApp.ApiKeys;

public class ApiKeyCreateOrUpdateDto
{
    [Required]
    [MaxLength(32)]
    public string Name { get; set; }
    [DisableDateTimeNormalization]
    public DateTime? ExpirationTime { get; set; }
}
