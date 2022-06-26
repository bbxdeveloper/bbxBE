using bbxBE.Common.Attributes;
using bbxBE.Common.Enums;
using bbxBE.Domain.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace bbxBE.Domain.Entities
{
    [Description("Készletkarton")]
    public class StockCard : BaseEntity
    {
        [ColumnLabel("Dátum")]
        [Description("Dátum")]
        public DateTime StockCardDate { get; set; }

        [ColumnLabel("Raktárkészlet ID")]
        [Description("Raktárkészlet ID")]
        public long StockID { get; set; }

        [ColumnLabel("Raktár ID")]
        [Description("Raktár ID")]
        public long WarehouseID { get; set; }

        [ColumnLabel("Termék ID")]
        [Description("Termék ID")]
        public long? ProductID { get; set; }

 
        [ColumnLabel("Felhasználó ID")]
        [Description("Felhasználó ID")]
        public long UserID { get; set; }

        [ColumnLabel("Számlasor ID")]
        [Description("Számlasor ID")]
        public long? InvoiceLineID { get; set; }

        [ColumnLabel("Partner ID")]
        [Description("Partner ID")]
        public long? CustomerID { get; set; }

        #region ScType
        private enStockCardTypes scType;

        [ColumnLabel("Típus")]
        [Description("Típus")]
        public string ScType
        {
            get { return Enum.GetName(typeof(enStockCardTypes), scType); }
            set
            {
                if (value != null)
                    scType = (enStockCardTypes)Enum.Parse(typeof(enStockCardTypes), value);
                else
                    scType = enStockCardTypes.INIT;
            }
        }
        #endregion 

        [ColumnLabel("E.Krt")]
        [Description("Eredeti karton szerinti mennyiség")]
        public decimal OCalcQty { get; set; }

        [ColumnLabel("E.Valós")]
        [Description("Eredeti valós mennyiség")]
        public decimal ORealQty { get; set; }

        [ColumnLabel("E.Kiadott")]
        [Description("Eredeti kiadott mennyiség")]
        public decimal OOutQty { get; set; }

        [ColumnLabel("V.Krt")]
        [Description("Karton szerinti mennyiség változás")]
        public decimal XCalcQty { get; set; }

        [ColumnLabel("V.Valós")]
        [Description("Valós mennyiség változás")]
        public decimal XRealQty { get; set; }

        [ColumnLabel("V.Kiadott")]
        [Description("Kiadott mennyiség változás")]
        public decimal XOutQty { get; set; }

        [ColumnLabel("Új Krt")]
        [Description("Új karton szerinti mennyiség")]
        public decimal NCalcQty { get; set; }

        [ColumnLabel("Új valós")]
        [Description("Új valós mennyiség")]
        public decimal NRealQty { get; set; }

        [ColumnLabel("Új kiadott")]
        [Description("Új kiadott mennyiség")]
        public decimal NOutQty { get; set; }

        [ColumnLabel("Eredeti ELÁBÉ")]
        [Description("Eredeti átlagolt beszerzési egységár")]
        public decimal OAvgCost { get; set; }

        [ColumnLabel("Új ELÁBÉ")]
        [Description("Eredeti átlagolt beszerzési egységár")]
        public decimal NAvgCost { get; set; }

        [ColumnLabel("Kapcsolódó adatok")]
        [Description("Kapcsolódó adatok")]
        public string XRel { get; set; }


        //Relációk
        [ForeignKey("StockID")]
        [ColumnLabel("Készlet")]
        [Description("Készlet")]
        public virtual Stock Stock { get; set; }

        [ForeignKey("WarehouseID")]
        [ColumnLabel("Raktár")]
        [Description("Raktár")]
        public virtual Warehouse Warehouse { get; set; }

        [ForeignKey("ProductID")]
        [ColumnLabel("Termék")]
        [Description("Termék")]
        public virtual Product Product { get; set; }

        [ForeignKey("UserID")]
        [ColumnLabel("Felhasználó")]
        [Description("Felhasználó")]
        public virtual USR_USER User { get; set; }

        [ForeignKey("InvoiceLineID")]
        [ColumnLabel("Számlasor")]
        [Description("Számlasor")]
        public virtual InvoiceLine InvoiceLine { get; set; }

        [ForeignKey("CustomerID")]
        [ColumnLabel("Partner")]
        [Description("Partner")]
        public virtual Customer Customer { get; set; }
    }
}
