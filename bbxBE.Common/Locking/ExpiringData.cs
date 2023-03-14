using AsyncKeyedLock;
using bbxBE.Common.Consts;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace bbxBE.Common.Locking
{

    public class ExpiringData
    {
        public class DataObject
        {
            public DataObject()
            {
            }
            public string Key { get; set; }
            public object Data { get; set; }
            public DateTime LastModifiedTimestamp { get; set; }
            public TimeSpan Lifetime { get; set; }
            public DateTime ExpiredTimeStamp
            {
                get { return LastModifiedTimestamp.Add(Lifetime); }
            }
        }
            private readonly AsyncKeyedLocker<string> _asyncKeyedLocker;
            private readonly IConfiguration _configuration;
            private System.Collections.Concurrent.ConcurrentDictionary<string, DataObject> StorageObjectList { get; } = new System.Collections.Concurrent.ConcurrentDictionary<string, DataObject>();
            public ExpiringData(IConfiguration p_Configuration, AsyncKeyedLocker<string> asyncKeyedLocker)
            {
                _asyncKeyedLocker = asyncKeyedLocker;
                _configuration = p_Configuration;


            }

        public async Task AddOrUpdateItemAsync(string Key, object Data, TimeSpan Lifetime)
        {
        }

            public async Task AddOrUpdateItemAsync(string Key, object Data, TimeSpan Lifetime)
            {
                DataObject dobj;


                bool bOK = await _asyncKeyedLocker.TryLockAsync(Key, async () =>
               {

                   StorageObjectList.TryGetValue(Key, out dobj);
                   if (dobj == null)
                   {
                       dobj = new DataObject() { Key = Key, Data = Data, LastModifiedTimestamp = DateTime.UtcNow, Lifetime = Lifetime };

                   }
                   else
                   {
                       dobj.LastModifiedTimestamp = DateTime.UtcNow.Add(Lifetime);
                   }

               }, bbxBEConsts.WaitForExpiringDataSec * 1000).ConfigureAwait(false);

            }

        }
    }
