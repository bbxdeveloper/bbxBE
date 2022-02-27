using AutoMapper.Configuration.Conventions;
using AxegazMobileSrv.Attrib;
using System;


namespace bbxBE.Application.Queries.ViewModels
{
    /// <summary>
    /// MapTo properties marks the names in the output Entity
    /// Don't use with AutoMapper, but with <see cref="Domain.Extensions.EntityExtensions.MapFieldsByMapToAnnotation"/>
    /// In this case, <see cref="GetProductViewModel"/> will be the value for the TDestination parameter.
    /// </summary>
    public class GetProductViewModel
    {
        [MapTo("ID")]
        public string ID { get; set; }

        public string ProductCode { get; set; }

        public string ProductDescription { get; set; }
    }
}
