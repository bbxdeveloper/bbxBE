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
using bbxBE.Application.Queries.qProduct;
using bbxBE.Application.Queries.ViewModels;
using bbxBE.Common;
using bbxBE.Application.Consts;
using bbxBE.Application.Exceptions;
using static bbxBE.Common.NAV.NAV_enums;
using bbxBE.Infrastructure.Persistence.Caches;
using Hangfire;
using System.Threading;
using EFCore.BulkExtensions;

namespace bbxBE.Infrastructure.Persistence.Repositories
{
    public class ProductRepositoryAsync : GenericRepositoryAsync<Product>, IProductRepositoryAsync
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly DbSet<Product> _Products;
        private readonly DbSet<ProductCode> _ProductCodes;
        private readonly DbSet<Origin> _Origins;
        private readonly DbSet<ProductGroup> _ProductGroups;
        private readonly DbSet<VatRate> _VatRates;
        private IDataShapeHelper<Product> _dataShaperProduct;
        private IDataShapeHelper<GetProductViewModel> _dataShaperGetProductViewModel;
        private readonly IMockService _mockData;
        private readonly IModelHelper _modelHelper;
        private readonly IMapper _mapper;
        private readonly ICacheService<Product> _cacheService;
        private readonly ICacheService<ProductGroup> _productGroupCacheService;
        private readonly ICacheService<Origin> _originCacheService;
        private readonly ICacheService<VatRate> _vatRateCacheService;
        private List<ProductCode> pcList = new List<ProductCode>();

        /*
           "id": 2272,
   "productCode": "QQQ-RANGEX",
  "description": "QQQ range teszt átírás",
  "productGroupCode": null,
  "originCode": null,
  "unitOfMeasure": "PIECE",
  "unitPrice1": 10,
  "unitPrice2": 20,
  "latestSupplyPrice": 30,
  "isStock": true,
  "minStock": 10,
  "ordUnit":20,
  "productFee": 0,
  "active": true,
  "vtsz": "12121211",
  "ean": null,
"vatRateCode" : "27%"
        */
        public ProductRepositoryAsync(ApplicationDbContext dbContext,
            IDataShapeHelper<Product> dataShaperProduct,
            IDataShapeHelper<GetProductViewModel> dataShaperGetProductViewModel,
            IModelHelper modelHelper, IMapper mapper, IMockService mockData,
            ICacheService<Product> cacheService,
            ICacheService<ProductGroup> productGroupCacheService,
            ICacheService<Origin> originCacheService,
            ICacheService<VatRate> vatRateCacheService
            ) : base(dbContext)
        {
            _dbContext = dbContext;
            _Products = dbContext.Set<Product>();
            _ProductCodes = dbContext.Set<ProductCode>();
            _Origins = dbContext.Set<Origin>();
            _ProductGroups = dbContext.Set<ProductGroup>();
            _VatRates = dbContext.Set<VatRate>();

            _dataShaperProduct = dataShaperProduct;
            _dataShaperGetProductViewModel = dataShaperGetProductViewModel;
            _modelHelper = modelHelper;
            _mapper = mapper;
            _mockData = mockData;
            _cacheService = cacheService;
            _productGroupCacheService = productGroupCacheService;
            _originCacheService = originCacheService;
            _vatRateCacheService = vatRateCacheService;

            var t = RefreshProductCache();
            t.GetAwaiter().GetResult();

            var t2 = RefreshVatRateCache();
            t2.GetAwaiter().GetResult();

        }


        public bool IsUniqueProductCode(string ProductCode, long? ProductID = null)
        {

            /*
            return !await _ProductCodes.AnyAsync(p => p.ProductCodeCategory == enCustproductCodeCategory.OWN.ToString()
                && p.ProductCodeValue.ToUpper() == ProductCode.ToUpper()
                && !p.Deleted && (ProductID == null || p.ProductID != ProductID.Value));
            */

            var query = _cacheService.QueryCache();
            return !query.ToList().Any(p => p.ProductCodes.Any(a => a.ProductCodeCategory == enCustproductCodeCategory.OWN.ToString()
               && a.ProductCodeValue.ToUpper() == ProductCode.ToUpper())
                && !p.Deleted && (ProductID == null || p.ID != ProductID.Value));

        }


