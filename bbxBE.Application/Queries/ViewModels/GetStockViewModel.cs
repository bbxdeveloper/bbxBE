using bbxBE.Common.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace bbxBE.Application.Queries.ViewModels
{
    public class GetStockViewModel
    {
        [ColumnLabel("Raktár ID")]
        [Description("Raktár ID")]
        public long WarehouseID { get; set; }

        [ColumnLabel("Raktár")]
        [Description("Raktár")]
        public string Warehouse { get; set; }

        [ColumnLabel("Termék ID")]
        [Description("Termék ID")]
        public long ProductID { get; set; }

        [ColumnLabel("Termékkód")]
        [Description("Termékkód")]
        public string ProductCode { get; set; }

        [ColumnLabel("Termék")]
        [Description("Termék")]
        public string Product { get; set; }

        [ColumnLabel("Megnevezés")]
        [Description("Megnevezés, leírás")]
        public string Description { get; set; }

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

    }
}
