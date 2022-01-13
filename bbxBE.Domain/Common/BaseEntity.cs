using System;

namespace bbxBE.Domain.Common
{
    public abstract class BaseEntity
    {
        public virtual long  ID { get; set; }
        public DateTime CTime{ get; set; }
        public DateTime UTime { get; set; }

    }
}