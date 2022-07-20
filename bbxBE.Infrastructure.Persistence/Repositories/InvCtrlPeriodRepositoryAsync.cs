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
using bbxBE.Application.Queries.qInvCtrlPeriod;
using bbxBE.Application.Queries.ViewModels;
using bbxBE.Application.Exceptions;
using bbxBE.Application.Consts;

namespace bbxBE.Infrastructure.Persistence.Repositories
{
    public class InvCtrlPeriodRepositoryAsync : GenericRepositoryAsync<InvCtrlPeriod>, IInvCtrlPeriodRepositoryAsync
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly DbSet<InvCtrlPeriod> _InvCtrlPeriods;
        private IDataShapeHelper<InvCtrlPeriod> _dataShaperInvCtrlPeriod;
        private IDataShapeHelper<GetInvCtrlPeriodViewModel> _dataShaperGetInvCtrlPeriodViewModel;
        private readonly IMockService _mockData;
        private readonly IModelHelper _modelHelper;
        private readonly IMapper _mapper;

        public InvCtrlPeriodRepositoryAsync(ApplicationDbContext dbContext,
            IDataShapeHelper<InvCtrlPeriod> dataShaperInvCtrlPeriod,
            IDataShapeHelper<GetInvCtrlPeriodViewModel> dataShaperGetInvCtrlPeriodViewModel,
            IModelHelper modelHelper, IMapper mapper, IMockService mockData) : base(dbContext)
        {
            _dbContext = dbContext;
            _InvCtrlPeriods = dbContext.Set<InvCtrlPeriod>();
            _dataShaperInvCtrlPeriod = dataShaperInvCtrlPeriod;
            _dataShaperGetInvCtrlPeriodViewModel = dataShaperGetInvCtrlPeriodViewModel;
            _modelHelper = modelHelper;
            _mapper = mapper;
            _mockData = mockData;
        }
        public async Task<InvCtrlPeriod> AddInvCtrlPeriodAsync(InvCtrlPeriod p_invCtrlPeriod)
        {
            await _InvCtrlPeriods.AddAsync(p_invCtrlPeriod);
            await _dbContext.SaveChangesAsync();
            return p_invCtrlPeriod;
        }
        public async Task<Entity> GetInvCtrlPeriodAsync(GetInvCtrlPeriod requestParameter)
        {


            var ID = requestParameter.ID;

            var item = await GetByIdAsync(ID);

            //            var fields = requestParameter.Fields;

            if (item == null)
            {
                throw new ResourceNotFoundException(string.Format(bbxBEConsts.ERR_INVCTRLPERIODNOTFOUND, ID));
            }

            var itemModel = _mapper.Map<InvCtrlPeriod, GetInvCtrlPeriodViewModel>(item);
            var listFieldsModel = _modelHelper.GetModelFields<GetInvCtrlPeriodViewModel>();

            // shape data
            var shapeData = _dataShaperGetInvCtrlPeriodViewModel.ShapeData(itemModel, String.Join(",", listFieldsModel));

            return shapeData;
        }
        public async Task<(IEnumerable<Entity> data, RecordsCount recordsCount)> QueryPagedInvCtrlPeriodAsync(QueryInvCtrlPeriod requestParameter)
        {

            var searchString = requestParameter.SearchString;

            var pageNumber = requestParameter.PageNumber;
            var pageSize = requestParameter.PageSize;
            var orderBy = requestParameter.OrderBy;
            //      var fields = requestParameter.Fields;
            var fields = _modelHelper.GetQueryableFields<GetInvCtrlPeriodViewModel, InvCtrlPeriod>();


            int recordsTotal, recordsFiltered;

            // Setup IQueryable
            var result = _InvCtrlPeriods.AsNoTracking()
                .Include(w => w.Warehouse).AsNoTracking()
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
                result = result.Select<InvCtrlPeriod>("new(" + fields + ")");
            }
            // paging
            result = result
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize);

            // retrieve data to list
            var resultData = await result.ToListAsync();

            //TODO: szebben megoldani
            var resultDataModel = new List<GetInvCtrlPeriodViewModel>();
            resultData.ForEach(i => resultDataModel.Add(
               _mapper.Map<InvCtrlPeriod, GetInvCtrlPeriodViewModel>(i))
            );


            var listFieldsModel = _modelHelper.GetModelFields<GetInvCtrlPeriodViewModel>();

            var shapeData = _dataShaperGetInvCtrlPeriodViewModel.ShapeData(resultDataModel, String.Join(",", listFieldsModel));

            return (shapeData, recordsCount);
        }

        private void FilterBySearchString(ref IQueryable<InvCtrlPeriod> p_item, string p_searchString)
        {
            if (!p_item.Any())
                return;

            if (string.IsNullOrWhiteSpace(p_searchString))
                return;

            var predicate = PredicateBuilder.New<InvCtrlPeriod>();


            p_item = p_item.Where(predicate);
        }

        public Task<bool> SeedDataAsync(int rowCount)
        {
            throw new System.NotImplementedException();
        }

        public async Task<bool> IsOverLappedPeriodAsync(DateTime DateFrom, DateTime DateTo)
        {
            var result = await _InvCtrlPeriods.AnyAsync(w => w.DateFrom < DateTo && DateFrom < w.DateTo);
            return result;
        }
    }
}