using AxegazMobileSrv.Attrib;
using bbxBE.Application.Interfaces;
using System;
using System.Linq;

namespace bbxBE.Application.Helpers
{
    public class ModelHelper : IModelHelper
    {
        /// <summary>
        /// Check field name in the model class
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="fieldName"></param>
        /// <returns></returns>
        public string ValidateModelFields<T>(string fields)
        {
            string retString = string.Empty;

            var bindingFlags = System.Reflection.BindingFlags.Instance |
                                System.Reflection.BindingFlags.NonPublic |
                                System.Reflection.BindingFlags.Public;
            var listFields = typeof(T).GetProperties(bindingFlags).Where(pi => !Attribute.IsDefined(pi, typeof(NotDBFieldAttribute))).Select(f => f.Name).ToList();
            string[] arrayFields = fields.Split(',');
            foreach (var field in arrayFields)
            {
                if (listFields.Contains(field.Trim(), StringComparer.OrdinalIgnoreCase))
                    retString += field + ",";
            };
            return retString;
        }

        /// <summary>
        /// Get list of field names in the model class
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public string GetModelFields<T>()
        {
            string retString = string.Empty;

            var bindingFlags = System.Reflection.BindingFlags.Instance |
                                System.Reflection.BindingFlags.NonPublic |
                                System.Reflection.BindingFlags.Public;


           var listFields = typeof(T).GetProperties(bindingFlags).Where(pi => !Attribute.IsDefined(pi, typeof(NotDBFieldAttribute))).Select(f => f.Name).ToList();

            foreach (string field in listFields)
            {
                retString += field + ",";
            }

            return retString;
        }


        /// <summary>
        /// Get list of field names in the model class
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public string GetDtoFields<T>()
        {
            string retString = string.Empty;

            var bindingFlags =  (System.Reflection.BindingFlags.Public |
                                System.Reflection.BindingFlags.SetField);


            var listFields = typeof(T).GetProperties(bindingFlags).Select(f => f.Name).ToList();

            foreach (string field in listFields)
            {
                retString += field + ",";
            }

            return retString;
        }

    }
}