using AutoMapper;
using bbxBE.Application.Helpers;
using bbxBE.Application.Interfaces;
using bbxBE.Application.Interfaces.Repositories;
using bbxBE.Application.Parameters;
using bbxBE.Application.Queries.qProduct;
using bbxBE.Application.Queries.ViewModels;
using bbxBE.Common.Consts;
using bbxBE.Common.Exceptions;
using bbxBE.Domain.Entities;
using bbxBE.Infrastructure.Persistence.Repository;
using bbxBE.Queries.Mappings;
using bxBE.Application.Commands.cmdProduct;
using EFCore.BulkExtensions;
using LinqKit;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;
using static bbxBE.Common.NAV.NAV_enums;

namespace bbxBE.Infrastructure.Persistence.Repositories
{
    public class ProductRepositoryAsync : GenericRepositoryAsync<Product>, IProductRepositoryAsync
    {
        private readonly IApplicationDbContext _dbContext;
        private IDataShapeHelper<Product> _dataShaperProduct;
        private IDataShapeHelper<GetProductViewModel> _dataShaperGetProductViewModel;
        private readonly IMockService _mockData;
        private readonly IModelHelper _modelHelper;
        private readonly IMapper _mapper;
        private readonly ICacheService<Product> _productcacheService;
        private readonly ICacheService<ProductGroup> _productGroupCacheService;
        private readonly ICacheService<Origin> _originCacheService;
        private readonly ICacheService<VatRate> _vatRateCacheService;

        private readonly IProductCodeRepositoryAsync _productCodeRepository;

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
        public ProductRepositoryAsync(IApplicationDbContext dbContext,
            IModelHelper modelHelper, IMapper mapper, IMockService mockData,
            ICacheService<Product> productCacheService,
            ICacheService<ProductGroup> productGroupCacheService,
            ICacheService<Origin> originCacheService,
            ICacheService<VatRate> vatRateCacheService
            ) : base(dbContext)
        {
            _dbContext = dbContext;
            _dataShaperProduct = new DataShapeHelper<Product>();
            _dataShaperGetProductViewModel = new DataShapeHelper<GetProductViewModel>();
            _modelHelper = modelHelper;
            _mapper = mapper;
            _mockData = mockData;
            _productcacheService = productCacheService;
            _productGroupCacheService = productGroupCacheService;
            _originCacheService = originCacheService;
            _vatRateCacheService = vatRateCacheService;
            _productCodeRepository = new ProductCodeRepositoryAsync(dbContext, modelHelper, mapper, mockData);
        }




        public bool IsUniqueProductCode(string ProductCode, long? ProductID = null)
        {

            /*
            return !await _dbContext.ProductCode.AnyAsync(p => p.ProductCodeCategory == enCustproductCodeCategory.OWN.ToString()
                && p.ProductCodeValue.ToUpper() == ProductCode.ToUpper()
                && !p.Deleted && (ProductID == null || p.ProductID != ProductID.Value));
            */

            var query = _productcacheService.QueryCache();
            return !query.ToList().Any(p => p.ProductCodes.Any(a => a.ProductCodeCategory == enCustproductCodeCategory.OWN.ToString()
               && a.ProductCodeValue.ToUpper() == ProductCode.ToUpper())
                && !p.Deleted && (ProductID == null || p.ID != ProductID.Value));

        }


        public async Task<bool> CheckProductGroupCodeAsync(string ProductGroupCode)
        {
            if (_productGroupCacheService.IsCacheNull())
            {
                return await _dbContext.ProductGroup.AnyAsync(p => p.ProductGroupCode == ProductGroupCode && !p.Deleted);
            }
            else
            {
                var query = _productGroupCacheService.QueryCache();
                return query.ToList().Any(p => p.ProductGroupCode == ProductGroupCode && !p.Deleted);
            }
        }

        public async Task<bool> CheckOriginCodeAsync(string OriginCode)
        {
            if (_originCacheService.IsCacheNull())
            {
                return await _dbContext.Origin.AnyAsync(p => p.OriginCode == OriginCode && !p.Deleted);
            }
            else
            {
                var query = _originCacheService.QueryCache();
                return query.ToList().Any(p => p.OriginCode == OriginCode && !p.Deleted);
            }
        }

