using AsyncKeyedLock;
using bbxBE.Application.Interfaces;
using bbxBE.Domain.Entities;
using bbxBE.Infrastructure.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Serilog;

namespace bbxBE.Infrastructure.Persistence.Caches
{
    public class ProductCacheService : BaseCacheService<Product>, ICacheService<Product>
    {
        public ProductCacheService(ILogger logger, IConfiguration p_Configuration, ApplicationDbContext dbContext, AsyncKeyedLocker<string> asyncKeyedLocker)
            : base(logger, p_Configuration, dbContext, asyncKeyedLocker)
        {
#if !DEBUG
           _cacheQuery = _dbContext.Product.AsNoTracking()
                         .Include(p => p.ProductCodes).AsNoTracking()
                         .Include(pg => pg.ProductGroup).AsNoTracking()
                         .Include(o => o.Origin).AsNoTracking()
                         .Include(v => v.VatRate).AsNoTracking()
                         .Include(v => v.Stocks).AsNoTracking();
#else
            _cacheQuery = _dbContext.Product.AsNoTracking()
                 .Include(p => p.ProductCodes).AsNoTracking()
                 .Include(pg => pg.ProductGroup).AsNoTracking()
                 .Include(o => o.Origin).AsNoTracking()
                 .Include(v => v.VatRate).AsNoTracking()
                 .Include(v => v.Stocks).AsNoTracking(); //.Where(w => w.ID == 1745); //.Take(1000);


#endif
        }
    }
}
