using System;
using System.Text;

namespace bbxBE.Common.NAV
{
    #region AES_128_ECB
    public static class NAVExtensions
    {
        static byte[] zeroIV = new byte[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        public static string AES_128_ECBEncryptString(this string strToEncrypt, string key, string IV = null)
            => NAVUtil.AES_128_ECB.EncryptString(strToEncrypt, key, IV);

        public static string AES_128_ECBDecrypt(this byte[] cryptedStringBytes, string key, string IV = null)
            => NAVUtil.AES_128_ECB.Decrypt(cryptedStringBytes, key, IV);
        public static string AES_128_ECBDecryptFromBase64String(this string rawCryptedString, string key, string IV = null)
            => NAVUtil.AES_128_ECB.DecryptFromBase64String(rawCryptedString, key, IV);


        #endregion AES_128_ECB


        #region SHA512
        public static string SHA512(this string p_input)
            => NAVUtil.SHA512(p_input);

        #endregion SHA512

        #region SHA3_512
        public static string SHA3_512(this string p_input)
             => NAVUtil.SHA3_512(p_input);
        #endregion SHA3_512

        #region Conversion
        public static string Base64Encode(this string p_str, Encoding p_enc = null)
              => NAVUtil.Base64Encode(p_str, p_enc);

        #endregion Conversion

        #region Comm
        public static string GetRequestID(this DateTime src)
                => NAVUtil.GetRequestID(src);

        #endregion Comm
    }

}
