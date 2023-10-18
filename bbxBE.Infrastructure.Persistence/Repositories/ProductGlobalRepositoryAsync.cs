using AutoMapper;
using bbxBE.Application.Interfaces;
using bbxBE.Application.Interfaces.Repositories;
using bbxBE.Application.Queries.ViewModels;
using bbxBE.Domain.Entities;

namespace bbxBE.Infrastructure.Persistence.Repositories
{
    public class ProductGlobalRepositoryAsync : ProductRepositoryAsync, IProductGlobalRepositoryAsync
    {
        public ProductGlobalRepositoryAsync(IApplicationCommandDbContext dbContext,
            IDataShapeHelper<Product> dataShaperProduct,
            IDataShapeHelper<GetProductViewModel> dataShaperGetProductViewModel,
            IModelHelper modelHelper, IMapper mapper, IMockService mockData,
            ICacheService<Product> productCacheService,
            ICacheService<ProductGroup> productGroupCacheService,
            ICacheService<Origin> originCacheService,
            ICacheService<VatRate> vatRateCacheService,
            IProductCodeGlobalRepositoryAsync productCodeGlobalRepository
            ) : base(dbContext,
            dataShaperProduct,
            dataShaperGetProductViewModel,
            modelHelper, mapper, mockData,
            productCacheService,
            productGroupCacheService,
            originCacheService,
            vatRateCacheService,
            productCodeGlobalRepository)
        {

        }

    }
}