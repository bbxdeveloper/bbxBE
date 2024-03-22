
using bbxBE.Common.Consts;
using System;

namespace bbxBE.Domain.Settings
{
    public class NAVSettings
    {
        public string Taxnum { get { return Environment.GetEnvironmentVariable(bbxBEConsts.ENV_NAV_TAXNUM); } }
        public string TechUser { get { return Environment.GetEnvironmentVariable(bbxBEConsts.ENV_NAV_TECHUSER); } }
        public string TechUserPwd { get { return Environment.GetEnvironmentVariable(bbxBEConsts.ENV_NAV_TECHUSER_PWD); } }
        public string SignKey { get { return Environment.GetEnvironmentVariable(bbxBEConsts.ENV_NAV_SIGNKEY); } }
        public string ExchangeKey { get { return Environment.GetEnvironmentVariable(bbxBEConsts.ENV_NAV_EXCHANGEKEY); } }

        public string TokenExchange { get; set; }
        public string ManageInvoice { get; set; }
        public string ManageAnnulment { get; set; }
        public string QueryTransactionStatus { get; set; }
        public string QueryInvoiceCheck { get; set; }
        public string QueryInvoiceDigest { get; set; }
        public string QueryInvoiceData { get; set; }
        public string QueryTaxPayer { get; set; }
        public bool SendInvoicesToNAV { get; set; }
        public int BatchRecordCnt { get; set; }
        public int ServiceRunIntervalMin { get; set; }
        public string NotificationEmailSubject { get; set; }
        public string NotificationEmailFrom { get; set; }
        public string NotificationEmailTo { get; set; }

    }
}