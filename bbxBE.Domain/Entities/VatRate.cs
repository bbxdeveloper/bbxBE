using bbxBE.Common.Attributes;
using bbxBE.Domain.Common;
using System.ComponentModel;

namespace bbxBE.Domain.Entities
{
    [Description("Áfa leíró")]
    public class VatRate : BaseEntity
    {
        [ColumnLabel("Áfaleíró kód")]
        [Description("Áfaleíró kód")]
        public string VatRateCode { get; set; }
        [ColumnLabel("Áfaleíró név")]
        [Description("Áfaleíró megnevezése")]
        public string VatRateName { get; set; }
        [ColumnLabel("Áfa mértéke")]
        [Description("Az alkalmazott adó mértéke - Áfa tv. 169. § j)")]
        public decimal VatPercentage { get; set; }
        [ColumnLabel("Áfatartalom egyszerűsített számla esetén")]
        [Description("Áfatartalom egyszerűsített számla esetén - NEM HASZNÁLJUK")]
        public decimal? VatContent { get; set; }
        [ColumnLabel("Adómentesség jelölés")]
        [Description("Az adómentesség jelölés kódja")]
        public string VatExemptionCase { get; set; }
        [ColumnLabel("Adómentesség leírás")]
        [Description("Az adómentesség jelölés leírása")]
        public string VatExemptionReason { get; set; }
        [ColumnLabel("Hatályon kívüliség kód")]
        [Description("Az Áfa tv.y hatályán kívüliség kódja")]
        public string VatOutOfScopeCase { get; set; }
        [ColumnLabel("Hatályon kívüliség leírás")]
        [Description("Az Áfa tv.y hatályán kívüliség leírása")]
        public string VatOutOfScopeReason { get; set; }
        [ColumnLabel("Fordított adózás")]
        [Description("A belföldi fordított adózás jelölése - Áfa tv. 142. §)")]
        public bool VatDomesticReverseCharge { get; set; }
        [ColumnLabel("Különbözet szerinti szabályozás")]
        [Description("Különbözet szerinti szabályozás jelölése - Áfa tv. 169. § p) q)")]
        public string MarginSchemeIndicator { get; set; }
        [ColumnLabel("Adómérték, adótartalom")]
        [Description("Adómérték, adótartalom - NEM HASZNÁLJUK")]
        public decimal? vatAmountMismatchVatRate { get; set; }
        [ColumnLabel("Adóalap és felszámított adó eltérésének kódja")]
        [Description("Adóalap és felszámított adó eltérésének kódja - NEM HASZNÁLJUK")]
        public string vatAmountMismatchCase { get; set; }
        [ColumnLabel("Nincs felszámított áfa")]
        [Description("Nincs felszámított áfa a 17. § alapján - NEM HASZNÁLJUK")]
        public bool NoVatCharge { get; set; }

    }
}
