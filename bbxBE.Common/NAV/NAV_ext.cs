using bbxBE.Common.NAV;
using System;
using System.Reflection;
using System.Xml.Serialization;

namespace bbxBE.Common.NAV
{
    public partial class CustomerInfoType
    {
        public CustomerInfoType()
        {
            customerVatData = null; //alapból ez legyen null
        }
    }


    public partial class InvoiceData
    {
        public InvoiceData()
        {
            completenessIndicator = false;
            invoiceMain = new InvoiceMainType();
        }
    }

    public partial class SupplierInfoType
    {
        public SupplierInfoType() { }

        public SupplierInfoType(bool p_individualExemptionSpecified)
        {
            individualExemptionSpecified = p_individualExemptionSpecified;
            if (p_individualExemptionSpecified)
            {
                individualExemption = true;         //Kiasadózó
            }

        }
    }

    public partial class InvoiceDetailType
    {
        public InvoiceDetailType() { }

        public InvoiceDetailType(bool p_invoiceDeliveryPeriodStartSpecified,
                bool p_invoiceDeliveryPeriodEndSpecified,
                bool p_invoiceAccountingDeliveryDateSpecified,
                bool p_periodicalSettlementSpecified,
                bool p_smallBusinessIndicatorSpecified,
                bool p_utilitySettlementIndicatorSpecified,
                bool p_selfBillingIndicatorSpecified,
                bool p_paymentMethodSpecified,
                bool p_paymentDateSpecified,
                bool p_cashAccountingIndicatorSpecified
        )
        {
            invoiceDeliveryPeriodStartSpecified = p_invoiceDeliveryPeriodStartSpecified;
            invoiceDeliveryPeriodEndSpecified = p_invoiceDeliveryPeriodEndSpecified;
            invoiceAccountingDeliveryDateSpecified = p_invoiceAccountingDeliveryDateSpecified;
            periodicalSettlementSpecified = p_periodicalSettlementSpecified;
            smallBusinessIndicatorSpecified = p_smallBusinessIndicatorSpecified;
            utilitySettlementIndicatorSpecified = p_utilitySettlementIndicatorSpecified;
            selfBillingIndicatorSpecified = p_selfBillingIndicatorSpecified;
            paymentMethodSpecified = p_paymentMethodSpecified;
            paymentDateSpecified = p_paymentDateSpecified;
            cashAccountingIndicatorSpecified = p_cashAccountingIndicatorSpecified;

        }

    }

    public partial class LineType
    {
        public LineType() { }

        public LineType(
            bool p_lineNatureIndicatorSpecified,
            bool p_quantitySpecified,
            bool p_unitOfMeasureSpecified,
            bool p_unitPriceSpecified,
            bool p_unitPriceHUFSpecified,
            bool p_intermediatedServiceSpecified,
            bool p_depositIndicatorSpecified,
            bool p_obligatedForProductFeeSpecified,
            bool p_GPCExciseSpecified,
            bool p_netaDeclarationSpecified
            )
        {
            lineNatureIndicatorSpecified = p_lineNatureIndicatorSpecified;
            quantitySpecified = p_quantitySpecified;
            unitOfMeasureSpecified = p_unitOfMeasureSpecified;
            unitPriceSpecified = p_unitPriceSpecified;
            unitPriceHUFSpecified = p_unitPriceHUFSpecified;
            intermediatedServiceSpecified = p_intermediatedServiceSpecified;
            depositIndicatorSpecified = p_depositIndicatorSpecified;
            obligatedForProductFeeSpecified = p_obligatedForProductFeeSpecified;
            GPCExciseSpecified = p_GPCExciseSpecified;
            netaDeclarationSpecified = p_netaDeclarationSpecified;
        }
    }
    public partial class SummaryType
    {
        public SummaryType()
        {
            summaryGrossData = new SummaryGrossDataType();
        }
    }

    public partial class SummaryByVatRateType
    {
        public SummaryByVatRateType() { }

