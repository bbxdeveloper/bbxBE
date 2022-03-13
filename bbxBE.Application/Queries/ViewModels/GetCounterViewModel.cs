using AutoMapper.Configuration.Conventions;
using AxegazMobileSrv.Attrib;
using System;


namespace bbxBE.Application.Queries.ViewModels
{
    /// <summary>
    /// MapTo properties marks the names in the output Entity
    /// Don't use with AutoMapper, but with <see cref="Domain.Extensions.EntityExtensions.MapFieldsByMapToAnnotation"/>
    /// In this case, <see cref="GetCounterViewModel"/> will be the value for the TDestination parameter.
    /// </summary>
    public class GetCounterViewModel
    {
        [MapTo("ID")]
        public string ID { get; set; }

        public string CounterCode { get; set; }

        public string CounterDescription { get; set; }

        public string Warehouse { get; set; }
        public string Prefix { get; set; }
        public long CurrentNumber { get; set; }
        public int NumbepartLength { get; set; }
        public string Suffix { get; set; }


    }
}
