using bbxBE.Common.Attributes;
using bbxBE.Domain.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
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
		public short LineNumberReference { get; set; }
		[ColumnLabel("Modosítás jellege")]
		[Description("A számlatétel módosításának jellege")]
		public string LineOperation { get; set; }

		//Gyűjtőszámla
		[ColumnLabel("A tételhez tartozó árfolyam")]
		[Description("A tételhez tartozó árfolyam, 1 (egy) egységre vonatkoztatva. Csak külföldi pénznemben kiállított gyűjtőszámla esetén kitöltendő")]
		public decimal LineExchangeRate { get; set; }
		[ColumnLabel("Teljesítés dátuma")]
		[Description("Gyűjtőszámla esetén az adott tétel teljesítési dátuma")]
		public DateTime LineDeliveryDate { get; set; }

        //Gyűjtőszámla - szállítólvék kapcsolat
        [ColumnLabel("Szállítólevél")]
		[Description("Kapcsolt szállítólevél száma")]
		public string OrigDeliveryNoteNumber { get; set; }

		[ColumnLabel("Szállítólevél ID")]
		[Description("Kapcsolt szállítólevél ID")]
		public long? OrigDeliveryNoteInvoiceID { get; set; }

		[ColumnLabel("Szállítólevél sor")]
		[Description("Kapcsolt szállítólevél sor")]
		public short? OrigDeliveryNoteInvoiceLineID { get; set; }

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


		//Relációk
		[JsonIgnore]					//ignorálni kell, mert körkörös hivatkozást eredményez
		[ForeignKey("InvoiceID")]
		[ColumnLabel("Számla")]
		[Description("Számla")]
		public virtual Invoice Invoice { get; set; }

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