        public SummaryByVatRateType(VatRateType p_vatrate)
        {
            vatRate = new VatRateType()
            {
                ItemElementName = p_vatrate.ItemElementName,
                Item = p_vatrate.Item
            };

            vatRateNetData = new VatRateNetDataType();
            vatRateVatData = new VatRateVatDataType();
            vatRateGrossData = new VatRateGrossDataType();
        }
    }

    public partial class LineAmountsNormalType
    {
        public LineAmountsNormalType()
        {
            lineNetAmountData = new LineNetAmountDataType();
            lineVatData = new LineVatDataType();
            lineVatRate = new VatRateType();
            lineGrossAmountData = new LineGrossAmountDataType();
        }
    }

    public partial class AggregateInvoiceLineDataType
    {
        public AggregateInvoiceLineDataType() { }

        public AggregateInvoiceLineDataType(bool p_lineExchangeRateSpecified)
        {
            lineExchangeRateSpecified = p_lineExchangeRateSpecified;
        }
    }

    public partial class ProductFeeTakeoverDataType
    {
        public ProductFeeTakeoverDataType() { }
        public ProductFeeTakeoverDataType(bool p_takeoverAmountSpecified)
        {
            takeoverAmountSpecified = p_takeoverAmountSpecified;
        }
    }

    public partial class UserHeaderType
    {
        public UserHeaderType() { }

        /*
         * A login tag a technikai felhasználó nevét tartalmazza. A login nevet a rendszer véletlenszerűen
            generálja a technikai felhasználó létrehozásakor 15 karakter hosszan. A login tag az
            authentikáció egyik eleme.
            2) A passwordHash a login tagban szereplő technikai felhasználó jelszavának SHA-512 hash
            értéke. A literál jelszót a technikai felhasználót létrehozó elsődleges felhasználó adja meg az
            Online Számla webfelületen. A passwordHash az authentikáció egyik eleme.
            3) A taxNumber azon adózó adószámának első 8 száma, aki nevében a technikai felhasználó
            tevékenykedik, és akihez tartozik. Csak magyar adószám az elfogadott.
            4) A requestSignature a kliens által generált aláírása az üzenetnek. Minden kéréshez kötelezően
            tartoznia kell egy requestSignature-nek. A szerver a kérésben szereplő adatok alapján elvégzi
            a saját requestSignature számítását, és csak akkor hajtja végre a kérést, ha a tárolt és kapott
            adatokból a helyes érték ténylegesen előállítható. A requestSignature számításáról a
            requestSignature számítása fejezet nyújt tájékoztatást
        */
        [XmlIgnore]
        public string RequestId { get; set; }

        [XmlIgnore]
        public DateTime TimeStamp { get; set; }

        public UserHeaderType(string p_requestId, DateTime p_timestamp, string p_taxnum, string p_login, string p_userPassword, string p_XMLSignKey)
        {
            RequestId = p_requestId;
            TimeStamp = p_timestamp;

            login = p_login;

            passwordHash = new CryptoType()
            {
                cryptoType = "SHA-512",
                Value = p_userPassword.SHA512()

            };
            taxNumber = p_taxnum;
            requestSignature = new CryptoType()
            {
                cryptoType = "SHA3-512",
                Value = (p_requestId + p_timestamp.ToString("yyyyMMddHHmmss") + p_XMLSignKey).SHA3_512()
            };
        }
    }



    public partial class BasicHeaderType
    {
        public BasicHeaderType() { }

        public BasicHeaderType(DateTime p_timestamp, string p_requestId)
        {
            this.timestamp = p_timestamp;
            requestVersion = "3.0";
            headerVersion = "1.0";
            requestId = p_requestId;
        }

    }

    public partial class BasicRequestType
    {
        public BasicRequestType() { }

        [XmlIgnore]
        public string RequestId { get { return header.requestId; } }

        public BasicRequestType(string p_taxnum, string p_techUserLogin, string p_techUserPwd, string p_XMLSignKey)
        {

            var now = DateTime.UtcNow;

            var requestId = now.GetRequestID();                         //A request mentjen a valós tick-el
            var timestamp = now.AddTicks(-1 * now.Ticks % 10000);        //így NAV_TIMESTAMPFORMAT -ban fog serializálódni
            header = new BasicHeaderType(timestamp, requestId);

            user = new UserHeaderType(requestId, timestamp, p_taxnum, p_techUserLogin, p_techUserPwd, p_XMLSignKey);

        }
    }

