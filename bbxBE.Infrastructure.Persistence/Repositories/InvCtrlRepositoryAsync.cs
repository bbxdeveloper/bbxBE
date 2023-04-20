using AutoMapper;
using bbxBE.Application.Interfaces;
using bbxBE.Application.Interfaces.Repositories;
using bbxBE.Application.Parameters;
using bbxBE.Application.Queries.qInvCtrl;
using bbxBE.Application.Queries.qStock;
using bbxBE.Application.Queries.ViewModels;
using bbxBE.Common.Consts;
using bbxBE.Common.Enums;
using bbxBE.Common.Exceptions;
using bbxBE.Domain.Entities;
using bbxBE.Infrastructure.Persistence.Repository;
using LinqKit;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using static bbxBE.Common.NAV.NAV_enums;

namespace bbxBE.Infrastructure.Persistence.Repositories
{

    /*
     {
  "items": [
    {
      "warehouseID": 1,
      "invCtlPeriodID": 10,
      "productID": 118562,
      "invCtrlDate": "2022-08-21T00:00:00",
      "nRealQty": 12,
      "userID": 0
    },
    {
      "warehouseID": 1,
      "invCtlPeriodID": 10,
      "productID": 118563,
      "invCtrlDate": "2022-08-21T00:00:00",
      "nRealQty": 3,
      "userID": 0
    },
    {
      "warehouseID": 1,
      "invCtlPeriodID": 10,
      "productID": 37032,
      "invCtrlDate": "2022-08-21T00:00:00",
      "nRealQty": 32,
      "userID": 0
    }
  ]
}
*/


    public class InvCtrlRepositoryAsync : GenericRepositoryAsync<InvCtrl>, IInvCtrlRepositoryAsync
    {
        private readonly IApplicationDbContext _dbContext;
        private IDataShapeHelper<InvCtrl> _dataShaperInvCtrl;
        private IDataShapeHelper<GetInvCtrlViewModel> _dataShaperGetInvCtrlViewModel;
        private IDataShapeHelper<GetStockViewModel> _dataShaperGetStockViewModel;
        private readonly IMockService _mockData;
        private readonly IModelHelper _modelHelper;
        private readonly IMapper _mapper;
        private readonly IProductRepositoryAsync _productRepository;
        private readonly IStockRepositoryAsync _stockRepository;
        private readonly ICustomerRepositoryAsync _customerRepository;
        private readonly ICacheService<Product> _productcacheService;

        public InvCtrlRepositoryAsync(IApplicationDbContext dbContext,
            IDataShapeHelper<InvCtrl> dataShaperInvCtrl,
            IDataShapeHelper<GetInvCtrlViewModel> dataShaperGetInvCtrlViewModel,
            IDataShapeHelper<GetStockViewModel> dataShaperGetStockViewModel,
            IModelHelper modelHelper, IMapper mapper, IMockService mockData,
            IProductRepositoryAsync productRepository,
            IStockRepositoryAsync stockRepository,
            IWarehouseRepositoryAsync warehouseRepository,
            ICacheService<Product> productCacheService,
            ICustomerRepositoryAsync customerRepository) : base(dbContext)
        {
            _dbContext = dbContext;
            _dataShaperInvCtrl = dataShaperInvCtrl;
            _dataShaperGetInvCtrlViewModel = dataShaperGetInvCtrlViewModel;
            _dataShaperGetStockViewModel = dataShaperGetStockViewModel;
            _modelHelper = modelHelper;
            _mapper = mapper;
            _mockData = mockData;
            _productRepository = productRepository;
            _stockRepository = stockRepository;
            _customerRepository = customerRepository;
            _productcacheService = productCacheService;
        }

        public async Task<InvCtrl> AddInvCtrlAsync(InvCtrl p_InvCtrl)
        {
            await AddAsync(p_InvCtrl);
            await _dbContext.SaveChangesAsync();
            return p_InvCtrl;
        }

        public async Task<InvCtrl> UpdateInvCtrlAsync(InvCtrl p_InvCtrl)
        {
            await UpdateAsync(p_InvCtrl);
            return p_InvCtrl;
        }

