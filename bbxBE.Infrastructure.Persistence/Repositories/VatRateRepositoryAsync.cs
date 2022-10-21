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
using bbxBE.Application.Queries.qVatRate;
using bbxBE.Application.Queries.ViewModels;
using bbxBE.Common.Exceptions;
using bbxBE.Common.Consts;

namespace bbxBE.Infrastructure.Persistence.Repositories
{
    public class VatRateRepositoryAsync : GenericRepositoryAsync<VatRate>, IVatRateRepositoryAsync
    {
        private readonly ApplicationDbContext _dbContext;
        private IDataShapeHelper<VatRate> _dataShaperVatRate;
        private IDataShapeHelper<GetVatRateViewModel> _dataShaperGetVatRateViewModel;
        private readonly IMockService _mockData;
        private readonly IModelHelper _modelHelper;
        private readonly IMapper _mapper;
        private readonly ICacheService<VatRate> _cacheService;

        public VatRateRepositoryAsync(ApplicationDbContext dbContext,
            IDataShapeHelper<VatRate> dataShaperVatRate,
            IDataShapeHelper<GetVatRateViewModel> dataShaperGetVatRateViewModel,
            IModelHelper modelHelper, IMapper mapper, IMockService mockData,
            ICacheService<VatRate> cacheService) : base(dbContext)
        {
            _dbContext = dbContext;
            _dataShaperVatRate = dataShaperVatRate;
            _dataShaperGetVatRateViewModel = dataShaperGetVatRateViewModel;
            _modelHelper = modelHelper;
            _mapper = mapper;
            _mockData = mockData;
            _cacheService = cacheService;


            var t = RefreshVatRateCache();
            t.GetAwaiter().GetResult();
        }

        public VatRate GetVatRateByCode(string vatRateCode)
        {
            VatRate vr = null;

            var query = _cacheService.QueryCache();
            vr = query.Where(x => x.VatRateCode.ToUpper() == vatRateCode.ToUpper()).FirstOrDefault();
            return vr;
        }




        public Entity GetVatRate(long ID)
        {
            var query = _cacheService.QueryCache();
            var item = query.Where(x => x.ID== ID).FirstOrDefault();

            if (item == null)
            {
                throw new ResourceNotFoundException(string.Format(bbxBEConsts.ERR_VATRATENOTFOUND, ID));
            }

            var itemModel = _mapper.Map<VatRate, GetVatRateViewModel>(item);
            var listFieldsModel = _modelHelper.GetModelFields<GetVatRateViewModel>();

            // shape data
            var shapeData = _dataShaperGetVatRateViewModel.ShapeData(itemModel, String.Join(",", listFieldsModel));

            return shapeData;
        }

        public async Task<VatRate> AddVatRateAsync(VatRate p_vatRate)
        {


            await AddAsync(p_vatRate);
            _cacheService.AddOrUpdate(p_vatRate);
            return p_vatRate;
        }
        public async Task<VatRate> UpdateVatRateAsync(VatRate p_vatRate)
        {
            _cacheService.AddOrUpdate(p_vatRate);
            await UpdateAsync(p_vatRate);
            return p_vatRate;
        }
        public async Task<VatRate> DeleteVatRateAsync(long ID)
        {

            VatRate vr = null;

            vr = _dbContext.VatRate.AsNoTracking().Where(x => x.ID == ID).FirstOrDefault();

            if (vr != null)
            {


                _cacheService.TryRemove(vr);
                await RemoveAsync(vr);

            }
            else
            {
                throw new ResourceNotFoundException(string.Format(bbxBEConsts.ERR_VATRATENOTFOUND, ID));
            }

            return vr;
        }

        public (IEnumerable<Entity> data, RecordsCount recordsCount) QueryPagedVatRate(QueryVatRate requestParameter)
        {

            var searchString = requestParameter.SearchString;

            var pageNumber = requestParameter.PageNumber;
            var pageSize = requestParameter.PageSize;
            var orderBy = requestParameter.OrderBy;
            //      var fields = requestParameter.Fields;
            var fields = _modelHelper.GetQueryableFields<GetVatRateViewModel, VatRate>();


            int recordsTotal, recordsFiltered;

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
                query = query.Select<VatRate>("new(" + fields + ")");
            }
            // paging
            query = query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize);

            // retrieve data to list
            var resultData = query.ToList();

            //TODO: szebben megoldani
            var resultDataModel = new List<GetVatRateViewModel>();
            resultData.ForEach(i => resultDataModel.Add(
               _mapper.Map<VatRate, GetVatRateViewModel>(i))
            );


            var listFieldsModel = _modelHelper.GetModelFields<GetVatRateViewModel>();

            var shapeData = _dataShaperGetVatRateViewModel.ShapeData(resultDataModel, String.Join(",", listFieldsModel));

            return (shapeData, recordsCount);
        }

        private void FilterBySearchString(ref IQueryable<VatRate> p_item, string p_searchString)
        {
            if (!p_item.Any())
                return;

            if (string.IsNullOrWhiteSpace(p_searchString))
                return;

            var predicate = PredicateBuilder.New<VatRate>();

            var srcFor = p_searchString.ToUpper().Trim();
            predicate = predicate.And(p => p.VatRateCode.ToUpper().Contains(srcFor));

            p_item = p_item.Where(predicate);
        }

        public Task<bool> SeedDataAsync(int rowCount)
        {
            throw new System.NotImplementedException();
        }

        public async Task RefreshVatRateCache()
        {

            if (_cacheService.IsCacheNull())
            {

                var q = _dbContext.VatRate
                .AsNoTracking()
                .AsExpandable();
                await _cacheService.RefreshCache(q);

            }
        }
    }
}