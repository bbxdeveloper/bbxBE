using AutoMapper.Configuration.Conventions;
using AxegazMobileSrv.Attrib;
using System;

namespace bbxBE.Application.Queries.ViewModels
{
    /// <summary>
    /// MapTo properties marks the names in the output Entity
    /// Don't use with AutoMapper, but with <see cref="Domain.Extensions.EntityExtensions.MapFieldsByMapToAnnotation"/>
    /// In this case, <see cref="GetOriginViewModel"/> will be the value for the TDestination parameter.
    /// </summary>
    public class GetOriginViewModel
    {
        [MapTo("ID")]
        public long ID { get; set; }

        public string OriginCode { get; set; }

        public string OriginDescription { get; set; }
    }
}
