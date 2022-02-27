using AutoMapper.Configuration.Conventions;
using AxegazMobileSrv.Attrib;
using System;


namespace bbxBE.Application.Queries.ViewModels
{
    /// <summary>
    /// MapTo properties marks the names in the output Entity
    /// Don't use with AutoMapper, but with <see cref="Domain.Extensions.EntityExtensions.MapFieldsByMapToAnnotation"/>
    /// In this case, <see cref="GetProductGroupViewModel"/> will be the value for the TDestination parameter.
    /// </summary>
    public class GetProductGroupViewModel
    {
        [MapTo("ID")]
        public string ID { get; set; }

        public string ProductGroupCode { get; set; }

        public string ProductGroupDescription { get; set; }
    }
}
