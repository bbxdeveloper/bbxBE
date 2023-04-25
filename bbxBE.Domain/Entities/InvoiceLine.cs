using bbxBE.Common.Attributes;
using bbxBE.Domain.Common;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace bbxBE.Domain.Entities
{
    [Description("Számlasor")]
    public class InvoiceLine : BaseEntity
    {
        [ColumnLabel("Számla ID")]
        [Description("Számla ID")]
        public long InvoiceID { get; set; }

        [ColumnLabel("Termék ID")]
        [Description("Termék ID")]
        public long? ProductID { get; set; }

        [ColumnLabel("#")]
        [Description("Sor száma")]
        public short LineNumber { get; set; }

        [ColumnLabel("Számlasor tartalom jelző")]
        [Description("Számlasor kötelező tartalmi elemeinek meghatározása (értéke minden esetben true)")]
        public bool LineExpressionIndicator { get; set; }
        [ColumnLabel("Termékértékesítés/szolgáltatásnyújtás")]
        [Description("Termékértékesítés vagy szolgáltatásnyújtás jelölése (egyelőre csak PRODUCT)")]
        public string LineNatureIndicator { get; set; }

        [ColumnLabel("Termékkód")]
        [Description("Termékkód")]
        public string ProductCode { get; set; }

        [ColumnLabel("VTSZ")]
        [Description("Vámtarifa szám")]
        public string VTSZ { get; set; }

        [ColumnLabel("Áfa ID")]
        [Description("Áfa ID")]
        public long VatRateID { get; set; }

        [ColumnLabel("Áfa%")]
        [Description("Az alkalmazott adó mértéke - Áfa tv. 169. § j)")]
        public decimal VatPercentage { get; set; }


        [ColumnLabel("Megnevezés")]
        [Description("A termék vagy szolgáltatás megnevezése")]
        public string LineDescription { get; set; }
        [ColumnLabel("Mennyiség")]
        [Description("Mennyiség")]
        public decimal Quantity { get; set; }
        [ColumnLabel("Me")]
        [Description("Mennyiségi egység")]
        public string UnitOfMeasure { get; set; }
        [ColumnLabel("Ár")]
        [Description("Ár")]
        public decimal UnitPrice { get; set; }
        [ColumnLabel("Ár HUF")]
        [Description("Ár forintban")]

        #region Nyomtatványon megjelenő mezők
        public decimal UnitPriceHUF { get; set; }
        [ColumnLabel("Nettó érték")]
        [Description("Ár a számla pénznemében")]
        public decimal LineNetAmount { get; set; }
        [ColumnLabel("Nettó érték HUF")]
        [Description("Ár forintban")]
        public decimal LineNetAmountHUF { get; set; }
        [ColumnLabel("Áfa érték")]
        [Description("Áfa a számla pénznemében")]
        public decimal LineVatAmount { get; set; }
        [ColumnLabel("Áfa érték HUF")]
        [Description("Áfa forintban")]
        public decimal LineVatAmountHUF { get; set; }
        [ColumnLabel("Bruttó érték")]
        [Description("Bruttó a számla pénznemében")]
        public decimal LineGrossAmountNormal { get; set; }
        [ColumnLabel("Bruttó érték HUF")]
        [Description("Bruttó forintban")]
        public decimal LineGrossAmountNormalHUF { get; set; }
        #endregion

        #region Feldolgozásokban résztvevő mezők
        [ColumnLabel("Kedvezményel csökkentett nettó érték HUF")]
        [Description("Kedvezményel csökkentett ár forintban")]
        public decimal LineNetDiscountedAmount { get; set; }
        [ColumnLabel("Kedvezményel csökkentett nettó érték HUF")]
        [Description("Kedvezményel csökkentett ár forintban")]
        public decimal LineNetDiscountedAmountHUF { get; set; }
        [ColumnLabel("Kedvezményel csökkentett áfa érték")]
        [Description("Kedvezményel csökkentett áfa a számla pénznemében")]
        public decimal LineVatDiscountedAmount { get; set; }
        [ColumnLabel("Kedvezményel csökkentett áfa érték HUF")]
        [Description("Kedvezményel csökkentett áfa forintban")]
        public decimal LineVatDiscountedAmountHUF { get; set; }
        [ColumnLabel("Kedvezményel csökkentett bruttó érték")]
        [Description("Kedvezményel csökkentett bruttó a számla pénznemében")]
        public decimal LineGrossDiscountedAmountNormal { get; set; }
        [ColumnLabel("Kedvezményel csökkentett bruttó érték HUF")]
        [Description("Kedvezményel csökkentett bruttó forintban")]
        public decimal LineGrossDiscountedAmountNormalHUF { get; set; }
        #endregion

        //javítószámla esetén töltött
        [ColumnLabel("Az eredeti számla tételének sorszáma")]
        [Description("Az eredeti számla módosítással érintett tételének sorszáma,(lineNumber).Új tétel létrehozása esetén az új tétel sorszáma, az eredeti számla folytatásaként")]
        public short LineNumberReference { get; set; } = 0;
        [ColumnLabel("Modosítás jellege")]
        [Description("A számlatétel módosításának jellege (javtószámla)")]
        public string LineOperation { get; set; }

        //Gyűjtőszámla
        [ColumnLabel("A tételhez tartozó árfolyam")]
        [Description("A tételhez tartozó árfolyam, 1 (egy) egységre vonatkoztatva. Csak külföldi pénznemben kiállított gyűjtőszámla esetén kitöltendő")]
        public decimal LineExchangeRate { get; set; }
        [ColumnLabel("Teljesítés dátuma")]
        [Description("Gyűjtőszámla esetén az adott tétel teljesítési dátuma")]
        public DateTime LineDeliveryDate { get; set; }

        //Gyűjtőszámla - szállítólvél kapcsolat
        [ColumnLabel("Szállítólevél")]
        [Description("Kapcsolt szállítólevél száma")]
        public string RelDeliveryNoteNumber { get; set; }

        [ColumnLabel("Szállítólevél ID")]
        [Description("Kapcsolt szállítólevél ID")]
        public long? RelDeliveryNoteInvoiceID { get; set; }

        [ColumnLabel("Szállítólevél sor")]
        [Description("Kapcsolt szállítólevél sor")]
        public long? RelDeliveryNoteInvoiceLineID { get; set; }

        //Csak szállítólevél esetén értelmezett mezők
        [ColumnLabel("Rendezetlen szállítólevél-mennyiség")]
        [Description("Rendezetlen szállítólevél-mennyiség")]
        public decimal PendingDNQuantity { get; set; }

        //Termékdíj - deklaráció
        [ColumnLabel("Átvállalás irány")]
        [Description("Az átvállalás iránya és jogszabályi alapja (02_ab, stb...)")]
        public string TakeoverReason { get; set; }
        [ColumnLabel("Termékdíj összeg")]
        [Description("Az átvállalt termékdíj összege forintban, ha a vevő vállalja át az eladó termékdíjkötelezettségét")]
        public decimal TakeoverAmount { get; set; }

        //Termékdíj tartalom

        [ColumnLabel("Termékdíj kat.")]
        [Description("Termékdíj kategória (Kt vagy Csk)")]
        public string ProductFeeProductCodeCategory { get; set; }
        [ColumnLabel("Termékdíj kód")]
        [Description("Termékdíj kód (Kt vagy Csk)")]
        public string ProductFeeProductCodeValue { get; set; }

        [ColumnLabel("Termékdíj mennyiség")]
        [Description("A termékdíjjal érintett termék mennyisége")]
        public decimal ProductFeeQuantity { get; set; }
        [ColumnLabel("Díjtétel egység")]
        [Description("A díjtétel egysége (kg vagy darab)")]
        public string ProductFeeMeasuringUnit { get; set; }
        [ColumnLabel("Díjtétel")]
        [Description("A termékdíj díjtétele (HUF / egység)")]
        public decimal ProductFeeRate { get; set; }
        [ColumnLabel("Termékdíj összege HUF")]
        [Description("Termékdíj összege forintban")]
        public decimal ProductFeeAmount { get; set; }

        [ColumnLabel("Ár felülvizsgálat?")]
        [Description("Ár felülvizsgálat?")]
        public bool? PriceReview { get; set; } = false;

        [ColumnLabel("Eng.tilt")]
        [Description("Engedmény adás tiltása")]
        public bool NoDiscount { get; set; }

        [ColumnLabel("Kedvezmény%")]
        [Description("A számlára v. gyűjtőszámlán, a kapcsolt szállítólevélre adott teljes kedvezmény %")]
        public decimal LineDiscountPercent { get; set; }        //NoDiscount esetén értéke 0!


        //Relációk
        [JsonIgnore]                    //ignorálni kell, mert körkörös hivatkozást eredményez
        [ForeignKey("InvoiceID")]
        [ColumnLabel("Számla")]
        [Description("Számla")]
        public virtual Invoice Invoice { get; set; }

        [JsonIgnore]                    //ignorálni kell, mert körkörös hivatkozást eredményez
        [ForeignKey("RelDeliveryNoteInvoiceID")]
        [ColumnLabel("Kapcsolt szállítólevél")]
        [Description("Kapcsolt szállítólevél")]
        public virtual Invoice? DeliveryNote { get; set; } = null;

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