        public async Task<bool> CheckVatRateCodeAsync(string VatRateCode)
        {
            if (_vatRateCacheService.IsCacheNull())
            {
                return await _dbContext.VatRate.AnyAsync(p => p.VatRateCode == VatRateCode && !p.Deleted);
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
                ProductGroup pg;
                if (_productGroupCacheService.IsCacheNull())
                {
                    pg = _dbContext.ProductGroup.AsNoTracking().SingleOrDefault(x => x.ProductGroupCode == p_ProductGroupCode);
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
                    _dbContext.Instance.Entry(pg).State = EntityState.Unchanged;

                }
            }

            if (!string.IsNullOrWhiteSpace(p_OriginCode))
            {
                Origin og;
                if (_originCacheService.IsCacheNull())
                {
                    og = _dbContext.Origin.AsNoTracking().SingleOrDefault(x => x.OriginCode == p_OriginCode);
                }
                else
                {
                    var query = _originCacheService.QueryCache();
                    og = query.SingleOrDefault(x => x.OriginCode == p_OriginCode);
                }
                if (og != null)
                {

                    p_product.OriginID = og.ID;
                    p_product.Origin = og;
                    _dbContext.Instance.Entry(og).State = EntityState.Unchanged;
                }
            }

            VatRate vr;
            if (!string.IsNullOrWhiteSpace(p_VatRateCode))
            {
                if (_vatRateCacheService.IsCacheNull())
                {
                    vr = _dbContext.VatRate.AsNoTracking().SingleOrDefault(x => x.VatRateCode == p_VatRateCode);
                }
                else
                {
                    var query = _vatRateCacheService.QueryCache();
                    vr = query.SingleOrDefault(x => x.VatRateCode == p_VatRateCode);
                }

                if (vr == null)
                {
                    vr = _dbContext.VatRate.AsNoTracking().SingleOrDefault(x => x.VatRateCode == bbxBEConsts.VATCODE_27);
                }

            }
            else
            {
                vr = _dbContext.VatRate.AsNoTracking().SingleOrDefault(x => x.VatRateCode == bbxBEConsts.VATCODE_27);
            }

            p_product.VatRateID = vr.ID;
            p_product.VatRate = vr;
            _dbContext.Instance.Entry(vr).State = EntityState.Unchanged;

            foreach (var pc in p_product.ProductCodes)
            {
                pc.ProductCodeValue = pc.ProductCodeValue.ToUpper();
                pc.ProductID = pc.ID;
            }

            return p_product;
        }


        public async Task<Product> AddProductAsync(Product p_product, string p_ProductGroupCode, string p_OriginCode, string p_VatRateCode)
        {
            using (var dbContextTransaction = await _dbContext.Instance.Database.BeginTransactionAsync())
            {
                try
                {
                    p_product = PrepareNewProduct(p_product, p_ProductGroupCode, p_OriginCode, p_VatRateCode);

                    await AddAsync(p_product);
                    await dbContextTransaction.CommitAsync();

                    _productcacheService.AddOrUpdate(p_product);
                }
                catch (Exception)
                {
                    await dbContextTransaction.RollbackAsync();
                    throw;
                }
            }
            return p_product;
        }



        public async Task<int> AddProductRangeAsync(List<Product> p_productList, List<string> p_ProductGroupCodeList, List<string> p_OriginCodeList, List<string> p_VatRateCodeList)
        {
            var item = 0;

            foreach (var prod in p_productList)
            {
                PrepareNewProduct(prod, p_ProductGroupCodeList[item], p_OriginCodeList[item], p_VatRateCodeList[item]);
                item++;
            }

            try
            {
                await _dbContext.Instance.BulkInsertAsync(p_productList, new BulkConfig { SetOutputIdentity = true, PreserveInsertOrder = true, BulkCopyTimeout = 0, WithHoldlock = false, BatchSize = 5000 });
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception e)
            {
                throw e;
            }


            var productCodes = new List<ProductCode>();
            foreach (var product in p_productList)
            {
                foreach (var productCode in product.ProductCodes)
                {
                    productCode.ProductID = product.ID; // sets FK to match its linked PK that was generated in DB
                }
                productCodes.AddRange(product.ProductCodes);
            }
            await _dbContext.Instance.BulkInsertAsync(productCodes);

            await RefreshProductCache();
            await _dbContext.SaveChangesAsync();

            return item;
        }


