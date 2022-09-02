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
using bbxBE.Application.Queries.qStock;
using bbxBE.Application.Queries.ViewModels;
using bbxBE.Common.Exceptions;
using bbxBE.Common.Consts;
using static bbxBE.Common.NAV.NAV_enums;
using bbxBE.Common.Enums;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using bbxBE.Infrastructure.Persistence.Caches;

namespace bbxBE.Infrastructure.Persistence.Repositories
{
    public class StockRepositoryAsync : GenericRepositoryAsync<Stock>, IStockRepositoryAsync
    {
        private readonly ApplicationDbContext _dbContext;
        private IDataShapeHelper<Stock> _dataShaperStock;
        private IDataShapeHelper<GetStockViewModel> _dataShaperGetStockViewModel;
        private readonly IMockService _mockData;
        private readonly IModelHelper _modelHelper;
        private readonly IMapper _mapper;
        private readonly IStockCardRepositoryAsync _stockCardRepository;
        private readonly IProductRepositoryAsync _productRepository;
        private readonly IInvCtrlRepositoryAsync _invCtrlRepository;
        private readonly ICustomerRepositoryAsync _customerRepository;
        private readonly ICacheService<Product> _productcacheService;

        public StockRepositoryAsync(ApplicationDbContext dbContext,
            IDataShapeHelper<Stock> dataShaperStock,
            IDataShapeHelper<GetStockViewModel> dataShaperGetStockViewModel,
            IModelHelper modelHelper, IMapper mapper, IMockService mockData,
            IStockCardRepositoryAsync stockCardRepository,
            IProductRepositoryAsync productRepository,
            IInvCtrlRepositoryAsync invCtrlRepository,
            ICustomerRepositoryAsync customerRepository,
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
            _invCtrlRepository = invCtrlRepository;
            _customerRepository = customerRepository;
            _productcacheService = productcacheService;
        }

        public async Task<List<Stock>> MaintainStockByInvoiceAsync(Invoice invoice)
        {
            var ret = new List<Stock>();

            foreach (var invoiceLine in invoice.InvoiceLines)
            {
                if (invoiceLine.ProductID.HasValue && invoiceLine.Product.IsStock)
                {

                    var stock = await _dbContext.Stock
                                .Where(x => x.WarehouseID == invoice.WarehouseID && x.ProductID == invoiceLine.ProductID && !x.Deleted)
                                .FirstOrDefaultAsync();

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
                        await _dbContext.Stock.AddAsync(stock);
                        await _dbContext.SaveChangesAsync();
                    }

                    var latestStockCard = await _stockCardRepository.CreateStockCard(stock, invoice.InvoiceDeliveryDate,
                                invoice.WarehouseID, invoiceLine.ProductID, invoice.UserID, invoiceLine.ID,
                                (invoice.Incoming ? invoice.SupplierID : invoice.CustomerID),
                                Common.Enums.enStockCardType.INVOICE,
                                invoiceLine.Quantity * (invoice.Incoming ? 1 : -1),
                                invoiceLine.Quantity * (invoice.Incoming ? 1 : -1),
                                0, invoiceLine.UnitPrice,
                                invoice.InvoiceNumber + ( invoice.Incoming ? ";" + invoice.CustomerInvoiceNumber : "" ));


                    if (invoice.Incoming)
                    {

                        stock.CalcQty += invoiceLine.Quantity;
                        stock.RealQty += invoiceLine.Quantity;
                        stock.LatestIn = DateTime.UtcNow;

                    }
                    else
                    {
                        stock.CalcQty -= invoiceLine.Quantity;
                        stock.RealQty -= invoiceLine.Quantity;
                        stock.LatestOut = DateTime.UtcNow;
                    }
                    stock.AvgCost = latestStockCard.NAvgCost;


                    _dbContext.Stock.Update(stock);

                    ret.Add(stock);
                }

            }
            return ret;
        }

