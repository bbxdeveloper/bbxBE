﻿using bbxBE.Common.Attributes;
using System;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace bbxBE.Application.Queries.ViewModels
{
    /// <summary>
    /// MapToEntity properties marks the names in the output Entity
    /// Don't use with AutoMapper, but with <see cref="Domain.Extensions.EntityExtensions.MapFieldsByMapToAnnotation"/>
    /// In this case, <see cref="GetInvCtrlViewModel"/> will be the value for the TDestination parameter.
    /// </summary>
    public class GetInvCtrlViewModel
    {

        #region InvCtrlType

        [DataMember]
        [ColumnLabel("Típus")]
        [Description("Típus")]
        public string InvCtrlType { get; set; }


        [ColumnLabel("Típus")]
        [Description("Típus megnevezés")]
        [DataMember]
        [NotDBField]
        public string InvCtrlTypeX { get; set; }
        #endregion


        [MapToEntity("ID")]
        public long ID { get; set; }

        [ColumnLabel("Raktár ID")]
        [Description("Raktár ID")]
        public long WarehouseID { get; set; }

        [ColumnLabel("Raktár")]
        [Description("Raktár")]
        public string Warehouse { get; set; }

        [ColumnLabel("Leltáridőszak ID")]
        [Description("Leltáridőszak ID")]
        public long? InvCtlPeriodID { get; set; }       //Opcionális, hogy a folyamatos leltárat is kezelni lehessen

        [ColumnLabel("Leltáridőszak")]
        [Description("Leltáridőszak")]
        public string InvCtlPeriod { get; set; }       //Opcionális, hogy a folyamatos leltárat is kezelni lehessen

        [ColumnLabel("Termék ID")]
        [Description("Termék ID")]
        public long ProductID { get; set; }

        [ColumnLabel("Termékkód")]
        [Description("Termékkód")]
        public string ProductCode { get; set; }


        [ColumnLabel("Termék")]
        [Description("Termék")]
        public string Product { get; set; }


        [ColumnLabel("Leltározás dátuma")]
        [Description("Leltározás dátuma")]
        public DateTime InvCtrlDate { get; set; }


        [ColumnLabel("E.mennyiség")]
        [Description("Eredeti mennyiség")]
        public decimal ORealQty { get; set; }

        [ColumnLabel("Új mennyiség")]
        [Description("Új mennyiség")]
        public decimal NRealQty { get; set; }

        [ColumnLabel("ELÁBÉ")]
        [Description("Átlagolt beszerzési egységár")]
        public decimal AvgCost { get; set; }

        [ColumnLabel("E. érték")]
        [Description("Eredeti érték")]
        public decimal ORealAmount { get; set; }

        [ColumnLabel("Új érték")]
        [Description("Új érték")]
        public decimal NRealAmount { get; set; }

        [ColumnLabel("Felhasználó ID")]
        [Description("Felhasználó ID")]
        public long? UserID { get; set; } = 0;

        [ColumnLabel("Felhasználó")]
        [Description("Felhasználó")]
        public string UserName { get; set; }

    }
}
