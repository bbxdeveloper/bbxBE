using bbxBE.Common.Attributes;
using bbxBE.Domain.Common;
using System.Collections.Generic;
using System.ComponentModel;

namespace bbxBE.Domain.Entities
{
    [Description("Hely")]
    public class Location : BaseEntity
    {
        [ColumnLabel("Kód")]
        [Description("Kód")]
        public string LocationCode { get; set; }

        [ColumnLabel("Leírás")]
        [Description("Leírás")]
        public string LocationDescription { get; set; }

        //relációk
        [ColumnLabel("Készletek")]
        [Description("Készletek")]
        public virtual ICollection<Stock> Stocks { get; set; }

    }
}