        public async Task<bool> AddOrUpdateRangeInvCtrlAsync(List<InvCtrl> p_InvCtrl)
        {
            using (var dbContextTransaction = await _dbContext.Instance.Database.BeginTransactionAsync())
            {
                try
                {


                    var AddInvCtrlItems = new List<InvCtrl>();
                    var UpdInvCtrlItems = new List<InvCtrl>();

                    foreach (var InvCtrl in p_InvCtrl)
                    {
                        var existing = await GetInvCtrlICPRecordAsync(InvCtrl.InvCtlPeriodID.Value, InvCtrl.ProductID);

                        //*************************************
                        //* leltári tétel rekord kiegészítése *
                        //************************************

                        //nyilvántartási egységár meghatározása
                        // 1. Raktárkészlet alapján?
                        var stock = await _dbContext.Stock
                        .Where(x => x.WarehouseID == InvCtrl.WarehouseID && x.ProductID == InvCtrl.ProductID && !x.Deleted)
                        .FirstOrDefaultAsync();
                        if (stock != null)
                        {
                            InvCtrl.AvgCost = stock.AvgCost;
                        }

                        // 2. raktárkészlet alapján nem sikerült, cikktörzs alapján
                        //
                        if (InvCtrl.AvgCost == 0)
                        {
                            var prod = _productRepository.GetProduct(InvCtrl.ProductID);
                            if (prod != null)
                            {
                                InvCtrl.AvgCost = (prod.LatestSupplyPrice != 0 ? prod.LatestSupplyPrice : prod.UnitPrice2);
                            }

                        }

                        if (existing != null)
                        {
                            InvCtrl.ID = existing.ID;
                            _dbContext.Instance.Entry(InvCtrl).State = EntityState.Modified;
                            UpdInvCtrlItems.Add(InvCtrl);

                        }
                        else
                        {
                            AddInvCtrlItems.Add(InvCtrl);
                        }
                    }

                    if (AddInvCtrlItems.Count > 0)
                    {
                        await AddRangeAsync(AddInvCtrlItems);
                    }
                    if (UpdInvCtrlItems.Count > 0)
                    {
                        await UpdateRangeAsync(UpdInvCtrlItems);
                    }

                    await _dbContext.SaveChangesAsync();
                    await dbContextTransaction.CommitAsync();

                }
                catch (Exception)
                {
                    await dbContextTransaction.RollbackAsync();
                    throw;
                }
            }
            return true;
        }

        public async Task<bool> AddRangeInvCtrlICCAsync(List<InvCtrl> p_InvCtrl, string p_XRel)
        {
            using (var dbContextTransaction = await _dbContext.Instance.Database.BeginTransactionAsync())
            {
                try
                {
                    var ownData = _customerRepository.GetOwnData();
                    if (ownData == null)
                    {
                        throw new ResourceNotFoundException(string.Format(bbxBEConsts.ERR_OWNNOTFOUND));
                    }

                    foreach (var InvCtrl in p_InvCtrl)
                    {
                        //*************************************
                        //* leltári tétel rekord kiegészítése *
                        //************************************

                        //nyilvántartási egységár meghatározása
                        // 1. Raktárkészlet alapján?
                        var stock = await _dbContext.Stock
                        .Where(x => x.WarehouseID == InvCtrl.WarehouseID && x.ProductID == InvCtrl.ProductID && !x.Deleted)
                        .FirstOrDefaultAsync();
                        if (stock != null)
                        {
                            InvCtrl.AvgCost = stock.AvgCost;
                        }

                        // 2. raktárkészlet alapján nem sikerült, cikktörzs alapján
                        //
                        if (InvCtrl.AvgCost == 0)
                        {
                            var prod = _productRepository.GetProduct(InvCtrl.ProductID);
                            if (prod != null)
                            {
                                InvCtrl.AvgCost = (prod.LatestSupplyPrice != 0 ? prod.LatestSupplyPrice : prod.UnitPrice2);
                            }
                        }
                    }

                    await AddRangeAsync(p_InvCtrl);
                    await _dbContext.SaveChangesAsync();            //ID-k legyenek

                    var stockList = await _stockRepository.MaintainStockByInvCtrlAsync(p_InvCtrl, ownData, p_XRel);
                    await UpdateRangeAsync(p_InvCtrl);

                    await _dbContext.SaveChangesAsync();

                    await dbContextTransaction.CommitAsync();

                }
                catch (Exception)
                {
                    await dbContextTransaction.RollbackAsync();
                    throw;
                }
            }
            return true;
        }

