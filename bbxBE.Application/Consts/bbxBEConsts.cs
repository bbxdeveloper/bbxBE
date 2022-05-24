using System;
using System.Collections.Generic;
using System.Text;

namespace bbxBE.Application.Consts
{
    public static  class bbxBEConsts
    {
        public static int CodeLen = 12;
        public static int DescriptionLen = 12;

        public static string CNTRY_HU = "HU";
        public static string CONF_PwdSalt = "PwdSalt";

        public static string CONF_CacheSettings = "CacheSettings";
        public static string CONF_WaitForCacheInSeconds = "WaitForCacheInSeconds";

        public static string ERR_REQUIRED = "{PropertyName}:  mező kitöltése kötelező.";
        public static string FV_LEN1 = "{PropertyName}:  mező karakterek száma:1.";
        public static string FV_LEN2 = "{PropertyName}:  mező karakterek száma:2.";
        public static string FV_LEN8 = "{PropertyName}:  mező karakterek száma:8.";

        public static string FV_MAXLEN = "{PropertyName}:  mező mérete nem lehet több, mint {MaxLength}.";
        public static string ERR_RANGE = "{PropertyName}:  mező értéke {from} {to} között lehet. A megadott érték:{PropertyValue}.";


        public static string FV_EXISTS = "{PropertyName}:  már létezik.";
        public static string FV_INVALIDEMAIL = "{PropertyName}:  érvénytelen email cím.";
        public static string FV_ERBANK = "{PropertyName}:  érvénytelen bankszámlaszám.";
        public static string FV_INVPRODUCTCROUPCODE = "{PropertyName}:  érvénytelen termékcsoport.";
        public static string FV_INVORIGINCODE = "{PropertyName}:  érvénytelen származási hely.";
        public static string FV_INVVATRATECODE = "{PropertyName}:  érvénytelen áfakód.";
        public static string FV_INVUNITOFMEASURE = "{PropertyName}:  érvénytelen mennyiségi egység:{PropertyValue} ";
        public static string FV_INVPAYMENTMETHOD = "{PropertyName}:  érvénytelen fizetési mód:{PropertyValue} ";
        public static string FV_EXCHANGERATE = "{PropertyName}:  Érvénytelen árfolyam::{PropertyValue} ";
        public static string FV_INVCURRENCY = "{PropertyName}:  érvénytelen pénznem:{PropertyValue}";
        public static string FV_PRODNOTFOUND = "Termék nem található, ID:{0} ";
        public static string FV_PRODCODENOTFOUND = "Termék nem található, kód:{0} ";
        public static string FV_VATRATECODENOTFOUND = "Áfakód található, kód:{0} ";
        public static string FV_CUSTNOTFOUND = "Partneradat nem található, ID:{0} ";
        public static string FV_OWNNOTFOUND = "Saját adat nem található!";
        public static string FV_INVALIDFORMAT = "{PropertyName}:  érvénytelen formátum.";
        public static string FV_COUNTERNOTFOUND = "Bizonylati tömb nem található, ID:{0} ";
        public static string FV_COUNTERNOTFOUND2 = "Bizonylati tömb nem található, Kód:{0}, Raktár ID:{1} ";
        public static string FV_PRODUCTGROUPNOTFOUND = "Termékcsoport nem található, ID:{0} ";
        public static string FV_ORIGINNOTFOUND = "Származási hely nem található, ID:{0} ";
        public static string FV_VATRATENOTFOUND = "Áfakód nem található, ID:{0} ";
        public static string FV_WAREHOUSENOTFOUND = "Raktár nem található, Kód:{0}";

        public static string FV_BASE = "Egy vagy több validációs hiba történt.";

        public static string FV_FILELISTISNULL = "A File lista nem lehet üres!";
        public static string FV_FILELISTISEMPTY = "A File lista nem lehet NULL!";
        public static string FV_FILELISTCONUTER = "2 File-t kell feltölteni!";
        public static string FV_FILESIZE = "Üres file lett feltöltve!";


        public static string ERR_NOCACHEQUERY = "Nincs lekérdezés a gyorstótárhoz!";
        public static string ERR_LOCKEDCACHE = "Gyorsítótár feltöltés alatt ! A műveletet később végrehajtható.";

        public static string ERR_INV_DATE1 = "{PropertyName}| A számla dátuma nem lehet korábi, mint a teljesítés dátuma";
        public static string ERR_INV_DATE2 = "{PropertyName}| A számla dátuma nem lehet későbbi, mint a fizetési határidő";
        public static string ERR_INV_LINES = "{PropertyName}| A számlán nincs tételsor";
        public static string ERR_INV_VATSUMS = "{PropertyName}| A számlán nincs áfánkénti összesítő";

        public static string ERR_CST_OWNEXISTS = "{PropertyName}:  Saját adat már létezik.";


        public static string ERR_OFFER_DATE1 = "{PropertyName}| A lejárati dátum nem lehet korábbi, mint a ajánlat kibocsátásának dátuma!";

        public static string NAV_INVDIRECTION = "{PropertyName}:  érvénytelen biyonylatirány:{PropertyValue} ";

  
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


        public static string VATCODE_27 = "27%";
        public static string VATCODE_KBAET = "KBAET";
        public static string VATCODE_FA = "FA";

        public static string FIELD_PRODUCTCODE = "PRODUCTCODE";
        public static string DEF_WAREHOUSE = "001";
        public static long DEF_WAREHOUSE_ID = 1;
        public static string DEF_OFFERCOUNTER = "AJ";

        public static string DEF_NOTICE = "Notice";
        public static string DEF_NOTICEDESC = "Megjegyzés"; 
    }
}
