using bbxBE.Common.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace bbxBE.Application.Queries.ViewModels
{


    /// <summary>
    /// Egy szállítólevélről bekerült gyűjtőszámla tételek
    /// </summary>
    public class GetAggregateInvoiceDeliveryNoteViewModel
    {

        [Description("Gyűjtőszámla-sor")]
        public class InvoiceLine
        {

            [MapToEntity("ID")]
            public string ID { get; set; }

            [ColumnLabel("#")]
            [Description("Sor száma")]
            public short LineNumber { get; set; }

            [ColumnLabel("Termékkód")]
            [Description("Termékkód")]
            public string ProductCode { get; set; }

            [ColumnLabel("VTSZ")]
            [Description("Vámtarifa szám")]
            public string VTSZ { get; set; }

            [ColumnLabel("Áfa ID")]
            [Description("Áfa ID")]
            public long VatRateID { get; set; }

            [ColumnLabel("Áfaleíró-kód")]
            [Description("Áfaleíró-kód")]
            public string VatRateCode { get; set; }

            [ColumnLabel("Áfa%")]
            [Description("Az alkalmazott adó mértéke - Áfa tv. 169. § j)")]
            public decimal VatPercentage { get; set; }

            [ColumnLabel("Megnevezés")]
            [Description("A termék vagy szolgáltatás megnevezése")]
            public string LineDescription { get; set; }

            [ColumnLabel("Mennyiség")]
            [Description("Mennyiség")]
            public decimal Quantity { get; set; }
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

            [ColumnLabel("Ár")]
            [Description("Ár")]
            public decimal UnitPrice { get; set; }

            [ColumnLabel("Nettó érték")]
            [Description("Ár a számla pénznemében")]
            public decimal LineNetAmount { get; set; }
            [ColumnLabel("Áfa érték")]
            [Description("Áfa a számla pénznemében")]
            public decimal LineVatAmount { get; set; }
            [ColumnLabel("Bruttó érték")]
            [Description("Bruttó a számla pénznemében")]
            public decimal LineGrossAmountNormal { get; set; }

            [ColumnLabel("Ár felülvizsgálat?")]
            [Description("Ár felülvizsgálat?")]
            public bool? PriceReview { get; set; } = false;

        }

        [ColumnLabel("Szállítólevél ID")]
        [Description("Szállítólevél ID")]
        public long? DeliveryNoteInvoiceID { get; set; }

        [ColumnLabel("Szállítólevél szám")]
        [Description("Szállítólevél szám")]
        public string DeliveryNoteNumber { get; set; }


        [ColumnLabel("Szállítólevél teljesítés")]
        [Description("Szállítólevél teljesítés dátuma")]
        public DateTime DeliveryNoteDate { get; set; }


        [ColumnLabel("Nettó")]
        [Description("A szállítólevéről bekerült tétel nettó összege a számla pénznemében")]
        public decimal DeliveryNoteNetAmount { get; set; }

        [ColumnLabel("Nettó HUF")]
        [Description("A szállítólevéről bekerült tétel nettó összege a szállítólevél forintban")]
        public decimal DeliveryNoteNetAmountHUF { get; set; }

        [ColumnLabel("Kedvezmény %")]
        [Description("Bizonylatra adott kedvezmény %")]
        public decimal DeliveryNoteDiscountPercent { get; set; }


        [ColumnLabel("Kedvezmény")]
        [Description("A szállítólevéről bekerült tétel nettó kedvezmény a szállítólevél pénznemében")]
        public decimal DeliveryNoteDiscountAmount { get; set; }

        [ColumnLabel("Kedvezmény HUF")]
        [Description("A szállítólevéről bekerült tétel nettó kedvezmény a szállítólevél forintban")]
        public decimal DeliveryNoteDiscountAmountHUF { get; set; }

        [ColumnLabel("Nettó összesen")]
        [Description("A szállítólevéről bekerült tétel nettó kedvezménnyel csökkentett összege a szállítólevél pénznemében")]
        public decimal DeliveryNoteDiscountedNetAmount { get; set; }

        [ColumnLabel("Nettó összesen HUF")]
        [Description("A szállítólevéről bekerült tétel nettó kedvezménnyel csökkentett összege a szállítólevél forintban")]
        public decimal DeliveryNoteDiscountedNetAmountHUF { get; set; }

        [ColumnLabel("Számlasorok")]
        [Description("Számlasorok")]
        [MapToEntity("invoiceLines")]
        public List<GetAggregateInvoiceDeliveryNoteViewModel.InvoiceLine> InvoiceLines { get; set; } = new List<GetAggregateInvoiceDeliveryNoteViewModel.InvoiceLine>();

        [ColumnLabel("Szállítólvelek száma")]
        [Description("A gyűjtőszámlán lévő szállítólvelek száma")]
        public int DeliveryNotesCount { get; set; }

    }
}
