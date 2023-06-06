﻿using bbxBE.Common.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace bbxBE.Application.Queries.ViewModels
{
    /// <summary>
    /// MapToEntity properties marks the names in the output Entity
    /// Don't use with AutoMapper, but with <see cref="Domain.Extensions.EntityExtensions.MapFieldsByMapToAnnotation"/>
    /// In this case, <see cref="GetWhsTransferViewModel"/> will be the value for the TDestination parameter.
    /// </summary>
    public class GetWhsTransferViewModel
    {
        public class WhsTransferLine
        {

            [MapToEntity("ID")]
            public long ID { get; set; }

            [ColumnLabel("Raktárközi átadás ID")]
            [Description("Raktárközi bizonylat átadás ID")]
            public long WhsTransferID { get; set; }

            [ColumnLabel("Sor száma")]
            [Description("Sor száma")]
            public short WhsTransferLineNumber { get; set; }

            [ColumnLabel("Termék ID")]
            [Description("Termék ID")]
            public long ProductID { get; set; }

            [ColumnLabel("Termékkód")]
            [Description("Termékkód")]
            public string ProductCode { get; set; }

            [ColumnLabel("Termék")]
            [Description("Termék")]
            public string Product { get; set; }

            [ColumnLabel("Mennyiség")]
            [Description("Mennyiség")]
            public decimal Quantity { get; set; }
            [ColumnLabel("Me")]
            [Description("Mennyiségi egység")]
            public string UnitOfMeasure { get; set; }

            [ColumnLabel("Me")]
            [Description("Mennyiségi egység")]
            public string UnitOfMeasureX { get; set; }

            [ColumnLabel("Aktuális beszerzési egységár")]
            [Description("Aktuális beszerzési egységár")]

            public decimal CurrAvgCost { get; set; }

            [ColumnLabel("Tétel értéke")]
            [Description("Tétel értéke")]

            public decimal ItemAmount { get { return Math.Round(Quantity * CurrAvgCost); } }
        }

        [MapToEntity("ID")]
        public long ID { get; set; }

        [ColumnLabel("Raktárközi átadás száma")]
        [Description("Raktárközi bizonylat átadás száma")]
        public string WhsTransferNumber { get; set; }

        [ColumnLabel("Kiadás raktár ID")]
        [Description("Kiadás raktár ID")]
        public long FromWarehouseID { get; set; }

        [ColumnLabel("Bevétel raktár ID")]
        [Description("Bevétel raktár ID")]
        public long ToWarehouseID { get; set; }

        [ColumnLabel("Kiadás dátuma")]
        [Description("Kiadás dátuma")]
        public DateTime TransferDate { get; set; }

        [ColumnLabel("Bevétel dátuma")]
        [Description("Bevétel dátuma")]
        public DateTime? TransferDateIn { get; set; }


        [ColumnLabel("Megjegyzés")]
        [Description("Megjegyzés")]
        public string Notice { get; set; }

        [ColumnLabel("Példány")]
        [Description("Nyomtatott példány száma")]
        public short Copies { get; set; }

        [ColumnLabel("Felhasználó ID")]
        [Description("Felhasználó ID")]
        public long? UserID { get; set; } = 0;

        [ColumnLabel("Státusz")]
        [Description("Státusz")]
        public string WhsTransferStatus { get; set; }

        [ColumnLabel("Státusz")]
        [Description("Státusz")]
        public string WhsTransferStatusX { get; set; }


        [ColumnLabel("Kiadás raktár")]
        [Description("Kiadás raktár")]
        public string FromWarehouse { get; set; }

        [ColumnLabel("Bevétel raktár")]
        [Description("Kiadás raktár")]
        public string ToWarehouse { get; set; }

        [ColumnLabel("Felhasználó")]
        [Description("Felhasználó")]
        public string User { get; set; }

        [ColumnLabel("Bizonylatsorok")]
        [Description("Bizonylatsorok")]
        public virtual ICollection<WhsTransferLine> WhsTransferLines { get; set; } = new List<WhsTransferLine>();

        [ColumnLabel("Bizonylat értéke")]
        [Description("Bizonylat értéke")]

        public decimal WhsTransferAmount { get { return WhsTransferLines.Sum(s => s.ItemAmount); } }

    }
}
