using System;
using System.Collections.Generic;
using System.Text;

namespace bbxBE.Application.Consts
{
    public static  class bbxBEConsts
    {
        public static string DB = "bbxdb";
        public static string PwdSalt = "PwdSalt";

        public static string FV_REQUIRED = "{PropertyName}|{PropertyName} is required.";
        public static string FV_LEN80 = "{PropertyName}|{PropertyName} must not exceed 80 characters.";
        public static string FV_LEN2000 = "{PropertyName}|{PropertyName} must not exceed 2000 characters.";
        public static string FV_EXISTS = "{PropertyName}|{PropertyName} already exists.";
        public static string FV_INVALIDEMAIL = "{PropertyName}|{PropertyName} email is invalid.";

    }
}
