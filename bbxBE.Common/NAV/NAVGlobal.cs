namespace bbxBE.Common.NAV
{
    public class NAVGlobal
    {
        public const string DEF_softwareId_Prefix = "";
        public const string DEF_softwareId = "HU66259428X";

        public const string DEF_softwareOperation = "LOCAL_SOFTWARE";       //vagy : ONLINE_SERVICE
        public const string DEF_softwareDevName = "bbx";
        public const string DEF_softwareDevContact = "bbxdeveloper@gmail.com";

        public const string DEF_annulmentReason = "";

        public const string originalInvoiceNumber = "";


        public const string NAV_HU = "HU";

        public const string NAV_OK = "OK";
        public const string NAV_ERROR = "ERROR";

        public const string NAV_NAMESPACE = "http://schemas.nav.gov.hu/OSA/2.0/api";
        public const string NAV_NAMESPACE_DATA = "http://schemas.nav.gov.hu/OSA/2.0/data";
        public const string NAV_NAMESPACE_ANNUL = "http://schemas.nav.gov.hu/OSA/2.0/annul";
        public const string NAV_DATEFORMAT = "yyyy-MM-dd";
        public const string NAV_TIMESTAMPFORMAT = "yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fffK";
        public const int NAV_DIGITS = 2;


        public const string NAV_TOKENEXCHANGE = @"https://api.onlineszamla.nav.gov.hu/invoiceService/v3/tokenExchange";
        public const string NAV_TOKENEXCHANGE_TEST = @"https://api-test.onlineszamla.nav.gov.hu/invoiceService/v3/tokenExchange";

        public const string NAV_MANAGEINVOICE = @"https://api.onlineszamla.nav.gov.hu/invoiceService/v3/manageInvoice";
        public const string NAV_MANAGEINVOICE_TEST = @"https://api-test.onlineszamla.nav.gov.hu/invoiceService/v3/manageInvoice";

        public const string NAV_MANAGEANNULMENT = @"https://api.onlineszamla.nav.gov.hu/invoiceService/v3/manageAnnulment";
        public const string NAV_MANAGEANNULMENT_TEST = @"https://api-test.onlineszamla.nav.gov.hu/invoiceService/v3/manageAnnulment";

        public const string NAV_QUERYTRANSACTIONSTATUS = @"https://api.onlineszamla.nav.gov.hu/invoiceService/v3/queryTransactionStatus";
        public const string NAV_QUERYTRANSACTIONSTATUS_TEST = @"https://api-test.onlineszamla.nav.gov.hu/invoiceService/v3/queryTransactionStatus";

        public const string NAV_QUERYINVOICECHECK = @"https://api.onlineszamla.nav.gov.hu/invoiceService/v3/queryInvoiceCheck";
        public const string NAV_QUERYINVOICECHECK_TEST = @"https://api-test.onlineszamla.nav.gov.hu/invoiceService/v3/queryInvoiceCheck";

        public const string NAV_QUERYINVOICEDIGEST = @"https://api.onlineszamla.nav.gov.hu/invoiceService/v3/queryInvoiceDigest";
        public const string NAV_QUERYINVOICEDIGEST_TEST = @"https://api-test.onlineszamla.nav.gov.hu/invoiceService/v3/queryInvoiceDigest";

        public const string NAV_QUERYINVOICEDATA = @"https://api.onlineszamla.nav.gov.hu/invoiceService/v3/queryInvoiceData";
        public const string NAV_QUERYINVOICEDATA_TEST = @"https://api-test.onlineszamla.nav.gov.hu/invoiceService/v3/queryInvoiceData";

        public const string NAV_QUERYTAXPAYER = @"https://api.onlineszamla.nav.gov.hu/invoiceService/v3/queryTaxpayer";
        public const string NAV_QUERYTAXPAYER_TEST = @"https://api-test.onlineszamla.nav.gov.hu/invoiceService/v3/queryTaxpayer";


        public const string NAV_STATUS_OK = "OK";
        public const string NAV_STATUS_CREATED = "CREATED";
        public const string NAV_STATUS_SENT = "SENT";
        public const string NAV_STATUS_ERROR = "ERROR";
        public const string NAV_STATUS_FINISHED = "INVOICE";
    }
}
