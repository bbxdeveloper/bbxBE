using AutoMapper;
using bbxBE.Application.Helpers;
using bbxBE.Application.Interfaces;
using bbxBE.Application.Interfaces.Repositories;
using bbxBE.Application.Parameters;
using bbxBE.Application.Queries.qLocation;
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
    public class LocationRepositoryAsync : GenericRepositoryAsync<Location>, ILocationRepositoryAsync
    {
        private readonly IApplicationDbContext _dbContext;
        private IDataShapeHelper<Location> _dataShaperLocation;
        private IDataShapeHelper<GetLocationViewModel> _dataShaperGetLocationViewModel;
        private readonly IMockService _mockData;
        private readonly IModelHelper _modelHelper;
        private readonly IMapper _mapper;

        public LocationRepositoryAsync(IApplicationDbContext dbContext,
            IModelHelper modelHelper, IMapper mapper, IMockService mockData) : base(dbContext)
        {
            _dbContext = dbContext;
            _dataShaperLocation = new DataShapeHelper<Location>();
            _dataShaperGetLocationViewModel = new DataShapeHelper<GetLocationViewModel>();
            _modelHelper = modelHelper;
            _mapper = mapper;
            _mockData = mockData;
        }


        public async Task<bool> IsUniqueLocationCodeAsync(string LocationCode, long? ID = null)
        {
            return !await _dbContext.Location.AsNoTracking().AnyAsync(p => p.LocationCode == LocationCode && !p.Deleted && (ID == null || p.ID != ID.Value));
        }


        public async Task<Location> AddLocationAsync(Location p_Location)
        {

            await AddAsync(p_Location);
            return p_Location;
        }
        public async Task<Location> UpdateLocationAsync(Location p_Location)
        {
            await UpdateAsync(p_Location);
            //                await dbContextTransaction.CommitAsync();

            return p_Location;
        }

        public async Task<Location> DeleteLocationAsync(long ID)
        {

            Location Location = null;
            using (var dbContextTransaction = await _dbContext.Instance.Database.BeginTransactionAsync())
            {
                Location = await _dbContext.Location.Where(x => x.ID == ID).SingleOrDefaultAsync();

                if (Location != null)
                {

                    await RemoveAsync(Location);
                    await dbContextTransaction.CommitAsync();
                }
                else
                {
                    throw new ResourceNotFoundException(string.Format(bbxBEConsts.ERR_LOCATIONOTFOUND, ID));
                }
            }
            return Location;
        }

        public async Task<Location> GetLocationByCodeAsync(string LocationCode)
        {
            return await _dbContext.Location.AsNoTracking().FirstOrDefaultAsync(p => p.LocationCode == LocationCode && !p.Deleted);
        }


        public async Task<Entity> GetLocationAsync(long ID)
        {

            var item = await GetByIdAsync(ID);

            //            var fields = requestParameter.Fields;

            if (item == null)
            {
                throw new ResourceNotFoundException(string.Format(bbxBEConsts.ERR_LOCATIONOTFOUND, ID));
            }

            var itemModel = _mapper.Map<Location, GetLocationViewModel>(item);
            var listFieldsModel = _modelHelper.GetModelFields<GetLocationViewModel>();

            // shape data
            var shapedData = _dataShaperGetLocationViewModel.ShapeData(itemModel, String.Join(",", listFieldsModel));

            return shapedData;
        }
        public async Task<(IEnumerable<Entity> data, RecordsCount recordsCount)> QueryPagedLocationAsync(QueryLocation requestParameter)
        {

            var searchString = requestParameter.SearchString;
            var orderBy = requestParameter.OrderBy;
            //      var fields = requestParameter.Fields;
            var fields = _modelHelper.GetQueryableFields<GetLocationViewModel, Location>();


            int recordsTotal, recordsFiltered;

            // Setup IQueryable
            var query = _dbContext.Location
                .AsNoTracking()
                .AsExpandable();

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
            if (!string.IsNullOrWhiteSpace(fields))
            {
                query = query.Select<Location>("new(" + fields + ")");
            }

            // retrieve data to list
            var resultData = await GetPagedData(query, requestParameter);

            //TODO: szebben megoldani
            var resultDataModel = new List<GetLocationViewModel>();
            resultData.ForEach(i => resultDataModel.Add(
               _mapper.Map<Location, GetLocationViewModel>(i))
            );


            var listFieldsModel = _modelHelper.GetModelFields<GetLocationViewModel>();

            var shapedData = _dataShaperGetLocationViewModel.ShapeData(resultDataModel, String.Join(",", listFieldsModel));

            return (shapedData, recordsCount);
        }

        private void FilterBySearchString(ref IQueryable<Location> p_item, string p_searchString)
        {
            if (!p_item.Any())
                return;

            if (string.IsNullOrWhiteSpace(p_searchString))
                return;

            var predicate = PredicateBuilder.New<Location>();

            var srcFor = p_searchString.ToUpper().Trim();
            predicate = predicate.And(p => p.LocationCode.ToUpper().Contains(srcFor) || p.LocationDescription.ToUpper().Contains(srcFor));

            p_item = p_item.Where(predicate);
        }

        public Task<bool> SeedDataAsync(int rowCount)
        {
            throw new System.NotImplementedException();
        }


    }
}