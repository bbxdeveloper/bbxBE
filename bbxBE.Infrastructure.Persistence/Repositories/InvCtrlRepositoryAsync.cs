﻿using LinqKit;
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
using bbxBE.Application.Queries.qInvCtrl;
using bbxBE.Application.Queries.ViewModels;
using bbxBE.Common.Exceptions;
using bbxBE.Common.Consts;
using bbxBE.Common.Enums;
using static bbxBE.Common.NAV.NAV_enums;
using static bxBE.Application.Commands.cmdInvCtrl.createInvCtrlICPCommand;
using bbxBE.Infrastructure.Persistence.Caches;
using Microsoft.AspNetCore.Mvc;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

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
        private readonly ApplicationDbContext _dbContext;
        private IDataShapeHelper<InvCtrl> _dataShaperInvCtrl;
        private IDataShapeHelper<GetInvCtrlViewModel> _dataShaperGetInvCtrlViewModel;
        private readonly IMockService _mockData;
        private readonly IModelHelper _modelHelper;
        private readonly IMapper _mapper;
        private readonly IProductRepositoryAsync _productRepository;
        private readonly ICacheService<Product> _productcacheService;

        public InvCtrlRepositoryAsync(ApplicationDbContext dbContext,
            IDataShapeHelper<InvCtrl> dataShaperInvCtrl,
            IDataShapeHelper<GetInvCtrlViewModel> dataShaperGetInvCtrlViewModel,
            IModelHelper modelHelper, IMapper mapper, IMockService mockData, 
            IProductRepositoryAsync productRepository,
            ICacheService<Product> productCacheService) : base(dbContext)
        {
            _dbContext = dbContext;
            _dataShaperInvCtrl = dataShaperInvCtrl;
            _dataShaperGetInvCtrlViewModel = dataShaperGetInvCtrlViewModel;
            _modelHelper = modelHelper;
            _mapper = mapper;
            _mockData = mockData;
            _productRepository = productRepository;
            _productcacheService = productCacheService;
        }
        public async Task<InvCtrl> AddInvCtrlAsync(InvCtrl p_InvCtrl)
        {
            await _dbContext.InvCtrl.AddAsync(p_InvCtrl);
            await _dbContext.SaveChangesAsync();
            return p_InvCtrl;
        }

        public async Task<InvCtrl> UpdateInvCtrlAsync(InvCtrl p_InvCtrl)
        {
            _dbContext.InvCtrl.Update(p_InvCtrl);
            await _dbContext.SaveChangesAsync();
            return p_InvCtrl;
        }

        public async Task<bool> AddOrUpdateRangeInvCtrlAsync(List<InvCtrl> p_InvCtrl)
        {
            using (var dbContextTransaction = await _dbContext.Database.BeginTransactionAsync())
            {
                try
                {


                    var AddInvCtrlItems = new List<InvCtrl>();
                    var UpdInvCtrlItems = new List<InvCtrl>();

                    foreach( var invCtrlItem in p_InvCtrl)
                    {
                        var InvCtrl = _mapper.Map<InvCtrl>(invCtrlItem);
                        var existing = await GetInvCtrlICPRecordAsync(InvCtrl.InvCtlPeriodID.Value, InvCtrl.ProductID);

                        //*************************************
                        //* leltári tétel rekord kiegészítése *
                        //************************************

                        invCtrlItem.NCalcQty = invCtrlItem.NRealQty;

                        //nyilvántartási egységár meghatározása
                        // 1. Raktárkészlet alapján?
                        var stock = await _dbContext.Stock
                        .Where(x => x.WarehouseID == invCtrlItem.WarehouseID && x.ProductID == invCtrlItem.ProductID && !x.Deleted)
                        .FirstOrDefaultAsync();
                        if (stock != null) 
                        {
                            invCtrlItem.AvgCost = stock.AvgCost;
                        }

                        // 2. raktárkészlet alapján nem sikerült, cikktörzs alapján
                        //
                        if (invCtrlItem.AvgCost == 0)
                        {
                            var prod = _productRepository.GetProduct(invCtrlItem.ProductID);
                            if( prod != null)
                            {
                                invCtrlItem.AvgCost = (prod.LatestSupplyPrice != 0 ? prod.LatestSupplyPrice : prod.UnitPrice2);
                            }

                        }

                        if (existing != null)
                        {
                            InvCtrl.ID = existing.ID;
                            _dbContext.Entry(InvCtrl).State = EntityState.Modified;
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
                catch (Exception ex)
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


                _dbContext.InvCtrl.Remove(icp);
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
            var item =  await _dbContext.InvCtrl.AsNoTracking()
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

        public async Task<(IEnumerable<Entity> data, RecordsCount recordsCount)> QueryPagedInvCtrlAsync(QueryInvCtrl requestParameter)
        {

            var InvCtrlPeriodID = requestParameter.InvCtrlPeriodID;
            var searchString = requestParameter.SearchString;

            var pageNumber = requestParameter.PageNumber;
            var pageSize = requestParameter.PageSize;
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
            recordsFiltered =  query.Count();

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
            // paging
            query = query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize);

            // retrieve data to list
            var resultData = query.ToList();

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