using AutoMapper.Configuration.Conventions;
using AxegazMobileSrv.Attrib;
using System;
using System.Text.Json.Serialization;

namespace bbxBE.Application.Queries.ViewModels
{
    /// <summary>
    /// MapTo properties marks the names in the output Entity
    /// Don't use with AutoMapper, but with <see cref="Domain.Extensions.EntityExtensions.MapFieldsByMapToAnnotation"/>
    /// In this case, <see cref="GetCustomerViewModel"/> will be the value for the TDestination parameter.
    /// </summary>
    public class GetCustomerViewModel
    {
        [MapTo("ID")]
        public string ID { get; set; }

        public string CustomerName { get; set; }

        public string CustomerBankAccountNumber { get; set; }
        public string CustomerVatStatus { get; set; }
        
   //     [NotDBField]
        public string TaxpayerNumber
        {
            get
            {
                return String.Format("{0,7}-{0,1}-{0,2}", TaxpayerId, VatCode, ThirdStateTaxId);
            }
        }

        [NotModelFieldAttribute]

        private string TaxpayerId { get; set; }
        [NotModelFieldAttribute]

        private string VatCode { get; set; }
        [NotModelFieldAttribute]
        private string CountyCode { get; set; }
        
        public string ThirdStateTaxId { get; set; }
        public string CountryCode { get; set; }
        public string Region { get; set; }
        public string PostalCode { get; set; }
        public string City { get; set; }
        public string AdditionalAddressDetail { get; set; }
    }
}
