namespace bbxBE.Common.Consts
{
    public static class bbxBEConsts
    {
        public const int CodeLen = 12;
        public const int DescriptionLen = 80;

        public const string CNTRY_HU = "HU";
        public const string CONF_PwdSalt = "PwdSalt";

        public const string CONF_JWTSettings = "JWTSettings";
        public const string CONF_JWTKey = "Key";
        public const string CONF_JWTIssuer = "Issuer";
        public const string CONF_JWTAudience = "Audience";
        public const string CONF_JWTDurationInMinutes = "DurationInMinutes";

        public const string DEF_ENCODING = "ISO-8859-2";
        public const string DEF_CSVSEP = ";";
        public const string DEF_DATEFORMAT = "yyyy-MM-dd";
        public const string DEF_NUMFORMAT = "#,#0.00";
        public const string DEF_INTFORMAT = "#,#0";
        public const string DEF_DATETIMEFORMAT = "yyyy.MM.dd HH:mm:ss";
        public const string DEF_TRUE = "true";
        public const string DEF_FALSE = "false";


        public const string DEF_CUSTOMERLOCK_KEY = "CUST_";
        public const string DEF_NAVLOCK_KEY = "NAV_";

        public const string CONF_CacheSettings = "CacheSettings";
        public const string CONF_WaitForCacheInSeconds = "WaitForCacheInSeconds";

        public const int WaitForExpiringDataSec = 3;
        public const int ExpiringDataMaxCount = 5000;
        public const int CustomerLockExpirationSec = 900; //15 perc
        public const string DEF_CUSTLOCK = "custlock";

        public const int NAVLockExpoirationSec = 60; //1 perc
        public const int CustomerLockExpoirationSec = 900; //15 perc

        public const string ERR_REQUIRED = "{PropertyName}: mező kitöltése kötelező.";
        public const string TOKEN_PROPERTYNAME = "{PropertyName}";
        public const string ERR_GREATGHERTHANZERO = "{PropertyName}: mező értéke csak pozitív szám lehet.";

        public const string ERR_MAXLEN = "{PropertyName}: mező mérete nem lehet több, mint {MaxLength}.";
        public const string ERR_RANGE = "{PropertyName}: mező értéke {from} {to} között lehet. A megadott érték:{PropertyValue}.";
        public const string ERR_DATEINTERVAL = "{PropertyName}: helytelen időszak.";


        public const string ERR_EXISTS = "{PropertyName}: már létezik.";
        public const string ERR_INVALIDEMAIL = "{PropertyName}: érvénytelen email cím.";
        public const string ERR_ERBANK = "{PropertyName}: érvénytelen bankszámlaszám.";
        public const string ERR_INVPRODUCTCROUPCODE = "{PropertyName}: érvénytelen termékcsoport.";
        public const string ERR_INVORIGINCODE = "{PropertyName}: érvénytelen származási hely.";
        public const string ERR_INVVATRATECODE = "{PropertyName}: érvénytelen áfakód.";
        public const string ERR_INVUNITOFMEASURE = "{PropertyName}: érvénytelen mennyiségi egység:{PropertyValue} ";
        public const string ERR_INVUNITOFMEASURE2 = "Sor: {0}, termék:{1} : érvénytelen mennyiségi egység:{2} ";
        public const string ERR_DETAIL_PREF = "Sor: {0}, termék:{1}";
        public const string ERR_DISCOUNT = "Sor: {0}, termék:{1}";
        public const string ERR_NOINPUTDATA = "Nincs input adat!";


