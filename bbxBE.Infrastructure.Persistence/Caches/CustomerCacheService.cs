using AsyncKeyedLock;
using bbxBE.Application.Interfaces;
using bbxBE.Domain.Entities;
using bbxBE.Infrastructure.Persistence.Contexts;
using LinqKit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;

namespace bbxBE.Infrastructure.Persistence.Caches
{
    public class CustomerCacheService : BaseCacheService<Customer>, ICacheService<Customer>
    {
        public CustomerCacheService(ILoggerFactory loggerFactory, IConfiguration p_Configuration, ApplicationDbContext dbcontext, AsyncKeyedLocker<string> asyncKeyedLocker) 
            : base(loggerFactory, p_Configuration, dbcontext, asyncKeyedLocker)
        {
            _cacheQuery = _dbContext.Customer
            .AsNoTracking()
            .AsExpandable();

        }
    }
}
