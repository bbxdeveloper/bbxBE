using bbxBE.Common.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace bbxBE.Application.Queries.ViewModels
{
    /// <summary>
    /// MapToEntity properties marks the names in the output Entity
    /// Don't use with AutoMapper, but with <see cref="Domain.Extensions.EntityExtensions.MapFieldsByMapToAnnotation"/>
    /// In this case, <see cref="GetProductViewModel"/> will be the value for the TDestination parameter.
    /// </summary>
    public class GetProductViewModel
    {

        public class ProductStock
        {
            [MapToEntity("ID")]
            public long ID { get; set; }

            [ColumnLabel("Raktár ID")]
            [Description("Raktár ID")]
            public long WarehouseID { get; set; }

            [ColumnLabel("Valós")]
            [Description("Valós mennyiség")]
            public decimal RealQty { get; set; }

            [ColumnLabel("ELÁBÉ")]
            [Description("Átlagolt beszerzési egységár")]
            public decimal AvgCost { get; set; }

            [ColumnLabel("Bev.")]
            [Description("Legutolsó bevétel dátuma")]
            public DateTime? LatestIn { get; set; }

            [ColumnLabel("Kiad.")]
            [Description("Legutolsó kiadás dátuma")]
            public DateTime? LatestOut { get; set; }

        }

        [MapToEntity("ID")]
        public long ID { get; set; }
        [ColumnLabel("Termékkód")]
        [Description("Termékkód")]
        public string ProductCode { get; set; }
        [ColumnLabel("Megnevezés")]
        [Description("Megnevezés, leírás")]
        public string Description { get; set; }

        [ColumnLabel("Termékcsoport kód")]
        [Description("Termékcsoport kód")]
        public string ProductGroupCode { get; set; }

        [ColumnLabel("Termékcsoport")]
        [Description("Termékcsoport")]
        public string ProductGroup { get; set; }

        [ColumnLabel("Árrés minimum%")]
        [Description("Árrés minimum%")]
        public decimal MinMargin { get; set; }

        [ColumnLabel("Származási hely")]
        [Description("Származási hely")]
        public string Origin { get; set; }

        #region UnitOfMeasure
        [DataMember]
        [ColumnLabel("Me.e")]
        [Description("Mennyiségi egység")]
        public string UnitOfMeasure { get; set; }

        [ColumnLabel("Me.e név")]
        [Description("Mennyiségi egység megnevezés")]
        [DataMember]
        [NotDBField]
        public string UnitOfMeasureX { get; set; }
        #endregion

        [ColumnLabel("Elad. ár1")]
        [Description("Eladási ár1")]
        public decimal UnitPrice1 { get; set; }
        [ColumnLabel("Elad. ár2")]
        [Description("Eladási ár2")]
        public decimal UnitPrice2 { get; set; }
        [ColumnLabel("Áfaleíró-kód")]
        [Description("Áfaleíró-kód")]
        public string VatRateCode { get; set; }
        [ColumnLabel("Áfa mértéke")]
        [Description("Az alkalmazott adó mértéke - Áfa tv. 169. § j)")]
        public decimal VatPercentage { get; set; }
        [ColumnLabel("Utolsó beszerzés")]
        [Description("Utolsó beszerzési ár")]
        public decimal LatestSupplyPrice { get; set; }
        [ColumnLabel("Készletes?")]
        [Description("Készletes?")]
        public bool IsStock { get; set; }
        [ColumnLabel("Min.")]
        [Description("Minimumkészlet")]
        public decimal MinStock { get; set; }
        [ColumnLabel("Rendelési egység")]
        [Description("Rendelési egység")]
        public decimal OrdUnit { get; set; }
        [ColumnLabel("Termékdíj")]
        [Description("Termékdíj")]
        public decimal ProductFee { get; set; }
        [ColumnLabel("Aktív?")]
        [Description("Aktív?")]
        public bool Active { get; set; }
        [ColumnLabel("VTSZ")]
        [Description("Vámtarifa szám")]
        public string VTSZ { get; set; }
        [ColumnLabel("EAN")]
        [Description("Vonalkód")]
        public string EAN { get; set; }
        [ColumnLabel("Eng.tilt")]
        [Description("Engedmény adás tiltása")]
        public bool NoDiscount { get; set; }
        [ColumnLabel("Súly")]
        [Description("Súly")]
        public Decimal UnitWeight { get; set; }

        [ColumnLabel("Készletek raktáranként")]
        [Description("Készletek raktáranként")]
        public List<ProductStock> Stocks { get; set; }
    }
}
