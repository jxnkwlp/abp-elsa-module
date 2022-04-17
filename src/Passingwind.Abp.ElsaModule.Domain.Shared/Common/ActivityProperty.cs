using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json.Linq;

namespace Passingwind.Abp.ElsaModule.Common
{
    public class ActivityProperty
    {
        [Required]
        public string Name { get; set; }
        public string Syntax { get; set; }
        public Dictionary<string, JToken> Expressions { get; set; }
    }
}
