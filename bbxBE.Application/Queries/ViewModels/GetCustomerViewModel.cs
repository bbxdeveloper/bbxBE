using bbxBE.Common.Attributes;
using System.ComponentModel;

namespace bbxBE.Application.Queries.ViewModels
{
    /// <summary>
    /// MapToEntity properties marks the names in the output Entity
    /// Don't use with AutoMapper, but with <see cref="Domain.Extensions.EntityExtensions.MapFieldsByMapToAnnotation"/>
    /// In this case, <see cref="GetCustomerViewModel"/> will be the value for the TDestination parameter.
    /// </summary>
    public class GetCustomerViewModel
    {
        [MapToEntity("ID")]
        public long ID { get; set; }

        [MapToEntity("customerName")]
        public string CustomerName { get; set; }

        [MapToEntity("customerBankAccountNumber")]
        public string CustomerBankAccountNumber { get; set; }
        [MapToEntity("customerVatStatus")]
        public string CustomerVatStatus { get; set; }

        //     [NotDBField]
        [MapToEntity("TaxpayerNumber")]
        [NotDBFieldAttribute]
        public string TaxpayerNumber { get; set; }

        [MapToEntity("FullAddress")]
        [NotDBFieldAttribute]
        public string FullAddress { get; set; }



        //        [NotModelFieldAttribute]

        [MapToEntity("taxpayerId")]
        public string TaxpayerId { get; set; }
        //       [NotModelFieldAttribute]

        [MapToEntity("vatCode")]
        public string VatCode { get; set; }
        //      [NotModelFieldAttribute]
        [MapToEntity("countyCode")]
        public string CountyCode { get; set; }

        [MapToEntity("thirdStateTaxId")]
        public string ThirdStateTaxId { get; set; }

        [MapToEntity("countryCode")]
        public string CountryCode { get; set; }

        [MapToEntity("countryCodeX")]
        public string CountryCodeX { get; set; }

        [MapToEntity("region")]
        public string Region { get; set; }

        //      [NotModelFieldAttribute]
        [MapToEntity("postalCode")]
        public string PostalCode { get; set; }
        //      [NotModelFieldAttribute]
        [MapToEntity("city")]
        public string City { get; set; }
        //     [NotModelFieldAttribute]
        [MapToEntity("additionalAddressDetail")]
        public string AdditionalAddressDetail { get; set; }

        [MapToEntity("email")]
        [ColumnLabel("Email")]
        [Description("Email")]
        public string Email { get; set; }

        [MapToEntity("comment")]
        [ColumnLabel("Megjegyzés")]
        [Description("Megjegyzés")]
        public string Comment { get; set; }

        [ColumnLabel("Eladási ártípus")]
        [Description("Eladási ártípus")]
        public string UnitPriceType { get; set; }

        [ColumnLabel("Eladási ártípus megnevezés")]
        [Description("Eladási ártípus megnevezés")]
        public string UnitPriceTypeX { get; set; }

        [ColumnLabel("Fizetési határidő")]
        [Description("Fizetési határidő (napban)")]
        public short PaymentDays { get; set; }

        [ColumnLabel("Figyelmeztetés limit")]
        [Description("Figyelmeztetés limit")]
        public decimal? WarningLimit { get; set; }

        [ColumnLabel("Maximális limit")]
        [Description("Maximális limit")]
        public decimal? MaxLimit { get; set; }

        [ColumnLabel("Alap.fiz.mód")]
        [Description("Alapértelmezett fizetési mód")]
        public string DefPaymentMethod { get; set; }

        [ColumnLabel("Alap.fiz.mód megnev.")]
        [Description("Alapértelmezett fizetési mód megnevezése")]
        public string DefPaymentMethodX { get; set; }

        [ColumnLabel("Legutoljára megadott kedvezmény %")]
        [Description("Legutoljára megadott bizonylatkedvezmény %")]
        public short? LatestDiscountPercent { get; set; }


        [MapToEntity("isOwnData")]
        [ColumnLabel("Saját adat?")]
        [Description("Saját adat? (csak egy ilyen rekord lehet)")]
        public bool IsOwnData { get; set; }

    }
}
