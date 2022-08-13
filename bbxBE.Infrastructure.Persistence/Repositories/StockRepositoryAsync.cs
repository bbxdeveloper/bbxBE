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
using bbxBE.Application.Exceptions;
using bbxBE.Application.Consts;
using static bbxBE.Common.NAV.NAV_enums;

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
        private readonly IInvCtrlPeriodRepositoryAsync _invCtrlPeriodRepository;
        private readonly IInvCtrlRepositoryAsync _invCtrlRepository;

        public StockRepositoryAsync(ApplicationDbContext dbContext,
            IDataShapeHelper<Stock> dataShaperStock,
            IDataShapeHelper<GetStockViewModel> dataShaperGetStockViewModel,
            IModelHelper modelHelper, IMapper mapper, IMockService mockData,
            IStockCardRepositoryAsync stockCardRepository,
            IProductRepositoryAsync productRepository,
            IInvCtrlPeriodRepositoryAsync invCtrlPeriodRepository,
            IInvCtrlRepositoryAsync invCtrlRepository
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
            _invCtrlPeriodRepository = invCtrlPeriodRepository;
            _invCtrlRepository = invCtrlRepository;
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

            // Setup IQueryable
            
      
            var query = _dbContext.Stock.AsNoTracking()
                        .Include(p => p.Product).ThenInclude(p2 => p2.ProductCodes).AsNoTracking()
                        .Include(w => w.Warehouse).AsNoTracking()
                        .Where( w => w.Product.ProductCodes.Any(pc => pc.ProductCodeCategory == enCustproductCodeCategory.OWN.ToString()));
            /*
            var result = _dbContext.Stock.AsNoTracking()
                            .Include(p => p.Product).AsNoTracking()
                            .Include(w => w.Warehouse).AsNoTracking()
                            ;
            */
            // Count records total
            recordsTotal = await query.CountAsync();

            // filter data
            FilterByParameters(ref query, requestParameter.WarehouseID, requestParameter.SearchString);

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
                if (orderBy.ToUpper() == bbxBEConsts.FIELD_PRODUCTCODE)
                {
                    //Kis heka...
                    query = query.OrderBy(o => o.Product.ProductCodes.Single(s =>
                                s.ProductCodeCategory == enCustproductCodeCategory.OWN.ToString()).ProductCodeValue);
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
            var resultData = await query.ToListAsync();

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

            var invCtrlPeriod = await _invCtrlPeriodRepository.GetInvCtrlPeriodRecordAsync(requestParameter.InvCtrlPeriodID);
            var invCtrlItems = await _invCtrlRepository.GetInvCtrlICPRecordsByPeriodAsync(requestParameter.InvCtrlPeriodID);
            var prodItems = _productRepository.GetAllProductsRecordFromCache();
            var stockItems = await _dbContext.Stock.AsNoTracking()
                                .Where(w => w.WarehouseID == invCtrlPeriod.WarehouseID && !w.Deleted).ToListAsync();


            var absenedItems = prodItems.Where(p => 
                        !invCtrlItems.Any(i => i.ProductID == p.ID) &&
                        (!requestParameter.IsInStock || 
                         (stockItems.Any(s=>s.ProductID == p.ID && (s.CalcQty != 0 || s.RealQty != 0))
                        ))).ToList();

            var absenedItems2 = stockItems.Where(s =>
                        !invCtrlItems.Any(i => i.ProductID == s.ProductID) &&
                        (!requestParameter.IsInStock || s.CalcQty != 0 || s.RealQty != 0)).ToList();

            //Hozzácsapjuk a absenedItems2-ből azokat a termékeket, amelyeknek nincs készletrekordja
            // itt tartok...
            var product2 = prodItems.Where(p => !stockItems.Any(s => s.ProductID == p.ID)).ToList();
            product2.ForEach(p =>
            {
                absenedItems2.Add(new Stock()
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

            // Count records total
            recordsTotal = absenedItems2.Count();

            // filter data
            if( !string.IsNullOrEmpty(requestParameter.SearchString))
            {
                absenedItems2 = absenedItems2.Where(p => p.Product.Description.ToUpper().Contains(requestParameter.SearchString) ||
                        p.Product.ProductCodes.Any(a => a.ProductCodeCategory == enCustproductCodeCategory.OWN.ToString() &&
                        a.ProductCodeValue.ToUpper().Contains(requestParameter.SearchString))).ToList();
            }    

            // Count records after filter
            recordsFiltered = absenedItems2.Count();

            //set Record counts
            var recordsCount = new RecordsCount
            {
                RecordsFiltered = recordsFiltered,
                RecordsTotal = recordsTotal
            };

            // set order by
            IOrderedEnumerable<Stock> absenedItems21;
            if (!string.IsNullOrWhiteSpace(orderBy))
            {
                if (orderBy.ToUpper() == bbxBEConsts.FIELD_PRODUCTCODE)
                {
                    //Kis heka...
                    absenedItems21 = absenedItems2.OrderBy(o => o.Product.ProductCodes.Single(s =>
                                s.ProductCodeCategory == enCustproductCodeCategory.OWN.ToString()).ProductCodeValue);
                }
                else
                {
   //                 absenedItems21 = absenedItems2.OrderBy(orderBy);
                }
            }


    //        var absenedItems211 = absenedItems21.Skip((pageNumber - 1) * pageSize).Take(pageSize);

            // retrieve data to list
          
            //TODO: szebben megoldani
            var resultDataModel = new List<GetStockViewModel>();
            /*
            resultDataModel.ForEach(i => resultDataModel.Add(
               _mapper.Map<Stock, GetStockViewModel>(i))
            );
            */
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