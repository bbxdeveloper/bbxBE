﻿using bbxBE.Common.Attributes;
using bbxBE.Common.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

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
        [MapToEntity("Name")]
        public string USR_NAME { get; set; }

        [ColumnLabel("Számlasor ID")]
        [Description("Számlasor ID")]
        public long? InvoiceLineID { get; set; }

        [ColumnLabel("Termékkód")]
        [Description("Termékkód")]
        public string ProductCode { get; set; }

        [ColumnLabel("Megnevezés")]
        [Description("A termék vagy szolgáltatás megnevezése")]
        public string LineDescription { get; set; }

        [ColumnLabel("Partner ID")]
        [Description("Partner ID")]
        public long? CustomerID { get; set; }

        [ColumnLabel("Ügyfélnév")]
        [Description("Ügyfélnév")]
        public string CustomerName { get; set; }

        [ColumnLabel("Város")]
        [Description("Ügyfél város")]
        public string CustomerCity { get; set; }

        [ColumnLabel("Cím")]
        [Description("Ügyfélcím")]
        public string CustomerAdditionalAddressDetail { get; set; }


        [ColumnLabel("Típus")]
        [Description("Típus")]
        public string ScType { get; set; }

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

    }
}
