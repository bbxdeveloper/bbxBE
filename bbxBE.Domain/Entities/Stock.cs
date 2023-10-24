﻿using bbxBE.Common.Attributes;
using bbxBE.Domain.Common;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace bbxBE.Domain.Entities
{
    [Description("Raktárkészlet")]
    public class Stock : BaseEntity
    {
        [ColumnLabel("Raktár ID")]
        [Description("Raktár ID")]
        public long WarehouseID { get; set; }

        [ColumnLabel("Termék ID")]
        [Description("Termék ID")]
        public long ProductID { get; set; }

        [ColumnLabel("Valós")]
        [Description("Valós mennyiség")]
        public decimal RealQty { get; set; }


        [ColumnLabel("ELÁBÉ")]
        [Description("Átlagolt beszerzési egységár")]
        public decimal AvgCost { get; set; }

        [ColumnLabel("Bev.")]
        [Description("Legutolsó bevétel dátuma")]
        public DateTime? LatestIn { get; set; }

        [ColumnLabel("Kiad.")]
        [Description("Legutolsó kiadás dátuma")]
        public DateTime? LatestOut { get; set; }

        [ColumnLabel("Elhelyezés ID")]
        [Description("Elhelyezés ID")]
        public long? LocationID { get; set; }

        //relációk
        [ForeignKey("WarehouseID")]
        [ColumnLabel("Raktár")]
        [Description("Raktár")]
        public virtual Warehouse Warehouse { get; set; }

        [ForeignKey("ProductID")]
        [ColumnLabel("Termék")]
        [Description("Termék")]
        public virtual Product Product { get; set; }

        [ForeignKey("LocationID")]
        [ColumnLabel("Elhelyezés")]
        [Description("Elhelyezés")]
        public virtual Location Location { get; set; }


    }
}
