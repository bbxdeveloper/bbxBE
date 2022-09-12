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
    public class ProductGroupCacheService : BaseCacheService<ProductGroup>, ICacheService<ProductGroup>
    {
        public ProductGroupCacheService(ILoggerFactory loggerFactory, IConfiguration p_Configuration, ApplicationDbContext dbContext) 
            : base(loggerFactory, p_Configuration, dbContext)
        {
            _cacheQuery = dbContext.ProductGroup
                          .AsNoTracking()
                          .AsExpandable();
        }
    }
}
