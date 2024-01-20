using AsyncKeyedLock;
using bbxBE.Common.Consts;
using bbxBE.Common.Exceptions;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace bbxBE.Common.ExpiringData
{

    public class ExpiringData<T> : IExpiringData<T> where T : ExpiringDataObject
    {

        private readonly AsyncKeyedLocker<string> _asyncKeyedLocker;
        private readonly IConfiguration _configuration;
        private ConcurrentDictionary<string, T> ExpiringDataList { get; set; } = new ConcurrentDictionary<string, T>();
        public ExpiringData(IConfiguration p_Configuration, AsyncKeyedLocker<string> asyncKeyedLocker)
        {
            _asyncKeyedLocker = asyncKeyedLocker;
            _configuration = p_Configuration;
        }

        public async Task<T> GetItem(string Key, bool silent = true)
        {

            T ret = null;
            bool bOK = await _asyncKeyedLocker.TryLockAsync(Key, async () =>
            {

                T dobj;
                ExpiringDataList.TryGetValue(Key, out dobj);
                if (dobj != null && dobj.ExpiredTimeStamp >= DateTime.UtcNow)
                {
                    ret = dobj;
                }

                if (dobj == null && !silent)
                {
                    throw new LockException(string.Format(bbxBEConsts.ERR_UNLOCK, Key));
                }

            }, bbxBEConsts.WaitForExpiringDataSec * 1000).ConfigureAwait(false);
            if (!bOK)
            {
                throw new LockException(string.Format(bbxBEConsts.ERR_LOCK, Key));
            }
            return ret;
        }

        public async Task<T> AddOrUpdateItemAsync(string Key, object Data, string SessionID, TimeSpan Lifetime)
        {

            T dobj = null;
            bool bOK = await _asyncKeyedLocker.TryLockAsync(Key, async () =>
           {

               ExpiringDataList.TryGetValue(Key, out dobj);
               if (dobj == null)
               {
                   dobj = Activator.CreateInstance<T>();
                   dobj.Key = Key;
                   dobj.Data = Data;
                   dobj.SessionID = SessionID;
                   dobj.LastModifiedTimestamp = DateTime.UtcNow;
                   dobj.Lifetime = Lifetime;
                   ExpiringDataList.TryAdd(Key, dobj);
               }
               else
               {
                   if (dobj.ExpiredTimeStamp > DateTime.UtcNow && dobj.SessionID != SessionID)
                   {
                       throw new LockException(string.Format(bbxBEConsts.ERR_LOCK, Key));
                   }
                   dobj.Data = Data;
                   dobj.Lifetime = Lifetime;
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
            return dobj;

        }
        public async Task DeleteItemAsync(string Key, bool silent = true)
        {

            bool bOK = await _asyncKeyedLocker.TryLockAsync(Key, async () =>
            {
                T dobj;

                ExpiringDataList.TryGetValue(Key, out dobj);
                if (dobj == null && !silent)
                {
                    throw new LockException(string.Format(bbxBEConsts.ERR_UNLOCK, Key));
                }
                if (dobj != null)
                {
                    ExpiringDataList.TryRemove(Key, out dobj);
                }
            }, bbxBEConsts.WaitForExpiringDataSec * 1000).ConfigureAwait(false);
            if (!bOK)
            {
                throw new LockException(string.Format(bbxBEConsts.ERR_LOCK, Key));
            }
        }

        public async Task DeleteItemsByKeyPartAsync(string KeyPart, bool silent = true)
        {

            bool bOK = await _asyncKeyedLocker.TryLockAsync(KeyPart, async () =>
            {
                T dobj;

                var locks = ExpiringDataList.Where(w => w.Key.ToUpper().Contains(KeyPart.ToUpper()));
                foreach (var lockDict in locks)
                {
                    ExpiringDataList.TryRemove(lockDict.Key, out dobj);

                }

            }, bbxBEConsts.WaitForExpiringDataSec * 1000).ConfigureAwait(false);
            if (!bOK)
            {
                throw new LockException(string.Format(bbxBEConsts.ERR_LOCK, KeyPart));
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

        public void Purge()
        {
            ExpiringDataList = new ConcurrentDictionary<string, T>();
        }

        public IList<T> List()
        {

            //List<T> ret = new List<T>();
            var ret = ExpiringDataList.Select(s => s.Value).ToList();
            return ret;
        }

    }
}