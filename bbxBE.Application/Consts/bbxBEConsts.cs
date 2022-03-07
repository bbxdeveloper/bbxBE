using System;
using System.Collections.Generic;
using System.Text;

namespace bbxBE.Application.Consts
{
    public static  class bbxBEConsts
    {
        public static string DB = "bbx";

        public static string CNTRY_HU = "HU";
        public static string PwdSalt = "PwdSalt";


        public static string FV_REQUIRED = "{PropertyName}|{PropertyName} mező kitöltése kötelezp.";
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
        public static string FV_INVALIDFORMAT = "{PropertyName}|{PropertyName} érvénytelen formátum.";

        public static string FV_BASE = "Egy vagy több validációs hiba történt.";
/*
        public static string FV_REQUIRED = "{PropertyName}|{PropertyName} is required.";
        public static string FV_LEN1 = "{PropertyName}|{PropertyName} must be one character.";
        public static string FV_LEN2 = "{PropertyName}|{PropertyName} must be two characters.";
        public static string FV_LEN8 = "{PropertyName}|{PropertyName} must be 8 characters.";
        public static string FV_LEN13 = "{PropertyName}|{PropertyName} must not exceed 13 characters.";
        public static string FV_LEN30 = "{PropertyName}|{PropertyName} must not exceed 30 characters.";
        public static string FV_LEN80 = "{PropertyName}|{PropertyName} must not exceed 80 characters.";
        public static string FV_LEN2000 = "{PropertyName}|{PropertyName} must not exceed 2000 characters.";
        public static string FV_EXISTS = "{PropertyName}|{PropertyName} already exists.";
        public static string FV_INVALIDEMAIL = "{PropertyName}|{PropertyName} email is invalid.";
        public static string FV_ERBANK = "{PropertyName}|{PropertyName} invalid bank acconunt.";
        public static string FV_INVPRODUCTCROUPID = "{PropertyName}|{PropertyName} invalid product group ID.";
        public static string FV_INVORIGINID = "{PropertyName}|{PropertyName} invalid origin ID.";
        public static string FV_INVUNITOFMEASURE = "{PropertyName}|{PropertyName} invalid unit of measure:{PropertyValue} ";
        public static string FV_PRODNOTFOUND = "Product not found$ ID:{0} ";
        public static string FV_INVALIDFORMAT = "{PropertyName}|{PropertyName} format is invalid.";

        public static string FV_BASE = "One or more validation failures have occurred.";
*/

    }
}
