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
using bbxBE.Application.Queries.qProduct;
using bbxBE.Application.Queries.ViewModels;
using bbxBE.Common;
using bbxBE.Application.Consts;
using bbxBE.Application.Exceptions;
using static bbxBE.Common.NAV.NAV_enums;

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
            IModelHelper modelHelper, IMapper mapper, IMockService mockData) : base(dbContext)
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


        }


        public async Task<bool> IsUniqueProductCodeAsync(string ProductCode, long? ProductID = null)
        {
            return !await _ProductCodes.AnyAsync(p => p.ProductCodeCategory == enCustproductCodeCategory.OWN.ToString()
                && p.ProductCodeValue.ToUpper() == ProductCode.ToUpper()
                && !p.Deleted && (ProductID == null || p.ProductID != ProductID.Value));
        }

        public async Task<bool> CheckProductGroupCodeAsync(string ProductGroupCode)
        {
            return await _ProductGroups.AnyAsync(p => p.ProductGroupCode == ProductGroupCode && !p.Deleted);
        }

        public async Task<bool> CheckOriginCodeAsync(string OriginCode)
        {
            return await _Origins.AnyAsync(p => p.OriginCode == OriginCode && !p.Deleted);
        }

        private Product PrepareNewProduct(Product p_product, string p_ProductGroupCode, string p_OriginCode, string p_VatRateCode)
        {
            if (!string.IsNullOrWhiteSpace(p_ProductGroupCode))
            {
                p_product.ProductGroupID = _ProductGroups.SingleOrDefault(x => x.ProductGroupCode == p_ProductGroupCode)?.ID;
            }

            if (!string.IsNullOrWhiteSpace(p_OriginCode))
            {
                p_product.OriginID = _Origins.SingleOrDefault(x => x.OriginCode == p_OriginCode)?.ID;
            }

            if (!string.IsNullOrWhiteSpace(p_VatRateCode))
            {
                p_product.VatRateID = _VatRates.SingleOrDefault(x => x.VatRateCode == p_VatRateCode).ID;
            }
            else
            {
                p_product.VatRateID = _VatRates.SingleOrDefault(x => x.VatRateCode == bbxBEConsts.VATCODE_27).ID;
            }
            return p_product;
        }

        public async Task<Product> AddProductAsync(Product p_product, string p_ProductGroupCode, string p_OriginCode, string p_VatRateCode)
        {
            using (var dbContextTransaction = _dbContext.Database.BeginTransaction())
            {

                p_product = PrepareNewProduct(p_product, p_ProductGroupCode, p_OriginCode, p_VatRateCode);

                await _Products.AddAsync(p_product);
                _dbContext.ChangeTracker.AcceptAllChanges();
                await _dbContext.SaveChangesAsync();

                await dbContextTransaction.CommitAsync();

            }
            return p_product;
        }


        public async Task<int> AddProductRangeAsync(List<Product> p_productList, List<string> p_ProductGroupCodeList, List<string> p_OriginCodeList, List<string> p_VatRateCodeList)
        {
            var item = 0;
            using (var dbContextTransaction = _dbContext.Database.BeginTransaction())
            {
                foreach (var prod in p_productList)
                {
                    PrepareNewProduct(prod, p_ProductGroupCodeList[item], p_OriginCodeList[item], p_VatRateCodeList[item]);

                    item++;
                }

                await _Products.AddRangeAsync(p_productList);
                await _dbContext.SaveChangesAsync();

                await dbContextTransaction.CommitAsync();

            }
            return item;
        }


        private Product PrepareUpdateProduct(Product p_product, string p_ProductGroupCode, string p_OriginCode, string p_VatRateCode)
        {
            var prod = _Products.AsNoTracking().Include(p => p.ProductCodes).Where(x => x.ID == p_product.ID).FirstOrDefault();

            if (prod != null)
            {
                if (!string.IsNullOrWhiteSpace(p_ProductGroupCode))
                {
                    p_product.ProductGroupID = _ProductGroups.AsNoTracking().SingleOrDefault(x => x.ProductGroupCode == p_ProductGroupCode)?.ID;
                }

                if (!string.IsNullOrWhiteSpace(p_OriginCode))
                {
                    p_product.OriginID = _Origins.AsNoTracking().SingleOrDefault(x => x.OriginCode == p_OriginCode)?.ID;
                }

                if (!string.IsNullOrWhiteSpace(p_VatRateCode))
                {
                    p_product.VatRateID = _VatRates.AsNoTracking().SingleOrDefault(x => x.VatRateCode == p_VatRateCode).ID;
                }

                if (prod.ProductCodes != null)
                {

                    var pc = prod.ProductCodes.SingleOrDefault(x => x.ProductCodeCategory == enCustproductCodeCategory.OWN.ToString());
                    if (pc != null)
                    {
                        p_product.ProductCodes.SingleOrDefault(x => x.ProductCodeCategory == enCustproductCodeCategory.OWN.ToString()).ID = pc.ID;
                    }

                    var vtsz = prod.ProductCodes.SingleOrDefault(x => x.ProductCodeCategory == enCustproductCodeCategory.VTSZ.ToString());
                    if (vtsz != null)
                    {
                        p_product.ProductCodes.SingleOrDefault(x => x.ProductCodeCategory == enCustproductCodeCategory.VTSZ.ToString()).ID = vtsz.ID;
                    }

                    var ean = prod.ProductCodes.SingleOrDefault(x => x.ProductCodeCategory == enCustproductCodeCategory.EAN.ToString());
                    if (ean != null)
                    {
                        var e = p_product.ProductCodes.SingleOrDefault(x => x.ProductCodeCategory == enCustproductCodeCategory.EAN.ToString());
                        if (e != null)
                            e.ID = ean.ID;
                        else
                            _ProductCodes.Remove(ean);

                    }
          
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


                _Products.Update(p_product);
                await _dbContext.SaveChangesAsync();
                dbContextTransaction.Commit();


            }
            return p_product;
        }

        public async Task<int> UpdateProductRangeAsync(List<Product> p_productList, List<string> p_ProductGroupCodeList, List<string> p_OriginCodeList, List<string> p_VatRateCodeList)
        {

            //   var manager = ((IObjectContextAdapter)_dbContext).ObjectContext.ObjectStateManager;
            var item = 0;
            using (var dbContextTransaction = _dbContext.Database.BeginTransaction())
            {

                foreach (var prod in p_productList)
                {
                    PrepareUpdateProduct(prod, p_ProductGroupCodeList[item], p_OriginCodeList[item], p_VatRateCodeList[item]);
                    item++;
                }
                _dbContext.ChangeTracker.AcceptAllChanges();

                _Products.UpdateRange(p_productList);
                await _dbContext.SaveChangesAsync();
                dbContextTransaction.Commit();


            }
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

                   if (prod.ProductCodes !=null)
                    {

                        _ProductCodes.RemoveRange(prod.ProductCodes.ToList());
                    }
                    _Products.Remove(prod);

                    await _dbContext.SaveChangesAsync();
                    dbContextTransaction.Commit();

                }
                else
                {
                    throw new ResourceNotFoundException(string.Format(bbxBEConsts.FV_PRODNOTFOUND, ID));
                }


            }
            return prod;
        }

        public async Task<Entity> GetProductAsync(GetProduct requestParameter)
        {

            var ID = requestParameter.ID;

            var item = _Products//.AsNoTracking().AsExpandable()
                                  .Include(i => i.Origin)
                                  .Include(i => i.ProductGroup)
                                   .Include(i => i.VatRate)
                                  .Include(i => i.ProductCodes)
                                  .Where(i => i.ID == ID).FirstOrDefaultAsync();

            //            var fields = requestParameter.Fields;

            var itemModel = _mapper.Map<Product, GetProductViewModel>(item.Result);
            var listFieldsModel = _modelHelper.GetModelFields<GetProductViewModel>();

            // shape data
            var shapeData = _dataShaperGetProductViewModel.ShapeData(itemModel, String.Join(",", listFieldsModel));

            return shapeData;
        }
        public async Task<Entity> GetProductByProductCodeAsync(GetProductByProductCode requestParameter)
        {

            var item = _Products//.AsNoTracking().AsExpandable()
                                  .Include(i => i.Origin)
                                  .Include(i => i.ProductGroup)
                                  .Include(i => i.ProductCodes)
                                  .Where(i => i.ProductCodes.Any( c => c.ProductCodeValue.ToUpper() == requestParameter.ProductCode.ToUpper()
                                                                    && c.ProductCodeCategory == enCustproductCodeCategory.OWN.ToString())).FirstOrDefaultAsync();

            //            var fields = requestParameter.Fields;
            if (item.Result != null)
            {
                var itemModel = _mapper.Map<Product, GetProductViewModel>(item.Result);
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

 
            var query = _Products//.AsNoTracking().AsExpandable()
                                .Include(i => i.Origin)
                                .Include(i => i.ProductGroup)
                                .Include(i => i.VatRate)
                               .Include(i => i.ProductCodes).AsQueryable();

            // Count records total
            recordsTotal = await query.CountAsync();

            // filter query
            FilterBySearchString(ref query, searchString);

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
                query = query.OrderBy(orderBy);
            }


            // paging
            query = query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize);

                // retrieve data to list
                var resultData = await query.ToListAsync();

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
                    p.ProductCodes.Any( a=>a.ProductCodeCategory == enCustproductCodeCategory.OWN.ToString() &&
                     a.ProductCodeValue.ToUpper().Contains(srcFor)));

            p_items = p_items.Where(predicate);
        }

        public Task<bool> SeedDataAsync(int rowCount)
        {
            throw new System.NotImplementedException();
        }


    }
}