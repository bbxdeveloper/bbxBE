using System;

namespace bbxBE.Domain.Common
{
    public abstract class BaseEntity
    {
        public virtual long  ID { get; set; }
        public DateTime CreateTime{ get; set; }
        public DateTime UPpdateTime { get; set; }

    }
}