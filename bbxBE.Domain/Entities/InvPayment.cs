using bbxBE.Common.Attributes;
using bbxBE.Common.Enums;
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

        [ColumnLabel("Fizetendő összeg")]
        [Description("Fizetendő (kiegyenlítetlen) összege")]
        public decimal PayableAmount { get; set; }

        [ColumnLabel("Fizetendő összeg HUF")]
        [Description("Fizetendő (kiegyenlítetlen) összege forintban")]
        public decimal PayableAmountHUF { get; set; }

        [ColumnLabel("Banki tranzakció")]
        [Description("Banki tranzakció azonosító")]
        public string BankTransaction { get; set; }

        [ColumnLabel("Dátum")]
        [Description("Banki tranzakció dátuma")]
        public DateTime InvPaymentDate { get; set; }

        [ColumnLabel("Összeg")]
        [Description("Banki tranzakció összege")]
        public decimal InvPaymentAmount { get; set; }

        [ColumnLabel("Pénznem")]
        [Description("Pénznem")]
        private enCurrencyCodes currencyCode;
        public string CurrencyCode
        {
            get { return Enum.GetName(typeof(enCurrencyCodes), currencyCode); }
            set
            {
                if (value != null)
                    currencyCode = (enCurrencyCodes)Enum.Parse(typeof(enCurrencyCodes), value);
                else
                    currencyCode = enCurrencyCodes.HUF;

            }
        }
        [ColumnLabel("Árfolyam")]
        [Description("Árfolyam")]
        public decimal ExchangeRate { get; set; }

        [ColumnLabel("Összeg HUF")]
        [Description("Banki tranzakció összege forintban")]
        public decimal InvPaymentAmountHUF { get; set; }

        [ColumnLabel("Felhasználó ID")]
        [Description("Felhasználó ID")]
        public long? UserID { get; set; } = 0;


        [JsonIgnore]                    //ignorálni kell, mert körkörös hivatkozást eredményezhet
        [ForeignKey("InvoiceID")]
        [ColumnLabel("Számla")]
        [Description("Számla")]
        public virtual Invoice Invoice { get; set; }


        [ForeignKey("UserID")]
        [ColumnLabel("Felhasználó")]
        [Description("Felhasználó")]
        public virtual Users User { get; set; }

    }
}
