using System;
using System.Collections.Generic;
using Volo.Abp.Domain.Entities.Auditing;

namespace Passingwind.Abp.ElsaModule.Common
{
    public class ActivityConnection : CreationAuditedEntity
    {
        public Guid DefinitionVersionId { get; set; }
        public Guid SourceId { get; set; }
        public Guid TargetId { get; set; }
        public string Outcome { get; set; }
        public Dictionary<string, object> Arrtibutes { get; set; }

        public override object[] GetKeys()
        {
            return new object[] { DefinitionVersionId, SourceId, TargetId, Outcome };
        }
    }
}