        public async Task<InvCtrl> DeleteInvCtrlAsync(long ID)
        {

            InvCtrl icp = null;

            icp = _dbContext.InvCtrl.AsNoTracking().Where(x => x.ID == ID).FirstOrDefault();

            if (icp != null)
            {


                await RemoveAsync(icp);
                await _dbContext.SaveChangesAsync();

            }
            else
            {
                throw new ResourceNotFoundException(string.Format(bbxBEConsts.ERR_INVCTRLNOTFOUND, ID));
            }

            return icp;
        }

        public async Task<bool> CheckInvCtrlDateAsync(long InvCtlPeriodID, DateTime InvCtrlDate)
        {
            var res = await _dbContext.InvCtrlPeriod.AnyAsync(a => a.ID == InvCtlPeriodID && !a.Closed && a.DateFrom <= InvCtrlDate && a.DateTo >= InvCtrlDate);
            return await Task.FromResult(res);
        }

        public async Task<Entity> GetInvCtrl(GetInvCtrl requestParameter)
        {
            var ID = requestParameter.ID;
            var item = await _dbContext.InvCtrl.AsNoTracking()
                .Include(w => w.Warehouse).AsNoTracking().AsExpandable()
                .Where(w => w.ID == ID && !w.Deleted).SingleOrDefaultAsync();


            if (item == null)
            {
                throw new ResourceNotFoundException(string.Format(bbxBEConsts.ERR_INVCTRLNOTFOUND, ID));
            }
            item.Product = _productcacheService.QueryCache().Where(w => w.ID == item.ProductID).SingleOrDefault();

            var itemModel = _mapper.Map<InvCtrl, GetInvCtrlViewModel>(item);
            var listFieldsModel = _modelHelper.GetModelFields<GetInvCtrlViewModel>();

            // shape data
            var shapeData = _dataShaperGetInvCtrlViewModel.ShapeData(itemModel, String.Join(",", listFieldsModel));

            return shapeData;
        }
        public async Task<Entity> GetLastestInvCtrlICC(GetLastestInvCtrlICC requestParameter)
        {
            var item = await _dbContext.InvCtrl.AsNoTracking()
                .Include(w => w.Warehouse).AsNoTracking().AsExpandable()
                .Where(w => w.InvCtrlType == enInvCtrlType.ICC.ToString() &&
                    w.Warehouse.WarehouseCode == requestParameter.WarehouseCode && w.ProductID == requestParameter.ProductID &&
                    w.InvCtrlDate >= DateTime.UtcNow.AddDays(requestParameter.RetroDays * -1).Date && !w.Deleted)
                .FirstOrDefaultAsync();

            if (item == null)
            {
                return null;
            }

            item.Product = _productcacheService.QueryCache().Where(w => w.ID == item.ProductID).SingleOrDefault();
            var itemModel = _mapper.Map<InvCtrl, GetInvCtrlViewModel>(item);

            // shape data
            var listFieldsModel = _modelHelper.GetModelFields<GetInvCtrlViewModel>();
            var shapeData = _dataShaperGetInvCtrlViewModel.ShapeData(itemModel, String.Join(",", listFieldsModel));

            return shapeData;
        }

        public Task<InvCtrl> GetInvCtrlICPRecordAsync(long InvCtlPeriodID, long ProductID)
        {
            var item = _dbContext.InvCtrl.AsNoTracking()
                .Where(x => x.InvCtrlType == enInvCtrlType.ICP.ToString() &&
                        x.InvCtlPeriodID == InvCtlPeriodID &&
                        x.ProductID == ProductID &&
                       !x.Deleted).SingleOrDefaultAsync();
            return item;
        }
        public Task<List<InvCtrl>> GetInvCtrlICPRecordsByPeriodAsync(long InvCtlPeriodID)
        {
            var items = _dbContext.InvCtrl.AsNoTracking()
                .Where(x => x.InvCtrlType == enInvCtrlType.ICP.ToString() &&
                        x.InvCtlPeriodID == InvCtlPeriodID &&
                        !x.Deleted).ToListAsync();
            return items;
        }

