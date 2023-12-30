﻿using bbxBE.Common.Attributes;
using bbxBE.Domain.Common;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace bbxBE.Domain.Entities
{
    [Description("Árajánlat-sor")]
    public class OfferLine : BaseEntity
    {
        [ColumnLabel("Árajánlat ID")]
        [Description("Árajánlat ID")]
        public long OfferID { get; set; }

        [ColumnLabel("#")]
        [Description("Sor száma")]
        public short LineNumber { get; set; }

        [ColumnLabel("Termék ID")]
        [Description("Termék ID")]
        public long? ProductID { get; set; }

        [ColumnLabel("Termékkód")]
        [Description("Termékkód")]
        public string ProductCode { get; set; }

        [ColumnLabel("Megnevezés")]
        [Description("A termék vagy szolgáltatás megnevezése")]
        public string LineDescription { get; set; }

        [ColumnLabel("Árengedmény %")]
        [Description("Árengedmény %)")]
        public decimal Discount { get; set; }
        [ColumnLabel("Árengedmény megjelenítés?")]
        [Description("Árengedmény megjelenítés)")]
        public bool ShowDiscount { get; set; }

        [ColumnLabel("Eng.tilt")]
        [Description("Engedmény adás tiltása")]
        public bool NoDiscount { get; set; }

        [ColumnLabel("Áfa ID")]
        [Description("Áfa ID")]
        public long VatRateID { get; set; }

        [ColumnLabel("Áfa%")]
        [Description("Az alkalmazott adó mértéke - Áfa tv. 169. § j)")]
        public decimal VatPercentage { get; set; }

        [ColumnLabel("Mennyiség")]
        [Description("Mennyiség")]
        public decimal Quantity { get; set; }

        [ColumnLabel("Me")]
        [Description("Mennyiségi egység")]
        public string UnitOfMeasure { get; set; }

        [ColumnLabel("Eredeti ár")]					//a törzsbeli ár
        [Description("Eredeti ár")]
        public decimal OriginalUnitPrice { get; set; }

        [ColumnLabel("Eredeti ár HUF")]                 //a törzsbeli ár
        [Description("Eredeti ár forintban")]
        public decimal OriginalUnitPriceHUF { get; set; }

        [ColumnLabel("E/L")]						//Eygségár/listaár flag
        [Description("Egységár/Listaár")]
        public bool UnitPriceSwitch { get; set; }

        [ColumnLabel("Ár")]
        [Description("Ár")]
        public decimal UnitPrice { get; set; }


        [ColumnLabel("Bruttó ár")]
        [Description("Bruttó ár")]
        public decimal UnitGross { get; set; }


        //Relációk
        [JsonIgnore]                    //ignorálni kell, mert körkörös hivatkozást eredményez
        [ForeignKey("OfferID")]
        [ColumnLabel("Árajánlat")]
        [Description("Árajánlat")]
        public virtual Offer Offer { get; set; }

        [ForeignKey("ProductID")]
        [ColumnLabel("Termék")]
        [Description("Termék")]
        public virtual Product Product { get; set; }

        [ForeignKey("VatRateID")]
        [ColumnLabel("Áfakulcs")]
        [Description("Áfakulcs")]
        public virtual VatRate VatRate { get; set; }

    }
}
