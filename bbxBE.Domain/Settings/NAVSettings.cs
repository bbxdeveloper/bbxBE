
namespace bbxBE.Domain.Settings
{
    public class NAVSettings
    {
        public string Taxnum { get; set; }
        public string TechUser { get; set; }
        public string TechUserPwd { get; set; }
        public string SignKey { get; set; }
        public string ExchangeKey { get; set; }

        public string TokenExchange { get; set; }
        public string ManageInvoice { get; set; }
        public string ManageAnnulment { get; set; }
        public string QueryTransactionStatus { get; set; }
        public string QueryInvoiceCheck { get; set; }
        public string QueryInvoiceDigest { get; set; }
        public string QueryInvoiceData { get; set; }
        public string QueryTaxPayer { get; set; }
        public int BatchRecordCnt { get; set; }
        public int ServiceRunIntervalMin { get; set; }
        public string NotificationEmailSubject { get; set; }
        public string NotificationEmailFrom { get; set; }
        public string NotificationEmailTo { get; set; }

    }
}