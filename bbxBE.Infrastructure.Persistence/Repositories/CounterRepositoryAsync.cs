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
using bbxBE.Common.Exceptions;
using bbxBE.Common.Consts;

namespace bbxBE.Infrastructure.Persistence.Repositories
{
    public class CounterRepositoryAsync : GenericRepositoryAsync<Counter>, ICounterRepositoryAsync
    {
        private readonly ApplicationDbContext _dbContext;
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
            _dataShaperCounter = dataShaperCounter;
            _dataShaperGetCounterViewModel = dataShaperGetCounterViewModel;
            _modelHelper = modelHelper;
            _mapper = mapper;
            _mockData = mockData;
        }


        public async Task<bool> IsUniqueCounterCodeAsync(string CounterCode, long? ID = null)
        {
            return !await _dbContext.Counter.AnyAsync(p => p.CounterCode == CounterCode && !p.Deleted && (ID == null || p.ID != ID.Value));
        }

        public async Task<Counter> AddCounterAsync(Counter p_Counter, string p_WarehouseCode)
        {
            using (var dbContextTransaction = await _dbContext.Database.BeginTransactionAsync())
            {

                if (!string.IsNullOrWhiteSpace(p_WarehouseCode))
                {
                    var wh = _dbContext.Warehouse.SingleOrDefault(x => x.WarehouseCode == p_WarehouseCode);
                    p_Counter.WarehouseID = wh.ID;
                }

                await AddAsync(p_Counter);
                await dbContextTransaction.CommitAsync();

            }
            return p_Counter;
        }

        public async Task<Counter> UpdateCounterAsync(Counter p_Counter, string p_WarehouseCode)
        {

            using (var dbContextTransaction = await _dbContext.Database.BeginTransactionAsync())
            {

                var cnt = _dbContext.Counter.Where(x => x.ID == p_Counter.ID).FirstOrDefault();

                if (cnt != null)
                {
                    if (!string.IsNullOrWhiteSpace(p_WarehouseCode))
                    {
                        var wh = _dbContext.Warehouse.SingleOrDefault(x => x.WarehouseCode == p_WarehouseCode);
                        p_Counter.WarehouseID = wh.ID;
                    }

                    await UpdateAsync(p_Counter);
                    await dbContextTransaction.CommitAsync();

                }
                else
                {
                     throw new ResourceNotFoundException(string.Format(bbxBEConsts.FV_COUNTERNOTFOUND, p_Counter.ID));
                }
            }
            return p_Counter;
        }

        public async Task<Entity> GetCounterAsync(long ID)
        {
            var item  = await _dbContext.Counter//.AsNoTracking().AsExpandable()
                    .Include(i => i.Warehouse).SingleOrDefaultAsync(s=>s.ID == ID);

            var itemModel = _mapper.Map<Counter, GetCounterViewModel>(item);
            var listFieldsModel = _modelHelper.GetModelFields<GetCounterViewModel>();

            // shape data
            var shapeData = _dataShaperGetCounterViewModel.ShapeData(itemModel, String.Join(",", listFieldsModel));

            return shapeData;
        }


        private const int ExpiredInMinutes = 30;
        public async Task<string> GetNextValueAsync(string CounterCode, long WarehouseID, bool useCounterPool = false)
        {
            var NextValue = "";
            using (var dbContextTransaction = await _dbContext.Database.BeginTransactionAsync())
            {
                var counter = await _dbContext.Counter.AsNoTracking()
                    .Where(x => x.CounterCode == CounterCode && x.WarehouseID == WarehouseID).FirstOrDefaultAsync();

                if (counter != null)
                {

                    if (useCounterPool && counter.CounterPool != null)
                    {
                        var expiredCounter = counter.CounterPool.Where(w => DateTime.UtcNow.Ticks - w.Ticks > TimeSpan.TicksPerMinute * ExpiredInMinutes)
                                        .OrderBy(o => o.Ticks).FirstOrDefault();
                        if (expiredCounter != null)
                        {
                            NextValue = expiredCounter.CounterValue;
                            expiredCounter.Ticks = DateTime.UtcNow.Ticks;
                        }
                    }

                    if (string.IsNullOrWhiteSpace(NextValue))
                    {
                        counter.CurrentNumber++;


                        NextValue = $"{counter.Prefix}{counter.CurrentNumber.ToString().PadLeft(counter.NumbepartLength, '0')}{counter.Suffix}{DateTime.UtcNow.Year.ToString().Substring(2, 2)}";

                        if (useCounterPool)
                        {
                            if (counter.CounterPool == null)
                            {
                                counter.CounterPool = new List<CounterPoolItem>();
                            }
                            counter.CounterPool.Add(new CounterPoolItem() { CounterValue = NextValue, Ticks = DateTime.UtcNow.Ticks });
                        }
                    }
                    
                    await UpdateAsync(counter);
                    await dbContextTransaction.CommitAsync();
                }
                else
                {
                    throw new ResourceNotFoundException(string.Format(bbxBEConsts.FV_COUNTERNOTFOUND2, CounterCode, WarehouseID));
                }
            }
            return NextValue;
        }

