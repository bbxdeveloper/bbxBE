using AsyncKeyedLock;
using bbxBE.Common.Consts;
using bbxBE.Common.Exceptions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace bbxBE.Common.ExpiringData
{

    public interface  IExpiringData<T> where T : ExpiringDataObject
    {
        Task<T> GetItem(string Key);
        Task AddOrUpdateItemAsync(string Key, object Data, string SessionID, TimeSpan Lifetime);
        Task DeleteItemAsync(string Key);
    }
}