        private Product PrepareUpdateProduct(Product p_productUpd, string p_ProductGroupCode, string p_OriginCode, string p_VatRateCode)
        {


            if (!string.IsNullOrWhiteSpace(p_ProductGroupCode))
            {
                ProductGroup pg = null;
                if (_productGroupCacheService.IsCacheNull())
                {

                    pg = _dbContext.ProductGroup.AsNoTracking().SingleOrDefault(x => x.ProductGroupCode == p_ProductGroupCode);
                }
                else
                {
                    var query = _productGroupCacheService.QueryCache();
                    pg = query.SingleOrDefault(x => x.ProductGroupCode == p_ProductGroupCode);
                }
                if (pg != null)
                {
                    p_productUpd.ProductGroupID = pg.ID;
                    p_productUpd.ProductGroup = pg;
                }
            }

            if (!string.IsNullOrWhiteSpace(p_OriginCode))
            {
                Origin origin = null;

                if (_originCacheService.IsCacheNull())
                {

                    origin = _dbContext.Origin.AsNoTracking().SingleOrDefault(x => x.OriginCode == p_OriginCode);
                }
                else
                {
                    var query = _originCacheService.QueryCache();
                    origin = query.SingleOrDefault(x => x.OriginCode == p_OriginCode);
                }
                if (origin != null)
                {
                    p_productUpd.OriginID = origin.ID;
                    p_productUpd.Origin = origin;
                }
            }

            if (!string.IsNullOrWhiteSpace(p_VatRateCode))
            {
                VatRate vatRate = null;


                if (_vatRateCacheService.IsCacheNull())
                {

                    vatRate = _dbContext.VatRate.AsNoTracking().SingleOrDefault(x => x.VatRateCode == p_VatRateCode);
                }
                else
                {
                    var query = _vatRateCacheService.QueryCache();
                    vatRate = query.SingleOrDefault(x => x.VatRateCode == p_VatRateCode);
                }
                if (vatRate != null)
                {
                    p_productUpd.VatRateID = vatRate.ID;
                    p_productUpd.VatRate = vatRate;
                }
            }
            return p_productUpd;
        }

        public async Task<Product> UpdateProductAsync(Product p_product, string p_ProductGroupCode, string p_OriginCode, string p_VatRateCode)
        {

            //   var manager = ((IObjectContextAdapter)_dbContext).ObjectContext.ObjectStateManager;

            Product prodOri = null;
            if (!_productcacheService.TryGetValue(p_product.ID, out prodOri))
                throw new ResourceNotFoundException(string.Format(bbxBEConsts.ERR_PRODNOTFOUND, p_product.ID));

            using (var dbContextTransaction = await _dbContext.Instance.Database.BeginTransactionAsync())
            {

                try


                {
                    p_product = PrepareUpdateProduct(p_product, p_ProductGroupCode, p_OriginCode, p_VatRateCode);

                    await _productCodeRepository.MaintainProductCodeListAsync(prodOri.ProductCodes, p_product.ProductCodes);
                    await UpdateAsync(p_product, false);
                    await _dbContext.SaveChangesAsync();
                    await dbContextTransaction.CommitAsync();

                    _productcacheService.AddOrUpdate(p_product);

                }
                catch (Exception)
                {
                    await dbContextTransaction.RollbackAsync();
                    throw;
                }
            }
            return p_product;
        }

