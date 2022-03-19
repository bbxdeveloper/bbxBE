using AutoMapper.Configuration.Conventions;
using System;


namespace bbxBE.Application.Queries.ViewModels
{
    /// <summary>
    /// MapTo properties marks the names in the output Entity
    /// Don't use with AutoMapper, but with <see cref="Domain.Extensions.EntityExtensions.MapFieldsByMapToAnnotation"/>
    /// In this case, <see cref="GetInvoiceViewModel"/> will be the value for the TDestination parameter.
    /// </summary>
    public class GetInvoiceViewModel
    {
        [MapTo("ID")]
        public string ID { get; set; }

        public string InvoiceCode { get; set; }

        public string InvoiceDescription { get; set; }

        public string Warehouse { get; set; }
        public string Prefix { get; set; }
        public long CurrentNumber { get; set; }
        public int NumbepartLength { get; set; }
        public string Suffix { get; set; }


    }
}