        public const string ERR_INVPAYMENTMETHOD = "{PropertyName}: érvénytelen fizetési mód:{PropertyValue} ";
        public const string ERR_EXCHANGERATE = "{PropertyName}: Érvénytelen árfolyam::{PropertyValue} ";
        public const string ERR_INVCURRENCY = "{PropertyName}: érvénytelen pénznem:{PropertyValue}";
        public const string ERR_PRODNOTFOUND = "Termék nem található, ID:{0} ";
        public const string ERR_PRODCODENOTFOUND = "Termék nem található, kód:{0} ";
        public const string ERR_VATRATECODENOTFOUND = "Áfakód nem található, kód:{0} ";
        public const string ERR_VATRATECODENOTFOUND2 = "Áfakód nem található, ID:{0} ";
        public const string ERR_CUSTNOTFOUND = "Partneradat nem található, ID:{0} ";
        public const string ERR_OWNNOTFOUND = "Saját adat nem található!";
        public const string ERR_INVALIDFORMAT = "{PropertyName}: érvénytelen formátum!";
        public const string ERR_INVALIDCONTENT = "{PropertyName}: érvénytelen tartalom!";
        public const string ERR_COUNTERNOTFOUND = "Bizonylati tömb nem található, ID:{0} ";
        public const string ERR_COUNTERNOTFOUND2 = "Bizonylati tömb nem található, Kód:{0}, Raktár ID:{1} ";
        public const string ERR_PRODUCTGROUPNOTFOUND = "Termékcsoport nem található, ID:{0} ";
        public const string ERR_ORIGINNOTFOUND = "Származási hely nem található, ID:{0} ";
        public const string ERR_OFFERNOTFOUND = "Árajánlat nem található, ID:{0} ";
        public const string ERR_INVOICENOTFOUND = "Számla/szállítólevél nem található, ID:{0} ";
        public const string ERR_INVOICENOTFOUND2 = "Számla nem található, bizonylatszám:{0} ";
        public const string ERR_INVOICEISNULL = "Számla adat nincs megadva (null)!";
        public const string ERR_VATRATENOTFOUND = "Áfakód nem található, ID:{0} ";
        public const string ERR_USERNOTFOUND = "Felhasználó nem található, ID:{0} ";
        public const string ERR_USERNOTFOUND2 = "Felhasználó nem található, név:{0} ";
        public const string ERR_WAREHOUSENOTFOUND = "Raktár nem található, Kód:{0}";
        public const string ERR_CUSTOMERNOTFOUND = "Partner nem található, ID:{0}";
        public const string ERR_STOCKNOTFOUND = "Raktárkészlet nem található, ID:{0} ";
        public const string ERR_STOCKCARDNOTFOUND = "Készletkarton nem található, ID:{0} ";
        public const string ERR_LOCATIONOTFOUND = "Helyiség nem található, ID:{0} ";
        public const string ERR_XCHANGEOTFOUND = "NAV adatcsere nem található, ID:{0} ";
        public const string ERR_INVOICELINENOTFOUND = "Bizonylatsor nem található, ID:{0}, bizonylat:{1}, sor ID:{2}";
        public const string ERR_ORIGINALINVOICENOTFOUND = "Eredeti bizonylat nem található, ID:{0}";
        public const string ERR_WRONGCORRECTIONQTY = "Eredeti bizonylaton lévőnél nagyobb mennyiség levonása! Termékkód:{0}, eredeti mennyiség:{1:#,#0.00}, előzőleg lejavítva:{2:#,#0.00}, javítószámlán:{3:#,#0.00}";
        public const string ERR_CORRECTIONUNKOWNPROD = "Eredeti bizonylaton nem létező termék: ID:{0}, Kód:{1}";
        public const string ERR_WHSTRANSFERNOTFOUND = "Raktárközi átadás nem található, ID:{0} ";
        public const string ERR_WHSTRANSFERSAMEWHS = "A kiadási- és bevételi raktár nem egyezhet meg!";
        public const string ERR_WHSTRANSFERWRONGINDATE = "Raktárközi átadás dátuma nem lehet korábbi, mint a kiadás:{0:yyyy.MM.dd} ";
        public const string ERR_WHSTRANSFERWRONGSTATE = "Feldolgozni/véglegesíteni csak 'Elkészült' státuszú tételt lehet!";
        public const string ERR_INVWHSTRANSFERSTATUS = "{PropertyName}: érvénytelen raktárközi átadás státusz:{PropertyValue} ";
        public const string ERR_INVOICEREPORT_NULL = "Invoice report result is null!";
        public const string ERR_INVOICEREPORT = "Invoice report finished with error:{0}";
        public const string ERR_OFFERREPORT_NULL = "Offer report result is null!";
        public const string ERR_OFFERREPORT = "Offer report finished with error:{0}";
        public const string ERR_CUSTOMERINVOICESUMMARYREPORT_NULL = "CustomerInvoiceSummary report result is null!";
        public const string ERR_CUSTOMERINVOICESUMMARYREPORT = "CustomerInvoiceSummary report finished with error:{0}";
        public const string ERR_CUSTOMERLATESTDISCOUNTPERCENT = "A legutoljára megadott kedvezmény %-os értékének 0 és 100 közé kell esnie!";

