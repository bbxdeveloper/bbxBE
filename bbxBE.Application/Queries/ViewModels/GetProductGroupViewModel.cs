using bbxBE.Common.Attributes;
using System.ComponentModel;

namespace bbxBE.Application.Queries.ViewModels
{
    /// <summary>
    /// MapTo properties marks the names in the output Entity
    /// Don't use with AutoMapper, but with <see cref="Domain.Extensions.EntityExtensions.MapFieldsByMapToAnnotation"/>
    /// In this case, <see cref="GetProductGroupViewModel"/> will be the value for the TDestination parameter.
    /// </summary>
    public class GetProductGroupViewModel
    {
        [MapToEntityAttribute("ID")]
        public long ID { get; set; }

        [ColumnLabel("Kód")]
        [Description("Termékcsoport-kód")]
        public string ProductGroupCode { get; set; }

        [ColumnLabel("Megnevezés")]
        [Description("Termékcsoport megnevezés, leírás")]
        public string ProductGroupDescription { get; set; }

        [ColumnLabel("Árrés minimum%")]
        [Description("Árrés minimum%")]
        public decimal MinMargin { get; set; }
    }
}
