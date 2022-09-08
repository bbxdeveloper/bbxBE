using System;
using System.Collections.Generic;
using System.Text;

namespace bbxBE.Common.Consts
{
    public static  class bbxBEConsts
    {
        public static int CodeLen = 12;
        public static int DescriptionLen = 12;

        public static string CNTRY_HU = "HU";
        public static string CONF_PwdSalt = "PwdSalt";

        public const string DEF_DATEFORMAT = "yyyy-MM-dd";


        public static string CONF_CacheSettings = "CacheSettings";
        public static string CONF_WaitForCacheInSeconds = "WaitForCacheInSeconds";

        public static string ERR_REQUIRED = "{PropertyName}:  mező kitöltése kötelező.";
        public static string ERR_LEN1 = "{PropertyName}:  mező karakterek száma:1.";
        public static string ERR_LEN2 = "{PropertyName}:  mező karakterek száma:2.";
        public static string ERR_LEN8 = "{PropertyName}:  mező karakterek száma:8.";

        public static string ERR_MAXLEN = "{PropertyName}:  mező mérete nem lehet több, mint {MaxLength}.";
        public static string ERR_RANGE = "{PropertyName}:  mező értéke {from} {to} között lehet. A megadott érték:{PropertyValue}.";


        public static string ERR_EXISTS = "{PropertyName}:  már létezik.";
        public static string ERR_INVALIDEMAIL = "{PropertyName}:  érvénytelen email cím.";
        public static string ERR_ERBANK = "{PropertyName}: érvénytelen bankszámlaszám.";
        public static string ERR_INVPRODUCTCROUPCODE = "{PropertyName}: érvénytelen termékcsoport.";
        public static string ERR_INVORIGINCODE = "{PropertyName}:  érvénytelen származási hely.";
        public static string ERR_INVVATRATECODE = "{PropertyName}:  érvénytelen áfakód.";
        public static string ERR_INVUNITOFMEASURE = "{PropertyName}:  érvénytelen mennyiségi egység:{PropertyValue} ";
        public static string ERR_INVUNITOFMEASURE2 = "Sor: {0}, termék:{1} : érvénytelen mennyiségi egység:{2} ";
        public static string ERR_DETAIL_PREF = "Sor: {0}, termék:{1}";
        public static string ERR_DISCOUNT = "Sor: {0}, termék:{1}";

        public static string ERR_INVPAYMENTMETHOD = "{PropertyName}:  érvénytelen fizetési mód:{PropertyValue} ";
        public static string ERR_EXCHANGERATE = "{PropertyName}:  Érvénytelen árfolyam::{PropertyValue} ";
        public static string ERR_INVCURRENCY = "{PropertyName}:  érvénytelen pénznem:{PropertyValue}";
        public static string ERR_PRODNOTFOUND = "Termék nem található, ID:{0} ";
        public static string ERR_PRODCODENOTFOUND = "Termék nem található, kód:{0} ";
        public static string FV_VATRATECODENOTFOUND = "Áfakód található, kód:{0} ";
        public static string FV_CUSTNOTFOUND = "Partneradat nem található, ID:{0} ";
        public static string FV_OWNNOTFOUND = "Saját adat nem található!";
        public static string FV_INVALIDFORMAT = "{PropertyName}:  érvénytelen formátum.";
        public static string FV_COUNTERNOTFOUND = "Bizonylati tömb nem található, ID:{0} ";
        public static string FV_COUNTERNOTFOUND2 = "Bizonylati tömb nem található, Kód:{0}, Raktár ID:{1} ";
        public static string FV_PRODUCTGROUPNOTFOUND = "Termékcsoport nem található, ID:{0} ";
        public static string FV_ORIGINNOTFOUND = "Származási hely nem található, ID:{0} ";
        public static string ERR_OFFERNOTFOUND = "Árajánlat nem található, ID:{0} ";
        public static string ERR_INVOICENOTFOUND = "Számla/szállítólevél nem található, ID:{0} ";
        public static string ERR_VATRATENOTFOUND = "Áfakód nem található, ID:{0} ";
        public static string ERR_USERNOTFOUND = "Felhasználó nem található, ID:{0} ";
        public static string ERR_WAREHOUSENOTFOUND = "Raktár nem található, Kód:{0}";
        public static string ERR_STOCKNOTFOUND = "Raktárkészlet nem található, ID:{0} ";
        public static string ERR_STOCKCARDNOTFOUND = "Készletkarton nem található, ID:{0} ";

