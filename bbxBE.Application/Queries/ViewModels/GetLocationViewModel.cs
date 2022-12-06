using AutoMapper.Configuration.Conventions;
using bbxBE.Common.Attributes;
using System;

namespace bbxBE.Application.Queries.ViewModels
{
    /// <summary>
    /// MapToEntity properties marks the names in the output Entity
    /// Don't use with AutoMapper, but with <see cref="Domain.Extensions.EntityExtensions.MapFieldsByMapToAnnotation"/>
    /// In this case, <see cref="GetLocationViewModel"/> will be the value for the TDestination parameter.
    /// </summary>
    public class GetLocationViewModel
    {
        [MapToEntity("ID")]
        public long ID { get; set; }

        public string LocationCode { get; set; }

        public string LocationDescription { get; set; }
    }
}
