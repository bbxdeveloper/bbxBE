﻿using bbxBE.Application.Interfaces;
using bbxBE.Domain.Entities;
using bbxBE.Infrastructure.Persistence.Contexts;
using LinqKit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;

namespace bbxBE.Infrastructure.Persistence.Caches
{

    public class OriginCacheService : BaseCacheService<Origin>, ICacheService<Origin>
    {
        public OriginCacheService(ILoggerFactory loggerFactory, IConfiguration p_Configuration, ApplicationDbContext dbcontext)
            : base(loggerFactory, p_Configuration, dbcontext)
        {
            _cacheQuery = dbcontext.Origin
                .AsNoTracking()
                .AsExpandable();
        }
    }
}
