using AutoMapper;
using bbxBE.Application.Interfaces;
using bbxBE.Application.Interfaces.Repositories;
using bbxBE.Application.Queries.ViewModels;
using bbxBE.Domain.Entities;
using bbxBE.Infrastructure.Persistence.Contexts;

namespace bbxBE.Infrastructure.Persistence.Repositories
{
    public class ProductGroupGlobalRepositoryAsync : ProductGroupRepositoryAsync, IProductGroupGlobalRepositoryAsync
    {

        public ProductGroupGlobalRepositoryAsync(ApplicationGlobalDbContext dbContext,
            IDataShapeHelper<ProductGroup> dataShaperProductGroup,
            IDataShapeHelper<GetProductGroupViewModel> dataShaperGetProductGroupViewModel,
            IModelHelper modelHelper, IMapper mapper, IMockService mockData,
            ICacheService<ProductGroup> productGroupCacheService)
            : base(dbContext,
                    dataShaperProductGroup,
                    dataShaperGetProductGroupViewModel,
                    modelHelper, mapper, mockData,
                    productGroupCacheService)
        {

        }

    }
}