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
using bbxBE.Infrastructure.Persistence.Locking;

namespace bbxBE.Infrastructure.Persistence.Caches
{
    public class ProductCacheService: ICacheService<Product>
    {
        private System.Collections.Concurrent.ConcurrentDictionary<long, Product> _cache = new System.Collections.Concurrent.ConcurrentDictionary<long, Product>();
        public ProductCacheService()
        {
        }
        public bool TryGetValue(long ID, out Product value)
        {
            value = null;
            return _cache.TryGetValue(ID, out value);
        }
        public Product AddOrUpdate( Product value)
        {
            return _cache.AddOrUpdate( value.ID, value, (key, oldValue) => value);
        }

        public bool TryRemove(Product value)
        {
            return _cache.TryRemove(value.ID, out value);
        }
        public async Task RefreshCacheAsynch( IQueryable<Product> query)
        {
            using (var lockObj = new ProductCacheLocker(GlobalLockObjs.ProductCacheLocker))
            {

                _cache = new System.Collections.Concurrent.ConcurrentDictionary<long, Product>();
                var result = await query.ToDictionaryAsync( i=>i.ID);
                _cache = new System.Collections.Concurrent.ConcurrentDictionary<long, Product>(result);
            }
        }
    }
}
