﻿using bbxBE.Application.Interfaces;
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
using bbxBE.Common.Globals;
using Microsoft.EntityFrameworkCore;
using bbxBE.Application.Exceptions;
using bbxBE.Application.Consts;
using bbxBE.Common.Locking;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
namespace bbxBE.Infrastructure.Persistence.Caches
{
    public class BaseCacheService<T> : ICacheService<T> where T : BaseEntity
    {
        private System.Collections.Concurrent.ConcurrentDictionary<long, T> _cache = new System.Collections.Concurrent.ConcurrentDictionary<long, T>();
        private IQueryable<T> _cacheQuery = null;
        private readonly ILogger _logger;
        private readonly IConfiguration _configuration;
        private readonly ILoggerFactory _loggerFactory;

        private readonly string _cacheID;
        public BaseCacheService(ILoggerFactory loggerFactory, IConfiguration p_Configuration)
        {
            //            _logger = p_Logger;
            _configuration = p_Configuration;
            _loggerFactory = loggerFactory;
            _logger = loggerFactory.CreateLogger("CacheService");
            _cacheID = System.Guid.NewGuid().ToString();
        }

        public bool TryGetValue(long ID, out T value)
        {
            value = null;
            return _cache.TryGetValue(ID, out value);
        }
        public T AddOrUpdate(T value)
        {
            T prod = null;
            prod = _cache.AddOrUpdate(value.ID, value, (key, oldValue) => value);
            return prod;
        }

        public bool TryRemove(T value)
        {
            bool succeed = false;
            succeed = _cache.TryRemove(value.ID, out value);
            return succeed;
        }

        public bool IsCacheEmpty()
        {
            return _cache == null || _cache.Count() == 0;
        }

        public IQueryable<T> QueryCache()
        {
            return _cache.Values.AsQueryable();
        }

        private static LockProvider<string> CacheLockProvider = new LockProvider<string>();

        public async Task RefreshCache(IQueryable<T> query = null)
        {
            var className = this.GetType().Name;

            _logger.LogInformation($"{className} cache refresh START");
            var waitForCacheInSeconds = _configuration
                .GetRequiredSection(bbxBEConsts.CONF_CacheSettings).GetValue<int>(bbxBEConsts.CONF_WaitForCacheInSeconds);

            bool locked = await CacheLockProvider.WaitAsync(_cacheID, waitForCacheInSeconds * 1000);
            if (!locked)
            {
                _logger.LogError($"{className} cache LOCKED");
                throw new LockedCacheException(string.Format($"{className}:"+bbxBEConsts.ERR_LOCKEDCACHE));
            }

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
                _cache = new System.Collections.Concurrent.ConcurrentDictionary<long, T>(result);
            }
            finally
            {
                // release the lock
                CacheLockProvider.Release(_cacheID);
                _logger.LogInformation($"{className} cache refresh END");
            }

        }
    }
}