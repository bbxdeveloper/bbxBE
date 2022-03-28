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

namespace bbxBE.Infrastructure.Persistence.Repositories
{
    public class VatRateRepositoryAsync : GenericRepositoryAsync<VatRate>, IVatRateRepositoryAsync
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly DbSet<VatRate> _VatRates;
        private IDataShapeHelper<VatRate> _dataShaperVatRate;
        private IDataShapeHelper<GetVatRateViewModel> _dataShaperGetVatRateViewModel;
        private readonly IMockService _mockData;
        private readonly IModelHelper _modelHelper;
        private readonly IMapper _mapper;

        public VatRateRepositoryAsync(ApplicationDbContext dbContext,
            IDataShapeHelper<VatRate> dataShaperVatRate,
            IDataShapeHelper<GetVatRateViewModel> dataShaperGetVatRateViewModel,
            IModelHelper modelHelper, IMapper mapper, IMockService mockData) : base(dbContext)
        {
            _dbContext = dbContext;
            _VatRates = dbContext.Set<VatRate>();
            _dataShaperVatRate = dataShaperVatRate;
            _dataShaperGetVatRateViewModel = dataShaperGetVatRateViewModel;
            _modelHelper = modelHelper;
            _mapper = mapper;
            _mockData = mockData;
        }


     

        public async Task<Entity> GetVatRateAsync(GetVatRate requestParameter)
        {


            var ID = requestParameter.ID;

            var item = await GetByIdAsync(ID);

            //            var fields = requestParameter.Fields;

            var itemModel = _mapper.Map<VatRate, GetVatRateViewModel>(item);
            var listFieldsModel = _modelHelper.GetModelFields<GetVatRateViewModel>();

            // shape data
            var shapeData = _dataShaperGetVatRateViewModel.ShapeData(itemModel, String.Join(",", listFieldsModel));

            return shapeData;
        }
        public async Task<(IEnumerable<Entity> data, RecordsCount recordsCount)> QueryPagedVatRateAsync(QueryVatRate requestParameter)
        {

            var searchString = requestParameter.SearchString;

            var pageNumber = requestParameter.PageNumber;
            var pageSize = requestParameter.PageSize;
            var orderBy = requestParameter.OrderBy;
            //      var fields = requestParameter.Fields;
            var fields = _modelHelper.GetQueryableFields<GetVatRateViewModel, VatRate>();


            int recordsTotal, recordsFiltered;

            // Setup IQueryable
            var result = _VatRates
                .AsNoTracking()
                .AsExpandable();

            // Count records total
            recordsTotal = await result.CountAsync();

            // filter data
            FilterBySearchString(ref result, searchString);

            // Count records after filter
            recordsFiltered = await result.CountAsync();

            //set Record counts
            var recordsCount = new RecordsCount
            {
                RecordsFiltered = recordsFiltered,
                RecordsTotal = recordsTotal
            };

            // set order by
            if (!string.IsNullOrWhiteSpace(orderBy))
            {
                result = result.OrderBy(orderBy);
            }

            // select columns
            if (!string.IsNullOrWhiteSpace(fields))
            {
                result = result.Select<VatRate>("new(" + fields + ")");
            }
            // paging
            result = result
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize);

            // retrieve data to list
            var resultData = await result.ToListAsync();

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

   
    }
}