﻿using bbxBE.Common.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace bbxBE.Application.Queries.ViewModels
{


    /// <summary>
    /// MapToEntity properties marks the names in the output Entity
    /// Don't use with AutoMapper, but with <see cref="Domain.Extensions.EntityExtensions.MapFieldsByMapToAnnotation"/>
    /// In this case, <see cref="GetOfferViewModel"/> will be the value for the TDestination parameter.
    /// </summary>
    public class GetOfferViewModel
    {

        [Description("Árajánlat-sor")]
        public class OfferLine
        {
            [MapToEntity("ID")]
            public long ID { get; set; }

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

            [ColumnLabel("Árengedmény %")]
            [Description("Árengedmény %)")]
            public decimal Discount { get; set; }
            [ColumnLabel("Árengedmény megjelenítés?")]
            [Description("Árengedmény megjelenítés)")]

            public bool ShowDiscount { get; set; }
            [ColumnLabel("Áfa ID")]
            [Description("Áfa ID")]
            public long VatRateID { get; set; }

            [ColumnLabel("Áfaleíró-kód")]
            [Description("Áfaleíró-kód")]
            public string VatRateCode { get; set; }

            [ColumnLabel("Áfa%")]
            [Description("Az alkalmazott adó mértéke - Áfa tv. 169. § j)")]
            public decimal VatPercentage { get; set; }

            [ColumnLabel("Listaár")]                 //aktuális törzsbeli UnitPrice1 (HUF)
            [Description("Aktuális egységár")]
            public decimal UnitPrice1 { get; set; }

            [ColumnLabel("Egységár")]                 //aktuális törzsbeli UnitPrice2 (HUF)
            [Description("Aktuális listaár")]
            public decimal UnitPrice2 { get; set; }

            [ColumnLabel("Eredeti ár")]                 //a törzsbeli ár
            [Description("Eredeti ár")]
            public decimal OriginalUnitPrice { get; set; }


            [ColumnLabel("E/L")]                        //Eygségár/listaár flag
            [Description("Listaár/Egységár")]
            public bool UnitPriceSwitch { get; set; }

            [ColumnLabel("Ár")]
            [Description("Ár")]
            public decimal UnitPrice { get; set; }

            [ColumnLabel("Áfa értéke")]
            [Description("Áfa értéke")]
            [DataMember]
            [NotDBField]
            public decimal UnitVat { get { return Math.Round(Quantity * UnitPrice * VatPercentage, 2); } }

            [ColumnLabel("Bruttó ár")]
            [Description("Bruttó ár")]
            public decimal UnitGross { get; set; }

            [ColumnLabel("Nettó érték")]
            [Description("Nettó érték")]
            [DataMember]
            [NotDBField]

            public decimal NetAmount { get { return Math.Round(Quantity * UnitPrice, 2); } }

            [ColumnLabel("Bruttó érték")]
            [Description("Bruttó érték")]
            [DataMember]
            [NotDBField]
            public decimal BrtAmount { get { return Math.Round(Quantity * UnitPrice * (1 + VatPercentage), 1); } }


            [ColumnLabel("Bruttó érték megjelenítés?")]
            [Description("Bruttó érték megjelenítés?")]
            [DataMember]
            [NotDBField]
            public bool ShowBrtAmount { get; set; }

        }



        [MapToEntity("ID")]
        public long ID { get; set; }

        [ColumnLabel("Ajánlat száma")]
        [Description("Ajánlat száma")]
        public string OfferNumber { get; set; }

        [ColumnLabel("Ajánlat száma")]
        [Description("Ajánlat száma verziózva")]
        public string OfferNumberX { get; set; }

        #region Customer
        [ColumnLabel("Ügyfél ID")]
        [Description("Ügyfél ID")]
        public long CustomerID { get; set; }

        [ColumnLabel("Ügyfélnév")]
        [Description("Ügyfélnév")]
        public string CustomerName { get; set; }

        [ColumnLabel("Bankszámlaszám")]
        [Description("Ügyfél bankszámlaszám")]
        public string CustomerBankAccountNumber { get; set; }


        [ColumnLabel("Adószám")]
        [Description("Ügyfél adószám")]
        public string CustomerTaxpayerNumber { get; set; }

        [ColumnLabel("Országkód")]
        [Description("Ügyfél országkód")]
        public string CustomerCountryCode { get; set; }

        [ColumnLabel("IRSZ")]
        [Description("Ügyfél irányítószám")]
        public string CustomerPostalCode { get; set; }

        [ColumnLabel("Város")]
        [Description("Ügyfél város")]
        public string CustomerCity { get; set; }

        [ColumnLabel("Cím")]
        [Description("Ügyfélcím")]
        public string CustomerAdditionalAddressDetail { get; set; }



        [ColumnLabel("Megjegyzés")]
        [Description("Ügyfél megjegyzés")]
        public string CustomerComment { get; set; }

        #endregion

        [ColumnLabel("Kelt")]
        [Description("Kiállítás dátuma")]
        public DateTime OfferIssueDate { get; set; }

        [ColumnLabel("Érvényesség")]
        [Description("Érvényesség dátuma")]
        public DateTime OfferVaidityDate { get; set; }


        [ColumnLabel("Példány")]
        [Description("Nyomtatott példány száma")]
        public short Copies { get; set; }

        [ColumnLabel("Pénznem kód")]
        [Description("Pénznem kód")]
        public string CurrencyCode { get; set; }

        [ColumnLabel("Pénznem")]
        [Description("Pénznem")]
        public string CurrencyCodeX { get; set; }

        [ColumnLabel("Árfolyam")]
        [Description("Árfolyam")]
        public decimal ExchangeRate { get; set; }

        [ColumnLabel("Megjegyzés")]
        [Description("Megjegyzés")]
        public string Notice { get; set; }

        [ColumnLabel("Verzió")]
        [Description("Verzió")]
        public short OfferVersion { get; set; }

        [ColumnLabel("Legutolsó verzió?")]
        [Description("Legutolsó verzió?")]
        public bool LatestVersion { get; set; }

        [ColumnLabel("Bruttó árak?")]
        [Description("Bruttó árak megjelenítése?")]
        public bool IsBrutto { get; set; }


        [ColumnLabel("Nettó összérték")]
        [Description("Nettó összérték")]
        [DataMember]
        [NotDBField]
        //Tétellap nélküli lekérdezés esetén is át kell adni ezt a mezőt. Emiatt nem használható a OfferLines.Sum
        //public decimal SumNetAmount { get { return Math.Round(OfferLines.Sum(s => s.NetAmount), 0); } }
        public decimal SumNetAmount { get; set; }

        [ColumnLabel("Áfa összérték")]
        [Description("Áfa összérték")]
        [DataMember]
        [NotDBField]
        public decimal SumVatAmount { get { return SumBrtAmount - SumNetAmount; } }


        [ColumnLabel("Bruttó összérték")]
        [Description("Bruttó összérték")]
        [DataMember]
        [NotDBField]

        //Tétellap nélküli lekérdezés esetén is át kell adni ezt a mezőt. Emiatt nem használható a OfferLines.Sum
        //public decimal SumBrtAmount { get { return Math.Round(OfferLines.Sum(s => s.BrtAmount), 0); } }
        public decimal SumBrtAmount { get; set; }

        [ColumnLabel("Felhasználó ID")]
        [Description("Felhasználó ID")]
        public long UserID { get; set; }

        [ColumnLabel("Felhasználó")]
        [Description("Felhasználó")]
        [MapToEntity("UserName")]
        public string UserName { get; set; }

        [ColumnLabel("Ajánlatsorok")]
        [Description("Ajánlatsorok")]
        [MapToEntity("offerLines")]
        public List<GetOfferViewModel.OfferLine> OfferLines { get; set; } = new List<GetOfferViewModel.OfferLine>();

        public void FillOfferLines()
        {
            foreach (var item in OfferLines)
            {
                item.ShowBrtAmount = IsBrutto;
            }
        }
    }
}