        public async Task<int> UpdateProductRangeAsync(List<Product> p_productList, List<string> p_ProductGroupCodeList, List<string> p_OriginCodeList, List<string> p_VatRateCodeList)
        {

            var item = 0;

            foreach (var prod in p_productList)
            {
                PrepareUpdateProduct(prod, p_ProductGroupCodeList[item], p_OriginCodeList[item], p_VatRateCodeList[item]);
                item++;
            }

            try
            {
                _dbContext.Instance.Database.SetCommandTimeout(3600);
                await _dbContext.Instance.BulkUpdateAsync(p_productList, new BulkConfig { SetOutputIdentity = true, PreserveInsertOrder = true, BulkCopyTimeout = 0, WithHoldlock = false, BatchSize = 5000 });
                await _dbContext.SaveChangesAsync();

            }
            catch (Exception e)
            {
                throw e;
            }

            var productCodes = new List<ProductCode>();
            foreach (var product in p_productList)
            {
                foreach (var productCode in product.ProductCodes)
                {
                    productCode.ProductID = product.ID; // sets FK to match its linked PK that was generated in DB
                }
                productCodes.AddRange(product.ProductCodes);
            }
            await _dbContext.Instance.BulkUpdateAsync(productCodes);
            //await RefreshProductCache();

            await _dbContext.SaveChangesAsync();

            return item;
        }
        public async Task<int> UpdateProductRangeAsync(List<Product> p_productList, bool bsaveChanges)
        {

            var item = 0;
            try
            {
                _dbContext.Instance.Database.SetCommandTimeout(3600);
                await _dbContext.Instance.BulkUpdateAsync(p_productList, new BulkConfig { SetOutputIdentity = true, PreserveInsertOrder = true, BulkCopyTimeout = 0, WithHoldlock = false, BatchSize = 5000 });
                if (bsaveChanges)
                {
                    await _dbContext.SaveChangesAsync();
                }

                p_productList.ForEach(p => _productcacheService.AddOrUpdate(p));


            }
            catch (Exception e)
            {
                throw e;
            }
            return item;
        }

        public async Task<Product> DeleteProductAsync(long ID)
        {

            //   var manager = ((IObjectContextAdapter)_dbContext).ObjectContext.ObjectStateManager;
            Product prod = null;
            using (var dbContextTransaction = await _dbContext.Instance.Database.BeginTransactionAsync())
            {
                prod = await _dbContext.Product.Include(p => p.ProductCodes).Where(x => x.ID == ID).FirstOrDefaultAsync();

                if (prod != null)
                {

                    _productcacheService.TryRemove(prod);
                    await RemoveAsync(prod);
                    await dbContextTransaction.CommitAsync();

                }
                else
                {
                    throw new ResourceNotFoundException(string.Format(bbxBEConsts.ERR_PRODNOTFOUND, ID));
                }
            }
            return prod;
        }

        public Product GetProduct(long ID)
        {
            var query = _productcacheService.QueryCache();
            var prod = query.Where(i => i.ID == ID).SingleOrDefault();
            return prod;
        }
        public Entity GetProductEntity(long ID)
        {

            Product prod = null;
            if (!_productcacheService.TryGetValue(ID, out prod))
                throw new ResourceNotFoundException(string.Format(bbxBEConsts.ERR_PRODNOTFOUND, ID));


            var itemModel = _mapper.Map<Product, GetProductViewModel>(prod);
            var listFieldsModel = _modelHelper.GetModelFields<GetProductViewModel>();

            // shape data
            var shapedData = _dataShaperGetProductViewModel.ShapeData(itemModel, String.Join(",", listFieldsModel));

            return shapedData;
        }

