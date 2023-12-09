using AsyncKeyedLock;
using bbxBE.Application.Interfaces;
using bbxBE.Domain.Entities;
using bbxBE.Infrastructure.Persistence.Contexts;
using LinqKit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Serilog;

namespace bbxBE.Infrastructure.Persistence.Caches
{
    public class ProductGroupCacheService : BaseCacheService<ProductGroup>, ICacheService<ProductGroup>
    {
        public ProductGroupCacheService(ILogger logger, IConfiguration p_Configuration, ApplicationDbContext dbContext, AsyncKeyedLocker<string> asyncKeyedLocker)
            : base(logger, p_Configuration, dbContext, asyncKeyedLocker)
        {
            _cacheQuery = dbContext.ProductGroup
                          .AsNoTracking()
                          .AsExpandable();
        }
    }
}
