using bbxBE.Common.Attributes;
using bbxBE.Domain.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace bbxBE.Domain.Entities
{
    [Description("Leltár")]
    public class InvCtrl : BaseEntity
    {
        [ColumnLabel("Raktár ID")]
        [Description("Raktár ID")]
        public long WarehouseID { get; set; }

        [ColumnLabel("Leltáridőszak ID")]
        [Description("Leltáridőszak ID")]       
        public long? InvCtlPeriodID { get; set; }       //Opcionális, hogy a folyamatos leltárat is kezelni lehessen

        [ColumnLabel("Termék ID")]
        [Description("Termék ID")]
        public long ProductID { get; set; }

        [ColumnLabel("Termékkód")]
        [Description("Termékkód")]
        public string ProductCode { get; set; }


        [ColumnLabel("Leltározás dátuma")]
        [Description("Leltározás dátuma")]
        public DateTime InvCtrlDate { get; set; }


        [ColumnLabel("E.Krt")]
        [Description("Eredeti karton szerinti mennyiség")]
        public decimal OCalcQty { get; set; }

        [ColumnLabel("E.Valós")]
        [Description("Eredeti valós mennyiség")]
        public decimal ORealQty { get; set; }

        [ColumnLabel("Új Krt")]
        [Description("Új karton szerinti mennyiség")]
        public decimal NCalcQty { get; set; }

        [ColumnLabel("Új valós")]
        [Description("Új valós mennyiség")]
        public decimal NRealQty { get; set; }

        [ColumnLabel("ELÁBÉ")]
        [Description("Átlagolt beszerzési egységár")]
        public decimal AvgCost { get; set; }


        [ColumnLabel("Felhasználó ID")]
        [Description("Felhasználó ID")]
        public long? UserID { get; set; } = 0;

        //relációk
        [ForeignKey("WarehouseID")]
        [ColumnLabel("Raktár")]
        [Description("Raktár")]
        public virtual Warehouse Warehouse { get; set; }

        [ForeignKey("InvCtlPeriodID")]
        [ColumnLabel("Leltáridőszak")]
        [Description("Leltáridőszak")]
        public virtual InvCtrlPeriod InvCtrlPeriod { get; set; }


        [ForeignKey("ProductID")]
        [ColumnLabel("Termék")]
        [Description("Termék")]
        public virtual Product Product { get; set; }

    }
}
