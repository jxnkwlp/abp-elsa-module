using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Passingwind.WorkflowApp.SettingManagement;
public class OAuth2SettingsDto
{
    public bool Enabled { get; set; }
    public string DisplayName { get; set; }
    public string Authority { get; set; }
    public string MetadataAddress { get; set; }
    public string ClientId { get; set; }
    public string ClientSecret { get; set; }
    public string Scope { get; set; }
}
