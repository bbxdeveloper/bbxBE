using AutoMapper;
using bbxBE.Application.Interfaces;
using bbxBE.Application.Interfaces.Repositories;

namespace bbxBE.Infrastructure.Persistence.Repositories
{
    public class ProductCodeGlobalRepositoryAsync : ProductCodeRepositoryAsync, IProductCodeGlobalRepositoryAsync
    {
        public ProductCodeGlobalRepositoryAsync(IApplicationDbContext dbContext,
            IModelHelper modelHelper, IMapper mapper, IMockService mockData)
            : base(dbContext,
                    modelHelper, mapper, mockData)
        {
        }
    }
}