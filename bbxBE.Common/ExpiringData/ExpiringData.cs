using AsyncKeyedLock;
using bbxBE.Common.Consts;
using bbxBE.Common.Exceptions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace bbxBE.Common.ExpiringData
{

    public class ExpiringData<T> : IExpiringData<T> where T : ExpiringDataObject
    {

        private readonly AsyncKeyedLocker<string> _asyncKeyedLocker;
        private readonly IConfiguration _configuration;
        private ConcurrentDictionary<string, T> ExpiringDataList { get; } = new ConcurrentDictionary<string, T>();
        public ExpiringData(IConfiguration p_Configuration, AsyncKeyedLocker<string> asyncKeyedLocker)
        {
            _asyncKeyedLocker = asyncKeyedLocker;
            _configuration = p_Configuration;
        }

        public async Task<T> GetItem(string Key)
        {

            T ret = null;
            bool bOK = await _asyncKeyedLocker.TryLockAsync(Key, async () =>
            {

                T dobj;
                ExpiringDataList.TryGetValue(Key, out dobj);
                if (dobj != null && dobj.ExpiredTimeStamp <= DateTime.UtcNow)
                {
                    ret = dobj;
                }
            }, bbxBEConsts.WaitForExpiringDataSec * 1000).ConfigureAwait(false);
            if (!bOK)
            {
                throw new LockException(string.Format(bbxBEConsts.ERR_LOCK, Key));
            }
            return ret;
        }

        public async Task AddOrUpdateItemAsync(string Key, object Data, TimeSpan Lifetime)
        {

            bool bOK = await _asyncKeyedLocker.TryLockAsync(Key, async () =>
           {

               T dobj;
               ExpiringDataList.TryGetValue(Key, out dobj);
               if (dobj == null)
               {
                   dobj = Activator.CreateInstance<T>();
                   dobj.Key = Key; dobj.Data = Data; dobj.LastModifiedTimestamp = DateTime.UtcNow;  dobj.Lifetime = Lifetime;
                   ExpiringDataList.TryAdd(Key, dobj);
               }
               else
               {
                   if (dobj.ExpiredTimeStamp > DateTime.UtcNow)
                   {
                       throw new LockException(string.Format(bbxBEConsts.ERR_LOCK, Key));
                   }
                   dobj.LastModifiedTimestamp = DateTime.UtcNow;
               }

               if (ExpiringDataList.Count > bbxBEConsts.ExpiringDataMaxCount)
               {
                   Maintain();
                   if (ExpiringDataList.Count > bbxBEConsts.ExpiringDataMaxCount)
                       throw new LockException(string.Format(bbxBEConsts.ERR_EXPIRINGDATA_FULL, bbxBEConsts.ExpiringDataMaxCount));
               }


           }, bbxBEConsts.WaitForExpiringDataSec * 1000).ConfigureAwait(false);
            if (!bOK)
            {
                throw new LockException(string.Format(bbxBEConsts.ERR_LOCK, Key));
            }

        }
        public async Task DeleteItemAsync(string Key)
        {

            bool bOK = await _asyncKeyedLocker.TryLockAsync(Key, async () =>
            {
                T dobj;

                ExpiringDataList.TryGetValue(Key, out dobj);
                if (dobj == null)
                {
                    throw new LockException(string.Format(bbxBEConsts.ERR_UNLOCK, Key));
                }

                ExpiringDataList.TryRemove(Key, out dobj);

            }, bbxBEConsts.WaitForExpiringDataSec * 1000).ConfigureAwait(false);
            if (!bOK)
            {
                throw new LockException(string.Format(bbxBEConsts.ERR_LOCK, Key));
            }

        }
        private void Maintain()
        {
            foreach (var kvp in ExpiringDataList)
            {
                if (kvp.Value.ExpiredTimeStamp < DateTime.UtcNow)
                {
                    ExpiringDataList.TryRemove(kvp.Key, out var dobj);
                }
            }
        }
    }
}