        public Product GetProductByProductCode(string productCode)
        {
            productCode = productCode.ToUpper();
            var query = _productcacheService.QueryCache();

            var prod = query.Where(i => i.ProductCodes.Any(c => c.ProductCodeValue.ToUpper() == productCode.ToUpper()
                                                                   && c.ProductCodeCategory == enCustproductCodeCategory.OWN.ToString())).FirstOrDefault();
            return prod;
        }
        public Entity GetProductEntityByProductCode(string productCode)
        {
            var prod = GetProductByProductCode(productCode);

            //            var fields = requestParameter.Fields;
            if (prod != null)
            {
                var itemModel = _mapper.Map<Product, GetProductViewModel>(prod);
                var listFieldsModel = _modelHelper.GetModelFields<GetProductViewModel>();

                // shape data
                var shapedData = _dataShaperGetProductViewModel.ShapeData(itemModel, String.Join(",", listFieldsModel));

                return shapedData;
            }
            else
            {
                return new Entity();
            }
        }
        public async Task<(IEnumerable<Entity> data, RecordsCount recordsCount)> QueryPagedProductAsync(QueryProduct requestParameter)
        {

            var orderBy = requestParameter.OrderBy;

            int recordsTotal, recordsFiltered;

            var query = _productcacheService.QueryCache();

            // Count records total
            recordsTotal = query.Count();

            // filter query
            FilterBySearchString(ref query, requestParameter.SearchString, requestParameter.FilterByCode, requestParameter.FilterByName, requestParameter.IDList);

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
                bool isDesc = orderBy.ToUpper().EndsWith(" DESC");
                if (isDesc)
                {
                    orderBy = orderBy.Substring(0, orderBy.Length - 5);
                }
                if (orderBy.ToUpper() == bbxBEConsts.FIELD_PRODUCTCODE)
                {
                    //Kis heka...
                    query = query.OrderBy(o =>
                            o.ProductCodes != null &&
                            o.ProductCodes.Any(s =>
                                    s.ProductCodeValue != null && s.ProductCodeCategory == enCustproductCodeCategory.OWN.ToString()) ?
                            o.ProductCodes.SingleOrDefault(s =>
                                    s.ProductCodeValue != null && s.ProductCodeCategory == enCustproductCodeCategory.OWN.ToString()).ProductCodeValue :
                           String.Empty
                        );

                }
                else if (orderBy.ToUpper() == bbxBEConsts.FIELD_PRODUCTGROUP)
                {
                    query = query.OrderBy(o => o.ProductGroup != null ? o.ProductGroup.ProductGroupCode : "");
                }
                else if (orderBy.ToUpper() == bbxBEConsts.FIELD_ORIGIN)
                {
                    query = query.OrderBy(o => o.Origin != null ? o.Origin.OriginCode : "");
                }
                else if (orderBy.ToUpper() == bbxBEConsts.FIELD_UNITOFMEASUREX)
                {
                    query = query.OrderBy(o => MapQueries.enUnitOfMeasureNameResolver(o.UnitOfMeasure));
                }
                else
                {
                    query = query.OrderBy(orderBy);
                }

                if (isDesc)
                {
                    query = query.Reverse();
                }
            }
            else
            {
                if (requestParameter.FilterByCode.HasValue && requestParameter.FilterByCode.Value)
                {
                    query = query.OrderBy(o =>
                            o.ProductCodes != null &&
                            o.ProductCodes.Any(s =>
                                    s.ProductCodeValue != null && s.ProductCodeCategory == enCustproductCodeCategory.OWN.ToString()) ?
                            o.ProductCodes.SingleOrDefault(s =>
                                    s.ProductCodeValue != null && s.ProductCodeCategory == enCustproductCodeCategory.OWN.ToString()).ProductCodeValue :
                           String.Empty
                        );
                }
                else if (requestParameter.FilterByName.HasValue && requestParameter.FilterByName.Value)
                {
                    query = query.OrderBy(o => o.Description);
                }

            }


            // retrieve data to list
            var resultData = await GetPagedData(query, requestParameter, false);

            var listFieldsModel = _modelHelper.GetModelFields<GetProductViewModel>();

            //TODO: szebben megoldani
            var resultDataModel = new List<GetProductViewModel>();
            resultData.ForEach(i => resultDataModel.Add(
                _mapper.Map<Product, GetProductViewModel>(i))
            );


            var shapedData = _dataShaperGetProductViewModel.ShapeData(resultDataModel, String.Join(",", listFieldsModel));

            return (shapedData, recordsCount);
        }

