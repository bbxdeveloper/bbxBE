using bbxBE.Common.Attributes;
using bbxBE.Domain.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

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


        [ColumnLabel("Krt.")]
        [Description("Karton szerinti mennyiség")]
        public decimal CalcQty { get; set; }

        [ColumnLabel("Valós")]
        [Description("Valós mennyiség")]
        public decimal RealQty { get; set; }

        [ColumnLabel("Kiadott")]
        [Description("Kiadott mennyiség")]
        public decimal OutQty { get; set; }

        [ColumnLabel("ELÁBÉ")]
        [Description("Átlagolt beszerzési egységár")]
        public decimal AvgCost { get; set; }

        [ColumnLabel("Bev.")]
        [Description("Legutolsó bevétel dátuma")]
        public DateTime? LatestIn { get; set; }

        [ColumnLabel("Kiad.")]
        [Description("Legutolsó kiadás dátuma")]
        public DateTime? LatestOut { get; set; }


        //relációk
        [ForeignKey("WarehouseID")]
        [ColumnLabel("Raktár")]
        [Description("Raktár")]
        public Warehouse Warehouse { get; set; }

        [ForeignKey("ProductID")]
        [ColumnLabel("Termék")]
        [Description("Termék")]
        public Product Product { get; set; }


    }
}
