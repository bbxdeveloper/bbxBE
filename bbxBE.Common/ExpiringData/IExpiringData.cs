using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace bbxBE.Common.ExpiringData
{

    public interface IExpiringData<T> where T : ExpiringDataObject
    {
        Task<T> GetItem(string Key, bool silent = true);
        Task<T> AddOrUpdateItemAsync(string Key, object Data, string SessionID, TimeSpan Lifetime);
        Task DeleteItemAsync(string Key, bool silent = true);
        Task DeleteItemsByKeyPartAsync(string KeyPart, bool silent = true);
        void Purge();
        IList<T> List();
    }
}
