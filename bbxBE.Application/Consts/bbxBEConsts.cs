using System;
using System.Collections.Generic;
using System.Text;

namespace bbxBE.Application.Consts
{
    public static  class bbxBEConsts
    {
        public static int CodeLen = 12;
        public static int DescriptionLen = 12;

        public static string DB = "bbx";

        public static string CNTRY_HU = "HU";
        public static string PwdSalt = "PwdSalt";


        public static string FV_REQUIRED = "{PropertyName}|{PropertyName} mező kitöltése kötelező.";
        public static string FV_LEN1 = "{PropertyName}|{PropertyName} mező karakterek száma:1.";
        public static string FV_LEN2 = "{PropertyName}|{PropertyName} mező karakterek száma:2.";
        public static string FV_LEN8 = "{PropertyName}|{PropertyName} mező karakterek száma:8.";

        public static string FV_MAXLEN = "{PropertyName}|{PropertyName} mező mérete nem lehet több, mint {MaxLength}.";
        public static string FV_RANGE = "{PropertyName}|{PropertyName} mező értéke {from} {to} között lehet. A megadott érték:{PropertyValue}.";


        public static string FV_EXISTS = "{PropertyName}|{PropertyName} már létezik.";
        public static string FV_INVALIDEMAIL = "{PropertyName}|{PropertyName} érvénytelen email cím.";
        public static string FV_ERBANK = "{PropertyName}|{PropertyName} érvénytelen bankszámlaszám.";
        public static string FV_INVPRODUCTCROUPID = "{PropertyName}|{PropertyName} érvénytelen termékcsoport.";
        public static string FV_INVORIGINID = "{PropertyName}|{PropertyName} érvénytelen származási hely.";
        public static string FV_INVUNITOFMEASURE = "{PropertyName}|{PropertyName} érvénytelen mennyiségi egység:{PropertyValue} ";
        public static string FV_PRODNOTFOUND = "Termék nem található, ID:{0} ";
        public static string FV_CUSTNOTFOUND = "Partneradat nem található, ID:{0} ";
        public static string FV_INVALIDFORMAT = "{PropertyName}|{PropertyName} érvénytelen formátum.";
        public static string FV_COUNTERNOTFOUND = "Bizonylati tömb található, ID:{0} ";
        public static string FV_COUNTERNOTFOUND2 = "Bizonylati tömb található, Kód:{0}, Raktár:{1} ";
        public static string FV_PRODUCTGROUPNOTFOUND = "Termékcsoport nem található, ID:{0} ";
        public static string FV_ORIGINNOTFOUND = "Származási hely nem található, ID:{0} ";

        public static string FV_BASE = "Egy vagy több validációs hiba történt.";

        public static string FV_FILELISTISNULL = "A File lista nem lehet üres!";
        public static string FV_FILELISTISEMPTY = "A File lista nem lehet NULL!";
        public static string FV_FILELISTCONUTER = "2 File-t kell feltölteni!";
        public static string FV_FILESIZE = "Üres file lett feltöltve!";


        public static string FV_NOCACHEQUERY = "Nincs lekérdezés a gyorstótárhoz!";

        public static string INV_DATE1 = "{PropertyName}| A számla dátuma nem lehet korábi, mint a teljesítés dátuma";
        public static string INV_DATE2 = "{PropertyName}| A számla dátuma nem lehet későbbi, mint a fizetési határidő";
        public static string INV_LINES = "{PropertyName}| A számlán nincs tételsor";
        public static string INV_VATSUMS = "{PropertyName}| A számlán nincs áfánkénti összesítő";

        public static string CST_OWNEXISTS = "{PropertyName}|{PropertyName} Saját adat már létezik.";

        public static string NAV_INVDIRECTION = "{PropertyName}|{PropertyName} érvénytelen biyonylatirány:{PropertyValue} ";

  
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

    }
}
