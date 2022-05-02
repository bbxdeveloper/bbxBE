using AutoMapper.Configuration.Conventions;
using bbxBE.Common.Attributes;
using System;
using System.ComponentModel;

namespace bbxBE.Application.Queries.ViewModels
{
    /// <summary>
    /// MapTo properties marks the names in the output Entity
    /// Don't use with AutoMapper, but with <see cref="Domain.Extensions.EntityExtensions.MapFieldsByMapToAnnotation"/>
    /// In this case, <see cref="GetVatRateViewModel"/> will be the value for the TDestination parameter.
    /// </summary>
    public class GetVatRateViewModel
    {
        [MapTo("ID")]
        public long ID { get; set; }

        [ColumnLabel("Áfaleíró-kód")]
        [Description("Áfaleíró-kód")]
        public string VatRateCode { get; set; }

        [ColumnLabel("Áfaleíró leírás")]
        [Description("Áfaleíró leírás")]
        public string VatRateDescription { get; set; }

        [ColumnLabel("Áfa mértéke")]
        [Description("Az alkalmazott adó mértéke - Áfa tv. 169. § j)")]
        public decimal VatPercentage { get; set; }
    }
}
