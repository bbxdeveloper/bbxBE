using bbxBE.Domain.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace bbxBE.Domain.Entities
{
    public class Warehouse : BaseEntity
    {
        public string WarehouseCode { get; set; }
        public string WarehouseDescription { get; set; }
    }
}