        public async Task<bool> FinalizeValueAsync(string CounterCode, long WarehouseID, string counterValue)
        {
            var result = false;
            using (var dbContextTransaction = await _dbContext.Database.BeginTransactionAsync())
            {
                var counter = await _dbContext.Counter.AsNoTracking()
                    .Where(x => x.CounterCode == CounterCode && x.WarehouseID == WarehouseID).FirstOrDefaultAsync();

                if (counter != null)
                {
                    
                    var entity = await _dbContext.FindAsync(typeof(Counter), counter.ID); //To Avoid tracking error
                    if(entity != null)
                        _dbContext.Entry(entity).State = EntityState.Detached;



                    if (counter.CounterPool != null)
                    {
                        counter.CounterPool = counter.CounterPool.Where(w => w .CounterValue != counterValue).ToList();
                        //       _dbContext.Entry<Counter>(counter).State = EntityState.Modified;

                        await UpdateAsync(counter);
                        await dbContextTransaction.CommitAsync();
                        result = true;
                    }

                }
                else
                {
                    throw new ResourceNotFoundException(string.Format(bbxBEConsts.FV_COUNTERNOTFOUND2, CounterCode, WarehouseID));
                }
            }
            return result;
        }

        public async Task<bool> RollbackValueAsync(string CounterCode, long WarehouseID, string counterValue)
        {
            var result = false;
            using (var dbContextTransaction = await _dbContext.Database.BeginTransactionAsync())
            {

                try
                {
                    var counter = await _dbContext.Counter.AsNoTracking()
                        .Where(x => x.CounterCode == CounterCode && x.WarehouseID == WarehouseID).FirstOrDefaultAsync();

                    if (counter != null)
                    {
                        var entity = await _dbContext.FindAsync(typeof(Counter), counter.ID); //To Avoid tracking error
                        if (entity != null)
                            _dbContext.Entry(entity).State = EntityState.Detached;

                        if (counter.CounterPool != null)
                        {
                            var c = counter.CounterPool.Where(w => w.CounterValue == counterValue).FirstOrDefault();
                            if (c != null)
                            {
                                c.Ticks = 0;
                                await UpdateAsync(counter);
                                await dbContextTransaction.CommitAsync();
                                result = true;
                            }
                            result = false;
                        }
                    }
                    else
                    {
                        throw new ResourceNotFoundException(string.Format(bbxBEConsts.FV_COUNTERNOTFOUND2, CounterCode, WarehouseID));
                    }
                }
                catch( Exception ex )
                {
                    await dbContextTransaction.RollbackAsync();
                    throw;
                }
            }
            return result;
        }

        public async Task<(IEnumerable<Entity> data, RecordsCount recordsCount)> QueryPagedCounterAsync(QueryCounter requestParameter)
        {

            var searchString = requestParameter.SearchString;

            var orderBy = requestParameter.OrderBy;
            //      var fields = requestParameter.Fields;
            var fields = _modelHelper.GetQueryableFields<GetCounterViewModel, Counter>();


            int recordsTotal, recordsFiltered;


            var query = _dbContext.Counter//.AsNoTracking().AsExpandable()
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

             // retrieve data to list
            var resultData = await GetPagedData(query, requestParameter);



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