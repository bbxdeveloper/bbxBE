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
using bbxBE.Application.Queries.qProductGroup;
using bbxBE.Application.Queries.ViewModels;
using bbxBE.Application.Exceptions;
using bbxBE.Application.Consts;

namespace bbxBE.Infrastructure.Persistence.Repositories
{
    public class ProductGroupRepositoryAsync : GenericRepositoryAsync<ProductGroup>, IProductGroupRepositoryAsync
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly DbSet<ProductGroup> _productGroups;
        private IDataShapeHelper<ProductGroup> _dataShaperProductGroup;
        private IDataShapeHelper<GetProductGroupViewModel> _dataShaperGetProductGroupViewModel;
        private readonly IMockService _mockData;
        private readonly IModelHelper _modelHelper;
        private readonly IMapper _mapper;
        private readonly ICacheService<ProductGroup> _cacheService;
        private readonly ICacheService<Product> _productCacheService;

        public ProductGroupRepositoryAsync(ApplicationDbContext dbContext,
            IDataShapeHelper<ProductGroup> dataShaperProductGroup,
            IDataShapeHelper<GetProductGroupViewModel> dataShaperGetProductGroupViewModel,
            IModelHelper modelHelper, IMapper mapper, IMockService mockData,
            ICacheService<ProductGroup> productGroupCacheService,
            ICacheService<Product> productCacheService) : base(dbContext)
        {
            _dbContext = dbContext;
            _productGroups = dbContext.Set<ProductGroup>();
            _dataShaperProductGroup = dataShaperProductGroup;
            _dataShaperGetProductGroupViewModel = dataShaperGetProductGroupViewModel;
            _modelHelper = modelHelper;
            _mapper = mapper;
            _mockData = mockData;
            _cacheService = productGroupCacheService;
            _productCacheService = productCacheService;

            var t = RefreshProductGroupCache();
            t.GetAwaiter().GetResult();
        }


        public async Task<bool> IsUniqueProductGroupCodeAsync(string ProductGroupCode, long? ID = null)
        {
            return !await _productGroups.AnyAsync(p => p.ProductGroupCode == ProductGroupCode && !p.Deleted && (ID == null || p.ID != ID.Value));
        }

        public async Task<ProductGroup> AddProudctGroupAsync(ProductGroup p_productGroup)
        {
            using (var dbContextTransaction = _dbContext.Database.BeginTransaction())
            {


                await _productGroups.AddAsync(p_productGroup);
                await _dbContext.SaveChangesAsync();

                await dbContextTransaction.CommitAsync();
                _cacheService.AddOrUpdate(p_productGroup);
            }
            return p_productGroup;
        }
        public async Task<ProductGroup> UpdateProductGroupAsync(ProductGroup p_productGroup)
        {

            using (var dbContextTransaction = _dbContext.Database.BeginTransaction())
            {

                _cacheService.AddOrUpdate(p_productGroup);

                _productGroups.Update(p_productGroup);
                await _dbContext.SaveChangesAsync();
                await dbContextTransaction.CommitAsync();
            }
            return p_productGroup;
        }

        public async Task<ProductGroup> DeleteProductGroupAsync(long ID)
        {

            ProductGroup pg = null;
            using (var dbContextTransaction = _dbContext.Database.BeginTransaction())
            {
                pg = _productGroups.Where(x => x.ID == ID).FirstOrDefault();

                if (pg != null)
                {


                    _cacheService.TryRemove(pg);

                    _productGroups.Remove(pg);

                    await _dbContext.SaveChangesAsync();
                    await dbContextTransaction.CommitAsync();

                }
                else
                {
                    throw new ResourceNotFoundException(string.Format(bbxBEConsts.FV_PRODUCTGROUPNOTFOUND, ID));
                }
            }
            return pg;
        }

        public Entity GetProductGroup(GetProductGroup requestParameter)
        {


            var ID = requestParameter.ID;
            ProductGroup productGroup = null;
            if (!_cacheService.TryGetValue(ID, out productGroup))
                throw new ResourceNotFoundException(string.Format(bbxBEConsts.FV_PRODUCTGROUPNOTFOUND, ID));

            //            var fields = requestParameter.Fields;

            var itemModel = _mapper.Map<ProductGroup, GetProductGroupViewModel>(productGroup);
            var listFieldsModel = _modelHelper.GetModelFields<GetProductGroupViewModel>();

            // shape data
            var shapeData = _dataShaperGetProductGroupViewModel.ShapeData(itemModel, String.Join(",", listFieldsModel));

            return shapeData;
        }
        public async Task<(IEnumerable<Entity> data, RecordsCount recordsCount)> QueryPagedProductGroupAsync(QueryProductGroup requestParameter)
        {

            var searchString = requestParameter.SearchString;

            var pageNumber = requestParameter.PageNumber;
            var pageSize = requestParameter.PageSize;
            var orderBy = requestParameter.OrderBy;
            //      var fields = requestParameter.Fields;
            var fields = _modelHelper.GetQueryableFields<GetProductGroupViewModel, ProductGroup>();


            int recordsTotal, recordsFiltered;

            // Setup IQueryable
            var query = _cacheService.QueryCache();


            // Count records total
            recordsTotal = query.Count();

            // filter data
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
                query = query.OrderBy(orderBy);
            }

            // select columns
            if (!string.IsNullOrWhiteSpace(fields))
            {
                query = query.Select<ProductGroup>("new(" + fields + ")");
            }
            // paging
            query = query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize);

            // retrieve data to list
            var resultData =  query.ToList();

            //TODO: szebben megoldani
            var resultDataModel = new List<GetProductGroupViewModel>();
            resultData.ForEach(i => resultDataModel.Add(
               _mapper.Map<ProductGroup, GetProductGroupViewModel>(i))
            );


            var listFieldsModel = _modelHelper.GetModelFields<GetProductGroupViewModel>();

            var shapeData = _dataShaperGetProductGroupViewModel.ShapeData(resultDataModel, String.Join(",", listFieldsModel));

            return (shapeData, recordsCount);
        }

        private void FilterBySearchString(ref IQueryable<ProductGroup> p_item, string p_searchString)
        {
            if (!p_item.Any())
                return;

            if (string.IsNullOrWhiteSpace(p_searchString))
                return;

            var predicate = PredicateBuilder.New<ProductGroup>();

            var srcFor = p_searchString.ToUpper().Trim();
            predicate = predicate.And(p => p.ProductGroupCode.ToUpper().Contains(srcFor)||
                                            p.ProductGroupDescription.ToUpper().Contains(srcFor));

            p_item = p_item.Where(predicate);
        }

        public Task<bool> SeedDataAsync(int rowCount)
        {
            throw new System.NotImplementedException();
        }

        public async Task RefreshProductGroupCache()
        {
            if (_cacheService.IsCacheEmpty())
            {
                var q = _productGroups
                .AsNoTracking()
                .AsExpandable();
                await _cacheService.RefreshCache(q);

            }

        }

    }
}