    public partial class BasicOnlineInvoiceRequestType : BasicRequestType
    {
        public BasicOnlineInvoiceRequestType()
        {
        }

        public BasicOnlineInvoiceRequestType(string p_taxnum, string p_techUserLogin, string p_techUserPwd, string p_XMLSignKey) :
               base(p_taxnum, p_techUserLogin, p_techUserPwd, p_XMLSignKey)
        {
            software = new SoftwareType()
            {
                softwareId = NAVGlobal.DEF_softwareId + "-" + Assembly.GetEntryAssembly().GetName().Version.ToString().Replace(".", "-"),         //A számlázó program azonosítója
                softwareName = Assembly.GetEntryAssembly().GetName().Name,                                  //A számlázó program neve
                softwareOperation = SoftwareOperationType.LOCAL_SOFTWARE,                                   //A számlázó program működési típusa
                softwareMainVersion = Assembly.GetEntryAssembly().GetName().Version.Major.ToString(),       //A számlázó program fő verziója
                softwareDevName = NAVGlobal.DEF_softwareDevName,                                            //A számlázó program fejlesztőjének neve
                softwareDevContact = NAVGlobal.DEF_softwareDevContact,                                      //A számlázó program fejlesztőjének működő email címe
                softwareDevCountryCode = NAVGlobal.NAV_HU,                                                  //A számlázó program fejlesztőjének országkódja
                softwareDevTaxNumber = null                                                                 //A számlázó program fejlesztőjének adószáma
            };
        }
    }

    public partial class TokenExchangeRequest : BasicOnlineInvoiceRequestType
    {
        public TokenExchangeRequest() { }

        //
        public TokenExchangeRequest(string p_taxnum, string p_techUserLogin, string p_techUserPwd, string p_XMLSignKey)
            : base(p_taxnum, p_techUserLogin, p_techUserPwd, p_XMLSignKey)
        {
        }
    }


    public partial class ManageInvoiceRequestType : BasicOnlineInvoiceRequestType
    {
        public ManageInvoiceRequestType() { }

        public ManageInvoiceRequestType(string p_taxnum, string p_techUserLogin, string p_techUserPwd, string p_XMLSignKey, string p_NAVExchangeKey, byte[] p_token)
            : base(p_taxnum, p_techUserLogin, p_techUserPwd, p_XMLSignKey)
        {
            /*
                Az exchangeToken tagban az adatbeküldést megelőzően a /tokenExchange operációban
                igényelt token dekódolt értékét kell küldeni. A dekódolást a tokent igénylő technikai
                felhasználó cserekulcsával kell elvégezni, AES-128 ECB titkosítási algoritmus alapján. A küldés
                időpontjában a tokennek a szerver oldalon érvényesnek kell lennie, lejárt vagy nem helyesen
                dekódolt tokennel adatszolgáltatás nem teljesíthető. Mivel a token nem technikai
                felhasználóra, hanem adózóra szól, más technikai felhasználó által igényelt token az
                adatszolgáltatásban felhasználható, feltéve, hogy a dekódolást annak a felhasználónak a
                cserekulcsával végezték, aki a tokent korábban igényelte.
            */

            exchangeToken = p_token.AES_128_ECBDecrypt(p_NAVExchangeKey);
        }
    }

    public partial class ManageInvoiceRequest : ManageInvoiceRequestType
    {
        public ManageInvoiceRequest() { }