        public async Task<(IEnumerable<Entity> data, RecordsCount recordsCount)> QueryInvCtrlStockAbsentAsync(QueryInvCtrlStockAbsent requestParameter)
        {


            var pageNumber = requestParameter.PageNumber;
            var pageSize = requestParameter.PageSize;
            var orderBy = requestParameter.OrderBy;
            //      var fields = requestParameter.Fields;
            var fields = _modelHelper.GetQueryableFields<GetStockViewModel, Stock>();


            int recordsTotal, recordsFiltered;

            var invCtrlPeriod = await _dbContext.InvCtrlPeriod.AsNoTracking()
                                        .Include(w => w.Warehouse).AsNoTracking()
                                        .Where(i => i.ID == requestParameter.InvCtrlPeriodID).SingleOrDefaultAsync();
            if (invCtrlPeriod == null)
            {
                throw new ResourceNotFoundException(string.Format(bbxBEConsts.ERR_INVCTRLPERIODNOTFOUND, requestParameter.InvCtrlPeriodID));
            }
            var invCtrlItems = await GetInvCtrlICPRecordsByPeriodAsync(requestParameter.InvCtrlPeriodID);
            var prodItems = _productcacheService.ListCache();
            var stockItems = await _dbContext.Stock.AsNoTracking()
                             .Include(w => w.Warehouse).AsNoTracking()
                             .Include(l => l.Location).AsNoTracking()
                             .Where(w => w.WarehouseID == invCtrlPeriod.WarehouseID && !w.Deleted).ToListAsync();

            stockItems.ForEach(i =>
                i.Product = prodItems.FirstOrDefault(f => f.ID == i.ProductID)
                );


            var absenedItems = stockItems.Where(s =>
                        !invCtrlItems.Any(i => i.ProductID == s.ProductID) &&
                        (!requestParameter.IsInStock || s.RealQty != 0)).ToList();

            if (!requestParameter.IsInStock)
            {
                //Hozzácsapjuk a nonStockedProducts-ből azokat a termékeket, amelyeknek nincs készletrekordja
                //és nincs leltárban
                var nonStockedProducts = prodItems.Where(p => !stockItems.Any(s => s.ProductID == p.ID) &&
                                                              !absenedItems.Any(s => s.ProductID == p.ID)).ToList();
                nonStockedProducts.ForEach(p =>
                {
                    absenedItems.Add(new Stock()
                    {
                        WarehouseID = invCtrlPeriod.WarehouseID,
                        ProductID = p.ID,
                        RealQty = 0,
                        AvgCost = 0,
                        LatestIn = null,
                        LatestOut = null,
                        Warehouse = invCtrlPeriod.Warehouse,
                        Product = p
                    });

                });
            }

            // Count records total
            recordsTotal = absenedItems.Count();

            // filter data
            if (!string.IsNullOrEmpty(requestParameter.SearchString))
            {
                absenedItems = absenedItems.Where(p => p.Product.Description.ToUpper().Contains(requestParameter.SearchString) ||
                        p.Product.ProductCodes.Any(a => a.ProductCodeCategory == enCustproductCodeCategory.OWN.ToString() &&
                        a.ProductCodeValue.ToUpper().Contains(requestParameter.SearchString))).ToList();
            }

            // Count records after filter
            recordsFiltered = absenedItems.Count();

            //set Record counts
            var recordsCount = new RecordsCount
            {
                RecordsFiltered = recordsFiltered,
                RecordsTotal = recordsTotal
            };

            // set order by
            var absenedItemsOrdered = absenedItems.AsQueryable();
            if (!string.IsNullOrWhiteSpace(orderBy))
            {
                //Kis heka...
                if (orderBy.ToUpper() == bbxBEConsts.FIELD_PRODUCTCODE)
                {
                    absenedItemsOrdered = absenedItemsOrdered.OrderBy(o => o.Product.ProductCodes.Single(s =>
                                s.ProductCodeCategory == enCustproductCodeCategory.OWN.ToString()).ProductCodeValue);
                }
                else if (orderBy.ToUpper() == bbxBEConsts.FIELD_PRODUCT)
                {
                    absenedItemsOrdered = absenedItemsOrdered.OrderBy(o => o.Product.Description);
                }
                else
                {
                    absenedItemsOrdered = absenedItemsOrdered.OrderBy(p => p.GetType()
                               .GetProperty(orderBy)
                               .GetValue(p, null));
                }
            }


            var absenedItemsPaged = absenedItemsOrdered.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();

            // retrieve data to list

            //TODO: szebben megoldani
            var resultDataModel = new List<GetStockViewModel>();
            absenedItemsPaged.ForEach(i => resultDataModel.Add(
               _mapper.Map<Stock, GetStockViewModel>(i))
            );
            var listFieldsModel = _modelHelper.GetModelFields<GetStockViewModel>();

            var shapeData = _dataShaperGetStockViewModel.ShapeData(resultDataModel, String.Join(",", listFieldsModel));

            return (shapeData, recordsCount);
        }


