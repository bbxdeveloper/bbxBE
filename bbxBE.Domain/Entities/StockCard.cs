using bbxBE.Common.Attributes;
using bbxBE.Common.Enums;
using bbxBE.Domain.Common;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

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
        public long? UserID { get; set; }

        [ColumnLabel("Számlasor ID")]
        [Description("Számlasor ID")]
        public long? InvoiceLineID { get; set; }

        [ColumnLabel("Leltári tétel ID")]
        [Description("Leltári tétel ID")]
        public long? InvCtrlID { get; set; }


        [ColumnLabel("Raktárközi átadás tétel ID")]
        [Description("Raktárközi átadás ID")]
        public long? WhsTransferLineID { get; set; }

        [ColumnLabel("Partner ID")]
        [Description("Partner ID")]
        public long? CustomerID { get; set; }

        #region ScType
        private enStockCardType scType;

        [ColumnLabel("Típus")]
        [Description("Típus")]
        public string ScType
        {
            get { return Enum.GetName(typeof(enStockCardType), scType); }
            set
            {
                if (value != null)
                    scType = (enStockCardType)Enum.Parse(typeof(enStockCardType), value);
                else
                    scType = enStockCardType.INIT;
            }
        }
        #endregion 

        [ColumnLabel("E.Valós")]
        [Description("Eredeti valós mennyiség")]
        public decimal ORealQty { get; set; }

        [ColumnLabel("V.Valós")]
        [Description("Valós mennyiség változás")]
        public decimal XRealQty { get; set; }

        [ColumnLabel("Új valós")]
        [Description("Új valós mennyiség")]
        public decimal NRealQty { get; set; }


        [ColumnLabel("Ár")]
        [Description("Ár")]
        public decimal UnitPrice { get; set; }

        [ColumnLabel("Eredeti ELÁBÉ")]
        [Description("Eredeti átlagolt beszerzési egységár")]
        public decimal OAvgCost { get; set; }

        [ColumnLabel("Új ELÁBÉ")]
        [Description("Eredeti átlagolt beszerzési egységár")]
        public decimal NAvgCost { get; set; }

        [ColumnLabel("Kapcsolt bizonylat")]
        [Description("Kapcsolt bizonylat")]
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
        public virtual Users User { get; set; }

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
