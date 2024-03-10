using bbxBE.Common.Attributes;
using bbxBE.Domain.Common;
using Newtonsoft.Json;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace bbxBE.Domain.Entities
{
    [Description("Partnerkedvezmények")]
    public class CustDiscount : BaseEntity
    {
        [ColumnLabel("Ügyfél ID")]
        [Description("Ügyfél ID")]
        public long CustomerID { get; set; }

        [ColumnLabel("Termékcsoport ID")]
        [Description("Termékcsoport ID")]
        public long ProductGroupID { get; set; }

        [ColumnLabel("Árengedmény %")]
        [Description("Árengedmény %)")]
        public decimal Discount { get; set; }

        //relációk
        [JsonIgnore]                    //ignorálni kell, mert körkörös hivatkozást eredményez
        [ForeignKey("CustomerID")]
        [ColumnLabel("Ügyfél")]
        [Description("Ügyfél")]
        public virtual Customer Customer { get; set; }

        [JsonIgnore]                    //ignorálni kell, mert körkörös hivatkozást eredményez
        [ForeignKey("ProductGroupID")]
        [ColumnLabel("Termékcsoport")]
        [Description("Termékcsoport")]
        public virtual ProductGroup ProductGroup { get; set; }


    }
}
