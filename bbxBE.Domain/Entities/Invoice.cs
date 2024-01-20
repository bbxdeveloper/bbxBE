using bbxBE.Common.Attributes;
using bbxBE.Common.Enums;
using bbxBE.Common.NAV;
using bbxBE.Domain.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace bbxBE.Domain.Entities
{
    [Description("Számla fej")]
    public class Invoice : BaseEntity
    {

        [ColumnLabel("B/K")]
        [Description("Bejővő/Kimenő")]
        public bool Incoming { get; set; }

        private enInvoiceType invoiceType;
        [ColumnLabel("Típus")]
        [Description("Típus")]
        public string InvoiceType
        {
            get { return Enum.GetName(typeof(enInvoiceType), invoiceType); }
            set
            {
                if (value != null)
                    invoiceType = (enInvoiceType)Enum.Parse(typeof(enInvoiceType), value);
                else
                    invoiceType = enInvoiceType.INV;

            }
        }

        [ColumnLabel("Raktár ID")]
        [Description("Raktár ID")]
        public long WarehouseID { get; set; }

        [ColumnLabel("Bizonylatszám")]
        [Description("Bizonylatszám")]
        public string InvoiceNumber { get; set; }

        [ColumnLabel("Kelt")]
        [Description("Kiállítás dátuma")]
        public DateTime InvoiceIssueDate { get; set; }

        [ColumnLabel("Adatszolgáltatás típusa")]
        [Description("Az adatszolgáltatás maga az elektronikus számla ? (értéke false)")]
        public bool CompletenessIndicator { get; set; }


        [ColumnLabel("Szállító ID")]
        [Description("Szállító ID")]
        public long SupplierID { get; set; }

        [ColumnLabel("Ügyfél ID")]
        [Description("Ügyfél ID")]
        public long? CustomerID { get; set; }           //A blokk miatt nullable!

        private InvoiceCategoryType invoiceCategory;

        [ColumnLabel("Típus")]
        [Description("Típus")]
        public string InvoiceCategory
        {
            get { return Enum.GetName(typeof(InvoiceCategoryType), invoiceCategory); }
            set
            {
                if (value != null)
                    invoiceCategory = (InvoiceCategoryType)Enum.Parse(typeof(InvoiceCategoryType), value);
                else
                    invoiceCategory = InvoiceCategoryType.NORMAL;

            }
        }

        [ColumnLabel("Teljesítés")]
        [Description("Teljesítés dátuma")]
        public DateTime InvoiceDeliveryDate { get; set; }

        [ColumnLabel("Fiz.hat")]
        [Description("Fizetési határidő dátuma")]
        public DateTime PaymentDate { get; set; }


        private PaymentMethodType paymentMethod;

        [ColumnLabel("Fiz.mód")]
        [Description("Fizetési mód")]
        public string PaymentMethod
        {
            get { return Enum.GetName(typeof(PaymentMethodType), paymentMethod); }
            set
            {
                if (value != null)
                    paymentMethod = (PaymentMethodType)Enum.Parse(typeof(PaymentMethodType), value);
                else
                    paymentMethod = PaymentMethodType.OTHER;

            }
        }

        [ColumnLabel("Pénznem")]
        [Description("Pénznem")]
        private enCurrencyCodes currencyCode;
        public string CurrencyCode
        {
            get { return Enum.GetName(typeof(enCurrencyCodes), currencyCode); }
            set
            {
                if (value != null)
                    currencyCode = (enCurrencyCodes)Enum.Parse(typeof(enCurrencyCodes), value);
                else
                    currencyCode = enCurrencyCodes.HUF;

            }
        }

        [ColumnLabel("Árfolyam")]
        [Description("Árfolyam")]
        public decimal ExchangeRate { get; set; }

        [ColumnLabel("Közmű elszámolószámla")]
        [Description("Közmű elszámolószámla  ?(értéke false)")]
        public bool UtilitySettlementIndicator { get; set; } = false;

        private InvoiceAppearanceType invoiceAppearance;

        [ColumnLabel("Megjelenési forma")]
        [Description("A számla vagy módosító okirat megjelenési formája")]
        public string InvoiceAppearance
        {
            get { return Enum.GetName(typeof(InvoiceAppearanceType), invoiceAppearance); }
            set
            {
                if (value != null)
                    invoiceAppearance = (InvoiceAppearanceType)Enum.Parse(typeof(InvoiceAppearanceType), value);
                else
                    invoiceAppearance = InvoiceAppearanceType.PAPER;

            }
        }

        [ColumnLabel("Példány")]
        [Description("Nyomtatott példány száma")]
        public short Copies { get; set; }

        [ColumnLabel("Bevétel biz.")]
        [Description("Bevétel alapjául szolgáló bizonylat")]
        public string IncomingInvReference { get; set; }

        // Javítószámla
        [ColumnLabel("Eredeti számla ID")]
        [Description("Az eredeti számla ID,amelyre a módosítás vonatkozik")]
        public long? OriginalInvoiceID { get; set; } = null;

        [ColumnLabel("Eredeti számla sorszám")]
        [Description("Az eredeti számla sorszáma,amelyre a módosítás vonatkozik")]
        public string OriginalInvoiceNumber { get; set; }

        [ColumnLabel("Alapszámla nélküli módosítás?")]
        [Description("Alapszámla nélküli módosítás jelölése (értéke false)")]
        public bool ModifyWithoutMaster { get; set; }

        [ColumnLabel("Módosító okirat egyedi sorszáma")]
        [Description("A számlára vonatkozó módosító okirat egyedi sorszáma")]
        public short ModificationIndex { get; set; }

        [ColumnLabel("Megrendelés száma")]
        [Description("Megrendelés száma")]
        public string OrderNumber { get; set; }

        [ColumnLabel("Kapcsolódó számla")]
        [Description("Bevételhez kapcsolódó számla")]
        public string CustomerInvoiceNumber { get; set; }

        [ColumnLabel("Kedvezmény%")]
        [Description("A számlára adott teljes kedvezmény %")]
        public decimal InvoiceDiscountPercent { get; set; }
        [ColumnLabel("Kedvezmény")]
        [Description("A számlára adott teljes kedvezmény % értéke a számla pénznemében")]
        public decimal InvoiceDiscount { get; set; }

        [ColumnLabel("Kedvezmény HUF")]
        [Description("A számlára adott teljes kedvezmény % értéke fortintban")]
        public decimal InvoiceDiscountHUF { get; set; }

        [ColumnLabel("Nettó")]
        [Description("A számla nettó összege a számla pénznemében")]
        public decimal InvoiceNetAmount { get; set; }

        [ColumnLabel("Nettó HUF")]
        [Description("A számla nettó összege forintban")]
        public decimal InvoiceNetAmountHUF { get; set; }

        [ColumnLabel("Áfa")]
        [Description("A számla áfa összege a számla pénznemében")]
        public decimal InvoiceVatAmount { get; set; }
        [ColumnLabel("Áfa HUF")]
        [Description("A számla áfa összege forintban")]
        public decimal InvoiceVatAmountHUF { get; set; }
        [ColumnLabel("Bruttó")]
        [Description("A számla végösszege a számla pénznemében")]
        public decimal InvoiceGrossAmount { get; set; }
        [ColumnLabel("Bruttó HUF")]
        [Description("A számla végösszege forintban")]
        public decimal InvoiceGrossAmountHUF { get; set; }

        [ColumnLabel("Módosító bizonylat?")]
        [Description("Módosító bizonylat jelölése (értéke false)")]
        public bool InvoiceCorrection { get; set; }

        [ColumnLabel("Felhasználó ID")]
        [Description("Felhasználó ID")]
        public long? UserID { get; set; } = 0;

        [ColumnLabel("Munkaszám")]
        [Description("Munkaszám")]
        public string WorkNumber { get; set; }

        //relációk
        [ForeignKey("WarehouseID")]
        [ColumnLabel("Raktár")]
        [Description("Raktár")]
        public virtual Warehouse Warehouse { get; set; }

        [ForeignKey("SupplierID")]
        [ColumnLabel("Szállító")]
        [Description("Szállító")]
        public virtual Customer Supplier { get; set; }


        [ForeignKey("CustomerID")]
        [ColumnLabel("Ügyfél")]
        [Description("Ügyfél")]
        public virtual Customer Customer { get; set; }

        [ForeignKey("OriginalInvoiceID")]
        [ColumnLabel("Eredeti számla")]
        [Description("Eredeti számla")]
        public virtual Invoice OriginalInvoice { get; set; }

        [ColumnLabel("Egyéb adat")]
        [Description("A számlára vonatkozó egyéb adat")]
        public virtual IList<AdditionalInvoiceData> AdditionalInvoiceData { get; set; }

        [ColumnLabel("Áfa összesítő")]
        [Description("Összesítés áfa-mérték szerint")]
        public virtual IList<SummaryByVatRate> SummaryByVatRates { get; set; }


        [ForeignKey("UserID")]
        [ColumnLabel("Felhasználó")]
        [Description("Felhasználó")]
        public virtual Users User { get; set; }


        [ColumnLabel("Számlasorok")]
        [Description("Számlasorok")]
        public virtual IList<InvoiceLine> InvoiceLines { get; set; }

        [ColumnLabel("NAV adatküldések")]
        [Description("NAV adatküldések")]
        public virtual IList<NAVXChange> NAVXChanges { get; set; }

        [ColumnLabel("Pénzügyi kiegyenlítések")]
        [Description("Pénzügyi kiegyenlítések")]
        public virtual IList<InvPayment> InvPayments { get; set; }

    }
}
