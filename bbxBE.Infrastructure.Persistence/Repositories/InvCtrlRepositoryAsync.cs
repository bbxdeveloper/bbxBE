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
using bbxBE.Application.Queries.qInvCtrl;
using bbxBE.Application.Queries.ViewModels;
using bbxBE.Common.Exceptions;
using bbxBE.Common.Consts;
using bbxBE.Common.Enums;
using static bbxBE.Common.NAV.NAV_enums;
using static bxBE.Application.Commands.cmdInvCtrl.createInvCtrlICPCommand;

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

        public InvCtrlRepositoryAsync(ApplicationDbContext dbContext,
            IDataShapeHelper<InvCtrl> dataShaperInvCtrl,
            IDataShapeHelper<GetInvCtrlViewModel> dataShaperGetInvCtrlViewModel,
            IModelHelper modelHelper, IMapper mapper, IMockService mockData, 
            IProductRepositoryAsync productRepository) : base(dbContext)
        {
            _dbContext = dbContext;
            _dataShaperInvCtrl = dataShaperInvCtrl;
            _dataShaperGetInvCtrlViewModel = dataShaperGetInvCtrlViewModel;
            _modelHelper = modelHelper;
            _mapper = mapper;
            _mockData = mockData;
            _productRepository = productRepository;
        }
        public async Task<InvCtrl> AddInvCtrlAsync(InvCtrl p_InvCtrl)
        {
            await _dbContext.InvCtrl.AddAsync(p_InvCtrl);
            await _dbContext.SaveChangesAsync();
            return p_InvCtrl;
        }
        public async Task<bool> AddRangeInvCtrlAsync(List<InvCtrl> p_InvCtrl)
        {
            await _dbContext.InvCtrl.AddRangeAsync(p_InvCtrl);
            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<InvCtrl> UpdateInvCtrlAsync(InvCtrl p_InvCtrl)
        {
            _dbContext.InvCtrl.Update(p_InvCtrl);
            await _dbContext.SaveChangesAsync();
            return p_InvCtrl;
        }
        public async Task<bool> UpdateRangeInvCtrlAsync(List<InvCtrl> p_InvCtrl)
        {
            _dbContext.InvCtrl.UpdateRange(p_InvCtrl);
            await _dbContext.SaveChangesAsync();
            return true;
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
                        await _dbContext.InvCtrl.AddRangeAsync(AddInvCtrlItems);
                    }
                    if (UpdInvCtrlItems.Count > 0)
                    {
                        _dbContext.InvCtrl.UpdateRange(UpdInvCtrlItems);
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
                .Include(p => p.Product).ThenInclude(p2 => p2.ProductCodes).AsNoTracking()
                .Where(w => w.ID == ID && w.Product.ProductCodes.Any(pc => pc.ProductCodeCategory == enCustproductCodeCategory.OWN.ToString())
                        && !w.Deleted).SingleOrDefaultAsync();

            if (item == null)
            {
                throw new ResourceNotFoundException(string.Format(bbxBEConsts.ERR_INVCTRLNOTFOUND, ID));
            }

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

            // Setup IQueryable
            var result = _dbContext.InvCtrl.AsNoTracking()
                .Include(w => w.Warehouse).AsNoTracking()
                .Include(p => p.Product).ThenInclude(p2 => p2.ProductCodes).AsNoTracking()
                .Where(w => w.Product.ProductCodes.Any(pc => pc.ProductCodeCategory == enCustproductCodeCategory.OWN.ToString())
                        && !w.Deleted);


            // Count records total
            recordsTotal = await result.CountAsync();

            // filter data
            FilterBy(ref result, InvCtrlPeriodID, searchString);

            // Count records after filter
            recordsFiltered = await result.CountAsync();

            //set Record counts
            var recordsCount = new RecordsCount
            {
                RecordsFiltered = recordsFiltered,
                RecordsTotal = recordsTotal
            };

            // set order by
            if (!string.IsNullOrWhiteSpace(orderBy))
            {
                result = result.OrderBy(orderBy);
            }

            // select columns
            if (!string.IsNullOrWhiteSpace(fields))
            {
                result = result.Select<InvCtrl>("new(" + fields + ")");
            }
            // paging
            result = result
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize);

            // retrieve data to list
            var resultData = await result.ToListAsync();

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