using bbxBE.Domain.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace bbxBE.Domain.Entities
{
    public class Origin : BaseEntity
    {
        public string OriginCode { get; set; }
        public string OriginDescription { get; set; }
    }
}
