using bbxBE.Common.Attributes;
using bbxBE.Domain.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace bbxBE.Domain.Entities
{
    [Description("Leltáridőszak")]
    public class InvCtrlPeriod : BaseEntity
    {
        [ColumnLabel("Raktár ID")]
        [Description("Raktár ID")]
        public long WarehouseID { get; set; }

        [ColumnLabel("Kezdődátum")]
        [Description("Kezdődátum")]
        public DateTime DateFrom { get; set; }

        [ColumnLabel("Végdátum")]
        [Description("Végdátum")]
        public DateTime DateTo { get; set; }

        [ColumnLabel("Lezárt?")]
        [Description("Lezárt?")]
        public bool Closed { get; set; }

        [ColumnLabel("Felhasználó ID")]
        [Description("Felhasználó ID")]
        public long? UserID { get; set; } = 0;

        //relációk
        [ForeignKey("WarehouseID")]
        [ColumnLabel("Raktár")]
        [Description("Raktár")]
        public virtual Warehouse Warehouse { get; set; }

    }
}
