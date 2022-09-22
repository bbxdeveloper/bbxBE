using LinqKit;
using Microsoft.EntityFrameworkCore;
using bbxBE.Application.Interfaces;
using bbxBE.Application.Interfaces.Repositories;
using bbxBE.Application.Parameters;
using bbxBE.Domain.Entities;
using bbxBE.Infrastructure.Persistence.Contexts;
using bbxBE.Infrastructure.Persistence.Repository;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using bbxBE.Application.Interfaces.Queries;
using bbxBE.Application.BLL;
using System;
using AutoMapper;
using bbxBE.Application.Queries.qCustDiscount;
using bbxBE.Application.Queries.ViewModels;
using bbxBE.Common.Exceptions;
using bbxBE.Common.Consts;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace bbxBE.Infrastructure.Persistence.Repositories
{
    public class CustDiscountRepositoryAsync : GenericRepositoryAsync<CustDiscount>, ICustDiscountRepositoryAsync
    {
        private readonly ApplicationDbContext _dbContext;
        private IDataShapeHelper<GetCustDiscountViewModel> _dataShaperGetCustDiscountViewModel;
        private readonly IMockService _mockData;
        private readonly IModelHelper _modelHelper;
        private readonly IMapper _mapper;
        private readonly ICacheService<Customer> _customerCacheService;
        private readonly ICacheService<ProductGroup> _productGroupCacheService;

        public CustDiscountRepositoryAsync(ApplicationDbContext dbContext,
            IDataShapeHelper<CustDiscount> dataShaperCustDiscount,
            IDataShapeHelper<GetCustDiscountViewModel> dataShaperGetCustDiscountViewModel,
            IModelHelper modelHelper, IMapper mapper, IMockService mockData,
            ICacheService<Customer> customerCacheService,
            ICacheService<ProductGroup> productGroupCacheService) : base(dbContext)
        {
            _dbContext = dbContext;
            _dataShaperGetCustDiscountViewModel = dataShaperGetCustDiscountViewModel;
            _modelHelper = modelHelper;
            _mapper = mapper;
            _mockData = mockData;

            _customerCacheService = customerCacheService;
            _productGroupCacheService = productGroupCacheService;
           
        }

        public async Task<long> MaintanenceCustDiscountRangeAsync(List<CustDiscount> p_CustDiscountList)
        {
            using (var dbContextTransaction = await _dbContext.Database.BeginTransactionAsync())
            {

                var customerID = p_CustDiscountList.First().CustomerID;
                Customer cust = null;
                if (!_customerCacheService.TryGetValue(customerID, out cust))
                    throw new ResourceNotFoundException(string.Format(bbxBEConsts.FV_CUSTNOTFOUND, customerID));

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
                    throw new ResourceNotFoundException(string.Format(bbxBEConsts.FV_PRODUCTGROUPNOTFOUND, String.Join(',', errPg)));
                }

                _dbContext.CustDiscount.RemoveRange(_dbContext.CustDiscount.Where(x => x.CustomerID == customerID));

                await _dbContext.CustDiscount.AddRangeAsync(p_CustDiscountList);
                await _dbContext.SaveChangesAsync();

                await dbContextTransaction.CommitAsync();
            }

            return p_CustDiscountList.Count();
        }

        public async Task<Entity>  GetCustDiscountAsync(long ID)
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
        
        public async Task<List<Entity>> GetCustDiscountForCustomerListAsync( long customerID)
        {

            var query = _dbContext.CustDiscount.AsNoTracking().AsExpandable()
                       .Include(i => i.Customer).AsNoTracking()
                       .Include(i => i.ProductGroup).AsNoTracking()
                       .Where(s => s.CustomerID == customerID);

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