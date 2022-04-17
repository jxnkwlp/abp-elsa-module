using System.Collections.Generic;
using Elsa.Metadata;

namespace Passingwind.Abp.ElsaModule.Designer
{
    public class ActivityTypeDescriptorDto
    {
        public string Type { get; set; }
        public string DisplayName { get; set; }
        public string Description { get; set; }
        public string Category { get; set; }
        public ActivityTraits Traits { get; set; }

        public string[] Outcomes { get; set; }

        public ActivityInputDescriptor[] InputProperties { get; set; }
        public ActivityOutputDescriptor[] OutputProperties { get; set; }
        public IDictionary<string, object> CustomAttributes { get; set; }
    }
}