        public const string ERR_VALIDATION = "Egy vagy több validációs hiba történt.";

        public const string ERR_FILELISTISNULL = "A File lista nem lehet üres!";
        public const string ERR_FILELISTISEMPTY = "A File lista nem lehet NULL!";
        public const string ERR_FILELISTCONUTER = "2 File-t kell feltölteni!";
        public const string ERR_FILESIZE = "Üres file lett feltöltve!";


        public const string ERR_NOCACHEQUERY = "Nincs lekérdezés a gyorsítótárhoz!";
        public const string ERR_LOCKEDCACHE = "Gyorsítótár feltöltés alatt ! A műveletet később végrehajtható.";
        public const string ERR_LOCK = "Zárolás nem sikerült. Kulcs foglalt:{0}";
        public const string ERR_UNLOCK = "Zárolás felszabadítása nem sikerült. Kulcs nem létezik:{0}";


        public const string ERR_EXPIRINGDATA_FULL = "Az ExpiringData tároló megtelt! Max. elemszám:{0}";

        public const string ERR_INV_DATE1 = "{PropertyName}: A számla dátuma nem lehet korábi, mint a teljesítés dátuma";
        public const string ERR_INV_DATE2 = "{PropertyName}: A számla dátuma nem lehet későbbi, mint a fizetési határidő";
        public const string ERR_INV_LINES = "{PropertyName}: A számlán nincs tételsor";
        public const string ERR_INV_VATSUMS = "{PropertyName}: A számlán nincs áfánkénti összesítő";

        public const string ERR_CST_OWNEXISTS = "{PropertyName}: Saját adat már létezik.";
        public const string ERR_CST_WRONGCOUNTRY = "{PropertyName}: Helytelen országkód:{PropertyValue}";
        public const string ERR_CST_WRONGUNITPRICETYPE = "{PropertyName}: Helytelen eladási ártípus:{PropertyValue}";
        public const string ERR_CST_WRONGDEFPAYMENTTYPE = "{PropertyName}: Helytelen alapértelmezett fizetési mód:{PropertyValue}";

        public const string ERR_CST_TAXNUMBER_INV = "{PropertyName}: Az adószám csak magyarországi partnerek esetén értelmezett.";
        public const string ERR_CST_TAXNUMBER_INV2 = "{PropertyName}: érvénytelen formátum/tartalom.";


        public const string ERR_OFFER_DATE1 = "{PropertyName}: A lejárati dátum nem lehet korábbi, mint a ajánlat kibocsátásának dátuma!";

        public const string NAV_INVDIRECTION = "{PropertyName}: Érvénytelen bizonylatirány:{PropertyValue}";
        public const string ERR_INVOICETYPE = "Érvénytelen bizonylattípus:{0}";

