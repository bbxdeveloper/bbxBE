using AutoMapper;
using bbxBE.Application.Interfaces;
using bbxBE.Application.Interfaces.Repositories;
using bbxBE.Domain.Entities;

namespace bbxBE.Infrastructure.Persistence.Repositories
{
    public class ProductGlobalRepositoryAsync : ProductRepositoryAsync, IProductGlobalRepositoryAsync
    {
        public ProductGlobalRepositoryAsync(IApplicationGlobalDbContext dbContext,
            IModelHelper modelHelper, IMapper mapper, IMockService mockData,
            ICacheService<Product> productCacheService,
            ICacheService<ProductGroup> productGroupCacheService,
            ICacheService<Origin> originCacheService,
            ICacheService<VatRate> vatRateCacheService,
            IProductCodeGlobalRepositoryAsync productCodeGlobalRepository
            ) : base(dbContext,
                modelHelper, mapper, mockData,
                productCacheService,
                productGroupCacheService,
                originCacheService,
                vatRateCacheService)
        {

        }

    }
}