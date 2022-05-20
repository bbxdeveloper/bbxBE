using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Runtime.ExceptionServices;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace bbxBE.Common
{
    public static class Utils
    {

        private static readonly Random _rnd = new Random();

        public static string GetTempFilePathWithExtension(string extension)
        {
            var path = Path.GetTempPath();
            var fileName = String.Concat(Guid.NewGuid().ToString(), extension);
            return Path.Combine(path, fileName);
        }

        public static void CopyStream(Stream input, Stream output)
        {
            byte[] buffer = new byte[8 * 1024];
            int len;
            while ((len = input.Read(buffer, 0, buffer.Length)) > 0)
            {
                output.Write(buffer, 0, len);
            }
        }

        public static string String2File(string p_s, string p_file, bool p_append, Encoding p_encoding = null)
        {

            if (p_file == "" || p_file == null)
                p_file = Path.GetTempFileName();

            if (p_encoding == null)
                p_encoding = Encoding.Default;

            string path = Path.GetDirectoryName(p_file);
            if (!System.IO.Directory.Exists(path))
                System.IO.Directory.CreateDirectory(path);

            try
            {
                TextWriter tw = new StreamWriter(p_file, p_append, p_encoding);
                tw.Write(p_s);
                tw.Close();
            }
            catch (Exception e) { }
            return p_file;

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public static string FileToString(string p_file, Encoding p_encding = null)
        {
            if (p_encding == null)
                p_encding = Encoding.Default;
            string s = "";
            TextReader tr = new StreamReader(p_file, p_encding);
            s = tr.ReadToEnd();
            tr.Close();
            return s;
        }

        /// <summary>
        /// Exception formázott szövege
        /// </summary>
        /// <param name="p_ecx"></param>
        /// <returns></returns>
        public static string GetExceptionText(Exception p_ecx)
        {
            string innerMsg = "";
            if (p_ecx.InnerException != null)
                innerMsg = p_ecx.InnerException.Message;

            return String.Format("{0} {1}", p_ecx.Message, innerMsg).Trim();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        public static byte[] FileToByteArray(string p_filename)
        {
            FileStream fs = File.OpenRead(p_filename);
            BinaryReader br = new BinaryReader(fs);

            byte[] b = br.ReadBytes((int)fs.Length);

            br.Close();
            fs.Close();
            return b;
        }

        public static void ByteArrayToFile(string p_filename, byte[] b)
        {
            FileStream fs = File.Create(p_filename);
            BinaryWriter bw = new BinaryWriter(fs);

            bw.Write(b);

            bw.Close();
            fs.Close();

        }

        public static bool IsDateTime(this Type type)
        {
            return type == typeof(DateTime) || type == typeof(DateTime?);
        }

        public static bool IsGuid(this Type type)
        {
            return type == typeof(Guid) || type == typeof(Guid?);
        }

        public static string GenerateHashCode(string decodeString)
        {
            System.Security.Cryptography.SHA1 hash = new System.Security.Cryptography.SHA1CryptoServiceProvider();
            var h2 = hash.ComputeHash(Encoding.Unicode.GetBytes(decodeString));
            var hh = HexStringFromBytes(h2);
            return hh;
        }

        private static string HexStringFromBytes(byte[] bytes)
        {
            var sb = new StringBuilder();
            foreach (byte b in bytes)
            {
                var hex = b.ToString("x2");
                sb.Append(hex);
            }
            return sb.ToString();
        }

        public static T ForceType<T>(this object o)
        {
            T res;
            res = Activator.CreateInstance<T>();

            Type x = o.GetType();
            Type y = res.GetType();

            foreach (var destinationProp in y.GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance))
            {
                var sourceProp = x.GetProperty(destinationProp.Name);
                if (destinationProp.CanWrite && sourceProp.CanRead && sourceProp != null)
                {
                    destinationProp.SetValue(res, sourceProp.GetValue(o));
                }
            }

            return res;
        }

        public static void CopyAllTo<T, T2>(this T source, T2 target)
        {
            var type = typeof(T);
            var type2 = typeof(T2);
            foreach (var sourceProperty in type.GetProperties())
            {
                var targetProperty = type2.GetProperty(sourceProperty.Name);
                if (targetProperty != null && targetProperty.CanWrite)
                    targetProperty.SetValue(target, sourceProperty.GetValue(source, null), null);
            }
        }
        public static Type GetGenericCollectionItemType(Type type)
        {
            return type.GetInterfaces()
                .Where(face => face.IsGenericType &&
                               face.GetGenericTypeDefinition() == typeof(ICollection<>))
                .Select(face => face.GetGenericArguments()[0])
                .FirstOrDefault();
        }


        public static Dictionary<string, string> GetEnumToDictionary<T>(T[] p_banned = null)
        {
            var dic = Enum.GetValues(typeof(T))
               .Cast<T>().Where(w => p_banned == null || !p_banned.Contains(w))

               .ToDictionary(k => k.ToString(), v => GetEnumDescription(v as Enum));
            return dic;
        }

        public static Dictionary<int, string> GetEnumToIndexedDictionary<T>(T[] p_banned = null)
        {
            var retDic = new Dictionary<int, string>();

            var dic = Enum.GetValues(typeof(T))
               .Cast<T>().Where(w => p_banned == null || !p_banned.Contains(w));

            var i = 0;
            foreach (var e in dic)
            {
                retDic.Add(i++, e.ToString());
            }

            return retDic;
        }


        public static string GetEnumDescription(Enum p_value)
        {
            FieldInfo fi = p_value.GetType().GetField(p_value.ToString());

            DescriptionAttribute[] attributes =
                (DescriptionAttribute[])fi.GetCustomAttributes(
                typeof(DescriptionAttribute),
                false);

            if (attributes != null &&
                attributes.Length > 0)
                return attributes[0].Description;
            else
                return p_value.ToString();
        }

    
        public static IEnumerable<TEnum> FilterEnumWithAttributeOf<TEnum, TAttribute>()
            where TEnum : struct
            where TAttribute : class
        {
            foreach (var field in
                typeof(TEnum).GetFields(BindingFlags.GetField |
                                         BindingFlags.Public |
                                         BindingFlags.Static))
            {

                if (field.GetCustomAttributes(typeof(TAttribute), false).Length > 0)
                    yield return (TEnum)field.GetValue(null);
            }
        }

        public static string StrLeft(string p_str, int p_len)
        {
            string result = p_str.Substring(0, p_len);
            return result;
        }
        public static string StrRight(string p_str, int p_len)
        {
            string result = p_str.Substring(p_str.Length - p_len, p_len);
            return result;
        }
        public static string GetISOYearAndWeek(DateTime p_date)
        {
            DayOfWeek day = CultureInfo.InvariantCulture.Calendar.GetDayOfWeek(p_date);
            if (day >= DayOfWeek.Monday && day <= DayOfWeek.Wednesday)
            {
                p_date = p_date.AddDays(3);
            }

            // Return the week of our adjusted day
            var ISOWeekNumber = CultureInfo.InvariantCulture.Calendar.GetWeekOfYear(p_date, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);

            return String.Format("{0}_{1}", p_date.Year, StrRight("0" + ISOWeekNumber, 2));
            //      return String.Format("{0}_{1}", p_date.Year, ISOWeekNumber );

        }
        public static DateTime FirstDateOfWeekISO8601(string yearWeek)
        {
            string[] items = yearWeek.Split('_');
            return FirstDateOfWeekISO8601(int.Parse(items[0]), int.Parse(items[1]));


        }

        public static DateTime FirstDateOfWeekISO8601(int year, int weekOfYear)
        {
            DateTime jan1 = new DateTime(year, 1, 1);
            int daysOffset = DayOfWeek.Thursday - jan1.DayOfWeek;

            DateTime firstThursday = jan1.AddDays(daysOffset);
            var cal = CultureInfo.CurrentCulture.Calendar;
            int firstWeek = cal.GetWeekOfYear(firstThursday, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);

            var weekNum = weekOfYear;
            if (firstWeek <= 1)
            {
                weekNum -= 1;
            }
            var result = firstThursday.AddDays(weekNum * 7);
            return result.AddDays(-3);
        }


        public static IEnumerable<string> ChunkString(string str, int maxChunkSize)
        {
            for (int i = 0; i < str.Length; i += maxChunkSize)
                yield return str.Substring(i, Math.Min(maxChunkSize, str.Length - i));
        }

 
        public static String HexColor(System.Drawing.Color c)
        {
            return "#" + c.R.ToString("X2") + c.G.ToString("X2") + c.B.ToString("X2");
        }

        public static String MinToTime(int p_minutes)
        {
            if (p_minutes < 0)
                return "?";
            var sHour = "0" + (p_minutes / 60).ToString();
            var sMin = "0" + (p_minutes % 60).ToString();
            return sHour.Substring(sHour.Length - 2, 2) + ":" + sMin.Substring(sMin.Length - 2, 2);
        }

        public static int TimeToMin(string p_time)
        {

            string[] parts = p_time.Split(':');
            if (parts.Length < 2)
            {
                return -1;
            }
            int hours = 0;
            if (!Int32.TryParse(parts[0], out hours))
                throw new Exception("Wrong time format:" + p_time);
            int minutes = 0;
            if (!Int32.TryParse(parts[1], out minutes))
                throw new Exception("Wrong time format:" + p_time);
            return hours * 60 + minutes;
        }

        public static int StringToInt(string toParse, int defVal = 0)
        {
            int result = defVal;
            Int32.TryParse(toParse, out result);
            return result;
        }


        /// <summary>
        /// Determines a text file's encoding by analyzing its byte order mark (BOM).
        /// Defaults to ASCII when detection of the text file's endianness fails.
        /// </summary>
        /// <param name="filename">The text file to analyze.</param>
        /// <returns>The detected encoding.</returns>
        /// <summary>
        /// Determines a text file's encoding by analyzing its byte order mark (BOM) and if not found try parsing into diferent encodings       
        /// Defaults to UTF8 when detection of the text file's endianness fails.
        /// </summary>
        /// <param name="filename">The text file to analyze.</param>
        /// <returns>The detected encoding or null.</returns>
        public static Encoding GetFileEncoding(string filename)
        {
            var encodingByBOM = GetEncodingByBOM(filename);
            if (encodingByBOM != null)
                return encodingByBOM;

            // BOM not found :(, so try to parse characters into several encodings
            var encodingByParsingUTF8 = GetEncodingByParsing(filename, Encoding.UTF8);
            if (encodingByParsingUTF8 != null)
                return encodingByParsingUTF8;

            var encodingByParsingLatin1 = GetEncodingByParsing(filename, Encoding.GetEncoding("iso-8859-1"));
            if (encodingByParsingLatin1 != null)
                return encodingByParsingLatin1;

            var encodingByParsingUTF7 = GetEncodingByParsing(filename, Encoding.UTF7);
            if (encodingByParsingUTF7 != null)
                return encodingByParsingUTF7;

            return null;   // no encoding found
        }

        /// <summary>
        /// Determines a text file's encoding by analyzing its byte order mark (BOM)  
        /// </summary>
        /// <param name="filename">The text file to analyze.</param>
        /// <returns>The detected encoding.</returns>
        private static Encoding GetEncodingByBOM(string filename)
        {
            // Read the BOM
            var byteOrderMark = new byte[4];
            using (var file = new FileStream(filename, FileMode.Open, FileAccess.Read))
            {
                file.Read(byteOrderMark, 0, 4);
            }

            // Analyze the BOM
            if (byteOrderMark[0] == 0x2b && byteOrderMark[1] == 0x2f && byteOrderMark[2] == 0x76) return Encoding.UTF7;
            if (byteOrderMark[0] == 0xef && byteOrderMark[1] == 0xbb && byteOrderMark[2] == 0xbf) return Encoding.UTF8;
            if (byteOrderMark[0] == 0xff && byteOrderMark[1] == 0xfe) return Encoding.Unicode; //UTF-16LE
            if (byteOrderMark[0] == 0xfe && byteOrderMark[1] == 0xff) return Encoding.BigEndianUnicode; //UTF-16BE
            if (byteOrderMark[0] == 0 && byteOrderMark[1] == 0 && byteOrderMark[2] == 0xfe && byteOrderMark[3] == 0xff) return Encoding.UTF32;

            return null;    // no BOM found
        }

        private static Encoding GetEncodingByParsing(string filename, Encoding encoding)
        {
            var encodingVerifier = Encoding.GetEncoding(encoding.BodyName, new EncoderExceptionFallback(), new DecoderExceptionFallback());

            try
            {
                using (var textReader = new StreamReader(filename, encodingVerifier, detectEncodingFromByteOrderMarks: true))
                {
                    while (!textReader.EndOfStream)
                    {
                        textReader.ReadLine();   // in order to increment the stream position
                    }

                    // all text parsed ok
                    return textReader.CurrentEncoding;
                }
            }
            catch (Exception ex) { }

            return null;    // 
        }
        public static double GetCsvDouble(string p_strNum)
        {
            double ret = 0;

            if (!double.TryParse(p_strNum.Replace(",", "."), out ret))
            {

                return double.Parse(p_strNum);
            }
            return ret;
        }

        public static string ConvertToBase64String(string p_str, Encoding p_enc = null)
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
        public static string ConvertFromBase64String(string p_strBase64, Encoding p_enc = null)
        {
            byte[] bytes = Convert.FromBase64String(p_strBase64);

            string ret = null;

            if (p_enc != null)
            {
                ret = p_enc.GetString(bytes);
            }
            else
            {
                ret = System.Text.Encoding.UTF8.GetString(bytes);
            }

            return Convert.ToBase64String(bytes);
        }
        public static string ConvertToHexString(string p_str, Encoding p_enc = null)
        {
            var sb = new StringBuilder();

            byte[] bytes;
            if (p_enc != null)
            {
                bytes = p_enc.GetBytes(p_str);
            }
            else
            {
                bytes = Encoding.UTF8.GetBytes(p_str);
            }

            foreach (var t in bytes)
            {
                sb.Append(t.ToString("X2"));
            }

            return sb.ToString();
        }

        public static string ConvertFromHexString(string p_hexstr, Encoding p_enc = null)
        {
            var bytes = new byte[p_hexstr.Length / 2];
            for (var i = 0; i < bytes.Length; i++)
            {
                bytes[i] = Convert.ToByte(p_hexstr.Substring(i * 2, 2), 16);
            }
            string ret = null;

            if (p_enc != null)
            {
                ret = p_enc.GetString(bytes);
            }
            else
            {
                ret = System.Text.Encoding.UTF8.GetString(bytes);
            }
            return ret;
        }

        public static bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }


        public static byte[] ObjectToByteArray(this object obj)
        {
            if (obj == null)
            {
                return null;
            }
            var bf = new BinaryFormatter();
            using (var ms = new MemoryStream())
            {
                bf.Serialize(ms, obj);
                return ms.ToArray();
            }
        }

        public static T ByteArrayToObject<T>(this byte[] byteArray) where T : class
        {
            if (byteArray == null)
            {
                return null;
            }
            using (var memStream = new MemoryStream())
            {
                var binForm = new BinaryFormatter();
                memStream.Write(byteArray, 0, byteArray.Length);
                memStream.Seek(0, SeekOrigin.Begin);
                var obj = (T)binForm.Deserialize(memStream);
                return obj;
            }
        }

        public static string ReplaceTokensInContent(string p_content, object p_obj)
        {
            var retContent = p_content;
            var t = p_obj.GetType();
            PropertyInfo[] props = t.GetProperties().ToArray<PropertyInfo>();


            foreach (var prop in props)
            {
                try
                {
                    retContent = retContent.Replace("@@" + prop.Name, prop.GetValue(p_obj).ToString());
                }
                catch
                {
                    retContent = retContent.Replace("@@" + prop.Name, "???");
                }
            }
            return retContent;
        }

        /// <summary>
        /// Extension for 'Object' that copies the properties to a destination object.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="p_destination">The destination.</param>
        public static void CopyByProperties(this object p_source, object p_destination)
        {
            // If any this null throw an exception
            if (p_source == null || p_destination == null)
                throw new Exception("Source or/and Destination Objects are null");
            // Getting the Types of the objects
            Type typeDest = p_destination.GetType();
            Type typeSrc = p_source.GetType();

            // Iterate the Properties of the source instance and  
            // populate them from their desination counterparts  
            PropertyInfo[] srcProps = typeSrc.GetProperties();
            foreach (PropertyInfo srcProp in srcProps)
            {
                if (!srcProp.CanRead)
                {
                    continue;
                }
                PropertyInfo targetProperty = typeDest.GetProperty(srcProp.Name);
                if (targetProperty == null)
                {
                    continue;
                }
                if (!targetProperty.CanWrite)
                {
                    continue;
                }
                if (targetProperty.GetSetMethod(true) != null && targetProperty.GetSetMethod(true).IsPrivate)
                {
                    continue;
                }
                if ((targetProperty.GetSetMethod().Attributes & MethodAttributes.Static) != 0)
                {
                    continue;
                }
                if (!targetProperty.PropertyType.IsAssignableFrom(srcProp.PropertyType))
                {
                    continue;
                }
                // Passed all tests, lets set the value
                targetProperty.SetValue(p_destination, srcProp.GetValue(p_source, null), null);
            }
        }

        public static bool JsonCompare(this object p_obj1, object p_obj2)
        {
            if (ReferenceEquals(p_obj1, p_obj2)) return true;
            if ((p_obj1 == null) || (p_obj2 == null)) return false;
            if (p_obj1.GetType() != p_obj2.GetType()) return false;

            var objJson = JsonConvert.SerializeObject(p_obj1);
            var anotherJson = JsonConvert.SerializeObject(p_obj2);

            return objJson == anotherJson;
        }

        public static bool EqualByProperties(object original, object altered)
        {
            bool result = true;

            //Get the class
            Type o = original.GetType();
            Type a = altered.GetType();

            //Cycle through the properties.
            foreach (PropertyInfo p in o.GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
 
                if (!p.PropertyType.IsGenericType)
                {
                    if (p.GetValue(original, null) != null && p.GetValue(altered, null) != null)
                    {
                        if (!p.GetValue(original, null).ToString().Equals(p.GetValue(altered, null).ToString()))
                        {
                            result = false;
                            break;
                        }
                    }
                    else
                    {
                        //If one is null, the other is not
                        if ((p.GetValue(original, null) == null && p.GetValue(altered, null) != null) || (p.GetValue(original, null) != null && p.GetValue(altered, null) == null))
                        {
                            result = false;
                            break;
                        }
                    }
                }
            }

            return result;
        }


        public static byte[] Zip(string str)
        {
            var bytes = Encoding.UTF8.GetBytes(str);

            using (var msi = new MemoryStream(bytes))
            using (var mso = new MemoryStream())
            {
                using (var gs = new GZipStream(mso, CompressionMode.Compress))
                {
                    //msi.CopyTo(gs);
                    CopyTo(msi, gs);
                }

                return mso.ToArray();
            }
        }

        public static string Unzip(byte[] bytes)
        {
            using (var msi = new MemoryStream(bytes))
            using (var mso = new MemoryStream())
            {
                using (var gs = new GZipStream(msi, CompressionMode.Decompress))
                {
                    //gs.CopyTo(mso);
                    CopyTo(gs, mso);
                }

                return Encoding.UTF8.GetString(mso.ToArray());
            }
        }
        private static void CopyTo(Stream src, Stream dest)
        {
            byte[] bytes = new byte[4096];

            int cnt;

            while ((cnt = src.Read(bytes, 0, bytes.Length)) != 0)
            {
                dest.Write(bytes, 0, cnt);
            }
        }

        public static Stream StringToStream(string s)
        {
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            writer.Write(s);
            writer.Flush();
            stream.Position = 0;
            return stream;
        }
    }

}
