﻿using System;
using System.Globalization;
using System.IO;
using System.Runtime.ExceptionServices;
using System.Security.Cryptography;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace bbxBE.Common.NAV
{
    public static class NAVUtil
    {
        #region AES_128_ECB
        public static class AES_128_ECB
        {
            static byte[] zeroIV = new byte[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
            public static string EncryptString(string strToEncrypt, string key, string IV = null)
            {
                byte[] encryptedStr;
                using (Aes aes = Aes.Create())
                {


                    aes.Mode = CipherMode.ECB;
                    aes.KeySize = 128;
                    aes.Padding = PaddingMode.PKCS7;
                    aes.BlockSize = 128;
                    //              aes.Key = Convert.FromBase64String(keyBase64);
                    aes.Key = Encoding.ASCII.GetBytes(key);
                    if (IV == null)
                        aes.IV = zeroIV;
                    else
                        aes.IV = Encoding.ASCII.GetBytes(IV);

                    //              ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
                    ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

                    using (MemoryStream msEncrypt = new MemoryStream())
                    {
                        using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                        {
                            using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                            {
                                swEncrypt.Write(strToEncrypt);
                            }
                            encryptedStr = msEncrypt.ToArray();
                        }
                    }
                }

                return Convert.ToBase64String(encryptedStr);
            }

            public static string DecryptString(string rawCryptedString, string key, string IV = null)
            {
                string decryptedString = null;
                var cryptedString = Convert.FromBase64String(rawCryptedString);


                using (Aes aes = Aes.Create())
                {

                    aes.Mode = CipherMode.ECB;
                    aes.KeySize = 128;
                    aes.Padding = PaddingMode.PKCS7;
                    aes.BlockSize = 128;
                    //              aes.Key = Convert.FromBase64String(keyBase64);
                    aes.Key = Encoding.ASCII.GetBytes(key);
                    if (IV == null)
                        aes.IV = zeroIV;
                    else
                        aes.IV = Encoding.ASCII.GetBytes(IV);
                    ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

                    using (MemoryStream msDecrypt = new MemoryStream(cryptedString))
                    {
                        using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                        {
                            using (StreamReader swDecrypt = new StreamReader(csDecrypt))
                            {
                                decryptedString = "";
                                decryptedString = string.Concat(decryptedString, swDecrypt.ReadToEnd());
                            }
                        }
                    }
                }
                return decryptedString;
            }

        }

      
        #endregion AES_128_ECB

        #region SHA3_512
        public static string SHA3_512(string p_input)
        {
            var hashAlgorithm = new Org.BouncyCastle.Crypto.Digests.Sha3Digest(512);

            byte[] input = Encoding.UTF8.GetBytes(p_input);

            hashAlgorithm.BlockUpdate(input, 0, input.Length);

            byte[] result = new byte[64]; // 512 / 8 = 64
            hashAlgorithm.DoFinal(result, 0);

            string hashString = BitConverter.ToString(result);
            hashString = hashString.Replace("-", "").ToUpperInvariant();
            return hashString;
        }
        #endregion SHA3_512

        #region SHA512
        public static string SHA512(string p_input)
        {
            var bytes = System.Text.Encoding.UTF8.GetBytes(p_input);
            using (var hash = System.Security.Cryptography.SHA512.Create())
            {
                var hashedInputBytes = hash.ComputeHash(bytes);

                // Convert to text
                // StringBuilder Capacity is 128, because 512 bits / 8 bits in byte * 2 symbols for byte 
                var hashedInputStringBuilder = new StringBuilder(128);
                foreach (var b in hashedInputBytes)
                    hashedInputStringBuilder.Append(b.ToString("X2"));
                return hashedInputStringBuilder.ToString();
            }
        }
        #endregion SHA512

        #region Conversion
        public static string Base64Encode(string p_str, Encoding p_enc = null)
        {
            byte[] bytes;
            if (p_enc != null)
            {
                bytes = p_enc.GetBytes(p_str);
            }
            else
            {
                bytes = Encoding.UTF8.GetBytes(p_str);
            }
            return Convert.ToBase64String(bytes);
        }

        public static string AsString(this byte[] src)
        {
            return Encoding.Default.GetString(src); ;
        }
        #endregion Conversion

        #region Comm
        public static string GetRequestID(DateTime src)
        {
            return src.ToString();
        }
        #endregion Comm


        #region XML
        public class Utf8StringWriter : StringWriter
        {


            public override Encoding Encoding
            {
                get { return Encoding.UTF8; }
            }

            public override IFormatProvider FormatProvider => base.FormatProvider;
        }
        public static string Object2XMLString<T>(this T value)
        {
            if (value == null)
            {
                return string.Empty;
            }

            DateTimeFormatInfo dtfi = CultureInfo.CurrentCulture.DateTimeFormat;

            try
            {
                XmlWriterSettings settings = new XmlWriterSettings()
                {
                    Encoding = Encoding.UTF8,
                    CloseOutput = false,
                    OmitXmlDeclaration = false,
                    Indent = true
                };

                var xmlserializer = new XmlSerializer(typeof(T), headerOverrideAttributes());

                var stringWriter = new Utf8StringWriter();
                using (var writer = XmlWriter.Create(stringWriter, settings))
                {
                    xmlserializer.Serialize(writer, value);
                    return stringWriter.ToString();
                }

            }
            catch (Exception ex)
            {
                ExceptionDispatchInfo.Capture(ex).Throw();
            }
            return string.Empty;
        }
        public static T XMLStringToObject<T>(string p_xmlString)
        {

            XmlSerializer serializer = new XmlSerializer(typeof(T), headerOverrideAttributes());
            StringReader rdr = new StringReader(p_xmlString);
            T res = (T)serializer.Deserialize(rdr);


            XmlDocument xd = new XmlDocument();
            xd.LoadXml(p_xmlString);
            return res;
        }

        private static XmlAttributeOverrides headerOverrideAttributes()
        {
            XmlAttributeOverrides headerOverride = new XmlAttributeOverrides();

            XmlAttributes requestId = new XmlAttributes();
            XmlElementAttribute xrequestId = new XmlElementAttribute();
            xrequestId.Order = 1;
            requestId.XmlElements.Add(xrequestId);
            headerOverride.Add(typeof(BasicHeaderType), "requestId", requestId);

            XmlAttributes timestamp = new XmlAttributes();
            timestamp.XmlIgnore = true;
            headerOverride.Add(typeof(BasicHeaderType), "timestamp", timestamp);

            XmlAttributes requestVersion = new XmlAttributes();
            XmlElementAttribute xrequestVersion = new XmlElementAttribute();
            xrequestVersion.Order = 3;
            requestVersion.XmlElements.Add(xrequestVersion);
            headerOverride.Add(typeof(BasicHeaderType), "requestVersion", requestVersion);

            XmlAttributes headerVersion = new XmlAttributes();
            XmlElementAttribute xheaderVersion = new XmlElementAttribute();
            xheaderVersion.Order = 4;
            headerVersion.XmlElements.Add(xheaderVersion);
            headerOverride.Add(typeof(BasicHeaderType), "headerVersion", headerVersion);

            return headerOverride;

        }

        #endregion

    }
}
