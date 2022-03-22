using AutoMapper.Configuration.Conventions;
using bbxBE.Common.Attributes;
using bbxBE.Common.Enums;
using Newtonsoft.Json;
using System;
using System.Runtime.Serialization;

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
        public long ID { get; set; }
        public string ProductCode { get; set; }
        public string Description { get; set; }
        public string ProductGroup  { get; set; }
        public string Origin { get; set; }
  
        #region UnitOfMeasure
        [IgnoreDataMember]
        [NotDBField]
        [NotModelField]
        private enUnitOfMeasure _UnitOfMeasure { get; set; } = enUnitOfMeasure.PIECE;

        [DataMember]
        public string UnitOfMeasure
        {
            get { return Enum.GetName(typeof(enUnitOfMeasure), _UnitOfMeasure); }
            set
            {
                if (value != null)
                    _UnitOfMeasure = (enUnitOfMeasure)Enum.Parse(typeof(enUnitOfMeasure), value);
                else
                    _UnitOfMeasure = enUnitOfMeasure.PIECE;
            }
        }

        [DataMember]
        [NotDBField]
        public string UnitOfMeasureX { get { return Common.Utils.GetEnumDescription(_UnitOfMeasure); } }
        #endregion

        public decimal UnitPrice1 { get; set; }
        public decimal UnitPrice2 { get; set; }
        public decimal LatestSupplyPrice { get; set; }
        public bool IsStock { get; set; }
        public decimal MinStock { get; set; }
        public decimal OrdUnit { get; set; }
        public decimal ProductFee { get; set; }
        public bool Active { get; set; }
        public string VTSZ { get; set; }
        public string EAN { get; set; }
    }
}
