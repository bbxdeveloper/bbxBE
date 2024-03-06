using bbxBE.Common.Attributes;
using bbxBE.Domain.Common;
using Newtonsoft.Json;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace bbxBE.Domain.Entities
{
    [Description("Számla egyéb adat")]
    public class AdditionalInvoiceData : BaseEntity
    {

        [ColumnLabel("Számla ID")]
        [Description("Számla ID")]
        public long InvoiceID { get; set; }
        [ColumnLabel("Megnevezés")]
        [Description("MEgnevezés")]
        public string DataName { get; set; }
        [ColumnLabel("Leírás")]
        [Description("Leírás")]
        public string DataDescription { get; set; }
        [ColumnLabel("Érték")]
        [Description("Érték")]
        public string DataValue { get; set; }

        //Relációk
        [JsonIgnore]                    //ignorálni kell, mert körkörös hivatkozást eredményez
        [ForeignKey("InvoiceID")]
        [ColumnLabel("Számla")]
        [Description("Számla")]
        public Invoice Invoice { get; set; }
    }
}
