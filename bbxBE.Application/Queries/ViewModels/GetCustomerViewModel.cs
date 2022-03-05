using AutoMapper.Configuration.Conventions;
using AxegazMobileSrv.Attrib;
using System;

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
        [MapTo("TaxpayerNumber")]
        public string TaxpayerNumber
        {
            get
            {
               return String.Format("{0,7}-{1,1}-{2,2}", TaxpayerId, VatCode, CountyCode);
            }
        }

        [MapTo("FullAddress")]
        public string FullAddress
        {
            get
            {
                return String.Format("{0} {1} {2}", PostalCode, City, AdditionalAddressDetail).Trim();
            }
        }


        [NotModelFieldAttribute]

        public string TaxpayerId { get; set; }
        [NotModelFieldAttribute]

        public string VatCode { get; set; }
        [NotModelFieldAttribute]
        public string CountyCode { get; set; }
        
        public string ThirdStateTaxId { get; set; }
        public string CountryCode { get; set; }
        public string Region { get; set; }
        public string PostalCode { get; set; }
        public string City { get; set; }
        public string AdditionalAddressDetail { get; set; }
    }
}