        public ManageInvoiceRequest(string p_taxnum, string p_techUserLogin, string p_techUserPwd, string p_XMLSignKey, string p_NAVExchangeKey, byte[] p_token, string[] p_invoicesData)
            : base(p_taxnum, p_techUserLogin, p_techUserPwd, p_XMLSignKey, p_NAVExchangeKey, p_token)
        {

            invoiceOperations = new InvoiceOperationListType();
            invoiceOperations.compressedContent = false;        //nem tömörítünk

            invoiceOperations.invoiceOperation = new InvoiceOperationType[p_invoicesData.Length];
            var index = 0;
            var invoiceHash = "";

            foreach (var invoiceData in p_invoicesData)
            {
                var invoiceData64 = invoiceData.Base64Encode();

                var invoper = new InvoiceOperationType()
                {
                    index = index + 1,
                    invoiceOperation = (invoiceData.Contains(NAVGlobal.originalInvoiceNumber) ? ManageInvoiceOperationType.MODIFY : ManageInvoiceOperationType.CREATE),

                    /*
                     Az invoiceData tag egy különálló XML-t tartalmaz, BASE64 formátumra elkódolva. A belül lévő
                     XML-nek jól formázottnak és séma-konformnak kell lennie az invoiceData.xsd-re. A
                     számlaadatok feldolgozása aszinkron módon történik, a feldolgozási eredmény lekérése a
                     /queryTransactionStatus operációban lehetséges
                    */

                    //NAGYON FONOTOS !!!
                    // Az invoiceData XmlElementAttribute(DataType = "base64Binary") annotácioója 
                    // miatt a serialuzálás a tartalmat Base64-re konvertálja. Emiatt ide az eredeti 
                    // tartalom byte tömbjét kell írni!!!
                    //
                    invoiceData = System.Text.Encoding.UTF8.GetBytes(invoiceData),

                    /*
                     Az electronicInvoiceHash tag az InvoiceOperationType komplex típusban szerepel. 
                     A hashlenyomat megadása opcionális sémaszinten, azonban completenessIndicator jelölő true
                     értéke esetén kötelező. Az electronicInvoiceHash tag a típusából adódóan kiegészül egy új,
                     kötelező attibútummal: cryptoType néven. Elfogadott értéke az adott számla
                     completenessIndicator (adatszolgáltatás maga a számla) tag értékétől függ.
                     Ha a completenessIndicator értéke true, az egyetlen elfogadott érték az SHA3-512. Ilyen
                     esetben a hash értékének nagybetűs változatát kell beküldeni. Ha a completenessIndicator
                     jelölő értéke false, az elfogadott értékek: SHA3-512, SHA-256

                     Az electronicInvoiceHash (opcionális) mező kikerült API szintre, mivel a hash-értékét a teljes
                     invoiceData csomópont BASE64 értékéből kell számolni.
                     
                     A kiszámított index hash-eket a parciális hash mögé kell fűzni az indexek által leírt sorrendben. Az így
                     és sorrendben konkatenált string nagybetűs SHA3-512 hash eredménye lesz a requestSignature értéke.

                    */

                    /* EZ NEM KELL
                    electronicInvoiceHash = new CryptoType()
                    {
                        
                        cryptoType = "SHA3-512",
                        Value = NAVUtil.SHA3_512(invoiceData64)
                    }
                    */
                };

                invoiceHash += (invoper.invoiceOperation.ToString() + invoiceData64).SHA3_512().ToUpper();

                invoiceOperations.invoiceOperation[index++] = invoper;
            }

            this.user.requestSignature = new CryptoType()
            {
                cryptoType = "SHA3-512",
                Value = (this.user.RequestId + this.user.TimeStamp.ToString("yyyyMMddHHmmss") + p_XMLSignKey + invoiceHash).SHA3_512()
            };

        }
    }

    public partial class InvoiceAnnulmentType
    {
        public InvoiceAnnulmentType() { }

        public InvoiceAnnulmentType(string p_annulmentReference)
        {
            annulmentReference = p_annulmentReference;

            var now = DateTime.UtcNow;
            annulmentTimestamp = now.AddTicks(-1 * now.Ticks % 10000);        //így NAV_TIMESTAMPFORMAT -ban fog serializálódni
            annulmentCode = AnnulmentCodeType.ERRATIC_DATA;
            annulmentReason = NAVGlobal.DEF_annulmentReason;
        }
    }

    public partial class InvoiceAnnulment : InvoiceAnnulmentType
    {
        public InvoiceAnnulment() { }

        public InvoiceAnnulment(string p_annulmentReference)
            : base(p_annulmentReference)
        {
        }
    }

    public partial class ManageAnnulmentRequestType : BasicOnlineInvoiceRequestType
    {
        public ManageAnnulmentRequestType() { }

