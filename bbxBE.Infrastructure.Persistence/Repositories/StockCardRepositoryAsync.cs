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
using bbxBE.Application.Queries.qStockCard;
using bbxBE.Application.Queries.ViewModels;
using bbxBE.Common.Exceptions;
using bbxBE.Common.Consts;
using static bbxBE.Common.NAV.NAV_enums;
using bbxBE.Common.Enums;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using bbxBE.Infrastructure.Persistence.Caches;

namespace bbxBE.Infrastructure.Persistence.Repositories
{
    public class StockCardRepositoryAsync : GenericRepositoryAsync<StockCard>, IStockCardRepositoryAsync
    {
        private readonly ApplicationDbContext _dbContext;
        private IDataShapeHelper<StockCard> _dataShaperStockCard;
        private IDataShapeHelper<GetStockCardViewModel> _dataShaperGetStockCardViewModel;
        private readonly IMockService _mockData;
        private readonly IModelHelper _modelHelper;
        private readonly IMapper _mapper;
        private readonly ICacheService<Product> _productcacheService;

        public StockCardRepositoryAsync(ApplicationDbContext dbContext,
            IDataShapeHelper<StockCard> dataShaperStockCard,
            IDataShapeHelper<GetStockCardViewModel> dataShaperGetStockCardViewModel,
            IModelHelper modelHelper, IMapper mapper, IMockService mockData, 
            ICacheService<Product> productcacheService) : base(dbContext)
        {
            _dbContext = dbContext;
            _dataShaperStockCard = dataShaperStockCard;
            _dataShaperGetStockCardViewModel = dataShaperGetStockCardViewModel;
            _modelHelper = modelHelper;
            _mapper = mapper;
            _mockData = mockData;
            _productcacheService = productcacheService;
        }