        private void FilterBySearchString(ref IQueryable<Product> p_items, string p_searchString, bool? p_filterByCode, bool? p_filterByName, IList<long> p_IDList)
        {
            if (!p_items.Any())
                return;


            var predicate = PredicateBuilder.New<Product>(i => true);

            var srcFor = p_searchString?.ToUpper().Trim();




            //Ha kódban és névben egyszerre kerseünk akkor kód/név részletre keresünk
            if (p_filterByName.HasValue && p_filterByCode.HasValue &&
            p_filterByName.Value && p_filterByCode.Value)
            {
                if (srcFor != null)
                {
                    predicate = predicate.And(p => (!p_filterByName.Value || p.Description.ToUpper().StartsWith(srcFor))
                        ||
                        (!p_filterByCode.Value || p.ProductCodes.Any(a => a.ProductCodeCategory == enCustproductCodeCategory.OWN.ToString() &&
                        a.ProductCodeValue.ToUpper().StartsWith(srcFor)))
                        );
                }
                else
                {
                    predicate = predicate.And(p => true);
                }

            }

            //csak kódra keresés, kódkezdetre keresünk
            else if (p_filterByCode.HasValue && p_filterByCode.Value)
            {
                if (srcFor != null)
                {
                    predicate = predicate.And(p => (p.ProductCodes.Any(a => a.ProductCodeCategory == enCustproductCodeCategory.OWN.ToString() &&
                                    a.ProductCodeValue.ToUpper().StartsWith(srcFor)))
                    );
                }
                else
                {
                    predicate = predicate.And(p => true);
                }
            }

            //csak névre keresés, névezdetre keresünk
            else if (p_filterByName.HasValue && p_filterByName.Value)
            {
                if (srcFor != null)
                {
                    predicate = predicate.And(p => (p.Description.ToUpper().StartsWith(srcFor)));
                }
                else
                {
                    predicate = predicate.And(p => true);
                }
            }


            if (p_IDList != null && p_IDList.Count > 0)
            {
                predicate = predicate.And(p => p_IDList.Contains(p.ID));
            }

            p_items = p_items.Where(predicate);

        }

        public Task<bool> SeedDataAsync(int rowCount)
        {
            throw new System.NotImplementedException();
        }

        public List<GetProductViewModel> GetAllProductsFromCache()
        {
            var resultData = _productcacheService.QueryCache().ToList();

            //TODO: szebben megoldani
            var resultDataModel = new List<GetProductViewModel>();
            resultData.ForEach(i => resultDataModel.Add(
                _mapper.Map<Product, GetProductViewModel>(i))
            );
            return resultDataModel;
        }


        public async Task<List<Product>> GetAllProductsFromDBAsync()
        {


            return await _dbContext.Product.AsNoTracking()
                 .Include(p => p.ProductCodes).AsNoTracking()
                 .Include(pg => pg.ProductGroup).AsNoTracking()
                 .Include(o => o.Origin).AsNoTracking()
                 .Include(v => v.VatRate).AsNoTracking()
                 .Include(v => v.Stocks).AsNoTracking()
                 .Include(v => v.Stocks).ThenInclude(w => w.Warehouse).AsNoTracking()
                 .ToListAsync();
        }


        public async Task RefreshProductCache()
        {
            await _productcacheService.RefreshCache();
        }

        public async Task<Product> CreateAsynch(CreateProductCommand request, CancellationToken cancellationToken)
        {
            var prod = _mapper.Map<Product>(request);

            prod.NatureIndicator = enCustlineNatureIndicatorType.PRODUCT.ToString();

            prod.ProductCodes = new List<ProductCode>();

            var pcCode = new ProductCode() { ProductCodeCategory = enCustproductCodeCategory.OWN.ToString(), ProductCodeValue = request.ProductCode };
            prod.ProductCodes.Add(pcCode);
            var pcVTSZ = new ProductCode() { ProductCodeCategory = enCustproductCodeCategory.VTSZ.ToString(), ProductCodeValue = request.VTSZ };
            prod.ProductCodes.Add(pcVTSZ);
            ProductCode pcEAN = null;
            if (!string.IsNullOrWhiteSpace(request.EAN))
            {
                pcEAN = new ProductCode() { ProductCodeCategory = enCustproductCodeCategory.EAN.ToString(), ProductCodeValue = request.EAN };
                prod.ProductCodes.Add(pcEAN);
            }

            prod = await this.AddProductAsync(prod, request.ProductGroupCode, request.OriginCode, request.VatRateCode);
            return prod;
        }