        public ManageAnnulmentRequestType(string p_taxnum, string p_techUserLogin, string p_techUserPwd, string p_XMLSignKey, string p_NAVExchangeKey, byte[] p_token)
        : base(p_taxnum, p_techUserLogin, p_techUserPwd, p_XMLSignKey)
        {
            /*
                Az exchangeToken tagban az adatbeküldést megelőzően a /tokenExchange operációban
                igényelt token dekódolt értékét kell küldeni. A dekódolást a tokent igénylő technikai
                felhasználó cserekulcsával kell elvégezni, AES-128 ECB titkosítási algoritmus alapján. A küldés
                időpontjában a tokennek a szerver oldalon érvényesnek kell lennie, lejárt vagy nem helyesen
                dekódolt tokennel adatszolgáltatás nem teljesíthető. Mivel a token nem technikai
                felhasználóra, hanem adózóra szól, más technikai felhasználó által igényelt token az
                adatszolgáltatásban felhasználható, feltéve, hogy a dekódolást annak a felhasználónak a
                cserekulcsával végezték, aki a tokent korábban igényelte.
            */

            exchangeToken = p_token.AES_128_ECBDecrypt( p_NAVExchangeKey);
        }
    }


    public partial class ManageAnnulmentRequest : ManageAnnulmentRequestType
    {
        public ManageAnnulmentRequest() { }

        public ManageAnnulmentRequest(string p_taxnum, string p_techUserLogin, string p_techUserPwd, string p_XMLSignKey, string p_NAVExchangeKey, byte[] p_token, string[] p_annulmentData)
            : base(p_taxnum, p_techUserLogin, p_techUserPwd, p_XMLSignKey, p_NAVExchangeKey, p_token)
        {
            annulmentOperations = new AnnulmentOperationType[p_annulmentData.Length];
            var index = 0;
            var annulmentHash = "";

            foreach (var annulmentData in p_annulmentData)
            {

                var annOper = new AnnulmentOperationType()
                {
                    index = index + 1,
                    annulmentOperation = ManageAnnulmentOperationType.ANNUL,

                    //NAGYON FONOTOS !!!
                    // Az annulmentData XmlElementAttribute(DataType = "base64Binary") annotációja 
                    // miatt a serialuzálás a tartalmat Base64-re konvertálja. Emiatt ide az eredeti 
                    // tartalom byte tömbjét kell írni!!!
                    //
                    invoiceAnnulment = System.Text.Encoding.UTF8.GetBytes(annulmentData),

                };

                var annulmentData64 = annulmentData.Base64Encode();
                annulmentHash += (annOper.annulmentOperation.ToString() + annulmentData64).SHA3_512().ToUpper();

                annulmentOperations[index++] = annOper;
            }

            this.user.requestSignature = new CryptoType()
            {
                cryptoType = "SHA3-512",
                Value = (this.user.RequestId + this.user.TimeStamp.ToString("yyyyMMddHHmmss") + p_XMLSignKey + annulmentHash).SHA3_512()
            };
        }
    }

    public partial class QueryTransactionStatusRequestType : BasicOnlineInvoiceRequestType
    {
        public QueryTransactionStatusRequestType() { }

        public QueryTransactionStatusRequestType(string p_taxnum, string p_techUserLogin, string p_techUserPwd, string p_XMLSignKey, string p_transactionID)
            : base(p_taxnum, p_techUserLogin, p_techUserPwd, p_XMLSignKey)
        {
            this.transactionId = p_transactionID;
        }
    }

    public partial class QueryTransactionStatusRequest : QueryTransactionStatusRequestType
    {
        public QueryTransactionStatusRequest() { }

