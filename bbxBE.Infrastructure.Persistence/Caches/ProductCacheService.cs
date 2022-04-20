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
using bbxBE.Common.Globals;
using Microsoft.EntityFrameworkCore;
using bbxBE.Application.Exceptions;
using bbxBE.Application.Consts;

namespace bbxBE.Infrastructure.Persistence.Caches
{
    public class ProductCacheService : ICacheService<Product>
    {
        private System.Collections.Concurrent.ConcurrentDictionary<long, Product> _cache = new System.Collections.Concurrent.ConcurrentDictionary<long, Product>();
        private IQueryable<Product> _cacheQuery = null;
        public ProductCacheService()
        {
        }
        public bool TryGetValue(long ID, out Product value)
        {
            value = null;
            return _cache.TryGetValue(ID, out value);
        }
        public Product AddOrUpdate(Product value)
        {
            Product prod = null;
            prod = _cache.AddOrUpdate(value.ID, value, (key, oldValue) => value);
            return prod;
        }

        public bool TryRemove(Product value)
        {
            bool succeed = false;
            succeed = _cache.TryRemove(value.ID, out value);
            return succeed;
        }

        public bool IsCacheEmpty()
        {
            return _cache == null || _cache.Count() == 0;
        }

        public IQueryable<Product> QueryCache()
        {
            return _cache.Values.AsQueryable();
        }
        public async Task RefreshCache(IQueryable<Product> query = null)
        {

            if (query != null)
            {
                _cacheQuery = query;
            }
            else
            {
                if (_cacheQuery == null)
                    throw new NoCacheQueryException(bbxBEConsts.FV_NOCACHEQUERY);
            }

            var result = await _cacheQuery.ToDictionaryAsync(i => i.ID);
            _cache = new System.Collections.Concurrent.ConcurrentDictionary<long, Product>(result);
        }
    }
}
