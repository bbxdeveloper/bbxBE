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
using bbxBE.Application.Queries.qOrigin;
using bbxBE.Application.Queries.ViewModels;
using bbxBE.Application.Exceptions;
using bbxBE.Application.Consts;

namespace bbxBE.Infrastructure.Persistence.Repositories
{
    public class OriginRepositoryAsync : GenericRepositoryAsync<Origin>, IOriginRepositoryAsync
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly DbSet<Origin> _Origins;
        private IDataShapeHelper<Origin> _dataShaperOrigin;
        private IDataShapeHelper<GetOriginViewModel> _dataShaperGetOriginViewModel;
        private readonly IMockService _mockData;
        private readonly IModelHelper _modelHelper;
        private readonly IMapper _mapper;
        private readonly ICacheService<Origin> _cacheService;
        private readonly ICacheService<Product> _productCacheService;

        public OriginRepositoryAsync(ApplicationDbContext dbContext,
            IDataShapeHelper<Origin> dataShaperOrigin,
            IDataShapeHelper<GetOriginViewModel> dataShaperGetOriginViewModel,
            IModelHelper modelHelper, IMapper mapper, IMockService mockData,
            ICacheService<Origin> originGroupCacheService,
            ICacheService<Product> productCacheService) : base(dbContext)
        {
            _dbContext = dbContext;
            _Origins = dbContext.Origin;
            _dataShaperOrigin = dataShaperOrigin;
            _dataShaperGetOriginViewModel = dataShaperGetOriginViewModel;
            _modelHelper = modelHelper;
            _mapper = mapper;
            _mockData = mockData;
            _cacheService = originGroupCacheService;
            _productCacheService = productCacheService;

            var t = RefreshOriginCache();
            t.GetAwaiter().GetResult();
        }


        public bool IsUniqueOriginCode(string OriginCode, long? ID = null)
        {
            var query = _cacheService.QueryCache();
            return !query.ToList().Any(p => p.OriginCode == OriginCode && !p.Deleted && (ID == null || p.ID != ID.Value));
        }

        public async Task<Origin> AddOriginAsync(Origin p_origin)
        {

            await _Origins.AddAsync(p_origin);
            await _dbContext.SaveChangesAsync();

            _cacheService.AddOrUpdate(p_origin);
            return p_origin;
        }

        public async Task<long> AddOriginRangeAsync(List<Origin> p_originList)
        {
            using (var dbContextTransaction = await _dbContext.Database.BeginTransactionAsync())
            {


                await _Origins.AddRangeAsync(p_originList);
                await _dbContext.SaveChangesAsync();

                await dbContextTransaction.CommitAsync();
            }
            await RefreshOriginCache();
            return p_originList.Count();
        }

        public async Task<Origin> UpdateOriginAsync(Origin p_origin)
        {
            _Origins.Update(p_origin);
            await _dbContext.SaveChangesAsync();
            //                await dbContextTransaction.CommitAsync();

            _cacheService.AddOrUpdate(p_origin);

            //Product cache aktualizálás (ha fel van töltve)
            if (!_productCacheService.IsCacheEmpty())
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
            _Origins.UpdateRange(p_originList);
            await _dbContext.SaveChangesAsync();
            //                await dbContextTransaction.CommitAsync();

            await RefreshOriginCache();

            //Product cache aktualizálás (ha fel van töltve)
            if (!_productCacheService.IsCacheEmpty())
            {
                await _productCacheService.RefreshCache();
            }
            return p_originList.Count();
        }

        public async Task<Origin> DeleteOriginAsync(long ID)
        {

            Origin origin = null;
            using (var dbContextTransaction = await _dbContext.Database.BeginTransactionAsync())
            {
                origin = _Origins.Where(x => x.ID == ID).FirstOrDefault();

                if (origin != null)
                {

                    _Origins.Remove(origin);

                    await _dbContext.SaveChangesAsync();
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
                    throw new ResourceNotFoundException(string.Format(bbxBEConsts.FV_ORIGINNOTFOUND, ID));
                }
            }
            return origin;
        }

        public Entity GetOrigin(GetOrigin requestParameter)
        {
            var ID = requestParameter.ID;
            Origin origin = null;
            if (!_cacheService.TryGetValue(ID, out origin))
                throw new ResourceNotFoundException(string.Format(bbxBEConsts.FV_ORIGINNOTFOUND, ID));


            var itemModel = _mapper.Map<Origin, GetOriginViewModel>(origin);
            var listFieldsModel = _modelHelper.GetModelFields<GetOriginViewModel>();

            // shape data
            var shapeData = _dataShaperGetOriginViewModel.ShapeData(itemModel, String.Join(",", listFieldsModel));

            return shapeData;
        }

        public List<Entity> GetOriginList()
        {

            var query = _cacheService.QueryCache();

            var listFields = _modelHelper.GetDBFields<Origin>();

            // shape data
            List<Entity> shapeData = new List<Entity>();
            query.ForEachAsync(i =>
            {
                shapeData.Add(_dataShaperOrigin.ShapeData(i, String.Join(",", listFields)));

            });


            return shapeData;
        }

        public async Task<(IEnumerable<Entity> data, RecordsCount recordsCount)> QueryPagedOriginAsync(QueryOrigin requestParameter)
        {

            var searchString = requestParameter.SearchString;

            var pageNumber = requestParameter.PageNumber;
            var pageSize = requestParameter.PageSize;
            var orderBy = requestParameter.OrderBy;
            //      var fields = requestParameter.Fields;
            var fields = _modelHelper.GetQueryableFields<GetOriginViewModel, Origin>();


            int recordsTotal, recordsFiltered;

            // Setup IQueryable
            var query = _cacheService.QueryCache();

            // Count records total
            recordsTotal =  query.Count();

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
            // paging
            query = query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize);

            // retrieve data to list
            var resultData = query.ToList();

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
            if (_cacheService.IsCacheEmpty())
            {
                var q = _Origins
                .AsNoTracking()
                .AsExpandable();
                await _cacheService.RefreshCache(q);
            }
        }
    }
}