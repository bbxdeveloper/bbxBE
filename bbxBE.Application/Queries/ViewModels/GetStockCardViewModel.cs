using bbxBE.Common.Attributes;
using System;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace bbxBE.Application.Queries.ViewModels
{
    public class GetStockCardViewModel
    {
        [MapToEntity("ID")]
        public long ID { get; set; }

        [ColumnLabel("Raktárkészlet ID")]
        [Description("Raktárkészlet ID")]
        public long StockID { get; set; }

        [ColumnLabel("Raktár ID")]
        [Description("Raktár ID")]
        public long WarehouseID { get; set; }

        [ColumnLabel("Raktár")]
        [Description("Raktár")]
        public string Warehouse { get; set; }

        [ColumnLabel("Felhasználó ID")]
        [Description("Felhasználó ID")]
        public long UserID { get; set; }

        [ColumnLabel("Felhasználó")]
        [Description("Felhasználó")]
        [MapToEntity("UserName")]
        public string UserName { get; set; }

        [ColumnLabel("Számlasor ID")]
        [Description("Számlasor ID")]
        public long? InvoiceLineID { get; set; }

        [ColumnLabel("Leltári tétel ID")]
        [Description("Leltári tétel ID")]
        public long? InvCtrlID { get; set; }

        [ColumnLabel("Raktárközi átadás tétel ID")]
        [Description("Raktárközi átadás ID")]
        public long? WhsTransferLineID { get; set; }

        [ColumnLabel("Termék ID")]
        [Description("Termék ID")]
        public long ProductID { get; set; }

        [ColumnLabel("Termékkód")]
        [Description("Termékkód")]
        public string ProductCode { get; set; }

        [ColumnLabel("Megnevezés")]
        [Description("Termékmegnevezés, leírás")]
        public string Product { get; set; }

        [ColumnLabel("Partner ID")]
        [Description("Partner ID")]
        public long? CustomerID { get; set; }

        [ColumnLabel("Ügyfélnév")]
        [Description("Ügyfélnév")]
        public string Customer { get; set; }

        [ColumnLabel("Város")]
        [Description("Ügyfél város")]
        public string CustomerCity { get; set; }

        [ColumnLabel("Cím")]
        [Description("Ügyfélcím")]
        public string CustomerAdditionalAddressDetail { get; set; }

        [ColumnLabel("Dátum")]
        [Description("Dátum")]
        public DateTime StockCardDate { get; set; }

        #region ScType

        [DataMember]
        [ColumnLabel("Típus")]
        [Description("Típus")]
        public string ScType { get; set; }


        [ColumnLabel("Típus")]
        [Description("Típus megnevezés")]
        [DataMember]
        [NotDBField]
        public string ScTypeX { get; set; }
        #endregion


        [ColumnLabel("E.Valós")]
        [Description("Eredeti valós mennyiség")]
        public decimal ORealQty { get; set; }



        [ColumnLabel("V.Valós")]
        [Description("Valós mennyiség változás")]
        public decimal XRealQty { get; set; }


        [ColumnLabel("Új Valós")]
        [Description("Új Valós mennyiség")]
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

        [ColumnLabel("Kapcsolódó adatok")]
        [Description("Kapcsolódó adatok")]
        public string XRel { get; set; }

    }
}