        public async Task<bool> CheckProductGroupCodeAsync(string ProductGroupCode)
        {
            if (_productGroupCacheService.IsCacheEmpty())
            {
                return await _ProductGroups.AnyAsync(p => p.ProductGroupCode == ProductGroupCode && !p.Deleted);
            }
            else
            {
                var query = _productGroupCacheService.QueryCache();
                return query.ToList().Any(p => p.ProductGroupCode == ProductGroupCode && !p.Deleted);
            }
        }

        public async Task<bool> CheckOriginCodeAsync(string OriginCode)
        {
            if (_originCacheService.IsCacheEmpty())
            {
                return await _Origins.AnyAsync(p => p.OriginCode == OriginCode && !p.Deleted);
            }
            else
            {
                var query = _originCacheService.QueryCache();
                return query.ToList().Any(p => p.OriginCode == OriginCode && !p.Deleted);
            }
        }

        public async Task<bool> CheckVatRateCodeAsync(string VatRateCode)
        {
            if (_vatRateCacheService.IsCacheEmpty())
            {
                return await _VatRates.AnyAsync(p => p.VatRateCode == VatRateCode && !p.Deleted);
            }
            else
            {
                var query = _vatRateCacheService.QueryCache();
                return query.ToList().Any(p => p.VatRateCode == VatRateCode && !p.Deleted);
            }
        }

        private Product PrepareNewProduct(Product p_product, string p_ProductGroupCode, string p_OriginCode, string p_VatRateCode)
        {
            if (!string.IsNullOrWhiteSpace(p_ProductGroupCode))
            {
                if (_productGroupCacheService.IsCacheEmpty())
                {
                    p_product.ProductGroupID = _ProductGroups.SingleOrDefault(x => x.ProductGroupCode == p_ProductGroupCode)?.ID;
                }
                else
                {
                    var query = _productGroupCacheService.QueryCache();
                    p_product.ProductGroupID = query.SingleOrDefault(x => x.ProductGroupCode == p_ProductGroupCode)?.ID;
                }
            }

            if (!string.IsNullOrWhiteSpace(p_OriginCode))
            {
                if (_originCacheService.IsCacheEmpty())
                {
                    p_product.OriginID = _Origins.SingleOrDefault(x => x.OriginCode == p_OriginCode)?.ID;
                }
                else
                {
                    var query = _originCacheService.QueryCache();
                    p_product.OriginID = query.SingleOrDefault(x => x.OriginCode == p_OriginCode)?.ID;
                }
            }

            if (!string.IsNullOrWhiteSpace(p_VatRateCode))
            {

                if (_vatRateCacheService.IsCacheEmpty())
                {
                    p_product.VatRateID = _VatRates.SingleOrDefault(x => x.VatRateCode == p_VatRateCode).ID;
                }
                else
                {
                    var query = _vatRateCacheService.QueryCache();
                    p_product.VatRateID = query.SingleOrDefault(x => x.VatRateCode == p_VatRateCode).ID;
                }

            }
            else
            {
                p_product.VatRateID = _VatRates.SingleOrDefault(x => x.VatRateCode == bbxBEConsts.VATCODE_27).ID;
            }

            foreach (var pc in p_product.ProductCodes)
            {
                pc.ProductCodeValue = pc.ProductCodeValue.ToUpper();
                pc.ProductID = pc.ID;
            }

            return p_product;
        }

        public async Task<Product> AddProductAsync(Product p_product, string p_ProductGroupCode, string p_OriginCode, string p_VatRateCode)
        {
            using (var dbContextTransaction = _dbContext.Database.BeginTransaction())
            {

                p_product = PrepareNewProduct(p_product, p_ProductGroupCode, p_OriginCode, p_VatRateCode);


                await _Products.AddAsync(p_product);


                //     _dbContext.ChangeTracker.AcceptAllChanges();
                await _dbContext.SaveChangesAsync();

                await dbContextTransaction.CommitAsync();
                _cacheService.AddOrUpdate(p_product);
            }
            return p_product;
        }


        public async Task<int> AddProductRangeAsync(List<Product> p_productList, List<string> p_ProductGroupCodeList, List<string> p_OriginCodeList, List<string> p_VatRateCodeList)
        {
            var item = 0;



            foreach (var prod in p_productList)
            {
                PrepareNewProduct(prod, p_ProductGroupCodeList[item], p_OriginCodeList[item], p_VatRateCodeList[item]);
                _cacheService.AddOrUpdate(prod);
                item++;
            }

            await _dbContext.BulkInsertAsync(p_productList);
            await _dbContext.SaveChangesAsync();
            await RefreshProductCache();

            //TODO: nem frissul a _cacheService a Product.ID-vel

            //TODO: frissiteni a ProductCodes -okat
            RefreshProductCode(_cacheService);

            await _dbContext.BulkInsertAsync(pcList);
            await _dbContext.SaveChangesAsync();



            return item;
        }