        public async Task<List<Stock>> MaintainStockByInvCtrlAsync(List<InvCtrl> invCtrlList, string XRel)
        {
            var ret = new List<Stock>();


            var ownData = _customerRepository.GetOwnData();
            if (ownData == null)
            {
                throw new ResourceNotFoundException(string.Format(bbxBEConsts.FV_OWNNOTFOUND));
            }
            foreach (var invCtrl in invCtrlList)
            {

                var stock = await _dbContext.Stock
                            .Where(x => x.WarehouseID == invCtrl.WarehouseID && x.ProductID == invCtrl.ProductID && !x.Deleted)
                            .FirstOrDefaultAsync();

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
                    await _dbContext.Stock.AddAsync(stock);
                    await _dbContext.SaveChangesAsync();
                }

                //beaktualizáljuk az InvCtrl-be az aktuális raktárkészletet
                invCtrl.OCalcQty = stock.CalcQty;
                invCtrl.ORealQty = stock.RealQty;


                var latestStockCard = await _stockCardRepository.CreateStockCard(stock, invCtrl.InvCtrlDate,
                            invCtrl.WarehouseID, invCtrl.ProductID, invCtrl.UserID, 0, ownData.ID,
                            invCtrl.InvCtrlType == enInvCtrlType.ICP.ToString() ? enStockCardType.ICP : enStockCardType.ICC,
                            invCtrl.NCalcQty - invCtrl.OCalcQty,        //csak a különbséget kell átadni!!!
                            invCtrl.NRealQty - invCtrl.ORealQty,        //csak a különbséget kell átadni!!!
                            0, stock.AvgCost,
                            XRel);



                stock.CalcQty = invCtrl.NCalcQty;
                stock.RealQty = invCtrl.NRealQty;
                stock.AvgCost = invCtrl.AvgCost;


                _dbContext.Stock.Update(stock);

                invCtrl.StockID = stock.ID;
                _dbContext.InvCtrl.Update(invCtrl);

                ret.Add(stock);
            }
            return ret;
        }

        public async Task<Entity> GetStockAsync(GetStock requestParameter)
        {

            var ID = requestParameter.ID;

            var item = await _dbContext.Stock.AsNoTracking()
             .Include(p => p.Product).ThenInclude(p2 => p2.ProductCodes).AsNoTracking()
             .Include(w => w.Warehouse).AsNoTracking()
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

        public async Task<Stock> GetStockRecordAsync(GetStockRecord request)
        {
            var item = await _dbContext.Stock.AsNoTracking()
             .Where(w => w.WarehouseID == request.WarehouseID && w.ProductID == request.ProductID && !w.Deleted).FirstOrDefaultAsync();

            if(item == null)        //Jeremi kérése
            {
                item = new Stock();
            }    
            return item;
        }

        public async Task<(IEnumerable<Entity> data, RecordsCount recordsCount)> QueryPagedStockAsync(QueryStock requestParameter)
        {


            var pageNumber = requestParameter.PageNumber;
            var pageSize = requestParameter.PageSize;
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
            // paging
            query = query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize);

            // retrieve data to list
            var resultData =  query.ToList();

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

            if ( p_warehouseID == 0 && string.IsNullOrWhiteSpace(p_searchString))
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
                                        .Where( i=>i.ID == requestParameter.InvCtrlPeriodID).SingleOrDefaultAsync();
            if (invCtrlPeriod == null)
            {
                throw new ResourceNotFoundException(string.Format(bbxBEConsts.ERR_INVCTRLPERIODNOTFOUND, requestParameter.InvCtrlPeriodID));
            }
            var invCtrlItems = await _invCtrlRepository.GetInvCtrlICPRecordsByPeriodAsync(requestParameter.InvCtrlPeriodID);
            var prodItems = _productcacheService.ListCache();
            var stockItems = await _dbContext.Stock.AsNoTracking()
                             .Include(w => w.Warehouse).AsNoTracking()
                             .Where(w => w.WarehouseID == invCtrlPeriod.WarehouseID && !w.Deleted).ToListAsync();

            stockItems.ForEach(i =>
                i.Product = prodItems.FirstOrDefault(f => f.ID == i.ProductID)
                );


            var absenedItems = stockItems.Where(s =>
                        !invCtrlItems.Any(i => i.ProductID == s.ProductID) &&
                        (!requestParameter.IsInStock || s.CalcQty != 0 || s.RealQty != 0)).ToList();

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
                        CalcQty = 0,
                        RealQty = 0,
                        OutQty = 0,
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
            if( !string.IsNullOrEmpty(requestParameter.SearchString))
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

        public Task<bool> SeedDataAsync(int rowCount)
        {
            throw new System.NotImplementedException();
        }


    }
}