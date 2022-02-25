using AxegazMobileSrv.Attrib;
using bbxBE.Application.Interfaces;
using System;
using System.Collections.Generic;
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
        public string ValidateModelFields<TModel, TDto>(string modelFields)
        {
            string retString = string.Empty;

            var dbFields = GetQueryableFields<TModel, TDto>();
            string[] dbFieldsArr = dbFields.Split(',');
            string[] modelFieldsArr = modelFields.Split(',');
            foreach (var field in modelFieldsArr)
            {
                if (dbFieldsArr.Contains(field.Trim(), StringComparer.OrdinalIgnoreCase))
                    retString += field + ",";
            };
            return retString;
        }

        /// <summary>
        /// Get list of queryable  DB field names 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public string GetQueryableFields<TModel, TDto>()
        {
            string retString = string.Empty;


            var listFieldsModel = GetModelFields<TModel>();
            var listFieldsDB = GetDBFields<TDto>();
            foreach (var field in listFieldsModel)
            {
                if (listFieldsDB.Contains(field.Trim(), StringComparer.OrdinalIgnoreCase))
                    retString += field + ",";
            };
            return retString;
        }


        public List<string> GetModelFields<T>()
        {

            var bindingFlags = System.Reflection.BindingFlags.Instance |
                                System.Reflection.BindingFlags.NonPublic |
                                System.Reflection.BindingFlags.Public;


  //          return typeof(T).GetProperties(bindingFlags).Where(pi => !Attribute.IsDefined(pi, typeof(NotModelFieldAttribute))).Select(f => f.Name).ToList();
            return typeof(T).GetProperties(bindingFlags).Select(f => f.Name).ToList();

        }

        /// <summary>
        /// Get list of DB field names in the DTO class
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public List<string> GetDBFields<T>()
        {
            string retString = string.Empty;

            var bindingFlags = System.Reflection.BindingFlags.Instance |
                                        System.Reflection.BindingFlags.NonPublic |
                                        System.Reflection.BindingFlags.Public;


            return typeof(T).GetProperties(bindingFlags).Select(f => f.Name).ToList();

        }

    }
}