using AutoMapper.Configuration.Conventions;
using bbxBE.Common.Attributes;
using System;
using System.ComponentModel;

namespace bbxBE.Application.Queries.ViewModels
{
    /// <summary>
    /// MapToEntity properties marks the names in the output Entity
    /// Don't use with AutoMapper, but with <see cref="Domain.Extensions.EntityExtensions.MapFieldsByMapToAnnotation"/>
    /// In this case, <see cref="GetInvCtrlPeriodViewModel"/> will be the value for the TDestination parameter.
    /// </summary>
    public class GetInvCtrlPeriodViewModel
    {
        [MapToEntity("ID")]
        public long ID { get; set; }
        [ColumnLabel("Raktár ID")]
        [Description("Raktár ID")]
        public long WarehouseID { get; set; }

        [ColumnLabel("Raktár")]
        [Description("Raktár")]
        public string Warehouse { get; set; }

        [ColumnLabel("Kezdődátum")]
        [Description("Kezdődátum")]
        public DateTime DateFrom { get; set; }

        [ColumnLabel("Végdátum")]
        [Description("Végdátum")]
        public DateTime DateTo { get; set; }
 
        [ColumnLabel("Lezárt?")]
        [Description("Lezárt?")]
        public bool Closed { get; set; }

    }
}