        private void RefreshProductCode(ICacheService<Product> prodList)
        {
            foreach (var prod in prodList.QueryCache())
            {
                if (prod.ProductCodes != null)
                {
                    foreach (var pcc in prod.ProductCodes)
                    {
                        if (!pcList.Any(x => x.ProductCodeValue == pcc.ProductCodeValue))
                        {
                            pcc.ProductID = prod.ID;
                            pcList.Add(pcc);
                        }
                    }
                }
            }
        }

        private int PrepareProductPart(List<Product> p_productList, List<string> p_ProductGroupCodeList, List<string> p_OriginCodeList, List<string> p_VatRateCodeList, int item)
        {
            foreach (var prod in p_productList)
            {
                PrepareNewProduct(prod, p_ProductGroupCodeList[item], p_OriginCodeList[item], p_VatRateCodeList[item]);

                _cacheService.AddOrUpdate(prod);
                item++;
            }

            return item;
        }

        private Product PrepareUpdateProduct(Product p_product, string p_ProductGroupCode, string p_OriginCode, string p_VatRateCode)
        {
            var prod = _Products.AsNoTracking()
                        .Include(p => p.ProductCodes).AsNoTracking()
                        .Include(pg => pg.ProductGroup).AsNoTracking()
                        .Include(o => o.Origin).AsNoTracking()
                        .Include(v => v.VatRate).AsNoTracking()
                        .Where(x => x.ID == p_product.ID).FirstOrDefault();

            if (prod != null)
            {

                if (!string.IsNullOrWhiteSpace(p_ProductGroupCode))
                {
                    ProductGroup pg = null;
                    if (_productGroupCacheService.IsCacheEmpty())
                    {

                        pg = _ProductGroups.AsNoTracking().SingleOrDefault(x => x.ProductGroupCode == p_ProductGroupCode);
                    }
                    else
                    {
                        var query = _productGroupCacheService.QueryCache();
                        pg = query.SingleOrDefault(x => x.ProductGroupCode == p_ProductGroupCode);
                    }
                    if (pg != null)
                    {
                        p_product.ProductGroupID = pg.ID;
                        p_product.ProductGroup = pg;
                    }
                }

                if (!string.IsNullOrWhiteSpace(p_OriginCode))
                {
                    Origin origin = null;

                    if (_originCacheService.IsCacheEmpty())
                    {

                        origin = _Origins.AsNoTracking().SingleOrDefault(x => x.OriginCode == p_OriginCode);
                    }
                    else
                    {
                        var query = _originCacheService.QueryCache();
                        origin = query.SingleOrDefault(x => x.OriginCode == p_OriginCode);
                    }
                    if (origin != null)
                    {
                        p_product.OriginID = origin.ID;
                        p_product.Origin = origin;
                    }
                }

                if (!string.IsNullOrWhiteSpace(p_VatRateCode))
                {
                    VatRate vatRate = null;


                    if (_vatRateCacheService.IsCacheEmpty())
                    {

                        vatRate = _VatRates.AsNoTracking().SingleOrDefault(x => x.VatRateCode == p_VatRateCode);
                    }
                    else
                    {
                        var query = _vatRateCacheService.QueryCache();
                        vatRate = query.SingleOrDefault(x => x.VatRateCode == p_VatRateCode);
                    }
                    if (vatRate != null)
                    { }
                    p_product.VatRateID = vatRate.ID;
                    p_product.VatRate = vatRate;
                }

                //egylőre ezt nem cache-oljuk
                //

                var pc = p_product.ProductCodes.SingleOrDefault(x => x.ProductCodeCategory == enCustproductCodeCategory.OWN.ToString());
                if (pc != null)
                {
                    var pcID = prod.ProductCodes.SingleOrDefault(x => x.ProductCodeCategory == enCustproductCodeCategory.OWN.ToString())?.ID;
                    if (pcID != null)
                        pc.ID = pcID.Value;
                }

                var vtsz = p_product.ProductCodes.SingleOrDefault(x => x.ProductCodeCategory == enCustproductCodeCategory.VTSZ.ToString());
                if (vtsz != null)
                {
                    var vtszID = prod.ProductCodes.SingleOrDefault(x => x.ProductCodeCategory == enCustproductCodeCategory.VTSZ.ToString())?.ID;
                    if (vtszID != null)
                        vtsz.ID = vtszID.Value;
                }



                //A changetracking mechanizmus miatt nem a produc-ból kérdezzük ki
                //                    var ean = prod.ProductCodes.SingleOrDefault(x => x.ProductCodeCategory == enCustproductCodeCategory.EAN.ToString());
                var ean = _ProductCodes.SingleOrDefault(x =>
                           x.ProductID == p_product.ID &&
                           x.ProductCodeCategory == enCustproductCodeCategory.EAN.ToString());
                if (ean != null)
                {
                    var eanOrig = p_product.ProductCodes.SingleOrDefault(x => x.ProductCodeCategory == enCustproductCodeCategory.EAN.ToString());
                    if (eanOrig != null)
                        eanOrig.ID = ean.ID;
                    else
                    {

                        _ProductCodes.Remove(ean);
                    }
                }



                foreach (var pcx in p_product.ProductCodes)
                {
                    pcx.ProductCodeValue = pcx.ProductCodeValue.ToUpper();
                }

            }
            else
            {
                throw new ResourceNotFoundException(string.Format(bbxBEConsts.FV_PRODNOTFOUND, p_product.ID));
            }

            return p_product;
        }

