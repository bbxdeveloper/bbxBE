using AutoMapper.Configuration.Conventions;
using System;


namespace bbxBE.Application.Queries.ViewModels
{
    /// <summary>
    /// MapTo properties marks the names in the output Entity
    /// Don't use with AutoMapper, but with <see cref="Domain.Extensions.EntityExtensions.MapFieldsByMapToAnnotation"/>
    /// In this case, <see cref="GetWarehouseViewModel"/> will be the value for the TDestination parameter.
    /// </summary>
    public class GetWarehouseViewModel
    {
        [MapTo("ID")]
        public long ID { get; set; }

        public string WarehouseCode { get; set; }

        public string WarehouseDescription { get; set; }
    }
}
