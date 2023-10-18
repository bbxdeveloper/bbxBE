using AutoMapper;
using bbxBE.Application.Interfaces;
using bbxBE.Application.Interfaces.Repositories;
using bbxBE.Application.Queries.ViewModels;
using bbxBE.Common.Consts;
using bbxBE.Common.Exceptions;
using bbxBE.Common.ExpiringData;
using bbxBE.Domain.Entities;
using bbxBE.Infrastructure.Persistence.Repository;
using LinqKit;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;

namespace bbxBE.Infrastructure.Persistence.Repositories
{
    public class CustDiscountRepositoryAsync : GenericRepositoryAsync<CustDiscount>, ICustDiscountRepositoryAsync
    {
        private readonly IApplicationDbContext _dbContext;
        private IDataShapeHelper<GetCustDiscountViewModel> _dataShaperGetCustDiscountViewModel;
        private readonly IMockService _mockData;
        private readonly IModelHelper _modelHelper;
        private readonly IMapper _mapper;
        private readonly ICacheService<Customer> _customerCacheService;
        private readonly ICacheService<ProductGroup> _productGroupCacheService;
        private readonly IExpiringData<ExpiringDataObject> _expiringData;

        public CustDiscountRepositoryAsync(IApplicationDbContext dbContext,
            IDataShapeHelper<CustDiscount> dataShaperCustDiscount,
            IDataShapeHelper<GetCustDiscountViewModel> dataShaperGetCustDiscountViewModel,
            IModelHelper modelHelper, IMapper mapper, IMockService mockData,
            ICacheService<Customer> customerCacheService,
            ICacheService<ProductGroup> productGroupCacheService, IExpiringData<ExpiringDataObject> expiringData) : base(dbContext)
        {
            _dbContext = dbContext;
            _dataShaperGetCustDiscountViewModel = dataShaperGetCustDiscountViewModel;
            _modelHelper = modelHelper;
            _mapper = mapper;
            _mockData = mockData;

            _customerCacheService = customerCacheService;
            _productGroupCacheService = productGroupCacheService;
            _expiringData = expiringData;
        }

        public async Task<long> MaintanenceCustDiscountRangeAsync(List<CustDiscount> p_CustDiscountList,
                    long customerID,
                    IExpiringData<ExpiringDataObject> expiringData)
        {
            using (var dbContextTransaction = await _dbContext.Instance.Database.BeginTransactionAsync())
            {

                Customer cust = null;
                if (!_customerCacheService.TryGetValue(customerID, out cust))
                    throw new ResourceNotFoundException(string.Format(bbxBEConsts.ERR_CUSTNOTFOUND, customerID));

                var errPg = new List<string>();
                p_CustDiscountList.ForEach(
                    i =>
                    {
                        ProductGroup pg = null;

                        if (!_productGroupCacheService.TryGetValue(i.ProductGroupID, out pg))
                            errPg.Add(i.ProductGroupID.ToString());
                    }
                    );
                if (errPg.Count > 0)
                {
                    throw new ResourceNotFoundException(string.Format(bbxBEConsts.ERR_PRODUCTGROUPNOTFOUND, String.Join(',', errPg)));
                }

                await RemoveRangeAsync(_dbContext.CustDiscount.Where(x => x.CustomerID == customerID), false);
                await AddRangeAsync(p_CustDiscountList);

                //szemafr ki�t�sek
                var key = bbxBEConsts.DEF_CUSTOMERLOCK_KEY + cust.ID.ToString();
                await expiringData.DeleteItemAsync(key);


                await dbContextTransaction.CommitAsync();
            }

            return p_CustDiscountList.Count();
        }

        public async Task<Entity> GetCustDiscountAsync(long ID)
        {

            var CustDiscount = await _dbContext.CustDiscount.AsNoTracking().AsExpandable()
                     .Include(i => i.Customer).AsNoTracking()
                     .Include(i => i.ProductGroup).AsNoTracking()
                     .SingleOrDefaultAsync(s => s.ID == ID);

            var itemModel = _mapper.Map<CustDiscount, GetCustDiscountViewModel>(CustDiscount);

            var listFieldsModel = _modelHelper.GetModelFields<GetCustDiscountViewModel>();

            // shape data
            var shapeData = _dataShaperGetCustDiscountViewModel.ShapeData(itemModel, String.Join(",", listFieldsModel));

            return shapeData;
        }

        public async Task<List<Entity>> GetCustDiscountForCustomerListAsync(long customerID)
        {

            var query = _dbContext.CustDiscount.AsNoTracking()
                       .Include(i => i.Customer).AsNoTracking()
                       .Include(i => i.ProductGroup).AsNoTracking()
                       .Where(s => s.CustomerID == customerID)
                       .OrderBy(o => o.ProductGroup.ProductGroupCode);

            var listFieldsModel = _modelHelper.GetModelFields<GetCustDiscountViewModel>();
            List<Entity> shapeData = new List<Entity>();

            await query.ForEachAsync(i =>
               {
                   var itemModel = _mapper.Map<CustDiscount, GetCustDiscountViewModel>(i);
                   shapeData.Add(_dataShaperGetCustDiscountViewModel.ShapeData(itemModel, String.Join(",", listFieldsModel)));

               });

            return shapeData;
        }

        public Task<bool> SeedDataAsync(int rowCount)
        {
            throw new System.NotImplementedException();
        }

    }
}