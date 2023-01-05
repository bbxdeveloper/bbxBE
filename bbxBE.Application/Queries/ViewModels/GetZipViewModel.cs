using AutoMapper.Configuration.Conventions;
using bbxBE.Common.Attributes;
using System;
using System.ComponentModel;

namespace bbxBE.Application.Queries.ViewModels
{
    /// <summary>
    /// MapToEntity properties marks the names in the output Entity
    /// Don't use with AutoMapper, but with <see cref="Domain.Extensions.EntityExtensions.MapFieldsByMapToAnnotation"/>
    /// In this case, <see cref="GetZipViewModel"/> will be the value for the TDestination parameter.
    /// </summary>
    public class GetZipViewModel
    {
        [MapToEntity("ID")]
        public long ID { get; set; }

        [ColumnLabel("Irányítószám")]
        [Description("Irányítószám")]
        public string ZipCode { get; set; }

        [ColumnLabel("Város")]
        [Description("Város")]
        public string ZipCity { get; set; }

        [ColumnLabel("Megye")]
        [Description("Megye")]
        public string ZipCounty { get; set; }

    }
}
