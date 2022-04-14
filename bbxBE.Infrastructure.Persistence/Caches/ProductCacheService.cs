using bbxBE.Application.Interfaces;
using bbxBE.Domain.Settings;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;

namespace bbxBE.Infrastructure.Persistence.Caches
{
    public class ProductCacheService : ICacheService
    {
        private readonly IMemoryCache _memoryCache;
        private readonly ProductCacheSettings _productCacheSettings;
        private MemoryCacheEntryOptions _cacheOptions;
        public ProductCacheService(IMemoryCache memoryCache, IOptions<ProductCacheSettings> productCacheSettings)
        {
            _memoryCache = memoryCache;
            _productCacheSettings = productCacheSettings.Value;
            if (_productCacheSettings != null)
            {
                _cacheOptions = new MemoryCacheEntryOptions
                {
                    AbsoluteExpiration = DateTime.Now.AddHours(_productCacheSettings.AbsoluteExpirationInHours),
                    Priority = CacheItemPriority.High,
                    SlidingExpiration = TimeSpan.FromMinutes(_productCacheSettings.SlidingExpirationInMinutes)
                };
            }
        }
        public bool TryGet<T>(long ID, out T value)
        {
            _memoryCache.TryGetValue(ID, out value);
            if (value == null) return false;
            else return true;
        }
        public T Set<T>(long ID, T value)
        {
            return _memoryCache.Set(ID, value, _cacheOptions);
        }
        public void Remove(long ID)
        {
            _memoryCache.Remove(ID);
        }
   

    }
}
