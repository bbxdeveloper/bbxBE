using bbxBE.Application.Interfaces;
using bbxBE.Domain.Entities;
using bbxBE.Infrastructure.Persistence.Contexts;
using LinqKit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace bbxBE.Infrastructure.Persistence.Caches
{
    public class VatRateCacheService : BaseCacheService<VatRate>, ICacheService<VatRate>
    {
        public VatRateCacheService(ILoggerFactory loggerFactory, IConfiguration p_Configuration, ApplicationDbContext dbContext) 
            : base(loggerFactory, p_Configuration, dbContext)
        {

            _cacheQuery = _dbContext.VatRate
                        .AsNoTracking()
                        .AsExpandable();

        }
    }
}
