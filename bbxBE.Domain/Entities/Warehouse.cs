using bbxBE.Common.Attributes;
using bbxBE.Domain.Common;
using System.Collections.Generic;
using System.ComponentModel;

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

        //relációk
        [ColumnLabel("Készletek")]
        [Description("Készletek")]
        public virtual ICollection<Stock> Stocks { get; set; }

        [ColumnLabel("Bizonylati tömbök")]
        [Description("Bizonylati tömbök")]
        public virtual IList<Counter> Counters { get; set; }


    }
}
