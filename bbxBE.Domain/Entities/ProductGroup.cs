using bbxBE.Common.Attributes;
using bbxBE.Domain.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace bbxBE.Domain.Entities
{
    [Description("Termékcsoport")]
    public class ProductGroup : BaseEntity
    {
        [ColumnLabel("Kód")]
        [Description("Kód")]
        public string ProductGroupCode { get; set; }
        [ColumnLabel("Leírás")]
        [Description("Leírás")]
        public string ProductGroupDescription { get; set; }
    }
}
