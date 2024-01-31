using AutoMapper;
using bbxBE.Application.Interfaces;
using bbxBE.Application.Interfaces.Repositories;
using bbxBE.Domain.Entities;

namespace bbxBE.Infrastructure.Persistence.Repositories
{
    public class ProductGroupGlobalRepositoryAsync : ProductGroupRepositoryAsync, IProductGroupGlobalRepositoryAsync
    {

        public ProductGroupGlobalRepositoryAsync(IApplicationGlobalDbContext dbContext,
            IModelHelper modelHelper, IMapper mapper, IMockService mockData,
            ICacheService<ProductGroup> productGroupCacheService, ICacheService<Product> cacheProduct)
            : base(dbContext,
                    modelHelper, mapper, mockData,
                    productGroupCacheService, cacheProduct)
        {

        }

    }
}