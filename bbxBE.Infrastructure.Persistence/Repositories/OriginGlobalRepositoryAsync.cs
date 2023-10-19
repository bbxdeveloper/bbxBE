using AutoMapper;
using bbxBE.Application.Interfaces;
using bbxBE.Application.Interfaces.Repositories;
using bbxBE.Domain.Entities;

namespace bbxBE.Infrastructure.Persistence.Repositories
{
    public class OriginGlobalRepositoryAsync : OriginRepositoryAsync, IOriginGlobalRepositoryAsync
    {
        public OriginGlobalRepositoryAsync(IApplicationGlobalDbContext dbContext,
            IModelHelper modelHelper, IMapper mapper, IMockService mockData,
            ICacheService<Origin> originGroupCacheService,
            ICacheService<Product> productCacheService)
            : base(dbContext,
                    modelHelper, mapper, mockData,
                    originGroupCacheService,
                    productCacheService)
        {

        }

    }
}