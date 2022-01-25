using System;

namespace bbxBE.Domain.Common
{
    public abstract class BaseEntity
    {
        
        public  long  ID { get; set; }
        public DateTime CreateTime{ get; set; }
        public DateTime UppdateTime { get; set; }
        

    }
}