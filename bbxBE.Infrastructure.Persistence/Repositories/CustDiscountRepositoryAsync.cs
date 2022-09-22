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
using bbxBE.Application.Queries.qCustDiscount;
using bbxBE.Application.Queries.ViewModels;
using bbxBE.Common.Exceptions;
using bbxBE.Common.Consts;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace bbxBE.Infrastructure.Persistence.Repositories
{
    public class CustDiscountRepositoryAsync : GenericRepositoryAsync<CustDiscount>, ICustDiscountRepositoryAsync
    {
        private readonly ApplicationDbContext _dbContext;
        private IDataShapeHelper<CustDiscount> _dataShaperCustDiscount;
        private IDataShapeHelper<GetCustDiscountViewModel> _dataShaperGetCustDiscountViewModel;
        private readonly IMockService _mockData;
        private readonly IModelHelper _modelHelper;
        private readonly IMapper _mapper;
        private readonly ICacheService<CustDiscount> _cacheService;
        private readonly ICacheService<Product> _productCacheService;

        public CustDiscountRepositoryAsync(ApplicationDbContext dbContext,
            IDataShapeHelper<CustDiscount> dataShaperCustDiscount,
            IDataShapeHelper<GetCustDiscountViewModel> dataShaperGetCustDiscountViewModel,
            IModelHelper modelHelper, IMapper mapper, IMockService mockData,
            ICacheService<CustDiscount> CustDiscountGroupCacheService,
            ICacheService<Product> productCacheService) : base(dbContext)
        {
            _dbContext = dbContext;
            _dataShaperCustDiscount = dataShaperCustDiscount;
            _dataShaperGetCustDiscountViewModel = dataShaperGetCustDiscountViewModel;
            _modelHelper = modelHelper;
            _mapper = mapper;
            _mockData = mockData;
            _cacheService = CustDiscountGroupCacheService;
            _productCacheService = productCacheService;

        }


        public bool IsUniqueCustDiscountCode(string CustDiscountCode, long? ID = null)
        {
            if (_cacheService.IsCacheNull())
            {
                return !_dbContext.CustDiscount.Any(p => p.CustDiscountCode == CustDiscountCode && !p.Deleted && (ID == null || p.ID != ID.Value));
            }
            else
            {
                var query = _cacheService.QueryCache();
                return !query.ToList().Any(p => p.CustDiscountCode == CustDiscountCode && !p.Deleted && (ID == null || p.ID != ID.Value));
            }
        }

        public async Task<CustDiscount> AddCustDiscountAsync(CustDiscount p_CustDiscount)
        {

            await _dbContext.CustDiscount.AddAsync(p_CustDiscount);
            await _dbContext.SaveChangesAsync();

            _cacheService.AddOrUpdate(p_CustDiscount);
            return p_CustDiscount;
        }

        public async Task<long> AddCustDiscountRangeAsync(List<CustDiscount> p_CustDiscountList)
        {
            using (var dbContextTransaction = await _dbContext.Database.BeginTransactionAsync())
            {


                await _dbContext.CustDiscount.AddRangeAsync(p_CustDiscountList);
                await _dbContext.SaveChangesAsync();

                await dbContextTransaction.CommitAsync();
            }

            await RefreshCustDiscountCache();
            return p_CustDiscountList.Count();
        }

        public async Task<CustDiscount> UpdateCustDiscountAsync(CustDiscount p_CustDiscount)
        {
            _dbContext.CustDiscount.Update(p_CustDiscount);
            await _dbContext.SaveChangesAsync();
            //                await dbContextTransaction.CommitAsync();

            _cacheService.AddOrUpdate(p_CustDiscount);

            //Product cache aktualizálás (ha fel van töltve)
            if (!_productCacheService.IsCacheNull())
            {
                foreach (var prod in _productCacheService.QueryCache())
                {
                    if (prod.CustDiscountID == p_CustDiscount.ID)
                    {
                        prod.CustDiscount = p_CustDiscount;
                    }
                }
            }
            return p_CustDiscount;
        }

        public async Task<long> UpdateCustDiscountRangeAsync(List<CustDiscount> p_CustDiscountList)
        {
            _dbContext.CustDiscount.UpdateRange(p_CustDiscountList);
            await _dbContext.SaveChangesAsync();
            //                await dbContextTransaction.CommitAsync();

            //await RefreshCustDiscountCache();

            //Product cache aktualizálás (ha fel van töltve)
            if (!_productCacheService.IsCacheNull())
            {
                await _productCacheService.RefreshCache();
            }
            return p_CustDiscountList.Count();
        }

        public async Task<CustDiscount> DeleteCustDiscountAsync(long ID)
        {

            CustDiscount CustDiscount = null;
            using (var dbContextTransaction = await _dbContext.Database.BeginTransactionAsync())
            {
                CustDiscount = _dbContext.CustDiscount.Where(x => x.ID == ID).FirstOrDefault();

                if (CustDiscount != null)
                {

                    _dbContext.CustDiscount.Remove(CustDiscount);

                    await _dbContext.SaveChangesAsync();
                    await dbContextTransaction.CommitAsync();

                    _cacheService.TryRemove(CustDiscount);
                    //Product cache aktualizálás
                    foreach (var prod in _productCacheService.QueryCache())
                    {
                        if (prod.CustDiscountID == CustDiscount.ID)
                        {
                            prod.CustDiscountID = 0;
                            prod.CustDiscount = null;
                        }
                    }
                }
                else
                {
                    throw new ResourceNotFoundException(string.Format(bbxBEConsts.FV_CustDiscountNOTFOUND, ID));
                }
            }
            return CustDiscount;
        }

        public Entity GetCustDiscount(GetCustDiscount requestParameter)
        {
            var ID = requestParameter.ID;
            CustDiscount CustDiscount = null;
            if (!_cacheService.TryGetValue(ID, out CustDiscount))
                throw new ResourceNotFoundException(string.Format(bbxBEConsts.FV_CustDiscountNOTFOUND, ID));


            var itemModel = _mapper.Map<CustDiscount, GetCustDiscountViewModel>(CustDiscount);
            var listFieldsModel = _modelHelper.GetModelFields<GetCustDiscountViewModel>();

            // shape data
            var shapeData = _dataShaperGetCustDiscountViewModel.ShapeData(itemModel, String.Join(",", listFieldsModel));

            return shapeData;
        }

        public List<Entity> GetCustDiscountList()
        {

            var query = _cacheService.QueryCache();

            var listFields = _modelHelper.GetDBFields<CustDiscount>();

            // shape data
            List<Entity> shapeData = new List<Entity>();
            query.ForEachAsync(i =>
            {
                shapeData.Add(_dataShaperCustDiscount.ShapeData(i, String.Join(",", listFields)));

            });


            return shapeData;
        }

        public async Task<(IEnumerable<Entity> data, RecordsCount recordsCount)> QueryPagedCustDiscountAsync(QueryCustDiscount requestParameter)
        {

            var searchString = requestParameter.SearchString;

            var pageNumber = requestParameter.PageNumber;
            var pageSize = requestParameter.PageSize;
            var orderBy = requestParameter.OrderBy;
            //      var fields = requestParameter.Fields;
            var fields = _modelHelper.GetQueryableFields<GetCustDiscountViewModel, CustDiscount>();


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
                query = query.Select<CustDiscount>("new(" + fields + ")");
            }
            // paging
            query = query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize);

            // retrieve data to list
            var resultData = query.ToList();

            //TODO: szebben megoldani
            var resultDataModel = new List<GetCustDiscountViewModel>();
            resultData.ForEach(i => resultDataModel.Add(
               _mapper.Map<CustDiscount, GetCustDiscountViewModel>(i))
            );


            var listFieldsModel = _modelHelper.GetModelFields<GetCustDiscountViewModel>();

            var shapeData = _dataShaperGetCustDiscountViewModel.ShapeData(resultDataModel, String.Join(",", listFieldsModel));

            return (shapeData, recordsCount);
        }

        private void FilterBySearchString(ref IQueryable<CustDiscount> p_item, string p_searchString)
        {
            if (!p_item.Any())
                return;

            if (string.IsNullOrWhiteSpace(p_searchString))
                return;

            var predicate = PredicateBuilder.New<CustDiscount>();

            var srcFor = p_searchString.ToUpper().Trim();
            predicate = predicate.And(p => p.CustDiscountCode.ToUpper().Contains(srcFor) ||
                                           p.CustDiscountDescription.ToUpper().Contains(srcFor));

            p_item = p_item.Where(predicate);
        }

        public Task<bool> SeedDataAsync(int rowCount)
        {
            throw new System.NotImplementedException();
        }

        public async Task RefreshCustDiscountCache()
        {
            await _cacheService.RefreshCache();
        }
    }
}