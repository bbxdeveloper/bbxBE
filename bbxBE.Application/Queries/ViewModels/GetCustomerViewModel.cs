using AutoMapper.Configuration.Conventions;
using bbxBE.Common.Attributes;
using System;
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

        public string CustomerName { get; set; }

        public string CustomerBankAccountNumber { get; set; }
        public string CustomerVatStatus { get; set; }

        
        //     [NotDBField]
        [MapToEntity("TaxpayerNumber")]
        [NotDBFieldAttribute]
        public string TaxpayerNumber { get; set; }
        

        [MapToEntity("FullAddress")]
        [NotDBFieldAttribute]
        public string FullAddress { get; set; }
      


//        [NotModelFieldAttribute]

        public string TaxpayerId { get; set; }
 //       [NotModelFieldAttribute]

        public string VatCode { get; set; }
  //      [NotModelFieldAttribute]
        public string CountyCode { get; set; }
        
        public string ThirdStateTaxId { get; set; }
        public string CountryCode { get; set; }
        public string Region { get; set; }

  //      [NotModelFieldAttribute]
        public string PostalCode { get; set; }
  //      [NotModelFieldAttribute]
        public string City { get; set; }
   //     [NotModelFieldAttribute]
        public string AdditionalAddressDetail { get; set; }

        [ColumnLabel("Email")]
        [Description("Email")]
        public string Email { get; set; }

        [ColumnLabel("Saját adat?")]
        [Description("Saját adat? (csak egy ilyen rekord lehet)")]
        public bool IsOwnData { get; set; }

    }
}
