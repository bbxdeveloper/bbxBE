using bbxBE.Application.Interfaces;
using bbxBE.Domain.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace bbxBE.Infrastructure.Persistence.Caches
{
    public class ProductCacheService : BaseCacheService<Product>, ICacheService<Product>
    {
        public ProductCacheService(ILoggerFactory loggerFactory, IConfiguration p_Configuration) 
            : base(loggerFactory, p_Configuration)
        {
        }
    }
}