        public async Task<StockCard> CreateStockCard(Stock p_Stock, DateTime p_StockCardDate,
            long p_WarehouseID, long? p_ProductID, long? p_UserID, long? p_InvoiceLineID, long? p_CustomerID,
            enStockCardType p_ScType,
            decimal p_XCalcQty, decimal p_XRealQty, decimal p_XOutQty, decimal p_UnitPrice,
            string p_XRel)
        {
            StockCard latestStockCard = null;
            decimal OCalcQty = 0;
            decimal ORealQty = 0;
            decimal OOutQty = 0;                //ha lesz árukiadás, töltjük!
            decimal OAvgCost = 0;

            var sc = new StockCard()
            {
                StockID = p_Stock.ID,
                StockCardDate = p_StockCardDate.Date,
                WarehouseID = p_WarehouseID,
                ProductID = p_ProductID,
                UserID = p_UserID,
                InvoiceLineID = p_InvoiceLineID,
                CustomerID = p_CustomerID,
                ScType = p_ScType.ToString(),
                XCalcQty = p_XCalcQty,
                XRealQty = p_XRealQty,
                XOutQty = p_XOutQty,               //ha lesz árukiadás, töltjük!
                UnitPrice = p_UnitPrice,
                XRel = p_XRel
            };
            latestStockCard = sc;

            if (p_ScType != enStockCardType.ICP && p_ScType != enStockCardType.ICC)
            {
                //nem leltári tétel
                var prevItem = await _dbContext.StockCard.AsNoTracking()
                        .Where(w => w.WarehouseID == p_WarehouseID && w.ProductID == p_ProductID &&
                                w.StockCardDate <= p_StockCardDate)
                         .OrderByDescending(o1 => o1.StockCardDate)
                         .ThenByDescending(o2 => o2.ID)
                         .FirstOrDefaultAsync();

                //Ha van előző elem, akkor azt vesszük a számítás alapjának
                if (prevItem != null)
                {
                    OCalcQty = prevItem.NCalcQty;
                    ORealQty = prevItem.NRealQty;
                    OOutQty = prevItem.NOutQty;                //ha lesz árukiadás, töltjük!
                    OAvgCost = prevItem.NAvgCost;

                }
                else
                {
                    //Ha nincs előző elem, elég spec helyzet, legyenek a készletek 0-ák
                    /*
                    OCalcQty = p_Stock.CalcQty;
                    ORealQty = p_Stock.RealQty;
                    OOutQty = p_Stock.OutQty;                //ha lesz árukiadás, töltjük!
                    */

                    OCalcQty = 0;
                    ORealQty = 0;
                    OOutQty = 0;                //ha lesz árukiadás, töltjük!

                    OAvgCost = p_Stock.AvgCost;
                }
            }
            else
            {
                //leltári tétel
 
                OCalcQty = p_Stock.CalcQty;
                ORealQty = p_Stock.RealQty;
                OOutQty = 0;                //ha lesz árukiadás, töltjük!
                OAvgCost = p_Stock.AvgCost;
            }
            //és ez alapján beupdate-eljük a currStockCard -ot
            sc.OCalcQty = OCalcQty;
            sc.ORealQty = ORealQty;
            sc.OOutQty = OOutQty;                //ha lesz árukiadás, töltjük!
            sc.OAvgCost = OAvgCost;

            sc.NCalcQty = OCalcQty + p_XCalcQty;
            sc.NRealQty = ORealQty + p_XRealQty;
            sc.NOutQty = OOutQty + p_XOutQty;                //ha lesz árukiadás, töltjük!

            sc.NAvgCost = (p_XCalcQty >= 0 || p_XRealQty > 0 ?
                            bllStock.GetNewAvgCost(OAvgCost, ORealQty, (ORealQty + p_XRealQty), p_UnitPrice) :
                            OAvgCost);
            await AddAsync(sc);

            OCalcQty = sc.NCalcQty;
            ORealQty = sc.NRealQty;
            OOutQty = sc.NOutQty;                //ha lesz árukiadás, töltjük!
            OAvgCost = sc.NAvgCost;



            //Ezután az utána következő, többi elemet számoljuk át

            var furtherItems = await _dbContext.StockCard
                     .Where(w => w.WarehouseID == p_WarehouseID && w.ProductID == p_ProductID &&
                             w.StockCardDate > p_StockCardDate)
                      .OrderBy(o1 => o1.StockCardDate)
                      .ThenBy(o2 => o2.ID).ToListAsync();
            foreach (var f in furtherItems)
            {

                if (f.ScType != enStockCardType.ICP.ToString() && f.ScType != enStockCardType.ICC.ToString())
                {
                    // NEM leltári tétel
                    var XCalcQty = f.NCalcQty - f.OCalcQty;
                    var XRealQty = f.NRealQty - f.ORealQty;
                    var XOutQty = f.NOutQty - f.OOutQty;

                    f.OCalcQty = OCalcQty;
                    f.ORealQty = ORealQty;
                    f.OOutQty = OOutQty;                //ha lesz árukiadás, töltjük!
                    f.OAvgCost = OAvgCost;

                    f.NCalcQty = OCalcQty + XCalcQty;
                    f.NRealQty = ORealQty + XRealQty;
                    f.NOutQty = OOutQty + XOutQty;                //ha lesz árukiadás, töltjük!

                    f.NAvgCost = (XCalcQty >= 0 || XRealQty > 0 ?
                                    bllStock.GetNewAvgCost(OAvgCost, ORealQty, (ORealQty + XRealQty), f.UnitPrice) :
                                    OAvgCost);

                    await UpdateAsync(f);
                }
                else
                {
                    // Leltári tétel esetén nincs átszámolás!!!!
                }

                OCalcQty = f.NCalcQty;
                ORealQty = f.NRealQty;
                OOutQty = f.NOutQty;                //ha lesz árukiadás, töltjük!
                OAvgCost = f.NAvgCost;

                latestStockCard = f;
            }
  //          await _dbContext.SaveChangesAsync();
            return latestStockCard;
        }