        public const string ERR_INVCTRLPERIOD_DATE1 = "{PropertyName}: Helytelen leltáridőszak!";
        public const string ERR_INVCTRLPERIOD_DATE2 = "{PropertyName}: A megadott leltáridőszak ütközik más időszakkal!";
        public const string ERR_INVCTRLPERIODNOTFOUND = "Leltáridőszak nem található, ID:{0} ";
        public const string ERR_INVCTRLPERIOD_CANTBEDELETED = "A leltáridőszak már nem törölhető!";
        public const string ERR_INVCTRLPERIOD_CANTBEUPDATED = "A leltáridőszak már nem módosítható!";
        public const string ERR_INVCTRLPERIOD_CANTBECLOSED = "A leltáridőszak nem zárható le!";
        public const string ERR_INVCTRLPERIOD_NOTCLOSED = "A leltáridőszak nincs lezárva!";
        public const string ERR_INVCTRLPERIOD_ALREADYCLOSED = "A leltáridőszak már lezárt!";
        public const string ERR_INVAGGR_RELATED_NOT_ASSIGNED = "A gyűjtőszámlán lévő tételhez nincs szállítólevél rendelve! Szállítólevél:{0}, sor száma:{1}, temékkód:{2} ";
        public const string ERR_INVAGGR_RELATED_NOT_FOUND = "A gyűjtőszámlán lévő tételhez nem található szállítólevél! Szállítólevél:{0}, sor száma:{1}, temékkód:{2}, kapcsolt szállító-sor ID:{3} ";
        public const string ERR_INVAGGR_WRONG_AGGR_QTY = "A gyűjtőszámlán lévő mennyiség több, mint a szállítólevélen lévő! Szállítólevél:{0}, sor száma:{1}, temékkód:{2}, gyűjtőszámla menny.:{3:#,#0.00}, szállítólevél menny.:{4:#,#0.00}, kapcsolt szállító-sor ID:{5} ";

        public const string ERR_INVCTRLNOTFOUND = "Leltári tétel nem található, ID:{0} ";
        public const string ERR_INVCTRL_DATE = "{PropertyName}:Helytelen leltárdátum!";


        public const string PROD_IMPORT_RESULT = "***Product import***\n" +
                                                    "Összes elem    :{0}\n" +
                                                    "Létrehozva     :{1}\n" +
                                                    "Aktualuzálva   :{2}\n" +
                                                    "Hibás          :{3}";


        public const string NAV_TOKENEXCHANGE_ERR = "{0} NAV tokenExchange error result:{1}";
        public const string NAV_MANAGEINVOICE_ERR = "{0} NAV manageInvoice error result:{1}";
        public const string NAV_QUERYTRANSACTION_ERR = "{0} NAV queryTransaction error (1) result:{1}";
        public const string NAV_QUERYTRANSACTION_ERR2 = "{0} NAV queryTransaction error (2) result:{1}";

        public const string NAV_QINVDIGEST_OK = "{0} NAV queryInvoiceDigest OK, invoiceDirection:{1}, issue:{2}, dateFromUTC:{3}, dateToUTC:{4}";
        public const string NAV_QINVDIGEST_FIRSTPG_ERR = "{0} NAV queryInvoiceDigest firstpage error result:{1}";
        public const string NAV_QINVDIGEST_NEXTPG_ERR = "{0} NAV queryInvoiceDigest nextpage error result:{1}";

        public const string NAV_QINVDATA_OK = "{0} NAV test result: funcCode:{1}, errorCode:{2}, message:{3}";
        public const string NAV_QINVDATA_ERR = "{0} NAV queryInvoiceData error result:{1}";
        public const string NAV_QINVDATA_NOTFND_ERR = "{0} invoice not found:{1}";

        public const string NAV_QTAXPAYER_ERR = "{0} NAV QueryTaxpayer, taxnumber:{1} error result:{2}";
        public const string NAV_QTAXPAYER_TOKEN_ERR = "{0} NAV QueryTaxpayer token, taxnumber:{1} error result:{2}";
        public const string NAV_QTAXPAYERT_OK = "{0} NAV QueryTaxpayer OK, taxnumber:{1}";

        public const string NAV_SENDINFO1 = "NAV küldés elindult, bizonylat:{0}, művelet:{1}";
        public const string NAV_SENDINFO2 = "NAV küldés eredmény, bizonylat:{0}, művelet:{1}, eredmény:{2}";

        public const string NAV_QUERYINFO1 = "NAV lekérdezés elindult, bizonylat:{0}, művelet:{1}, tranzakció ID:{2}";
        public const string NAV_QUERYINFO2 = "NAV lekérdezés eredmény, bizonylat:{0}, művelet:{1}, eredmény:{2}, tranzakció ID:{3}";

