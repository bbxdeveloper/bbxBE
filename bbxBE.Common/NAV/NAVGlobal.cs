using System.Xml;
using System.Xml.Serialization;

namespace bbxBE.Common.NAV
{
    public class NAVGlobal
    {

        public const string DEF_softwareId = "HU-BBX";

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



        public const string NAV_STATUS_OK = "OK";
        public const string NAV_STATUS_CREATED = "CREATED";
        public const string NAV_STATUS_SENT = "SENT";
        public const string NAV_STATUS_ERROR = "ERROR";
        public const string NAV_STATUS_FINISHED = "INVOICE";


        public static XmlSerializerNamespaces XMLNamespaces = new XmlSerializerNamespaces(
            new XmlQualifiedName[] {
                new XmlQualifiedName( "common", "http://schemas.nav.gov.hu/NTCA/1.0/common"),
                new XmlQualifiedName( "base", "http://schemas.nav.gov.hu/OSA/3.0/base")
                    });
    }
}