        public async Task<int> CreateRangeAsynch(List<CreateProductCommand> requestList, CancellationToken cancellationToken)
        {
            var prodList = new List<Product>();
            var productGroupCodeList = new List<string>();
            var originCodeList = new List<string>();
            var vatRateCodeList = new List<string>();
            foreach (var request in requestList)
            {
                var prod = _mapper.Map<Product>(request);
                prod.CreateTime = DateTime.UtcNow;
                prod.NatureIndicator = enCustlineNatureIndicatorType.PRODUCT.ToString();

                prod.ProductCodes = new List<ProductCode>();

                var pcCode = new ProductCode() { ProductCodeCategory = enCustproductCodeCategory.OWN.ToString(), ProductCodeValue = request.ProductCode };
                prod.ProductCodes.Add(pcCode);
                var pcVTSZ = new ProductCode() { ProductCodeCategory = enCustproductCodeCategory.VTSZ.ToString(), ProductCodeValue = request.VTSZ };
                prod.ProductCodes.Add(pcVTSZ);
                ProductCode pcEAN = null;
                if (!string.IsNullOrWhiteSpace(request.EAN))
                {
                    pcEAN = new ProductCode() { ProductCodeCategory = enCustproductCodeCategory.EAN.ToString(), ProductCodeValue = request.EAN };
                    prod.ProductCodes.Add(pcEAN);
                };
                prodList.Add(prod);
                productGroupCodeList.Add(request.ProductGroupCode);
                originCodeList.Add(request.OriginCode);
                vatRateCodeList.Add(request.VatRateCode);
            }
            return await this.AddProductRangeAsync(prodList, productGroupCodeList, originCodeList, vatRateCodeList);

        }

        public async Task<Product> UpdateAsynch(UpdateProductCommand request, CancellationToken cancellationToken)
        {


            var prod = _mapper.Map<Product>(request);
            prod.NatureIndicator = enCustlineNatureIndicatorType.PRODUCT.ToString();
            var pcCode = new ProductCode() { ProductID = prod.ID, ProductCodeCategory = enCustproductCodeCategory.OWN.ToString(), ProductCodeValue = request.ProductCode };
            prod.ProductCodes = new List<ProductCode>();
            prod.ProductCodes.Add(pcCode);
            var pcVTSZ = new ProductCode() { ProductID = prod.ID, ProductCodeCategory = enCustproductCodeCategory.VTSZ.ToString(), ProductCodeValue = request.VTSZ };
            prod.ProductCodes.Add(pcVTSZ);

            ProductCode pcEAN = null;
            if (!string.IsNullOrWhiteSpace(request.EAN))
            {
                pcEAN = new ProductCode() { ProductID = prod.ID, ProductCodeCategory = enCustproductCodeCategory.EAN.ToString(), ProductCodeValue = request.EAN };
                prod.ProductCodes.Add(pcEAN);
            }
            return await this.UpdateProductAsync(prod, request.ProductGroupCode, request.OriginCode, request.VatRateCode);
        }


        public async Task<int> UpdateRangeAsynch(List<UpdateProductCommand> requestList, CancellationToken cancellationToken)
        {
            var prodList = new List<Product>();
            var productGroupCodeList = new List<string>();
            var originCodeList = new List<string>();
            var vatRateCodeList = new List<string>();
            foreach (var request in requestList)
            {
                var prod = _mapper.Map<Product>(request);
                prod.UpdateTime = DateTime.UtcNow;

                prod.NatureIndicator = enCustlineNatureIndicatorType.PRODUCT.ToString();
                var pcCode = new ProductCode() { ProductID = prod.ID, ProductCodeCategory = enCustproductCodeCategory.OWN.ToString(), ProductCodeValue = request.ProductCode };
                prod.ProductCodes = new List<ProductCode>();
                prod.ProductCodes.Add(pcCode);
                var pcVTSZ = new ProductCode() { ProductID = prod.ID, ProductCodeCategory = enCustproductCodeCategory.VTSZ.ToString(), ProductCodeValue = request.VTSZ };
                prod.ProductCodes.Add(pcVTSZ);

                ProductCode pcEAN = null;
                if (!string.IsNullOrWhiteSpace(request.EAN))
                {
                    pcEAN = new ProductCode() { ProductID = prod.ID, ProductCodeCategory = enCustproductCodeCategory.EAN.ToString(), ProductCodeValue = request.EAN };
                    prod.ProductCodes.Add(pcEAN);
                }

                prodList.Add(prod);
                productGroupCodeList.Add(request.ProductGroupCode);
                originCodeList.Add(request.OriginCode);
                vatRateCodeList.Add(request.VatRateCode);
            }
            return await this.UpdateProductRangeAsync(prodList, productGroupCodeList, originCodeList, vatRateCodeList);
        }

    }
}