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

        public OriginRepositoryAsync(ApplicationDbContext dbContext,
            IDataShapeHelper<Origin> dataShaperOrigin,
            IDataShapeHelper<GetOriginViewModel> dataShaperGetOriginViewModel,
            IModelHelper modelHelper, IMapper mapper, IMockService mockData) : base(dbContext)
        {
            _dbContext = dbContext;
            _Origins = dbContext.Set<Origin>();
            _dataShaperOrigin = dataShaperOrigin;
            _dataShaperGetOriginViewModel = dataShaperGetOriginViewModel;
            _modelHelper = modelHelper;
            _mapper = mapper;
            _mockData = mockData;
        }


        public async Task<bool> IsUniqueOriginCodeAsync(string OriginCode, long? ID = null)
        {
            return !await _Origins.AnyAsync(p => p.OriginCode == OriginCode && !p.Deleted && (ID == null || p.ID != ID.Value));
        }


        public async Task<Entity> GetOriginAsync(GetOrigin requestParameter)
        {
            var ID = requestParameter.ID;

            var item = await GetByIdAsync(ID);
            var listFields = _modelHelper.GetDBFields<Origin>();
            // shape data
            var shapeData = _dataShaperOrigin.ShapeData(item, String.Join(",", listFields));

            return shapeData;
        }

        public async Task<List<Entity>> GetOriginListAsync()
        {
          
            var items = _Origins
                .AsNoTracking()
                .AsExpandable();

            var listFields = _modelHelper.GetDBFields<Origin>();

            // shape data
            List<Entity> shapeData = new List<Entity>();
            items.ForEachAsync(i =>
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
            var query = _Origins
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
                query = query.Select<Origin>("new(" + fields + ")");
            }
            // paging
            query = query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize);

            // retrieve data to list
            var resultData = await query.ToListAsync();

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
            predicate = predicate.And(p => p.OriginDescription.ToUpper().Contains(srcFor));

            p_item = p_item.Where(predicate);
        }

        public Task<bool> SeedDataAsync(int rowCount)
        {
            throw new System.NotImplementedException();
        }
    }
}