        public QueryTransactionStatusRequest(string p_taxnum, string p_techUserLogin, string p_techUserPwd, string p_XMLSignKey, string p_transactionID)
            : base(p_taxnum, p_techUserLogin, p_techUserPwd, p_XMLSignKey, p_transactionID)
        {
        }
    }

   
    public partial class QueryInvoiceDigestRequestType : BasicOnlineInvoiceRequestType
    {
        public QueryInvoiceDigestRequestType() { }
        public QueryInvoiceDigestRequestType(string p_taxnum, string p_techUserLogin, string p_techUserPwd, string p_XMLSignKey,
                 int p_page, InvoiceDirectionType p_invoiceDirection, bool p_issue, DateTime p_dateFromUTC, DateTime p_dateToUTC)
               : base(p_taxnum, p_techUserLogin, p_techUserPwd, p_XMLSignKey)
        {
        }
    }
    public partial class QueryInvoiceDigestRequest : QueryInvoiceDigestRequestType
    {
        public QueryInvoiceDigestRequest() { }


        public QueryInvoiceDigestRequest(string p_taxnum, string p_techUserLogin, string p_techUserPwd, string p_XMLSignKey,
            int p_page, InvoiceDirectionType p_invoiceDirection, bool p_issue, DateTime p_dateFromUTC, DateTime p_dateToUTC)
          : base(p_taxnum, p_techUserLogin, p_techUserPwd, p_XMLSignKey, p_page, p_invoiceDirection,
                p_issue, p_dateFromUTC, p_dateToUTC)
        {
            this.page = p_page;
            this.invoiceDirection = p_invoiceDirection;
            var mandatoryparams = new MandatoryQueryParamsType();

            if (p_issue)        //Számla dátumára lekérdezés
            {
                DateIntervalParamType invoiceIssueDate = new DateIntervalParamType() { dateFrom = p_dateFromUTC, dateTo = p_dateToUTC };
                mandatoryparams.Item = invoiceIssueDate;
            }
            else
            {   // adatszolgáltatás időpontjára lekérdezés
                DateTimeIntervalParamType insDate = new DateTimeIntervalParamType() { dateTimeFrom = p_dateFromUTC, dateTimeTo = p_dateToUTC };
                mandatoryparams.Item = insDate;
            }

            this.invoiceQueryParams = new InvoiceQueryParamsType() { mandatoryQueryParams = mandatoryparams };

        }
    }

    public partial class QueryInvoiceDataRequestType : BasicOnlineInvoiceRequestType
    {
        public QueryInvoiceDataRequestType() { }
        public QueryInvoiceDataRequestType(string p_taxnum, string p_techUserLogin, string p_techUserPwd, string p_XMLSignKey,
                 string p_invoiceNumber, InvoiceDirectionType p_invoiceDirection)
               : base(p_taxnum, p_techUserLogin, p_techUserPwd, p_XMLSignKey)
        {
        }
    }
    public partial class QueryInvoiceDataRequest : QueryInvoiceDataRequestType
    {
        public QueryInvoiceDataRequest() { }


        public QueryInvoiceDataRequest(string p_taxnum, string p_techUserLogin, string p_techUserPwd, string p_XMLSignKey,
           string p_invoiceNumber, InvoiceDirectionType p_invoiceDirection)
          : base(p_taxnum, p_techUserLogin, p_techUserPwd, p_XMLSignKey, p_invoiceNumber, p_invoiceDirection)
        {
            this.invoiceNumberQuery = new InvoiceNumberQueryType() { invoiceNumber = p_invoiceNumber, invoiceDirection = p_invoiceDirection };
        }
    }
   public partial class QueryTaxpayerRequestType : BasicOnlineInvoiceRequestType
    {
        public QueryTaxpayerRequestType() { }
        public QueryTaxpayerRequestType(string p_taxnum, string p_techUserLogin, string p_techUserPwd, string p_XMLSignKey,
                 string p_taxNumber)
               : base(p_taxnum, p_techUserLogin, p_techUserPwd, p_XMLSignKey)
        {
        }
    }
    public partial class QueryTaxpayerRequest : QueryTaxpayerRequestType
    {
        public QueryTaxpayerRequest() { }


        public QueryTaxpayerRequest(string p_taxnum, string p_techUserLogin, string p_techUserPwd, string p_XMLSignKey,
           string p_taxNumber)
          : base(p_taxnum, p_techUserLogin, p_techUserPwd, p_XMLSignKey, p_taxNumber)
        {
            this.taxNumber = p_taxNumber;
        }
    }

}
