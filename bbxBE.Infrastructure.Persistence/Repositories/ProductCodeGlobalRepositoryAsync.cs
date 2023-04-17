using AutoMapper;
using bbxBE.Application.Interfaces;
using bbxBE.Application.Interfaces.Repositories;
using bbxBE.Application.Queries.ViewModels;
using bbxBE.Domain.Entities;
using bbxBE.Infrastructure.Persistence.Contexts;

namespace bbxBE.Infrastructure.Persistence.Repositories
{
    public class ProductCodeGlobalRepositoryAsync : ProductCodeRepositoryAsync, IProductCodeGlobalRepositoryAsync
    {
        public ProductCodeGlobalRepositoryAsync(ApplicationGlobalDbContext dbContext,
            IDataShapeHelper<ProductGroup> dataShaperProductGroup,
            IDataShapeHelper<GetProductGroupViewModel> dataShaperGetProductGroupViewModel,
            IModelHelper modelHelper, IMapper mapper, IMockService mockData)
            : base(dbContext,
                    dataShaperProductGroup,
                    dataShaperGetProductGroupViewModel,
                    modelHelper, mapper, mockData)
        {
        }
    }
}