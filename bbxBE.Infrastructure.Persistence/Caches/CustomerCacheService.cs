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


namespace bbxBE.Infrastructure.Persistence.Caches
{
    public class CustomerCacheService : ICacheService<Customer>
    {
        private System.Collections.Concurrent.ConcurrentDictionary<long, Customer> _cache = new System.Collections.Concurrent.ConcurrentDictionary<long, Customer>();
        public CustomerCacheService()
        {
        }
        public bool TryGetValue(long ID, out Customer value)
        {
            value = null;
            return _cache.TryGetValue(ID, out value);
        }
        public Customer AddOrUpdate(Customer value)
        {
            Customer prod = null;
            prod = _cache.AddOrUpdate(value.ID, value, (key, oldValue) => value);
            return prod;
        }

        public bool TryRemove(Customer value)
        {
            bool succeed = false;
            succeed = _cache.TryRemove(value.ID, out value);
            return succeed;
        }

        public bool IsCacheEmpty()
        {
            return _cache == null || _cache.Count() == 0;
        }

        public IQueryable<Customer> QueryCache()
        {
            return _cache.Values.AsQueryable();
        }
        public async Task RefreshCache(IQueryable<Customer> query)
        {

            var result = await query.ToDictionaryAsync(i => i.ID);
            _cache = new System.Collections.Concurrent.ConcurrentDictionary<long, Customer>(result);
        }
    }
}
