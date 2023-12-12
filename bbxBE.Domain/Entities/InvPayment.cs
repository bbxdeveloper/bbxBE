using bbxBE.Common.Attributes;
using bbxBE.Domain.Common;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace bbxBE.Domain.Entities
{
    [Description("Kiegyenlítések")]
    public class InvPayment : BaseEntity
    {
        [ColumnLabel("Számla ID")]
        [Description("Számla ID")]
        public long InvoiceID { get; set; }

        [ColumnLabel("Banki tranzakció")]
        [Description("Banki tranzakció azonosító")]
        public string BankTransaction { get; set; }

        [ColumnLabel("Dátum")]
        [Description("Banki tranzakció dátuma")]
        public DateTime InvPaymentDate { get; set; }

        [ColumnLabel("Összeg")]
        [Description("Banki tranzakció összege")]
        public decimal InvPaymentAmount { get; set; }


        [JsonIgnore]                    //ignorálni kell, mert körkörös hivatkozást eredményezhet
        [ForeignKey("InvoiceID")]
        [ColumnLabel("Számla")]
        [Description("Számla")]
        public virtual Invoice Invoice { get; set; }

    }
}
