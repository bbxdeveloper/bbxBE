using AutoMapper.Configuration.Conventions;

namespace bbxBE.Queries.ViewModels
{
    /// <summary>
    /// MapTo properties marks the names in the output Entity
    /// Don't use with AutoMapper, but with <see cref="Domain.Extensions.EntityExtensions.MapFieldsByMapToAnnotation"/>
    /// In this case, <see cref="GetUSR_USERViewModel"/> will be the value for the TDestination parameter.
    /// </summary>
    class GetCustomerViewModel
    {
        [MapTo("ID")]
        public string ID { get; set; }

        public string CustomerName { get; set; }

        public string CustomerBankAccountNumber { get; set; }
        public string CustomerVatStatus { get; set; }
        public string TaxpayerId { get; set; }
        public string VatCode { get; set; }
        public string ThirdStateTaxId { get; set; }
        public string CountryCode { get; set; }
        public string CountyCode { get; set; }
        public string Region { get; set; }
        public string PostalCode { get; set; }
        public string City { get; set; }
        public string AdditionalAddressDetail { get; set; }
    }
}
