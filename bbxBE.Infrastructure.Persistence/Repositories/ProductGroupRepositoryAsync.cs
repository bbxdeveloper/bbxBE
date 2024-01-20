using AutoMapper;
using bbxBE.Application.Helpers;
using bbxBE.Application.Interfaces;
using bbxBE.Application.Interfaces.Repositories;
using bbxBE.Application.Parameters;
using bbxBE.Application.Queries.qProductGroup;
using bbxBE.Application.Queries.ViewModels;
using bbxBE.Common.Consts;
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

namespace bbxBE.Infrastructure.Persistence.Repositories
{
    public class ProductGroupRepositoryAsync : GenericRepositoryAsync<ProductGroup>, IProductGroupRepositoryAsync
    {
        private readonly IApplicationDbContext _dbContext;
        private IDataShapeHelper<ProductGroup> _dataShaperProductGroup;
        private IDataShapeHelper<GetProductGroupViewModel> _dataShaperGetProductGroupViewModel;
        private readonly IMockService _mockData;
        private readonly IModelHelper _modelHelper;
        private readonly IMapper _mapper;
        private readonly ICacheService<ProductGroup> _cacheProductGroup;
        private readonly ICacheService<Product> _cacheProduct;

        public ProductGroupRepositoryAsync(IApplicationDbContext dbContext,
            IModelHelper modelHelper, IMapper mapper, IMockService mockData,
            ICacheService<ProductGroup> cacheProductGroup,
            ICacheService<Product> cacheProduct) : base(dbContext)
        {
            _dbContext = dbContext;
            _dataShaperProductGroup = new DataShapeHelper<ProductGroup>();
            _dataShaperGetProductGroupViewModel = new DataShapeHelper<GetProductGroupViewModel>();
            _modelHelper = modelHelper;
            _mapper = mapper;
            _mockData = mockData;
            _cacheProductGroup = cacheProductGroup;
            _cacheProduct = cacheProduct;
        }


        public bool IsUniqueProductGroupCode(string ProductGroupCode, long? ID = null)
        {

            if (_cacheProductGroup.IsCacheNull())
            {
                return !_dbContext.ProductGroup.AsNoTracking().Any(p => p.ProductGroupCode == ProductGroupCode && !p.Deleted && (ID == null || p.ID != ID.Value)); ;
            }
            else
            {
                var query = _cacheProductGroup.QueryCache();
                return !query.ToList().Any(p => p.ProductGroupCode == ProductGroupCode && !p.Deleted && (ID == null || p.ID != ID.Value));
            }

        }

        public async Task<ProductGroup> AddProudctGroupAsync(ProductGroup p_productGroup)
        {


            await AddAsync(p_productGroup);
            _cacheProductGroup.AddOrUpdate(p_productGroup);
            return p_productGroup;
        }

        public async Task<long> AddProudctGroupRangeAsync(List<ProductGroup> p_productGroupList)
        {


            await AddRangeAsync(p_productGroupList);
            await RefreshProductGroupCache();
            return p_productGroupList.Count;
        }

        public async Task<ProductGroup> UpdateProductGroupAsync(ProductGroup p_productGroup)
        {
            _cacheProductGroup.AddOrUpdate(p_productGroup);

            //Product cache befrissítése
            var pq = _cacheProduct.QueryCache();
            var products = pq.Where(w => w.ProductGroupID == p_productGroup.ID).ToList();
            products.ForEach(i => i.ProductGroup = p_productGroup);



            await UpdateAsync(p_productGroup);
            return p_productGroup;
        }

        public async Task<long> UpdateProductGroupRangeAsync(List<ProductGroup> p_productGroupList)
        {
            await UpdateRangeAsync(p_productGroupList);
            await RefreshProductGroupCache();

            //Product cache befrissítése
            var pq = _cacheProduct.QueryCache();
            var products = pq.Where(w => p_productGroupList.Any(a => a.ID == w.ProductGroupID)).ToList();
            products.ForEach(i => i.ProductGroup = p_productGroupList.FirstOrDefault(f => f.ID == i.ProductGroupID));

            return p_productGroupList.Count;
        }

        public async Task<ProductGroup> DeleteProductGroupAsync(long ID)
        {

            ProductGroup pg = null;
            using (var dbContextTransaction = await _dbContext.Instance.Database.BeginTransactionAsync())
            {
                pg = await _dbContext.ProductGroup.Where(x => x.ID == ID).FirstOrDefaultAsync();

                if (pg != null)
                {

                    _cacheProductGroup.TryRemove(pg);

                    await RemoveAsync(pg);
                    await dbContextTransaction.CommitAsync();

                }
                else
                {
                    throw new ResourceNotFoundException(string.Format(bbxBEConsts.ERR_PRODUCTGROUPNOTFOUND, ID));
                }
            }
            return pg;
        }

        public Entity GetProductGroup(long ID)
        {
            ProductGroup productGroup = null;
            if (!_cacheProductGroup.TryGetValue(ID, out productGroup))
                throw new ResourceNotFoundException(string.Format(bbxBEConsts.ERR_PRODUCTGROUPNOTFOUND, ID));

            //            var fields = requestParameter.Fields;

            var itemModel = _mapper.Map<ProductGroup, GetProductGroupViewModel>(productGroup);
            var listFieldsModel = _modelHelper.GetModelFields<GetProductGroupViewModel>();

            // shape data
            var shapedData = _dataShaperGetProductGroupViewModel.ShapeData(itemModel, String.Join(",", listFieldsModel));

            return shapedData;
        }
        public async Task<(IEnumerable<Entity> data, RecordsCount recordsCount)> QueryPagedProductGroupAsync(QueryProductGroup requestParameter)
        {

            var searchString = requestParameter.SearchString;
            var orderBy = requestParameter.OrderBy;
            //      var fields = requestParameter.Fields;
            var fields = _modelHelper.GetQueryableFields<GetProductGroupViewModel, ProductGroup>();


            int recordsTotal, recordsFiltered;

            // Setup IQueryable
            var query = _cacheProductGroup.QueryCache();


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

            // retrieve data to list
            var resultData = await GetPagedData(query, requestParameter, false);

            //TODO: szebben megoldani
            var resultDataModel = new List<GetProductGroupViewModel>();
            resultData.ForEach(i => resultDataModel.Add(
               _mapper.Map<ProductGroup, GetProductGroupViewModel>(i))
            );


            var listFieldsModel = _modelHelper.GetModelFields<GetProductGroupViewModel>();

            var shapedData = _dataShaperGetProductGroupViewModel.ShapeData(resultDataModel, String.Join(",", listFieldsModel));

            return (shapedData, recordsCount);
        }

        private void FilterBySearchString(ref IQueryable<ProductGroup> p_item, string p_searchString)
        {
            if (!p_item.Any())
                return;

            if (string.IsNullOrWhiteSpace(p_searchString))
                return;

            var predicate = PredicateBuilder.New<ProductGroup>();

            var srcFor = p_searchString.ToUpper().Trim();
            predicate = predicate.And(p => p.ProductGroupCode.ToUpper().Contains(srcFor) ||
                                            p.ProductGroupDescription.ToUpper().Contains(srcFor));

            p_item = p_item.Where(predicate);
        }

        public Task<bool> SeedDataAsync(int rowCount)
        {
            throw new System.NotImplementedException();
        }

        public async Task RefreshProductGroupCache()
        {
            await _cacheProductGroup.RefreshCache();
        }



    }
}