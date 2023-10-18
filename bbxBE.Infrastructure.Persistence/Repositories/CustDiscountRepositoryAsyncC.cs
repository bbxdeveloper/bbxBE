using AutoMapper;
using bbxBE.Application.Interfaces;
using bbxBE.Application.Interfaces.Repositories;
using bbxBE.Application.Queries.ViewModels;
using bbxBE.Common.ExpiringData;
using bbxBE.Domain.Entities;

namespace bbxBE.Infrastructure.Persistence.Repositories
{
    public class CustDiscountRepositoryAsyncC : CustDiscountRepositoryAsync, ICustDiscountRepositoryAsyncC
    {
        public CustDiscountRepositoryAsyncC(IApplicationCommandDbContext dbContext,
            IDataShapeHelper<CustDiscount> dataShaperCustDiscount,
            IDataShapeHelper<GetCustDiscountViewModel> dataShaperGetCustDiscountViewModel,
            IModelHelper modelHelper, IMapper mapper, IMockService mockData,
            ICacheService<Customer> customerCacheService,
            ICacheService<ProductGroup> productGroupCacheService, IExpiringData<ExpiringDataObject> expiringData)
            : base(dbContext, dataShaperCustDiscount, dataShaperGetCustDiscountViewModel,
                modelHelper, mapper, mockData, customerCacheService, productGroupCacheService, expiringData)
        {
        }

    }
}