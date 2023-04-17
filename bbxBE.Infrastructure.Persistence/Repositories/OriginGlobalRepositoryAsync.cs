using AutoMapper;
using bbxBE.Application.Interfaces;
using bbxBE.Application.Interfaces.Repositories;
using bbxBE.Application.Queries.ViewModels;
using bbxBE.Domain.Entities;
using bbxBE.Infrastructure.Persistence.Contexts;

namespace bbxBE.Infrastructure.Persistence.Repositories
{
    public class OriginGlobalRepositoryAsync : OriginRepositoryAsync, IOriginGlobalRepositoryAsync
    {
        public OriginGlobalRepositoryAsync(ApplicationGlobalDbContext dbContext,
            IDataShapeHelper<Origin> dataShaperOrigin,
            IDataShapeHelper<GetOriginViewModel> dataShaperGetOriginViewModel,
            IModelHelper modelHelper, IMapper mapper, IMockService mockData,
            ICacheService<Origin> originGroupCacheService,
            ICacheService<Product> productCacheService)
            : base(dbContext,
                    dataShaperOrigin,
                    dataShaperGetOriginViewModel,
                    modelHelper, mapper, mockData,
                    originGroupCacheService,
                    productCacheService)
        {

        }

    }
}