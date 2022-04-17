using System;
using Volo.Abp.Domain.Entities.Auditing;

namespace Passingwind.Abp.ElsaModule.Common
{
    public class ActivityConnection : CreationAuditedEntity
    {
        public Guid DefinitionVersionId { get; set; }

        public long SourceId { get; set; }
        public long TargetId { get; set; }
        public string Outcome { get; set; }

        public override object[] GetKeys()
        {
            return new object[] { DefinitionVersionId, SourceId, TargetId, Outcome };
        }
    }
}
