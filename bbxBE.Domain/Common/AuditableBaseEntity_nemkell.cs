using System;

namespace bbxBE.Domain.Common
{
    public abstract class AuditableBaseEntity_nemkell : BaseEntity
    {
        public string CreatedBy { get; set; }
        public DateTime Created { get; set; }
        public string LastModifiedBy { get; set; }
        public DateTime? LastModified { get; set; }
    }
}