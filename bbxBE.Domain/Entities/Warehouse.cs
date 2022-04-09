using bbxBE.Common.Attributes;
using bbxBE.Domain.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace bbxBE.Domain.Entities
{
    [Description("Raktár")]
    public class Warehouse : BaseEntity
    {
        [ColumnLabel("Kód")]
        [Description("Kód")]
        public string WarehouseCode { get; set; }
        [ColumnLabel("Leírás")]
        [Description("Leírás")]
        public string WarehouseDescription { get; set; }
    }
}
