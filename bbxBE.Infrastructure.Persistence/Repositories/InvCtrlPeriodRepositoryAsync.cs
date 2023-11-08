using AutoMapper;
using bbxBE.Application.Helpers;
using bbxBE.Application.Interfaces;
using bbxBE.Application.Interfaces.Repositories;
using bbxBE.Application.Parameters;
using bbxBE.Application.Queries.qInvCtrlPeriod;
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
    public class InvCtrlPeriodRepositoryAsync : GenericRepositoryAsync<InvCtrlPeriod>, IInvCtrlPeriodRepositoryAsync
    {
        private readonly IApplicationDbContext _dbContext;
        private IDataShapeHelper<InvCtrlPeriod> _dataShaperInvCtrlPeriod;
        private IDataShapeHelper<GetInvCtrlPeriodViewModel> _dataShaperGetInvCtrlPeriodViewModel;
        private IDataShapeHelper<GetStockViewModel> _dataShaperGetStockViewModel;
        private readonly IMockService _mockData;
        private readonly IModelHelper _modelHelper;
        private readonly IMapper _mapper;
        private readonly IStockRepositoryAsync _stockRepository;
        private readonly IInvCtrlRepositoryAsync _invCtrlRepository;
        private readonly ICustomerRepositoryAsync _customerRepository;

        public InvCtrlPeriodRepositoryAsync(IApplicationDbContext dbContext,
            IModelHelper modelHelper, IMapper mapper, IMockService mockData,
            IExpiringData<ExpiringDataObject> expiringData,
            ICacheService<Product> productCacheService,
            ICacheService<Customer> customerCacheService,
            ICacheService<ProductGroup> productGroupCacheService,
            ICacheService<Origin> originCacheService,
            ICacheService<VatRate> vatRateCacheService,
            ICustomerRepositoryAsync customerRepository) : base(dbContext)
        {
            _dbContext = dbContext;
            _dataShaperInvCtrlPeriod = new DataShapeHelper<InvCtrlPeriod>();
            _dataShaperGetInvCtrlPeriodViewModel = new DataShapeHelper<GetInvCtrlPeriodViewModel>();
            _dataShaperGetStockViewModel = new DataShapeHelper<GetStockViewModel>();
            _modelHelper = modelHelper;
            _mapper = mapper;
            _mockData = mockData;
            _stockRepository = new StockRepositoryAsync(dbContext, modelHelper, mapper, mockData, productCacheService, productGroupCacheService, originCacheService, vatRateCacheService);
            _invCtrlRepository = new InvCtrlRepositoryAsync(dbContext, modelHelper, mapper, mockData, expiringData, productCacheService, customerCacheService, productGroupCacheService, originCacheService, vatRateCacheService);
            _customerRepository = new CustomerRepositoryAsync(dbContext, modelHelper, mapper, mockData, expiringData, customerCacheService);
        }
        public async Task<InvCtrlPeriod> AddInvCtrlPeriodAsync(InvCtrlPeriod p_invCtrlPeriod)
        {
            await AddAsync(p_invCtrlPeriod);
            return p_invCtrlPeriod;
        }

        public async Task<InvCtrlPeriod> UpdateInvCtrlPeriodAsync(InvCtrlPeriod p_invCtrlPeriod)
        {
            await UpdateAsync(p_invCtrlPeriod);
            return p_invCtrlPeriod;
        }
        public async Task<InvCtrlPeriod> DeleteInvCtrlPeriodAsync(long ID)
        {

            InvCtrlPeriod icp = null;

            icp = await _dbContext.InvCtrlPeriod.AsNoTracking().Where(x => x.ID == ID).SingleOrDefaultAsync();

            if (icp != null)
            {


                await RemoveAsync(icp);
                await _dbContext.SaveChangesAsync();

            }
            else
            {
                throw new ResourceNotFoundException(string.Format(bbxBEConsts.ERR_INVCTRLPERIODNOTFOUND, ID));
            }

            return icp;
        }

        public async Task<bool> CanDeleteAsync(long ID)
        {
            var itemExisting = await _dbContext.InvCtrl.AsNoTracking()
                          .Where(x => x.InvCtlPeriodID == ID).AnyAsync();
            return !itemExisting;
        }
        public async Task<bool> CanCloseAsync(long ID)
        {
            var item = await _dbContext.InvCtrlPeriod.AsNoTracking()
                          .Where(x => x.ID == ID).SingleOrDefaultAsync();
            if (item == null)
            {
                throw new ResourceNotFoundException(string.Format(bbxBEConsts.ERR_INVCTRLPERIODNOTFOUND, ID));
            }
            return !item.Closed;
        }

        public async Task<bool> CanUpdateAsync(long ID)
        {
            var item = await _dbContext.InvCtrlPeriod.AsNoTracking()
                          .Where(x => x.ID == ID).SingleOrDefaultAsync();
            if (item == null)
            {
                throw new ResourceNotFoundException(string.Format(bbxBEConsts.ERR_INVCTRLPERIODNOTFOUND, ID));
            }
            return !item.Closed;
        }
        public async Task<Entity> GetInvCtrlPeriodAsync(GetInvCtrlPeriod requestParameter)
        {
            var ID = requestParameter.ID;
            var item = await _dbContext.InvCtrlPeriod.AsNoTracking()
                .Include(w => w.Warehouse).AsNoTracking()
                .AsExpandable()
                .Where(x => x.ID == ID).SingleOrDefaultAsync();

            //        var item = await _dbContext.InvCtrlPeriod.FindAsync(ID);

            if (item == null)
            {
                throw new ResourceNotFoundException(string.Format(bbxBEConsts.ERR_INVCTRLPERIODNOTFOUND, ID));
            }

            var itemModel = _mapper.Map<InvCtrlPeriod, GetInvCtrlPeriodViewModel>(item);
            var listFieldsModel = _modelHelper.GetModelFields<GetInvCtrlPeriodViewModel>();

            // shape data
            var shapeData = _dataShaperGetInvCtrlPeriodViewModel.ShapeData(itemModel, String.Join(",", listFieldsModel));

            return shapeData;
        }
        public async Task<InvCtrlPeriod> GetInvCtrlPeriodRecordAsync(long ID)
        {
            var item = await _dbContext.InvCtrlPeriod.AsNoTracking()
                .Include(w => w.Warehouse).AsNoTracking()
                .AsExpandable()
                .Where(x => x.ID == ID).SingleOrDefaultAsync();
            return item;
        }

        public async Task<(IEnumerable<Entity> data, RecordsCount recordsCount)> QueryPagedInvCtrlPeriodAsync(QueryInvCtrlPeriod requestParameter)
        {

            var orderBy = requestParameter.OrderBy;
            //      var fields = requestParameter.Fields;
            var fields = _modelHelper.GetQueryableFields<GetInvCtrlPeriodViewModel, InvCtrlPeriod>();


            int recordsTotal, recordsFiltered;

            // Setup IQueryable
            var query = _dbContext.InvCtrlPeriod.AsNoTracking()
                .Include(w => w.Warehouse).AsNoTracking()
                .AsExpandable();

            // Count records total
            recordsTotal = await query.CountAsync();

            // filter data
            // nincs keresőfilter FilterBySearchString(ref result, searchString);

            // Count records after filter
            recordsFiltered = await query.CountAsync();

            //set Record counts
            var recordsCount = new RecordsCount
            {
                RecordsFiltered = recordsFiltered,
                RecordsTotal = recordsTotal
            };

            // set order by
            if (!string.IsNullOrWhiteSpace(orderBy))
            {
                query = query.OrderBy(orderBy);
            }

            // select columns
            if (!string.IsNullOrWhiteSpace(fields))
            {
                query = query.Select<InvCtrlPeriod>("new(" + fields + ")");
            }

            // retrieve data to list
            var resultData = await GetPagedData(query, requestParameter);

            //TODO: szebben megoldani
            var resultDataModel = new List<GetInvCtrlPeriodViewModel>();
            resultData.ForEach(i => resultDataModel.Add(
               _mapper.Map<InvCtrlPeriod, GetInvCtrlPeriodViewModel>(i))
            );


            var listFieldsModel = _modelHelper.GetModelFields<GetInvCtrlPeriodViewModel>();

            var shapeData = _dataShaperGetInvCtrlPeriodViewModel.ShapeData(resultDataModel, String.Join(",", listFieldsModel));

            return (shapeData, recordsCount);
        }

        private void FilterBySearchString(ref IQueryable<InvCtrlPeriod> p_item, string p_searchString)
        {
            if (!p_item.Any())
                return;

            if (string.IsNullOrWhiteSpace(p_searchString))
                return;

            var predicate = PredicateBuilder.New<InvCtrlPeriod>();


            p_item = p_item.Where(predicate);
        }

        public Task<bool> SeedDataAsync(int rowCount)
        {
            throw new System.NotImplementedException();
        }

        public async Task<bool> IsOverLappedPeriodAsync(DateTime DateFrom, DateTime DateTo, long? ID, long WarehouseID)
        {
            var result = await _dbContext.InvCtrlPeriod.AnyAsync(w => !w.Deleted && w.WarehouseID == WarehouseID && (ID == null || w.ID != ID.Value) && w.DateFrom < DateTo && DateFrom < w.DateTo);
            return !result;
        }

        public async Task<bool> CloseAsync(long ID, long userID)
        {
            var invCtrlPeriod = await _dbContext.InvCtrlPeriod.AsNoTracking()
                      .Include(w => w.Warehouse).AsNoTracking()
                      .Where(x => x.ID == ID).SingleOrDefaultAsync();
            if (invCtrlPeriod == null)
            {
                throw new ResourceNotFoundException(string.Format(bbxBEConsts.ERR_INVCTRLPERIODNOTFOUND, ID));
            }
            var ownData = _customerRepository.GetOwnData();
            if (ownData == null)
            {
                throw new ResourceNotFoundException(string.Format(bbxBEConsts.ERR_OWNNOTFOUND));
            }


            using (var dbContextTransaction = await _dbContext.Instance.Database.BeginTransactionAsync())
            {
                try
                {
                    var invCtrlItems = await _dbContext.InvCtrl.AsNoTracking().Where(x => x.InvCtlPeriodID == ID).ToListAsync();
                    var stockList = await _stockRepository.MaintainStockByInvCtrlAsync(invCtrlItems, ownData,
                                invCtrlPeriod.Warehouse.WarehouseCode + "-" + invCtrlPeriod.Warehouse.WarehouseDescription + " " + invCtrlPeriod.DateFrom.ToString(bbxBEConsts.DEF_DATEFORMAT) + "-" + invCtrlPeriod.DateTo.ToString(bbxBEConsts.DEF_DATEFORMAT));


                    invCtrlPeriod.Closed = true;
                    invCtrlPeriod.UserID = userID;
                    await UpdateAsync(invCtrlPeriod);
                    await _invCtrlRepository.UpdateRangeAsync(invCtrlItems);

                    await dbContextTransaction.CommitAsync();


                }
                catch (Exception)
                {
                    await dbContextTransaction.RollbackAsync();
                    throw;
                }
                return !invCtrlPeriod.Closed;
            }
        }
    }
}