        public async Task<(IEnumerable<Entity> data, RecordsCount recordsCount)> QueryPagedInvCtrlAsync(QueryInvCtrl requestParameter)
        {

            var InvCtrlPeriodID = requestParameter.InvCtrlPeriodID;
            var searchString = requestParameter.SearchString;

            var orderBy = requestParameter.OrderBy;
            //      var fields = requestParameter.Fields;
            var fields = _modelHelper.GetQueryableFields<GetInvCtrlViewModel, InvCtrl>();


            int recordsTotal, recordsFiltered;
            var prodCachedList = _productcacheService.ListCache();

            /*******************************************************************************************/
            //Gyorsításként cache.bél töltjük fel a terméktörzset. Ezt nem lehet összehozni hatékonyan
            //az EF linq-val, ezért először lekérdezzük a teljes listát, feltöltjük a termékekek és 
            /*******************************************************************************************/
            var resultList = await _dbContext.InvCtrl.AsNoTracking()
                .Include(w => w.Warehouse).AsNoTracking()
                .Include(i => i.InvCtrlPeriod).AsNoTracking()
                .Include(s => s.Stock).AsNoTracking()
                .Where(w => !w.Deleted && w.InvCtlPeriodID == InvCtrlPeriodID).ToListAsync();

            resultList.ForEach(i =>
                i.Product = prodCachedList.FirstOrDefault(f => f.ID == i.ProductID)
                );


            var query = resultList.AsQueryable();
            // Count records total
            //    recordsTotal = await result.CountAsync();
            recordsTotal = query.Count();

            // filter data
            FilterBy(ref query, InvCtrlPeriodID, searchString);

            // Count records after filter
            recordsFiltered = query.Count();

            //set Record counts
            var recordsCount = new RecordsCount
            {
                RecordsFiltered = recordsFiltered,
                RecordsTotal = recordsTotal
            };

            // set order by
            if (!string.IsNullOrWhiteSpace(orderBy))
            {
                //Kis heka...
                if (orderBy.ToUpper() == bbxBEConsts.FIELD_PRODUCTCODE)
                {
                    query = query.OrderBy(o => o.Product.ProductCodes.Single(s =>
                                s.ProductCodeCategory == enCustproductCodeCategory.OWN.ToString()).ProductCodeValue);
                }
                else if (orderBy.ToUpper() == bbxBEConsts.FIELD_PRODUCT)
                {
                    query = query.OrderBy(o => o.Product.Description);
                }
                else
                {
                    query = query.OrderBy(orderBy);
                }
            }


            // select columns
            if (!string.IsNullOrWhiteSpace(fields))
            {
                query = query.Select<InvCtrl>("new(" + fields + ")");
            }

            // retrieve data to list
            var resultData = await GetPagedData(query, requestParameter, false);


            //TODO: szebben megoldani
            var resultDataModel = new List<GetInvCtrlViewModel>();
            resultData.ForEach(i => resultDataModel.Add(
               _mapper.Map<InvCtrl, GetInvCtrlViewModel>(i))
            );


            var listFieldsModel = _modelHelper.GetModelFields<GetInvCtrlViewModel>();

            var shapeData = _dataShaperGetInvCtrlViewModel.ShapeData(resultDataModel, String.Join(",", listFieldsModel));

            return (shapeData, recordsCount);
        }
        private void FilterBy(ref IQueryable<InvCtrl> p_item, long? InvCtrlPeriodID, string p_searchString)
        {
            if (!p_item.Any())
                return;

            if (!InvCtrlPeriodID.HasValue && string.IsNullOrWhiteSpace(p_searchString))
                return;

            var predicate = PredicateBuilder.New<InvCtrl>();

            if (InvCtrlPeriodID.HasValue)
            {
                predicate = predicate.And(p => p.InvCtlPeriodID == InvCtrlPeriodID.Value);
            }
            if (!string.IsNullOrWhiteSpace(p_searchString))
            {
                p_searchString = p_searchString.ToUpper();
                predicate = predicate.And(p => p.Product.Description.ToUpper().Contains(p_searchString) ||
                        p.Product.ProductCodes.Any(a => a.ProductCodeCategory == enCustproductCodeCategory.OWN.ToString() &&
                        a.ProductCodeValue.ToUpper().Contains(p_searchString)));
            }

            p_item = p_item.Where(predicate);
        }

        public Task<bool> SeedDataAsync(int rowCount)
        {
            throw new System.NotImplementedException();
        }

    }
}