        public const string ERR_NAV_TAXPAYER = "A lekérdezéshez használt adószámnak 8 jegyűnek kell lennie!";
        public const string ERR_NAVXML_NOINV = "Csak kimenő számláról készíthető NAV beküldő XML, ID:{0}, bizonylatszám:{1}";
        public const string ERR_NAVXML_VATRATEMISSING = "Hiányzó áfa a kedvezmény XML meghatározásánál, bizonylatszám:{0}";
        public const string ERR_NAVINV = "Csak kimenő számlabizonylat küldhető a NAV felé, bizonylatszám:{0}";
        public const string ERR_NAVINV_ANULMENT = "Csak kimenő számlabizonylat küldése vonható vissza a NAV-tól, bizonylatszám:{0}";
        public const string ERR_INVOICEALREADYSETSEND = "A számlabizonylat már küldésre kijelölve, bizonylatszám:{0}";
        public const string NAV_SENDINVOICETONAV_ERR = "NAV adatküldés hiba:{0}";


        public const string DEF_NAVAnnulmentReason = "Hibás adatszolgáltatás";

        public const string DEF_NAVGeneralExceptionResponse = "GeneralExceptionResponse";
        public const string DEF_NAVGeneralErrorResponse = "GeneralErrorResponse";

        public const string DEF_NAVEMPTYTOKEN = "EMPTYTOKEN";
        public const string DEF_NAVNULLTOKEN = "NULLTOKEN";

        public const string ERR_CITYNOTFOUND = "Város nem található, IRSZ:{0} ";
        public const string ERR_ZIPNOTFOUND = "IRSZ nem található, város:{0} ";


        public const string ERR_CACHE_NOTUSED = "A gyorsítótár nincs használatban:{0}!";
        public const string ERR_LINEDESC_MISSING = "Hiányzó terméknév! Bizonylat:{0}, Termékkód:{1}";

        public const string DEF_NOTFILLED = "*** nincs kitöltve ***";

        public const string DEF_RELDELIVERYNOTE = "Kapcsolt szállítólevél";
        public const string DEF_RELDELIVERYDISCOUNTPERCENT = "Szállítólevél engedmény/felár";
        public const string VATCODE_27 = "27%";
        public const string VATCODE_KBAET = "KBAET";
        public const string VATCODE_FA = "FA";
        public const string VATCODE_TAM = "TAM";
        public const string PRODUCTFEE_TAKEOVERCODE = "02_ga";
        public const string DEF_VATREASON_AAM = "Alanyi adómentes";
        public const string DEF_VATREASON_TAM = "Tárgyi adómentes";
        public const string DEF_VATREASON_KBAET = "A Közösség másik tagállamában regisztrált adóalany számára";

        public const string DEF_VAT_FAFA = "FAFA";  //fordított áfa, <vatDomesticReverseCharge>true</vatDomesticReverseCharge>

        public const string DEF_CHARGE = "felár";
        public const string DEF_DISCOUNT = "engedmény";


        public const string FIELD_PRODUCTCODE = "PRODUCTCODE";
        public const string FIELD_PRODUCTGROUP = "PRODUCTGROUP";
        public const string FIELD_ORIGIN = "ORIGIN";
        public const string FIELD_UNITOFMEASUREX = "UNITOFMEASUREX";
        public const string FIELD_PRODUCT = "PRODUCT";
        public const string FIELD_STOCKCARDDATE = "STOCKCARDDATE";

        public const string DEF_WAREHOUSE = "001";
        public const long DEF_WAREHOUSE_ID = 1;
        public const string DEF_OFFERCOUNTER = "AJ";
        public const string DEF_BLKCOUNTER = "BLK";
        public const string DEF_BLCCOUNTER = "BLC";
        public const string DEF_JSCOUNTER = "JS";

        public const string DEF_NOTICE = "Notice";
        public const string DEF_NOTICEDESC = "Megjegyzés";


        public const string EMAIL_FORMAT_ERROR = "{PropertyName}: Hibás email cím!";

    }
}
