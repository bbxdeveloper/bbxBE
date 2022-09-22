using bbxBE.Common.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace bbxBE.Application.Queries.ViewModels
{
    public class GetCustDiscountViewModel
    {

        [ColumnLabel("Ügyfél ID")]
        [Description("Ügyfél ID")]
        public long CustomerID { get; set; }

        [ColumnLabel("Ügyfél")]
        [Description("Ügyfél")]
        public string Customer { get; set; }

        [ColumnLabel("Termékcsoport ID")]
        [Description("Termékcsoport ID")]
        public long ProductGroupID { get; set; }

        [ColumnLabel("Termékcsoport kód")]
        [Description("Termékcsoport kód")]
        public string ProductGroupCode { get; set; }

        [ColumnLabel("Termékcsoport")]
        [Description("Termékcsoport")]
        public string ProductGroup { get; set; }

        [ColumnLabel("Árengedmény %")]
        [Description("Árengedmény %)")]
        public decimal Discount { get; set; }

    }
}
