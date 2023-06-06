using bbxBE.Common.Attributes;
using System;
using System.ComponentModel;

namespace bbxBE.Application.Queries.ViewModels
{
    public class GetStockViewModel
    {
        [MapToEntity("ID")]
        public long ID { get; set; }

        [ColumnLabel("Raktár ID")]
        [Description("Raktár ID")]
        public long WarehouseID { get; set; }

        [ColumnLabel("Raktár")]
        [Description("Raktár")]
        public string Warehouse { get; set; }

        [ColumnLabel("Raktárkód")]
        [Description("Raktárkód")]
        public string WarehouseCode { get; set; }

        [ColumnLabel("Termék ID")]
        [Description("Termék ID")]
        public long ProductID { get; set; }

        [ColumnLabel("Termékkód")]
        [Description("Termékkód")]
        public string ProductCode { get; set; }

        [ColumnLabel("Megnevezés")]
        [Description("Termékmegnevezés, leírás")]
        public string Product { get; set; }

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

        [ColumnLabel("Elhelyezés")]
        [Description("Elhelyezés")]
        public string Location { get; set; }


    }
}