        public async Task<Entity> GetStockCardAsync(long ID)
        {

            var item = await _dbContext.StockCard.AsNoTracking()
                    .Include(p => p.Product).ThenInclude(p2 => p2.ProductCodes).AsNoTracking()
                    .Include(w => w.Warehouse).AsNoTracking()
                    .Where(w => w.Product.ProductCodes.Any(pc => pc.ProductCodeCategory == enCustproductCodeCategory.OWN.ToString())
                    && w.ID == ID && !w.Deleted).FirstOrDefaultAsync();

            //            var fields = requestParameter.Fields;

            if (item == null)
            {
                throw new ResourceNotFoundException(string.Format(bbxBEConsts.ERR_STOCKCARDNOTFOUND, ID));
            }

            var itemModel = _mapper.Map<StockCard, GetStockCardViewModel>(item);
            var listFieldsModel = _modelHelper.GetModelFields<GetStockCardViewModel>();

            // shape data
            var shapeData = _dataShaperGetStockCardViewModel.ShapeData(itemModel, String.Join(",", listFieldsModel));

            return shapeData;
        }
        public async Task<(IEnumerable<Entity> data, RecordsCount recordsCount)> QueryPagedStockCardAsync(QueryStockCard requestParameter)
        {


            var pageNumber = requestParameter.PageNumber;
            var pageSize = requestParameter.PageSize;
            var orderBy = requestParameter.OrderBy;
            //      var fields = requestParameter.Fields;
            var fields = _modelHelper.GetQueryableFields<GetStockCardViewModel, StockCard>();


            int recordsTotal, recordsFiltered;


            /*********************************************/
            /* Gyorsítás: a product-ot cache-ből töltjük */
            /*********************************************/

            // Először lekérdezünk
            var preQuery = _dbContext.StockCard.AsNoTracking()
                        .Include(w => w.Warehouse).AsNoTracking();

            // Count records total
            recordsTotal = await preQuery.CountAsync();

            FilterBy(ref preQuery, requestParameter.WarehouseID, requestParameter.StockCardDateFrom, requestParameter.StockCardDateTo, requestParameter.XRel, requestParameter.ProductID);

            var resultList = await preQuery.ToListAsync();

            //Ezután feltöltjük a cache-ből a productot
            var prodCachedList = _productcacheService.ListCache();
            resultList.ForEach(i =>
                i.Product = prodCachedList.FirstOrDefault(f => f.ID == i.ProductID)
                );

            var query = resultList.AsQueryable();


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
                query = query.Select<StockCard>("new(" + fields + ")");
            }
            // paging
            query = query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize);

            // retrieve data to list
            var resultData = query.ToList();

            //TODO: szebben megoldani
            var resultDataModel = new List<GetStockCardViewModel>();
            resultData.ForEach(i => resultDataModel.Add(
               _mapper.Map<StockCard, GetStockCardViewModel>(i))
            );


            var listFieldsModel = _modelHelper.GetModelFields<GetStockCardViewModel>();

            var shapeData = _dataShaperGetStockCardViewModel.ShapeData(resultDataModel, String.Join(",", listFieldsModel));

            return (shapeData, recordsCount);
        }

        private void FilterBy(ref IQueryable<StockCard> p_item,
            long? WarehouseID, DateTime? StockCardDateFrom, DateTime? StockCardDateTo, string XRel, long? ProductID)


        {
            if (!p_item.Any() ||
                (!WarehouseID.HasValue && !StockCardDateFrom.HasValue && !StockCardDateTo.HasValue && XRel == null && !ProductID.HasValue))
                return;

            var predicate = PredicateBuilder.New<StockCard>();
            if (WarehouseID.HasValue && WarehouseID.Value > 0)
            {
                predicate = predicate.And(p => p.WarehouseID == WarehouseID.Value);
            }
            if (StockCardDateFrom.HasValue)
            {
                predicate = predicate.And(p => p.StockCardDate >= StockCardDateFrom.Value);
            }
            if (StockCardDateTo.HasValue)
            {
                predicate = predicate.And(p => p.StockCardDate <= StockCardDateTo.Value);
            }
            if (XRel != null)
            {
                XRel = XRel.ToUpper();
                predicate = predicate.And(p => p.XRel.ToUpper().Contains(XRel));
            }
            if (ProductID.HasValue && ProductID.Value > 0)
            {
                predicate = predicate.And(p => p.ProductID == ProductID.Value);
            }


            p_item = p_item.Where(predicate);
        }

        public Task<bool> SeedDataAsync(int rowCount)
        {
            throw new System.NotImplementedException();
        }
    }
}