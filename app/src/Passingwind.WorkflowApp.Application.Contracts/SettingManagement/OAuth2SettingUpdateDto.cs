using System.ComponentModel.DataAnnotations;

namespace Passingwind.WorkflowApp.SettingManagement;
public class OAuth2SettingUpdateDto
{
    public bool Enabled { get; set; }
    [Required]
    [MaxLength(16)]
    public string DisplayName { get; set; }
    [Required]
    [MaxLength(256)]
    public string Authority { get; set; }
    [Required]
    [MaxLength(256)]
    public string MetadataAddress { get; set; }
    [Required]
    [MaxLength(256)]
    public string ClientId { get; set; }
    public string ClientSecret { get; set; }
    public string Scope { get; set; }
}
