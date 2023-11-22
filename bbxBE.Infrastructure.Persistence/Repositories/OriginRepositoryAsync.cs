using AutoMapper;
using bbxBE.Application.Helpers;
using bbxBE.Application.Interfaces;
using bbxBE.Application.Interfaces.Repositories;
using bbxBE.Application.Parameters;
using bbxBE.Application.Queries.qOrigin;
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
    public class OriginRepositoryAsync : GenericRepositoryAsync<Origin>, IOriginRepositoryAsync
    {
        private readonly IApplicationDbContext _dbContext;
        private IDataShapeHelper<Origin> _dataShaperOrigin;
        private IDataShapeHelper<GetOriginViewModel> _dataShaperGetOriginViewModel;
        private readonly IMockService _mockData;
        private readonly IModelHelper _modelHelper;
        private readonly IMapper _mapper;
        private readonly ICacheService<Origin> _cacheService;
        private readonly ICacheService<Product> _productCacheService;


        public OriginRepositoryAsync(IApplicationDbContext dbContext,
            IModelHelper modelHelper, IMapper mapper, IMockService mockData,
            ICacheService<Origin> originGroupCacheService,
            ICacheService<Product> productCacheService) : base(dbContext)
        {
            _dbContext = dbContext;
            _dataShaperOrigin = new DataShapeHelper<Origin>();
            _dataShaperGetOriginViewModel = new DataShapeHelper<GetOriginViewModel>();
            _modelHelper = modelHelper;
            _mapper = mapper;
            _mockData = mockData;
            _cacheService = originGroupCacheService;
            _productCacheService = productCacheService;
        }


        public bool IsUniqueOriginCode(string OriginCode, long? ID = null)
        {
            if (_cacheService.IsCacheNull())
            {
                return !_dbContext.Origin.Any(p => p.OriginCode == OriginCode && !p.Deleted && (ID == null || p.ID != ID.Value));
            }
            else
            {
                var query = _cacheService.QueryCache();
                return !query.ToList().Any(p => p.OriginCode == OriginCode && !p.Deleted && (ID == null || p.ID != ID.Value));
            }
        }

        public async Task<Origin> AddOriginAsync(Origin p_origin)
        {

            await AddAsync(p_origin);

            _cacheService.AddOrUpdate(p_origin);
            return p_origin;
        }

        public async Task<long> AddOriginRangeAsync(List<Origin> p_originList)
        {
            using (var dbContextTransaction = await _dbContext.Instance.Database.BeginTransactionAsync())
            {


                await AddRangeAsync(p_originList);

                await dbContextTransaction.CommitAsync();
            }

            await RefreshOriginCache();
            return p_originList.Count();
        }

        public async Task<Origin> UpdateOriginAsync(Origin p_origin)
        {
            await UpdateAsync(p_origin);
            //                await dbContextTransaction.CommitAsync();

            _cacheService.AddOrUpdate(p_origin);

            //Product cache aktualizálás (ha fel van töltve)
            if (!_productCacheService.IsCacheNull())
            {
                foreach (var prod in _productCacheService.QueryCache())
                {
                    if (prod.OriginID == p_origin.ID)
                    {
                        prod.Origin = p_origin;
                    }
                }
            }
            return p_origin;
        }

        public async Task<long> UpdateOriginRangeAsync(List<Origin> p_originList)
        {
            await UpdateRangeAsync(p_originList);
            //                await dbContextTransaction.CommitAsync();

            //await RefreshOriginCache();

            //Product cache aktualizálás (ha fel van töltve)
            if (!_productCacheService.IsCacheNull())
            {
                await _productCacheService.RefreshCache();
            }
            return p_originList.Count();
        }

        public async Task<Origin> DeleteOriginAsync(long ID)
        {

            Origin origin = null;
            using (var dbContextTransaction = await _dbContext.Instance.Database.BeginTransactionAsync())
            {
                origin = _dbContext.Origin.Where(x => x.ID == ID).FirstOrDefault();

                if (origin != null)
                {

                    await RemoveAsync(origin);
                    await dbContextTransaction.CommitAsync();

                    _cacheService.TryRemove(origin);
                    //Product cache aktualizálás
                    foreach (var prod in _productCacheService.QueryCache())
                    {
                        if (prod.OriginID == origin.ID)
                        {
                            prod.OriginID = 0;
                            prod.Origin = null;
                        }
                    }
                }
                else
                {
                    throw new ResourceNotFoundException(string.Format(bbxBEConsts.ERR_ORIGINNOTFOUND, ID));
                }
            }
            return origin;
        }

        public Entity GetOrigin(long ID)
        {
            Origin origin = null;
            if (!_cacheService.TryGetValue(ID, out origin))
                throw new ResourceNotFoundException(string.Format(bbxBEConsts.ERR_ORIGINNOTFOUND, ID));


            var itemModel = _mapper.Map<Origin, GetOriginViewModel>(origin);
            var listFieldsModel = _modelHelper.GetModelFields<GetOriginViewModel>();

            // shape data
            var shapedData = _dataShaperGetOriginViewModel.ShapeData(itemModel, String.Join(",", listFieldsModel));

            return shapedData;
        }

        public List<Entity> GetOriginList()
        {

            var query = _cacheService.QueryCache();

            var listFields = _modelHelper.GetDBFields<Origin>();

            // shape data
            List<Entity> shapedData = new List<Entity>();
            query.ForEachAsync(i =>
            {
                shapedData.Add(_dataShaperOrigin.ShapeData(i, String.Join(",", listFields)));

            });


            return shapedData;
        }

        public async Task<(IEnumerable<Entity> data, RecordsCount recordsCount)> QueryPagedOriginAsync(QueryOrigin requestParameter)
        {

            var searchString = requestParameter.SearchString;

            var orderBy = requestParameter.OrderBy;
            //      var fields = requestParameter.Fields;
            var fields = _modelHelper.GetQueryableFields<GetOriginViewModel, Origin>();


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
                query = query.Select<Origin>("new(" + fields + ")");
            }

            // retrieve data to list
            var resultData = await GetPagedData(query, requestParameter, false);


            //TODO: szebben megoldani
            var resultDataModel = new List<GetOriginViewModel>();
            resultData.ForEach(i => resultDataModel.Add(
               _mapper.Map<Origin, GetOriginViewModel>(i))
            );


            var listFieldsModel = _modelHelper.GetModelFields<GetOriginViewModel>();

            var shapeData = _dataShaperGetOriginViewModel.ShapeData(resultDataModel, String.Join(",", listFieldsModel));

            return (shapeData, recordsCount);
        }

        private void FilterBySearchString(ref IQueryable<Origin> p_item, string p_searchString)
        {
            if (!p_item.Any())
                return;

            if (string.IsNullOrWhiteSpace(p_searchString))
                return;

            var predicate = PredicateBuilder.New<Origin>();

            var srcFor = p_searchString.ToUpper().Trim();
            predicate = predicate.And(p => p.OriginCode.ToUpper().Contains(srcFor) ||
                                           p.OriginDescription.ToUpper().Contains(srcFor));

            p_item = p_item.Where(predicate);
        }

        public Task<bool> SeedDataAsync(int rowCount)
        {
            throw new System.NotImplementedException();
        }

        public async Task RefreshOriginCache()
        {
            await _cacheService.RefreshCache();
        }
    }
}