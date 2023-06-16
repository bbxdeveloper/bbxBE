namespace bbxBE.Common.Consts
{
    public static class bbxBEConsts
    {
        public static int CodeLen = 12;
        public static int DescriptionLen = 80;

        public static string CNTRY_HU = "HU";
        public static string CONF_PwdSalt = "PwdSalt";

        public static string CONF_JWTSettings = "JWTSettings";
        public static string CONF_JWTKey = "Key";
        public static string CONF_JWTIssuer = "Issuer";
        public static string CONF_JWTAudience = "Audience";
        public static string CONF_JWTDurationInMinutes = "DurationInMinutes";

        public const string DEF_ENCODING = "ISO-8859-2";
        public const string DEF_CSVSEP = ";";
        public const string DEF_DATEFORMAT = "yyyy-MM-dd";
        public const string DEF_NUMFORMAT = "#,#0.00";
        public const string DEF_INTFORMAT = "#,#0";
        public const string DEF_DATETIMEFORMAT = "yyyy.MM.dd HH:mm:ss";
        public const string DEF_TRUE = "true";
        public const string DEF_FALSE = "false";


        public static string DEF_CUSTOMERLOCK_KEY = "CUST_";

        public static string CONF_CacheSettings = "CacheSettings";
        public static string CONF_WaitForCacheInSeconds = "WaitForCacheInSeconds";

        public static int WaitForExpiringDataSec = 3;
        public static int ExpiringDataMaxCount = 5000;
        public static int CustomerLockExpoirationSec = 900; //15 perc
        public static string DEF_CUSTLOCK = "custlock";


        public static string ERR_REQUIRED = "{PropertyName}: mező kitöltése kötelező.";
        public static string TOKEN_PROPERTYNAME = "{PropertyName}";
        public static string ERR_GREATGHERTHANZERO = "{PropertyName}: mező értéke csak pozitív szám lehet.";

        public static string ERR_MAXLEN = "{PropertyName}: mező mérete nem lehet több, mint {MaxLength}.";
        public static string ERR_RANGE = "{PropertyName}: mező értéke {from} {to} között lehet. A megadott érték:{PropertyValue}.";
        public static string ERR_DATEINTERVAL = "{PropertyName}: helytelen időszak.";


        public static string ERR_EXISTS = "{PropertyName}: már létezik.";
        public static string ERR_INVALIDEMAIL = "{PropertyName}: érvénytelen email cím.";
        public static string ERR_ERBANK = "{PropertyName}: érvénytelen bankszámlaszám.";
        public static string ERR_INVPRODUCTCROUPCODE = "{PropertyName}: érvénytelen termékcsoport.";
        public static string ERR_INVORIGINCODE = "{PropertyName}: érvénytelen származási hely.";
        public static string ERR_INVVATRATECODE = "{PropertyName}: érvénytelen áfakód.";
        public static string ERR_INVUNITOFMEASURE = "{PropertyName}: érvénytelen mennyiségi egység:{PropertyValue} ";
        public static string ERR_INVUNITOFMEASURE2 = "Sor: {0}, termék:{1} : érvénytelen mennyiségi egység:{2} ";
        public static string ERR_DETAIL_PREF = "Sor: {0}, termék:{1}";
        public static string ERR_DISCOUNT = "Sor: {0}, termék:{1}";
        public static string ERR_NOINPUTDATA = "Nincs input adat!";


        public static string ERR_INVPAYMENTMETHOD = "{PropertyName}: érvénytelen fizetési mód:{PropertyValue} ";
        public static string ERR_EXCHANGERATE = "{PropertyName}: Érvénytelen árfolyam::{PropertyValue} ";
        public static string ERR_INVCURRENCY = "{PropertyName}: érvénytelen pénznem:{PropertyValue}";
        public static string ERR_PRODNOTFOUND = "Termék nem található, ID:{0} ";
        public static string ERR_PRODCODENOTFOUND = "Termék nem található, kód:{0} ";
        public static string ERR_VATRATECODENOTFOUND = "Áfakód nem található, kód:{0} ";
        public static string ERR_VATRATECODENOTFOUND2 = "Áfakód nem található, ID:{0} ";
        public static string ERR_CUSTNOTFOUND = "Partneradat nem található, ID:{0} ";
        public static string ERR_OWNNOTFOUND = "Saját adat nem található!";
        public static string ERR_INVALIDFORMAT = "{PropertyName}: érvénytelen formátum!";
        public static string ERR_INVALIDCONTENT = "{PropertyName}: érvénytelen tartalom!";
        public static string ERR_COUNTERNOTFOUND = "Bizonylati tömb nem található, ID:{0} ";
        public static string ERR_COUNTERNOTFOUND2 = "Bizonylati tömb nem található, Kód:{0}, Raktár ID:{1} ";
        public static string ERR_PRODUCTGROUPNOTFOUND = "Termékcsoport nem található, ID:{0} ";
        public static string ERR_ORIGINNOTFOUND = "Származási hely nem található, ID:{0} ";
        public static string ERR_OFFERNOTFOUND = "Árajánlat nem található, ID:{0} ";
        public static string ERR_INVOICENOTFOUND = "Számla/szállítólevél nem található, ID:{0} ";
        public static string ERR_VATRATENOTFOUND = "Áfakód nem található, ID:{0} ";
        public static string ERR_USERNOTFOUND = "Felhasználó nem található, ID:{0} ";
        public static string ERR_USERNOTFOUND2 = "Felhasználó nem található, név:{0} ";
        public static string ERR_WAREHOUSENOTFOUND = "Raktár nem található, Kód:{0}";
        public static string ERR_CUSTOMERNOTFOUND = "Partner nem található, ID:{0}";
        public static string ERR_STOCKNOTFOUND = "Raktárkészlet nem található, ID:{0} ";
        public static string ERR_STOCKCARDNOTFOUND = "Készletkarton nem található, ID:{0} ";
        public static string ERR_LOCATIONOTFOUND = "Helyiség nem található, ID:{0} ";
        public static string ERR_INVOICELINENOTFOUND = "Bizonylatsor nem található, ID:{0}, bizonylat:{1}, sor ID:{2}";
        public static string ERR_ORIGINALINVOICENOTFOUND = "Eredeti bionylat nem található, ID:{0}";
        public static string ERR_WRONGCORRECTIONQTY = "Eredeti bionylaton lévőnél nagyobb mennyiség levonása! Termékkód:{0}, eredeti mennyiség:{1:#,#0.00}, előzőleg lejavítva:{2:#,#0.00}, javítószámlán:{3:#,#0.00}";
        public static string ERR_CORRECTIONUNKOWNPROD = "Eredeti bionylaton nem létező termék: ID:{0}, Kód:{1}";
        public static string ERR_WHSTRANSFERNOTFOUND = "Raktárközi átadás nem található, ID:{0} ";
        public static string ERR_INVWHSTRANSFERSTATUS = "{PropertyName}: érvénytelen raktárközi átadás státusz:{PropertyValue} ";
        public static string ERR_INVOICEREPORT_NULL = "Invoice report result is null!";
        public static string ERR_INVOICEREPORT = "Invoice report finished with error:{0}";
        public static string ERR_OFFERREPORT_NULL = "Offer report result is null!";
        public static string ERR_OFFERREPORT = "Offer report finished with error:{0}";
        public static string ERR_CUSTOMERINVOICESUMMARYREPORT_NULL = "CustomerInvoiceSummary report result is null!";
        public static string ERR_CUSTOMERINVOICESUMMARYREPORT = "CustomerInvoiceSummary report finished with error:{0}";

        public static string ERR_VALIDATION = "Egy vagy több validációs hiba történt.";

        public static string ERR_FILELISTISNULL = "A File lista nem lehet üres!";
        public static string ERR_FILELISTISEMPTY = "A File lista nem lehet NULL!";
        public static string ERR_FILELISTCONUTER = "2 File-t kell feltölteni!";
        public static string ERR_FILESIZE = "Üres file lett feltöltve!";


        public static string ERR_NOCACHEQUERY = "Nincs lekérdezés a gyorsítótárhoz!";
        public static string ERR_LOCKEDCACHE = "Gyorsítótár feltöltés alatt ! A műveletet később végrehajtható.";
        public static string ERR_LOCK = "Zárolás nem sikerült. Kulcs foglalt:{0}";
        public static string ERR_UNLOCK = "Zárolás felszabadítása nem sikerült. Kulcs nem létezik:{0}";


        public static string ERR_EXPIRINGDATA_FULL = "Az ExpiringData tároló megtelt! Max. elemszám:{0}";

        public static string ERR_INV_DATE1 = "{PropertyName}: A számla dátuma nem lehet korábi, mint a teljesítés dátuma";
        public static string ERR_INV_DATE2 = "{PropertyName}: A számla dátuma nem lehet későbbi, mint a fizetési határidő";
        public static string ERR_INV_LINES = "{PropertyName}: A számlán nincs tételsor";
        public static string ERR_INV_VATSUMS = "{PropertyName}: A számlán nincs áfánkénti összesítő";

        public static string ERR_CST_OWNEXISTS = "{PropertyName}: Saját adat már létezik.";
        public static string ERR_CST_WRONGCOUNTRY = "{PropertyName}: Helytelen országkód:{PropertyValue}";
        public static string ERR_CST_WRONGUNITPRICETYPE = "{PropertyName}: Helytelen eladási ártípus:{PropertyValue}";
        public static string ERR_CST_WRONGDEFPAYMENTTYPE = "{PropertyName}: Helytelen alapértelmezett fizetési mód:{PropertyValue}";

        public static string ERR_CST_TAXNUMBER_INV = "{PropertyName}: Az adószám csak magyarországi partnerek esetén értelmezett.";
        public static string ERR_CST_TAXNUMBER_INV2 = "{PropertyName}: érvénytelen formátum/tartalom.";


        public static string ERR_OFFER_DATE1 = "{PropertyName}: A lejárati dátum nem lehet korábbi, mint a ajánlat kibocsátásának dátuma!";

        public static string NAV_INVDIRECTION = "{PropertyName}: Érvénytelen bizonylatirány:{PropertyValue}";
        public static string ERR_INVOICETYPE = "Érvénytelen bizonylattípus:{0}";

        public static string ERR_INVCTRLPERIOD_DATE1 = "{PropertyName}: Helytelen leltáridőszak!";
        public static string ERR_INVCTRLPERIOD_DATE2 = "{PropertyName}: A megadott leltáridőszak ütközik más időszakkal!";
        public static string ERR_INVCTRLPERIODNOTFOUND = "Leltáridőszak nem található, ID:{0} ";
        public static string ERR_INVCTRLPERIOD_CANTBEDELETED = "A leltáridőszak már nem törölhető!";
        public static string ERR_INVCTRLPERIOD_CANTBEUPDATED = "A leltáridőszak már nem módosítható!";
        public static string ERR_INVCTRLPERIOD_CANTBECLOSED = "A leltáridőszak nem zárható le!";
        public static string ERR_INVCTRLPERIOD_NOTCLOSED = "A leltáridőszak nincs lezárva!";
        public static string ERR_INVCTRLPERIOD_ALREADYCLOSED = "A leltáridőszak már lezárt!";
        public static string ERR_INVAGGR_RELATED_NOT_ASSIGNED = "A gyűjtőszámlán lévő tételhez nincs szállítólevél rendelve! Szállítólevél:{0}, sor száma:{1}, temékkód:{2} ";
        public static string ERR_INVAGGR_RELATED_NOT_FOUND = "A gyűjtőszámlán lévő tételhez nem található szállítólevél! Szállítólevél:{0}, sor száma:{1}, temékkód:{2}, kapcsolt szállító-sor ID:{3} ";
        public static string ERR_INVAGGR_WRONG_AGGR_QTY = "A gyűjtőszámlán lévő mennyiség több, mint a szállítólevélen lévő! Szállítólevél:{0}, sor száma:{1}, temékkód:{2}, gyűjtőszámla menny.:{3:#,#0.00}, szállítólevél menny.:{4:#,#0.00}, kapcsolt szállító-sor ID:{5} ";

        public static string ERR_INVCTRLNOTFOUND = "Leltári tétel nem található, ID:{0} ";
        public static string ERR_INVCTRL_DATE = "{PropertyName}:Helytelen leltárdátum!";


        public static string PROD_IMPORT_RESULT = "***Product import***\n" +
                                                    "Összes elem    :{0}\n" +
                                                    "Létrehozva     :{1}\n" +
                                                    "Aktualuzálva   :{2}\n" +
                                                    "Hibás          :{3}";


        public static string NAV_TOKENEXCHANGE_ERR = "{0} NAV tokenExchange error result:{1}";

        public static string NAV_QINVDIGEST_OK = "{0} NAV queryInvoiceDigest OK, invoiceDirection:{1}, issue:{2}, dateFromUTC:{3}, dateToUTC:{4}";
        public static string NAV_QINVDIGEST_FIRSTPG_ERR = "{0} NAV queryInvoiceDigest firstpage error result:{1}";
        public static string NAV_QINVDIGEST_NEXTPG_ERR = "{0} NAV queryInvoiceDigest nextpage error result:{1}";

        public static string NAV_QINVDATA_OK = "{0} NAV test result: funcCode:{1}, errorCode:{2}, message:{3}";
        public static string NAV_QINVDATA_ERR = "{0} NAV queryInvoiceData error result:{1}";
        public static string NAV_QINVDATA_NOTFND_ERR = "{0} invoice not found:{1}";

        public static string NAV_QTAXPAYER_ERR = "{0} NAV QueryTaxpayer, taxnumber:{1} error result:{2}";
        public static string NAV_QTAXPAYER_TOKEN_ERR = "{0} NAV QueryTaxpayer token, taxnumber:{1} error result:{2}";
        public static string NAV_QTAXPAYERT_OK = "{0} NAV QueryTaxpayer OK, taxnumber:{1}";

        public static string ERR_NAV_TAXPAYER = "A lekérdezéshez használt adószámnak 8 jeygűnek kell lennie!";

        public static string ERR_CITYNOTFOUND = "Város nem található, IRSZ:{0} ";
        public static string ERR_ZIPNOTFOUND = "IRSZ nem található, város:{0} ";


        public static string ERR_CACHE_NOTUSED = "A gyorsítótár nincs használatban:{0}!";


        public static string VATCODE_27 = "27%";
        public static string VATCODE_KBAET = "KBAET";
        public static string VATCODE_FA = "FA";

        public static string FIELD_PRODUCTCODE = "PRODUCTCODE";
        public static string FIELD_PRODUCTGROUP = "PRODUCTGROUP";
        public static string FIELD_ORIGIN = "ORIGIN";
        public static string FIELD_UNITOFMEASUREX = "UNITOFMEASUREX";
        public static string FIELD_PRODUCT = "PRODUCT";
        public static string DEF_WAREHOUSE = "001";
        public static long DEF_WAREHOUSE_ID = 1;
        public static string DEF_OFFERCOUNTER = "AJ";
        public static string DEF_BLKCOUNTER = "BLK";
        public static string DEF_BLCCOUNTER = "BLC";
        public static string DEF_JSCOUNTER = "JS";

        public static string DEF_NOTICE = "Notice";
        public static string DEF_NOTICEDESC = "Megjegyzés";


        public static string EMAIL_FORMAT_ERROR = "{PropertyName}: Hibás email cím!";

    }
}
