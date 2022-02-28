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


        public async Task<bool> IsUniqueProductCodeAsync(string ProductCode, long? ID = null)
        {
            return !await _ProductCodes.AnyAsync(p => p.ProductCodeCategory == enCustproductCodeCategory.OWN.ToString()
                && p.ProductCodeValue.ToUpper() == ProductCode.ToUpper()
                && !p.Deleted && (ID == null || p.ID != ID.Value));
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
                _Products.AddAsync(p_product);
                await _dbContext.SaveChangesAsync();
                p_productCode.ProductID = p_product.ID;
                p_VTSZ.ProductID = p_product.ID;
                _ProductCodes.AddAsync(p_productCode);
                _ProductCodes.AddAsync(p_VTSZ);
                if (p_EAN != null)
                {
                    p_EAN.ProductID = p_product.ID;
                    _ProductCodes.AddAsync(p_EAN);
                }
                await _dbContext.SaveChangesAsync();
                dbContextTransaction.Commit();

            }
            return p_product;
        }
        public async Task<Product> UpdateProductAsync(Product p_product, ProductCode p_productCode, ProductCode p_VTSZ, ProductCode p_EAN)
        {


            /*
             var myObjectState=myContext.ObjectStateManager.GetObjectStateEntry(myObject);
var modifiedProperties=myObjectState.GetModifiedProperties();
foreach(var propName in modifiedProperties)
{
    Console.WriteLine("Property {0} changed from {1} to {2}", 
         propName,
         myObjectState.OriginalValues[propName],
         myObjectState.CurrentValues[propName]);
}
             */
            //   var manager = ((IObjectContextAdapter)_dbContext).ObjectContext.ObjectStateManager;

            /*
         using (var dbContextTransaction = _dbContext.Database.BeginTransaction())
            {
                _ProductCodes.AddAsync(p_productCode);
                _ProductCodes.AddAsync(p_VTSZ);
                if (p_EAN != null)
                {
                    _ProductCodes.AddAsync(p_EAN);
                }
                _Products.AddAsync(p_product);
                await _dbContext.SaveChangesAsync();
                dbContextTransaction.Commit();

            }
            */
            return p_product;
        }

        public async Task<Entity> GetProductReponseAsync(GetProduct requestParameter)
        {


            var ID = requestParameter.ID;

            var item = await GetByIdAsync(ID);

            //            var fields = requestParameter.Fields;

            var itemModel = _mapper.Map<Product, GetProductViewModel>(item);
            var listFieldsModel = _modelHelper.GetModelFields<GetProductViewModel>();

            // shape data
            var shapeData = _dataShaperGetProductViewModel.ShapeData(itemModel, String.Join(",", listFieldsModel));

            return shapeData;
        }
        public async Task<(IEnumerable<Entity> data, RecordsCount recordsCount)> QueryPagedProductReponseAsync(QueryProduct requestParameter)
        {

            var searchString = requestParameter.SearchString;

            var pageNumber = requestParameter.PageNumber;
            var pageSize = requestParameter.PageSize;
            var orderBy = requestParameter.OrderBy;

            //itt csak a modelt vesszük figyelembe
            var fields = string.Join(",", _modelHelper.GetModelFields<GetProductViewModel>());


            int recordsTotal, recordsFiltered;



            IQueryable<GetProductViewModel> query = from prod in _Products.AsNoTracking().AsExpandable()
                                                    join pco in _ProductCodes.DefaultIfEmpty()
                                                       on prod.ID equals pco.ProductID
                                                    join pcVTSZ in _ProductCodes.DefaultIfEmpty()
                                                        on prod.ID equals pcVTSZ.ProductID
                                                    join pcEAN in _ProductCodes.DefaultIfEmpty()
                                                        on prod.ID equals pcEAN.ProductID
                                                    join pg in _ProductGroups.DefaultIfEmpty()
                                                      on prod.ProductGroupID equals pg.ID
                                                    join og in _Origins.DefaultIfEmpty()
                                                      on prod.OriginID equals og.ID
                                                    where pco.ProductCodeCategory == enCustproductCodeCategory.OWN.ToString() &&
                                                          pcVTSZ.ProductCodeCategory == enCustproductCodeCategory.VTSZ.ToString() &&
                                                          pcEAN.ProductCodeCategory == enCustproductCodeCategory.EAN.ToString()
                                                    select new GetProductViewModel()
                                                    {
                                                        ID = prod.ID,
                                                        ProductCode = pco.ProductCodeValue,
                                                        Description = prod.Description,
                                                        ProductGroup = pg.ProductGroupDescription,
                                                        Origin = og.OriginDescription,
                                                        UnitOfMeasure = prod.UnitOfMeasure,
                                                        UnitPrice1 = prod.UnitPrice1,
                                                        UnitPrice2 = prod.UnitPrice2,
                                                        LatestSupplyPrice = prod.LatestSupplyPrice,
                                                        IsStock = prod.IsStock,
                                                        MinStock = prod.MinStock,
                                                        OrdUnit = prod.OrdUnit,
                                                        ProductFee = prod.ProductFee,
                                                        Active = prod.Active,
                                                        VTSZ = pcVTSZ.ProductCodeValue,
                                                        EAN = pcEAN.ProductCodeValue
                                                    };

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

            // select columns
            if (!string.IsNullOrWhiteSpace(fields))
            {
                query = query.Select<GetProductViewModel>("new(" + fields + ")");
            }
            // paging
            query = query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize);

            // retrieve data to list
            var resultDataModel = await query.ToListAsync();

               var listFieldsModel = _modelHelper.GetModelFields<GetProductViewModel>();

            var shapeData = _dataShaperGetProductViewModel.ShapeData(resultDataModel, String.Join(",", listFieldsModel));

            return (shapeData, recordsCount);
        }

        private void FilterBySearchString(ref IQueryable<GetProductViewModel> p_item, string p_searchString)
        {
            if (!p_item.Any())
                return;

            if (string.IsNullOrWhiteSpace(p_searchString))
                return;

            var predicate = PredicateBuilder.New<GetProductViewModel>();

            var srcFor = p_searchString.ToUpper().Trim();

            predicate = predicate.And(p => p.Description.ToUpper().Contains(srcFor) || 
                    p.ProductCode.ToUpper().Contains(srcFor));

            p_item = p_item.Where(predicate);
        }

        public Task<bool> SeedDataAsync(int rowCount)
        {
            throw new System.NotImplementedException();
        }
    }
}