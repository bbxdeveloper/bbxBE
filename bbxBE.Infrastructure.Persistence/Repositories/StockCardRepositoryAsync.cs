using AutoMapper;
using bbxBE.Application.BLL;
using bbxBE.Application.Helpers;
using bbxBE.Application.Interfaces;
using bbxBE.Application.Interfaces.Repositories;
using bbxBE.Application.Parameters;
using bbxBE.Application.Queries.qStockCard;
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
    public class StockCardRepositoryAsync : GenericRepositoryAsync<StockCard>, IStockCardRepositoryAsync
    {
        private readonly IApplicationDbContext _dbContext;
        private IDataShapeHelper<StockCard> _dataShaperStockCard;
        private IDataShapeHelper<GetStockCardViewModel> _dataShaperGetStockCardViewModel;
        private readonly IMockService _mockData;
        private readonly IModelHelper _modelHelper;
        private readonly IMapper _mapper;
        private readonly ICacheService<Product> _productCacheService;

        public StockCardRepositoryAsync(IApplicationDbContext dbContext,
            IModelHelper modelHelper, IMapper mapper, IMockService mockData,
            ICacheService<Product> productCacheService) : base(dbContext)
        {
            _dbContext = dbContext;
            _dataShaperStockCard = new DataShapeHelper<StockCard>();
            _dataShaperGetStockCardViewModel = new DataShapeHelper<GetStockCardViewModel>();
            _modelHelper = modelHelper;
            _mapper = mapper;
            _mockData = mockData;
            _productCacheService = productCacheService;
        }


        public async Task<StockCard> CreateStockCard(Stock p_Stock, DateTime p_StockCardDate,
            long p_WarehouseID, long? p_ProductID, long? p_UserID, long? p_InvoiceLineID, long? p_InvCtrlID, long? p_WhsTransferLineID, long? p_CustomerID,
            enStockCardType p_ScType, decimal p_XRealQty, decimal p_UnitPrice,
            string p_XRel)
        {
            StockCard latestStockCard = null;
            decimal ORealQty = 0;
            decimal OAvgCost = 0;

            var sc = new StockCard()
            {
                StockID = p_Stock.ID,
                StockCardDate = p_StockCardDate.Date,
                WarehouseID = p_WarehouseID,
                ProductID = p_ProductID,
                UserID = p_UserID,
                InvoiceLineID = p_InvoiceLineID,
                InvCtrlID = p_InvCtrlID,
                WhsTransferLineID = p_WhsTransferLineID,
                CustomerID = p_CustomerID,
                ScType = p_ScType.ToString(),
                XRealQty = p_XRealQty,
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
                    ORealQty = prevItem.NRealQty;
                    OAvgCost = prevItem.NAvgCost;

                }
                else
                {
                    //Ha nincs előző elem, elég spec helyzet, legyenek a készletek 0-ák
                    /*
                    ORealQty = p_Stock.RealQty;
                    */

                    ORealQty = 0;
                    OAvgCost = p_Stock.AvgCost;
                }
            }
            else
            {
                //leltári tétel

                ORealQty = p_Stock.RealQty;
                OAvgCost = p_Stock.AvgCost;
            }
            //és ez alapján beupdate-eljük a currStockCard -ot
            sc.ORealQty = ORealQty;
            sc.OAvgCost = OAvgCost;

            sc.NRealQty = ORealQty + p_XRealQty;

            sc.NAvgCost = (p_XRealQty > 0 ?
                            bllStock.GetNewAvgCost(OAvgCost, ORealQty, p_XRealQty, p_UnitPrice) :   //csak bevételezés esetén változik az ELÁBÉ
                            OAvgCost);

            //következő napokra vonatkozó készletmódosító tételek
            //
            var furtherItems = await _dbContext.StockCard
                     .Where(w => w.WarehouseID == p_WarehouseID && w.ProductID == p_ProductID &&
                             w.StockCardDate > p_StockCardDate)
                      .OrderBy(o1 => o1.StockCardDate)
                      .ThenBy(o2 => o2.ID).ToListAsync();

            sc.Correction = furtherItems.Any();
            await AddAsync(sc);

            ORealQty = sc.NRealQty;
            OAvgCost = sc.NAvgCost;



            //Ezután az következő napok elemeit számoljuk át
            //
            foreach (var f in furtherItems)
            {

                f.ORealQty = ORealQty;
                f.OAvgCost = OAvgCost;

                if (f.ScType != enStockCardType.ICP.ToString() && f.ScType != enStockCardType.ICC.ToString())
                {
                    // NEM Leltári tétel esetén a NRealQty változik, a XRealQty nem változik
                    f.NRealQty = ORealQty + f.XRealQty;


                    f.NAvgCost = (f.XRealQty > 0 ?
                                     bllStock.GetNewAvgCost(OAvgCost, ORealQty, f.XRealQty, f.UnitPrice) :
                                     OAvgCost);
                    f.Correction = false;       //előző napok alapján lett átszámolva, emiatt már nem lehet korrekciós tétel
                }
                else
                {
                    // Leltári tétel esetén a XRealQty változik, a NRealQty nem változik
                    f.XRealQty = f.NRealQty - ORealQty;
                    f.NAvgCost = OAvgCost;
                    // leltár esetén meg kell tartani a f.Correction flag értékét, hogy lásssuk, mely tételek voltak visszamenőlegesen leltározva
                }

                await UpdateAsync(f);

                ORealQty = f.NRealQty;
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

            var orderBy = requestParameter.OrderBy;
            //      var fields = requestParameter.Fields;
            var fields = _modelHelper.GetQueryableFields<GetStockCardViewModel, StockCard>();


            int recordsTotal, recordsFiltered;


            /*********************************************/
            /* Gyorsítás: a product-ot cache-ből töltjük */
            /*********************************************/

            // Először lekérdezünk
            var preQuery = _dbContext.StockCard.AsNoTracking()
                        .Include(w => w.Warehouse).AsNoTracking()
                        .Include(c => c.Customer).AsNoTracking()
                        .Include(u => u.User).AsNoTracking();

            // Count records total
            recordsTotal = await preQuery.CountAsync();

            FilterBy(ref preQuery, requestParameter.WarehouseID, requestParameter.StockCardDateFrom, requestParameter.StockCardDateTo, requestParameter.XRel, requestParameter.ProductID);

            var resultList = await preQuery.ToListAsync();

            //Ezután feltöltjük a cache-ből a productot
            var prodCachedList = _productCacheService.ListCache();
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
                    query = query.OrderBy(o =>
                            o.Product != null &&
                            o.Product.ProductCodes != null &&
                            o.Product.ProductCodes.Any(s =>
                                    s.ProductCodeValue != null && s.ProductCodeCategory == enCustproductCodeCategory.OWN.ToString()) ?
                            o.Product.ProductCodes.SingleOrDefault(s =>
                                    s.ProductCodeValue != null && s.ProductCodeCategory == enCustproductCodeCategory.OWN.ToString()).ProductCodeValue :
                           String.Empty
                        );
                }
                else if (orderBy.ToUpper() == bbxBEConsts.FIELD_PRODUCT)
                {
                    query = query.OrderBy(o => o.Product != null ? o.Product.Description : string.Empty);
                }
                else if (orderBy.ToUpper() == bbxBEConsts.FIELD_STOCKCARDDATE)
                {
                    query = query.OrderBy(o => o.StockCardDate).ThenBy(o => o.ID);


                }
                else
                {
                    query = query.OrderBy(orderBy);
                }
            }
            else
            {
                // ha nincs rendezettség, akkor alapból ey legyen:
                query = query.OrderBy(o => o.StockCardDate).ThenBy(o => o.ID);

            }


            // select columns
            if (!string.IsNullOrWhiteSpace(fields))
            {
                query = query.Select<StockCard>("new(" + fields + ")");
            }

            // retrieve data to list
            var resultData = await GetPagedData(query, requestParameter, false);

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