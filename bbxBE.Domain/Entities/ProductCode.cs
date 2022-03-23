using bbxBE.Common.Attributes;
using bbxBE.Domain.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace bbxBE.Domain.Entities
{
    [Description("Termékkódok")]
    public class ProductCode : BaseEntity
    {
        [ColumnLabel("Termék ID")]
        [Description("Termék ID")]
        public long ProductID { get; set; }
        [ColumnLabel("Kategória")]
        [Description("Kategória : OWN, VTSZ, EAN, stb...")]
        public string ProductCodeCategory { get; set; }
        [ColumnLabel("Érték")]
        [Description("Kódérték")]
        public string ProductCodeValue { get; set; }

        [ForeignKey("ProductID")]
        [ColumnLabel("Termékkódok")]
        [Description("Termékkódok")]
        public Product Product { get; set; }

    }
}