        public async Task<Product> UpdateProductAsync(Product p_product, string p_ProductGroupCode, string p_OriginCode, string p_VatRateCode)
        {

            //   var manager = ((IObjectContextAdapter)_dbContext).ObjectContext.ObjectStateManager;

            using (var dbContextTransaction = _dbContext.Database.BeginTransaction())
            {

                p_product = PrepareUpdateProduct(p_product, p_ProductGroupCode, p_OriginCode, p_VatRateCode);

                _cacheService.AddOrUpdate(p_product);


                _Products.Update(p_product);
                await _dbContext.SaveChangesAsync();
                await dbContextTransaction.CommitAsync();


            }
            return p_product;
        }

        public async Task<int> UpdateProductRangeAsync(List<Product> p_productList, List<string> p_ProductGroupCodeList, List<string> p_OriginCodeList, List<string> p_VatRateCodeList)
        {

            //   var manager = ((IObjectContextAdapter)_dbContext).ObjectContext.ObjectStateManager;
            var item = 0;
            //using (var dbContextTransaction = _dbContext.Database.BeginTransaction())
            //{

            foreach (var prod in p_productList)
            {
                PrepareUpdateProduct(prod, p_ProductGroupCodeList[item], p_OriginCodeList[item], p_VatRateCodeList[item]);

                item++;
            }

            await _dbContext.BulkUpdateAsync(p_productList);

            //_Products.UpdateRange(p_productList);
            await _dbContext.SaveChangesAsync();
            //await dbContextTransaction.CommitAsync();

            await RefreshProductCache();

            //}
            return item;
        }

        public async Task<Product> DeleteProductAsync(long ID)
        {

            //   var manager = ((IObjectContextAdapter)_dbContext).ObjectContext.ObjectStateManager;
            Product prod = null;
            using (var dbContextTransaction = _dbContext.Database.BeginTransaction())
            {
                prod = _Products.Include(p => p.ProductCodes).Where(x => x.ID == ID).FirstOrDefault();

                if (prod != null)
                {

                    if (prod.ProductCodes != null)
                    {

                        _ProductCodes.RemoveRange(prod.ProductCodes.ToList());
                    }
                    _cacheService.TryRemove(prod);

                    _Products.Remove(prod);

                    await _dbContext.SaveChangesAsync();
                    await dbContextTransaction.CommitAsync();

                }
                else
                {
                    throw new ResourceNotFoundException(string.Format(bbxBEConsts.FV_PRODNOTFOUND, ID));
                }
            }
            return prod;
        }

