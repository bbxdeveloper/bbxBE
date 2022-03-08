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
using static bbxBE.Common.NAV.NAV_enums;
using bbxBE.Common;
using bbxBE.Application.Enums;
using bbxBE.Application.Consts;
using bbxBE.Application.Exceptions;

namespace bbxBE.Infrastructure.Persistence.Repositories
{
    public class ProductRepositoryAsync : GenericRepositoryAsync<Product>, IProductRepositoryAsync
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly DbSet<Product> _Products;
        private readonly DbSet<ProductCode> _ProductCodes;
        private readonly DbSet<Origin> _Origins;
        private readonly DbSet<ProductGroup> _ProductGroups;
        private IDataShapeHelper<Product> _dataShaperProduct;
        private IDataShapeHelper<GetProductViewModel> _dataShaperGetProductViewModel;
        private readonly IMockService _mockData;
        private readonly IModelHelper _modelHelper;
        private readonly IMapper _mapper;

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

        public async Task<bool> CheckProductGroupIDAsync(long ProductGroupID)
        {
            return await _ProductGroups.AnyAsync(p => p.ID == ProductGroupID && !p.Deleted);
        }

        public async Task<bool> CheckOriginIDAsync(long OriginID)
        {
            return await _Origins.AnyAsync(p => p.ID == OriginID && !p.Deleted);
        }

        public async Task<Product> AddProductAsync(Product p_product, ProductCode p_productCode, ProductCode p_VTSZ, ProductCode p_EAN)
        {
            using (var dbContextTransaction = _dbContext.Database.BeginTransaction())
            {
                _Products.Add(p_product);
                await _dbContext.SaveChangesAsync();
                p_productCode.ProductID = p_product.ID;
                p_VTSZ.ProductID = p_product.ID;
                _ProductCodes.Add(p_productCode);
                _ProductCodes.Add(p_VTSZ);
                if (p_EAN != null)
                {
                    p_EAN.ProductID = p_product.ID;
                    _ProductCodes.Add(p_EAN);
                }
                await _dbContext.SaveChangesAsync();
                dbContextTransaction.Commit();

            }
            return p_product;
        }
        public async Task<Product> UpdateProductAsync(Product p_product, ProductCode p_productCode, ProductCode p_VTSZ, ProductCode p_EAN)
        {

            //   var manager = ((IObjectContextAdapter)_dbContext).ObjectContext.ObjectStateManager;

          using (var dbContextTransaction = _dbContext.Database.BeginTransaction())
            {

                var prod = _Products.Include(p => p.ProductCodes).Where(x => x.ID == p_product.ID).FirstOrDefault();

                if (prod != null)
                {

                    if (prod.ProductCodes != null)
                    {
                        var pc = prod.ProductCodes.SingleOrDefault( x=> x.ProductCodeCategory == p_productCode.ProductCodeCategory);
                        if (pc != null)
                        {
                            p_productCode.ID = pc.ID;
                            _ProductCodes.Update(p_productCode);
                        }

                        var vtsz = prod.ProductCodes.SingleOrDefault(x => x.ProductCodeCategory == p_VTSZ.ProductCodeCategory);
                        if (vtsz != null)
                        {
                            p_VTSZ.ID = vtsz.ID;
                            _ProductCodes.Update(p_VTSZ);
                        }

                        var ean = prod.ProductCodes.SingleOrDefault(x => x.ProductCodeCategory == p_EAN.ProductCodeCategory);
                        if (ean != null)
                        {
                            if (p_EAN != null)
                            {
                                p_EAN.ID = ean.ID;
                                _ProductCodes.Update(p_EAN);
                            }
                            else
                            {
                                _ProductCodes.Remove(ean);
                            }
                        }
                        else
                        {
                            if (p_EAN != null)
                            {
                                p_EAN.ProductID = p_product.ID;
                                _ProductCodes.Add(p_EAN);
                            }
                        }

                    }


                    _Products.Update(p_product);
                    await _dbContext.SaveChangesAsync();
                    dbContextTransaction.Commit();


                }
                else
                {
                    throw new ResourceNotFoundException(string.Format(bbxBEConsts.FV_PRODNOTFOUND, p_product.ID));
                }
            }
            return p_product;
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
                                  .Include(i => i.ProductCodes)
                                  .Where(i=>i.ID == ID).FirstOrDefaultAsync();

            //            var fields = requestParameter.Fields;

            var itemModel = _mapper.Map<Product, GetProductViewModel>(item.Result);
            var listFieldsModel = _modelHelper.GetModelFields<GetProductViewModel>();

            // shape data
            var shapeData = _dataShaperGetProductViewModel.ShapeData(itemModel, String.Join(",", listFieldsModel));

            return shapeData;
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