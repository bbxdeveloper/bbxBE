using bbxBE.Domain.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace bbxBE.Domain.Entities
{
    public class ProductCode : BaseEntity
    {
        public string Description { get; set; }
        public long ProductGroupID { get; set; }
        public long OriginID { get; set; }
        public string UnitOfMeasure { get; set; }
        public decimal UnitPrice1 { get; set; }
        public decimal UnitPrice2 { get; set; }
        public decimal LatestSupplyPrice { get; set; }
        public bool IsStock { get; set; }
        public decimal MinStock { get; set; }
        public decimal OrdUnit { get; set; }
        public decimal ProductFee { get; set; }
        public bool Active { get; set; }
    }
}