        public Entity GetProduct(GetProduct requestParameter)
        {

            var ID = requestParameter.ID;

            Product prod = null;
            if (!_cacheService.TryGetValue(ID, out prod))
                throw new ResourceNotFoundException(string.Format(bbxBEConsts.FV_PRODNOTFOUND, ID));


            var itemModel = _mapper.Map<Product, GetProductViewModel>(prod);
            var listFieldsModel = _modelHelper.GetModelFields<GetProductViewModel>();

            // shape data
            var shapeData = _dataShaperGetProductViewModel.ShapeData(itemModel, String.Join(",", listFieldsModel));

            return shapeData;
        }
        public Entity GetProductByProductCode(GetProductByProductCode requestParameter)
        {

            var query = _cacheService.QueryCache();

            var prod = query.Where(i => i.ProductCodes.Any(c => c.ProductCodeValue.ToUpper() == requestParameter.ProductCode.ToUpper()
                                                                   && c.ProductCodeCategory == enCustproductCodeCategory.OWN.ToString())).FirstOrDefault();

            //            var fields = requestParameter.Fields;
            if (prod != null)
            {
                var itemModel = _mapper.Map<Product, GetProductViewModel>(prod);
                var listFieldsModel = _modelHelper.GetModelFields<GetProductViewModel>();

                // shape data
                var shapeData = _dataShaperGetProductViewModel.ShapeData(itemModel, String.Join(",", listFieldsModel));

                return shapeData;
            }
            else
            {
                return new Entity();
            }
        }
        public async Task<(IEnumerable<Entity> data, RecordsCount recordsCount)> QueryPagedProductAsync(QueryProduct requestParameter)
        {

            var searchString = requestParameter.SearchString;

            var pageNumber = requestParameter.PageNumber;
            var pageSize = requestParameter.PageSize;
            var orderBy = requestParameter.OrderBy;

            int recordsTotal, recordsFiltered;



            var query = _cacheService.QueryCache();

            // Count records total
            recordsTotal = query.Count();

            // filter query
            FilterBySearchString(ref query, searchString);

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
                if (orderBy.ToUpper() == bbxBEConsts.FIELD_PRODUCTCODE)
                {
                    //Kis heka...
                    query = query.OrderBy(o => o.ProductCodes.Single(s =>
                                s.ProductCodeCategory == enCustproductCodeCategory.OWN.ToString()).ProductCodeValue);
                }
                else
                {
                    query = query.OrderBy(orderBy);
                }

            }
            // paging
            query = query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize);

            // retrieve data to list
            var resultData = query.ToList();

            var listFieldsModel = _modelHelper.GetModelFields<GetProductViewModel>();

            //TODO: szebben megoldani
            var resultDataModel = new List<GetProductViewModel>();
            resultData.ForEach(i => resultDataModel.Add(
                _mapper.Map<Product, GetProductViewModel>(i))
            );


            var shapeData = _dataShaperGetProductViewModel.ShapeData(resultDataModel, String.Join(",", listFieldsModel));

            return (shapeData, recordsCount);
        }

        private void FilterBySearchString(ref IQueryable<Product> p_items, string p_searchString)
        {
            if (!p_items.Any())
                return;

            if (string.IsNullOrWhiteSpace(p_searchString))
                return;

            var predicate = PredicateBuilder.New<Product>();

            var srcFor = p_searchString.ToUpper().Trim();

            predicate = predicate.And(p => p.Description.ToUpper().Contains(srcFor) ||
                    p.ProductCodes.Any(a => a.ProductCodeCategory == enCustproductCodeCategory.OWN.ToString() &&
                    a.ProductCodeValue.ToUpper().Contains(srcFor)));

            p_items = p_items.Where(predicate);
        }

        public Task<bool> SeedDataAsync(int rowCount)
        {
            throw new System.NotImplementedException();
        }


        public async Task<List<Product>> GetAllProductsFromDBAsync()
        {
            return await _Products.AsNoTracking()
                 .Include(p => p.ProductCodes).AsNoTracking()
                 .Include(pg => pg.ProductGroup).AsNoTracking()
                 .Include(o => o.Origin).AsNoTracking()
                 .Include(v => v.VatRate).AsNoTracking().ToListAsync();
        }

        public async Task RefreshProductCache()
        {

            if (_cacheService.IsCacheEmpty())
            {

                var q = _Products.AsNoTracking()
                     .Include(p => p.ProductCodes).AsNoTracking()
                     .Include(pg => pg.ProductGroup).AsNoTracking()
                     .Include(o => o.Origin).AsNoTracking()
                     .Include(v => v.VatRate).AsNoTracking();
                await _cacheService.RefreshCache(q);

            }
        }
        public async Task RefreshVatRateCache()
        {

            if (_vatRateCacheService.IsCacheEmpty())
            {

                var q = _VatRates
                .AsNoTracking()
                .AsExpandable();
                await _vatRateCacheService.RefreshCache(q);

            }
        }
    }
}