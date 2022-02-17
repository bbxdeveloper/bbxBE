
using System.Collections.Generic;

namespace bbxBE.Application.Interfaces
{
    public interface IModelHelper
    {
     
        string ValidateModelFields<TModel, TDto>(string modelFields);
        public string GetQueryableFields<TModel, TDto>();
        public List<string> GetModelFields<T>();
        public List<string> GetDBFields<T>();
    }
}