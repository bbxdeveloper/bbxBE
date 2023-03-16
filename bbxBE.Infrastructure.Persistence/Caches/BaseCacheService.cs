using bbxBE.Application.Interfaces;
using bbxBE.Domain.Entities;
using bbxBE.Domain.Settings;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using bbxBE.Domain.Common;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using bbxBE.Common.Exceptions;
using bbxBE.Common.Consts;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using System.Linq.Dynamic.Core;
using bbxBE.Infrastructure.Persistence.Contexts;
using Microsoft.Extensions.DependencyInjection;
using AsyncKeyedLock;

namespace bbxBE.Infrastructure.Persistence.Caches
{
    public class BaseCacheService<T> : ICacheService<T> where T : BaseEntity
    {
        public System.Collections.Concurrent.ConcurrentDictionary<long, T> Cache { get; private set; } = null;
        internal IQueryable<T> _cacheQuery = null;
        private readonly ILogger _logger;
        private readonly IConfiguration _configuration;
        private readonly ILoggerFactory _loggerFactory;
        internal readonly ApplicationDbContext _dbContext;
        private readonly AsyncKeyedLocker<string> _asyncKeyedLocker;

        private readonly string _cacheID;
        public BaseCacheService(ILoggerFactory loggerFactory, IConfiguration p_Configuration, ApplicationDbContext dbContext,
            AsyncKeyedLocker<string> asyncKeyedLocker)
        {
            //            _logger = p_Logger;
            _configuration = p_Configuration;
            _loggerFactory = loggerFactory;
            _logger = loggerFactory.CreateLogger("CacheService");
            _cacheID = System.Guid.NewGuid().ToString();
            _dbContext = dbContext;
            _asyncKeyedLocker = asyncKeyedLocker;
        }

        public bool TryGetValue(long ID, out T value)
        {
            if (!IsCacheNull())
            {
                value = null;
                return Cache.TryGetValue(ID, out value);
            }
            else
            {
                throw new CacheException(bbxBEConsts.ERR_CACHE_NOTUSED, typeof(T).FullName);
            }
        }

        public T AddOrUpdate(T value)
        {
            if (!IsCacheNull())
            {
                T prod = null;
                prod = Cache.AddOrUpdate(value.ID, value, (key, oldValue) => value);
                return prod;
            }
            else
            {
                throw new CacheException(bbxBEConsts.ERR_CACHE_NOTUSED, typeof(T).FullName);
            }
        }

        public bool TryRemove(T value)
        {
            if (!IsCacheNull())
            {
                bool succeed = false;
                succeed = Cache.TryRemove(value.ID, out value);
                return succeed;
            }
            else
            {
                throw new CacheException(bbxBEConsts.ERR_CACHE_NOTUSED, typeof(T).FullName);
            }
        }

        public bool IsCacheNull()
        {
            return _cacheQuery == null || Cache == null;
        }

        public IQueryable<T> QueryCache()
        {
            if (!IsCacheNull())
            {
                return Cache.Values.AsQueryable();
            }
            else
            {
                throw new CacheException(bbxBEConsts.ERR_CACHE_NOTUSED, typeof(T).FullName);
            }
        }
        public IList<T> ListCache()
        {
            if (!IsCacheNull())
            {
                return Cache.Values.ToList();
            }
            else
            {
                throw new CacheException(bbxBEConsts.ERR_CACHE_NOTUSED, typeof(T).FullName);
            }
        }

        
        public async Task RefreshCache(IQueryable<T> query = null)
        {
            var className = this.GetType().Name;

            _logger.LogInformation($"{className} cache refresh START");
            var waitForCacheInSeconds = _configuration
                .GetRequiredSection(bbxBEConsts.CONF_CacheSettings).GetValue<int>(bbxBEConsts.CONF_WaitForCacheInSeconds);

            bool bOK = await _asyncKeyedLocker.TryLockAsync(_cacheID, async () =>
            {
                try
                {

                    if (query != null)
                    {
                        _cacheQuery = query;
                    }
                    else
                    {
                        if (_cacheQuery == null)
                            throw new NoCacheQueryException(bbxBEConsts.ERR_NOCACHEQUERY);
                    }
                    var result = await _cacheQuery.ToDictionaryAsync(i => i.ID);
                    Cache = new System.Collections.Concurrent.ConcurrentDictionary<long, T>(result);
                }
                finally
                {
                    _logger.LogInformation($"{className} cache refresh END");
                }
            }, waitForCacheInSeconds * 1000).ConfigureAwait(false);

            if (!bOK)
            {
                _logger.LogError($"{className} cache LOCKED");
                throw new LockedCacheException(string.Format($"{className}:" + bbxBEConsts.ERR_LOCKEDCACHE));
            }

        }

        public void EmptyCache()
        {
            Cache = new System.Collections.Concurrent.ConcurrentDictionary<long, T>();
        }

    }
}