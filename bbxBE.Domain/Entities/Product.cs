using bbxBE.Common.Attributes;
using bbxBE.Domain.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace bbxBE.Domain.Entities
{
    [Description("Termék")]
    public class Product : BaseEntity
    {
        [ColumnLabel("Megnevezés")]
        [Description("Megnevezés, leírás")]
        public string Description { get; set; }

        [ColumnLabel("Termékcsoport ID")]
        [Description("Termékcsoport ID")]
        public long? ProductGroupID { get; set; }
        [ColumnLabel("Termékcsoport ID")]
        [Description("Termékcsoport ID")]
        public long? OriginID { get; set; }
        [ColumnLabel("Áfa ID")]
        [Description("Áfa ID")]
        public long VatRateID { get; set; }
        [ColumnLabel("Me.e")]
        [Description("Mennyiségi egység")]
        public string UnitOfMeasure { get; set; }
        [ColumnLabel("Ár1")]
        [Description("Ár1")]
        public decimal UnitPrice1 { get; set; }
        [ColumnLabel("Ár2")]
        [Description("Ár2")]
        public decimal UnitPrice2 { get; set; }
        [ColumnLabel("Legutolsó besz.ár")]
        [Description("Legutolsó beszerzési ár")]
        public decimal LatestSupplyPrice { get; set; }
        [ColumnLabel("Készletes?")]
        [Description("Készletes?")]
        public bool IsStock { get; set; }
        [ColumnLabel("Min.")]
        [Description("Minimumkészlet")]
        public decimal MinStock { get; set; }
        [ColumnLabel("Rendelési egység")]
        [Description("Rendelési egység")]
        public decimal OrdUnit { get; set; }
        [ColumnLabel("Termékdíj")]
        [Description("Termékdíj")]
        public decimal ProductFee { get; set; }
        [ColumnLabel("Termékértékesítés/szolgáltatásnyújtás")]
        [Description("Termékértékesítés vagy szolgáltatásnyújtás jelölése (egyelőre csak PRODUCT)")]
        public string NatureIndicator { get; set; }
        [ColumnLabel("Aktív?")]
        [Description("Aktív?")]
        public bool Active { get; set; }


        //relációk
        [ForeignKey("ProductGroupID")]
        [ColumnLabel("Termékcsoport")]
        [Description("Termékcsoport")]
        public ProductGroup ProductGroup { get; set; }

        [ForeignKey("OriginID")]
        [ColumnLabel("Származási hely")]
        [Description("Származási hely")]
        public Origin Origin { get; set; }

        [ForeignKey("VatRateID")]
        [ColumnLabel("Áfa leíró")]
        [Description("Áfa leíró")]
        public VatRate VatRate { get; set; }

        [ForeignKey("ProductID")]
        [ColumnLabel("Termékkódok")]
        [Description("Termékkódok")]
        public ICollection<ProductCode> ProductCodes { get; set; }
    }
}
