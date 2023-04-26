﻿using AutoMapper;
using bbxBE.Application.Interfaces;
using bbxBE.Application.Interfaces.Repositories;
using bbxBE.Application.Parameters;
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
    public class StockRepositoryAsync : GenericRepositoryAsync<Stock>, IStockRepositoryAsync
    {
        private readonly IApplicationDbContext _dbContext;
        private IDataShapeHelper<Stock> _dataShaperStock;
        private IDataShapeHelper<GetStockViewModel> _dataShaperGetStockViewModel;
        private readonly IMockService _mockData;
        private readonly IModelHelper _modelHelper;
        private readonly IMapper _mapper;
        private readonly IStockCardRepositoryAsync _stockCardRepository;
        private readonly IProductRepositoryAsync _productRepository;
        private readonly ILocationRepositoryAsync _locationRepository;
        private readonly ICacheService<Product> _productcacheService;

        public StockRepositoryAsync(IApplicationDbContext dbContext,
            IDataShapeHelper<Stock> dataShaperStock,
            IDataShapeHelper<GetStockViewModel> dataShaperGetStockViewModel,
            IModelHelper modelHelper, IMapper mapper, IMockService mockData,
            IStockCardRepositoryAsync stockCardRepository,
            IProductRepositoryAsync productRepository,
            ILocationRepositoryAsync locationRepository,
            ICacheService<Product> productcacheService
          ) : base(dbContext)
        {
            _dbContext = dbContext;
            _dataShaperStock = dataShaperStock;
            _dataShaperGetStockViewModel = dataShaperGetStockViewModel;
            _modelHelper = modelHelper;
            _mapper = mapper;
            _mockData = mockData;
            _stockCardRepository = stockCardRepository;
            _productRepository = productRepository;
            _productcacheService = productcacheService;
            _locationRepository = locationRepository;
        }

        public async Task<List<Stock>> MaintainStockByInvoiceAsync(Invoice invoice)
        {
            var lstStock = new List<Stock>();

            foreach (var invoiceLine in invoice.InvoiceLines)
            {
                if (invoiceLine.ProductID.HasValue && invoiceLine.Product.IsStock)
                {

                    var stock = lstStock.FirstOrDefault(x => x.WarehouseID == invoice.WarehouseID && x.ProductID == invoiceLine.ProductID);
                    if (stock == null)
                    {

                        stock = await _dbContext.Stock
                                .Where(x => x.WarehouseID == invoice.WarehouseID && x.ProductID == invoiceLine.ProductID && !x.Deleted)
                                .FirstOrDefaultAsync();
                    }

                    if (stock == null)
                    {
                        stock = new Stock()
                        {
                            WarehouseID = invoice.WarehouseID,
                            //Warehouse = invoice.Warehouse,
                            ProductID = invoiceLine.ProductID.Value,
                            //Product = invoiceLine.Product,
                            AvgCost = invoiceLine.UnitPrice
                        };
                        await AddAsync(stock);
                    }

                    //Ha van bizonylatkedvezmény, akkor a LineNetDiscountedAmountHUF-ból kell visszaszámolni a  realInvoiceUnitPriceHUF-t
                    var realInvoiceUnitPriceHUF = invoiceLine.UnitPriceHUF;
                    if (!invoiceLine.Product.NoDiscount && invoice.InvoiceDiscountPercent != 0)
                    {
                        realInvoiceUnitPriceHUF = Math.Round(invoiceLine.LineNetDiscountedAmountHUF / invoiceLine.Quantity, 1);
                    }

                    var latestStockCard = await _stockCardRepository.CreateStockCard(stock, invoice.InvoiceDeliveryDate,
                                invoice.WarehouseID, invoiceLine.ProductID, invoice.UserID, invoiceLine.ID,
                                (invoice.Incoming ? invoice.SupplierID : invoice.CustomerID),
                                Common.Enums.enStockCardType.INV_DLV,
                                invoiceLine.Quantity * (invoice.Incoming ? 1 : -1),
                                realInvoiceUnitPriceHUF,
                                invoice.InvoiceNumber + (invoice.Incoming ? ";" + invoice.CustomerInvoiceNumber : ""));


                    if (invoice.Incoming)
                    {

                        stock.RealQty += invoiceLine.Quantity;
                        stock.LatestIn = DateTime.UtcNow;

                    }
                    else
                    {
                        stock.RealQty -= invoiceLine.Quantity;
                        stock.LatestOut = DateTime.UtcNow;
                    }
                    stock.AvgCost = latestStockCard.NAvgCost;


                    lstStock.Add(stock);
                }
            }
            await UpdateRangeAsync(lstStock);
            return lstStock;
        }

        public async Task<List<Stock>> MaintainStockByInvCtrlAsync(List<InvCtrl> invCtrlList, Customer p_ownData, string XRel)
        {
            var lstStock = new List<Stock>();       //updatelendő készlet gyűjtő
            foreach (var invCtrl in invCtrlList)
            {

                var stock = lstStock.FirstOrDefault(x => x.WarehouseID == invCtrl.WarehouseID && x.ProductID == invCtrl.ProductID); //már foglalkoztunk a készlettel ?
                if (stock == null)
                {

                    stock = await _dbContext.Stock
                             .Where(x => x.WarehouseID == invCtrl.WarehouseID && x.ProductID == invCtrl.ProductID && !x.Deleted)
                             .FirstOrDefaultAsync();
                }

                if (stock == null)
                {
                    stock = new Stock()
                    {
                        WarehouseID = invCtrl.WarehouseID,
                        //Warehouse = invoice.Warehouse,
                        ProductID = invCtrl.ProductID,
                        //Product = invoiceLine.Product,
                        AvgCost = invCtrl.AvgCost             //ez nem változik
                    };
                    await AddAsync(stock);
                }

                //beaktualizáljuk az InvCtrl-be az aktuális raktárkészletet
                invCtrl.ORealQty = stock.RealQty;

                var latestStockCard = await _stockCardRepository.CreateStockCard(stock, invCtrl.InvCtrlDate,
                            invCtrl.WarehouseID, invCtrl.ProductID, invCtrl.UserID, 0, p_ownData.ID,
                            invCtrl.InvCtrlType == enInvCtrlType.ICP.ToString() ? enStockCardType.ICP : enStockCardType.ICC,
                            invCtrl.NRealQty - invCtrl.ORealQty,        //csak a különbséget kell átadni!!!
                            stock.AvgCost,
                            XRel);



                stock.RealQty = invCtrl.NRealQty;
                stock.AvgCost = invCtrl.AvgCost;


                invCtrl.StockID = stock.ID;

                lstStock.Add(stock);
            }

            await UpdateRangeAsync(lstStock);

            return lstStock;
        }

        public async Task<Entity> GetStockAsync(long ID)
        {

            var item = await _dbContext.Stock.AsNoTracking()
             .Include(p => p.Product).ThenInclude(p2 => p2.ProductCodes).AsNoTracking()
             .Include(w => w.Warehouse).AsNoTracking()
             .Include(l => l.Location).AsNoTracking()
             .Where(w => w.Product.ProductCodes.Any(pc => pc.ProductCodeCategory == enCustproductCodeCategory.OWN.ToString())
                        && w.ID == ID && !w.Deleted).FirstOrDefaultAsync();

            //            var fields = requestParameter.Fields;

            if (item == null)
            {
                throw new ResourceNotFoundException(string.Format(bbxBEConsts.ERR_STOCKNOTFOUND, ID));
            }

            var itemModel = _mapper.Map<Stock, GetStockViewModel>(item);
            var listFieldsModel = _modelHelper.GetModelFields<GetStockViewModel>();

            // shape data
            var shapeData = _dataShaperGetStockViewModel.ShapeData(itemModel, String.Join(",", listFieldsModel));

            return shapeData;
        }

        public async Task<Stock> GetStockRecordAsync(long warehouseID, long productID)
        {
            var item = await _dbContext.Stock.AsNoTracking()
             .Include(l => l.Location).AsNoTracking()
             .Where(w => w.WarehouseID == warehouseID && w.ProductID == productID && !w.Deleted).FirstOrDefaultAsync();

            if (item == null)        //Jeremi kérése
            {
                item = new Stock();
            }
            return item;
        }

        public async Task<(IEnumerable<Entity> data, RecordsCount recordsCount)> QueryPagedStockAsync(QueryStock requestParameter)
        {

            var orderBy = requestParameter.OrderBy;
            //      var fields = requestParameter.Fields;
            var fields = _modelHelper.GetQueryableFields<GetStockViewModel, Stock>();


            int recordsTotal, recordsFiltered;

            /*********************************************/
            /* Gyorsítás: a product-ot cache-ből töltjük */
            /*********************************************/

            //raktárra lekrédezés
            var preQuery = _dbContext.Stock.AsNoTracking()
                        .Include(w => w.Warehouse).AsNoTracking()
                        .Include(l => l.Location).AsNoTracking()
                        .Where(w => !w.Deleted && w.WarehouseID == requestParameter.WarehouseID);

            // Count records total
            recordsTotal = await preQuery.CountAsync();


            var resultList = await preQuery.ToListAsync();

            //Ezután feltöltjük a cache-ből a productot
            var prodCachedList = _productcacheService.ListCache();
            resultList.ForEach(i =>
                i.Product = prodCachedList.FirstOrDefault(f => f.ID == i.ProductID)
                );


            var query = resultList.AsQueryable();

            // filter data
            FilterByParameters(ref query, requestParameter.WarehouseID, requestParameter.SearchString);

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
                query = query.Select<Stock>("new(" + fields + ")");
            }

            // retrieve data to list
            var resultData = await GetPagedData(query, requestParameter, false);

            //TODO: szebben megoldani
            var resultDataModel = new List<GetStockViewModel>();
            resultData.ForEach(i => resultDataModel.Add(
               _mapper.Map<Stock, GetStockViewModel>(i))
            );


            var listFieldsModel = _modelHelper.GetModelFields<GetStockViewModel>();

            var shapeData = _dataShaperGetStockViewModel.ShapeData(resultDataModel, String.Join(",", listFieldsModel));

            return (shapeData, recordsCount);
        }

        private void FilterByParameters(ref IQueryable<Stock> p_item, long p_warehouseID, string p_searchString)
        {
            if (!p_item.Any())
                return;

            if (p_warehouseID == 0 && string.IsNullOrWhiteSpace(p_searchString))
                return;

            var predicate = PredicateBuilder.New<Stock>();
            if (p_warehouseID > 0)
            {
                predicate = predicate.And(p => p.WarehouseID == p_warehouseID);
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

        public async Task<Stock> UpdateStockLocationAsync(long ID, long? LocationID)
        {
            var stock = await GetByIdAsync(ID);

            if (stock == null)
            {
                throw new ResourceNotFoundException(string.Format(bbxBEConsts.ERR_STOCKNOTFOUND, ID));
            }

            if (LocationID.HasValue)
            {
                var loc = await _locationRepository.GetByIdAsync(LocationID.Value);
                if (loc == null)
                {
                    throw new ResourceNotFoundException(string.Format(bbxBEConsts.ERR_LOCATIONOTFOUND, LocationID.Value));
                }
                stock.LocationID = LocationID;
            }
            else
            {
                stock.LocationID = null;
            }

            await UpdateAsync(stock);

            return stock;
        }
    }
}