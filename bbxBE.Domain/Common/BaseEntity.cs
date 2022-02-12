using System;
using System.Runtime.Serialization;

namespace bbxBE.Domain.Common
{
    [DataContract]
    public abstract class BaseEntity
    {

        [DataMember]
        public long  ID { get; set; }
        [DataMember]
        public DateTime CreateTime{ get; set; }
        [DataMember]
        public DateTime UpdateTime { get; set; }

        [DataMember]
        public bool Deleted { get; set; }

    }
}