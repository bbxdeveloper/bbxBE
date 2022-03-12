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
using bbxBE.Application.Queries.qCounter;
using bbxBE.Application.Queries.ViewModels;
using bbxBE.Application.Exceptions;
using bbxBE.Application.Consts;

namespace bbxBE.Infrastructure.Persistence.Repositories
{
    public class CounterRepositoryAsync : GenericRepositoryAsync<Counter>, ICounterRepositoryAsync
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly DbSet<Counter> _Counters;
        private readonly DbSet<Warehouse> _Warehouses;
        private IDataShapeHelper<Counter> _dataShaperCounter;
        private IDataShapeHelper<GetCounterViewModel> _dataShaperGetCounterViewModel;
        private readonly IMockService _mockData;
        private readonly IModelHelper _modelHelper;
        private readonly IMapper _mapper;

        public CounterRepositoryAsync(ApplicationDbContext dbContext,
            IDataShapeHelper<Counter> dataShaperCounter,
            IDataShapeHelper<GetCounterViewModel> dataShaperGetCounterViewModel,
            IModelHelper modelHelper, IMapper mapper, IMockService mockData) : base(dbContext)
        {
            _dbContext = dbContext;
            _Counters = dbContext.Set<Counter>();
            _Warehouses = dbContext.Set<Warehouse>();
            _dataShaperCounter = dataShaperCounter;
            _dataShaperGetCounterViewModel = dataShaperGetCounterViewModel;
            _modelHelper = modelHelper;
            _mapper = mapper;
            _mockData = mockData;
        }


        public async Task<bool> IsUniqueCounterCodeAsync(string CounterCode, long? ID = null)
        {
            return !await _Counters.AnyAsync(p => p.CounterCode == CounterCode && !p.Deleted && (ID == null || p.ID != ID.Value));
        }

        public async Task<Counter> AddCounterAsync(Counter p_Counter, string p_WarehouseCode)
        {
            using (var dbContextTransaction = _dbContext.Database.BeginTransaction())
            {

                if (!string.IsNullOrWhiteSpace(p_WarehouseCode))
                {
                    p_Counter.WarehouseID = _Warehouses.SingleOrDefault(x => x.WarehouseCode == p_WarehouseCode)?.ID;
                }

                _Counters.Add(p_Counter);
                await _dbContext.SaveChangesAsync();
                dbContextTransaction.Commit();

            }
            return p_Counter;
        }

        public async Task<Counter> UpdateCounterAsync(Counter p_Counter, string p_WarehouseCode)
        {

            using (var dbContextTransaction = _dbContext.Database.BeginTransaction())
            {

                var cnt = _Counters.Where(x => x.ID == p_Counter.ID).FirstOrDefault();

                if (cnt != null)
                {
                    if (!string.IsNullOrWhiteSpace(p_WarehouseCode))
                    {
                        p_Counter.WarehouseID = _Warehouses.SingleOrDefault(x => x.WarehouseCode == p_WarehouseCode)?.ID;
                    }

                    _Counters.Update(p_Counter);
                    await _dbContext.SaveChangesAsync();
                    dbContextTransaction.Commit();


                }
                else
                {
                    throw new ResourceNotFoundException(string.Format(bbxBEConsts.FV_COUNTERNOTFOUND, p_Counter.ID));
                }
            }
            return p_Counter;
        }

        public async Task<Entity> GetCounterAsync(GetCounter requestParameter)
        {


            var ID = requestParameter.ID;

            var item = await GetByIdAsync(ID);

            //            var fields = requestParameter.Fields;

            var itemModel = _mapper.Map<Counter, GetCounterViewModel>(item);
            var listFieldsModel = _modelHelper.GetModelFields<GetCounterViewModel>();

            // shape data
            var shapeData = _dataShaperGetCounterViewModel.ShapeData(itemModel, String.Join(",", listFieldsModel));

            return shapeData;
        }
        public async Task<(IEnumerable<Entity> data, RecordsCount recordsCount)> QueryPagedCounterAsync(QueryCounter requestParameter)
        {

            var searchString = requestParameter.SearchString;

            var pageNumber = requestParameter.PageNumber;
            var pageSize = requestParameter.PageSize;
            var orderBy = requestParameter.OrderBy;
            //      var fields = requestParameter.Fields;
            var fields = _modelHelper.GetQueryableFields<GetCounterViewModel, Counter>();


            int recordsTotal, recordsFiltered;


            var query = _Counters//.AsNoTracking().AsExpandable()
                    .Include(i => i.Warehouse).AsQueryable();


            // Count records total
            recordsTotal = await query.CountAsync();

            // filter data
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
            /*
            if (!string.IsNullOrWhiteSpace(fields))
            {
                result = result.Select<Counter>("new(" + fields + ")");
            }
            */

            // paging
            query = query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize);

            // retrieve data to list
            var resultData = await query.ToListAsync();

            //TODO: szebben megoldani
            var resultDataModel = new List<GetCounterViewModel>();
            resultData.ForEach(i => resultDataModel.Add(
               _mapper.Map<Counter, GetCounterViewModel>(i))
            );


            var listFieldsModel = _modelHelper.GetModelFields<GetCounterViewModel>();

            var shapeData = _dataShaperGetCounterViewModel.ShapeData(resultDataModel, String.Join(",", listFieldsModel));

            return (shapeData, recordsCount);
        }

        private void FilterBySearchString(ref IQueryable<Counter> p_item, string p_searchString)
        {
            if (!p_item.Any())
                return;

            if (string.IsNullOrWhiteSpace(p_searchString))
                return;

            var predicate = PredicateBuilder.New<Counter>();

            var srcFor = p_searchString.ToUpper().Trim();
            predicate = predicate.And(p => p.CounterDescription.ToUpper().Contains(srcFor));

            p_item = p_item.Where(predicate);
        }

        public Task<bool> SeedDataAsync(int rowCount)
        {
            throw new System.NotImplementedException();
        }

   
    }
}