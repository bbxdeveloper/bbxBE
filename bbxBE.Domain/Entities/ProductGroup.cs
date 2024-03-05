using bbxBE.Common.Attributes;
using bbxBE.Domain.Common;
using System.Collections.Generic;
using System.ComponentModel;

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

        //relációk
        [ColumnLabel("Ügyfélkedvezmények")]
        [Description("Ügyfélkedvezmények")]
        public virtual ICollection<CustDiscount> CustDiscounts { get; set; }

    }
}