        public static string ERR_VALIDATION = "Egy vagy több validációs hiba történt.";

        public static string ERR_FILELISTISNULL = "A File lista nem lehet üres!";
        public static string ERR_FILELISTISEMPTY = "A File lista nem lehet NULL!";
        public static string ERR_FILELISTCONUTER = "2 File-t kell feltölteni!";
        public static string ERR_FILESIZE = "Üres file lett feltöltve!";


        public static string ERR_NOCACHEQUERY = "Nincs lekérdezés a gyorstótárhoz!";
        public static string ERR_LOCKEDCACHE = "Gyorsítótár feltöltés alatt ! A műveletet később végrehajtható.";

        public static string ERR_INV_DATE1 = "{PropertyName}: A számla dátuma nem lehet korábi, mint a teljesítés dátuma";
        public static string ERR_INV_DATE2 = "{PropertyName}: A számla dátuma nem lehet későbbi, mint a fizetési határidő";
        public static string ERR_INV_LINES = "{PropertyName}: A számlán nincs tételsor";
        public static string ERR_INV_VATSUMS = "{PropertyName}: A számlán nincs áfánkénti összesítő";

        public static string ERR_CST_OWNEXISTS = "{PropertyName}:  Saját adat már létezik.";


        public static string ERR_OFFER_DATE1 = "{PropertyName}: A lejárati dátum nem lehet korábbi, mint a ajánlat kibocsátásának dátuma!";

        public static string NAV_INVDIRECTION = "{PropertyName}:  érvénytelen bizonylatirány:{PropertyValue} ";

        public static string ERR_INVCTRLPERIOD_DATE1 = "{PropertyName}: Helytelen leltáridőszak!";
        public static string ERR_INVCTRLPERIOD_DATE2 = "{PropertyName}: A megadott leltáridőszak ütközik más időszakkal!";
        public static string ERR_INVCTRLPERIODNOTFOUND = "Leltáridőszak nem található, ID:{0} ";
        public static string ERR_INVCTRLPERIOD_CANTBEDELETED = "{PropertyName}: A leltáridőszak már nem törölhető!";
        public static string ERR_INVCTRLPERIOD_CANTBECLOSED = "A leltáridőszak nem zárható le!";
        public static string ERR_INVCTRLPERIOD_NOTCLOSED = "A leltáridőszak nincs lezárva!";
        public static string ERR_INVCTRLPERIOD_ALREADYCLOSED = "A leltáridőszak már lezárt!";

        public static string ERR_INVCTRLNOTFOUND = "Leltári tétel nem található, ID:{0} ";
        public static string ERR_INVCTRL_DATE = "{PropertyName}:Helytelen leltárdátum!";


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


        public static string VATCODE_27 = "27%";
        public static string VATCODE_KBAET = "KBAET";
        public static string VATCODE_FA = "FA";

        public static string FIELD_PRODUCTCODE = "PRODUCTCODE";
        public static string FIELD_PRODUCT = "PRODUCT";
        public static string DEF_WAREHOUSE = "001";
        public static long DEF_WAREHOUSE_ID = 1;
        public static string DEF_OFFERCOUNTER = "AJ";

        public static string DEF_NOTICE = "Notice";
        public static string DEF_NOTICEDESC = "Megjegyzés";


        public static string EMAIL_FORMAT_ERROR = "{PropertyName}:  Hibás email cím!";

    }
}
