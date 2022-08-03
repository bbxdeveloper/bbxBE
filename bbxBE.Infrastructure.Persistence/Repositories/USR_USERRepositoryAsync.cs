using LinqKit;
using Microsoft.EntityFrameworkCore;
//using bbxBE.Application.Features.Positions.Queries.GetPositions;
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
using bbxBE.Application.Queries.qUSR_USER;
using bbxBE.Application.Queries.ViewModels;
using bbxBE.Application.Exceptions;
using bbxBE.Application.Consts;

namespace bbxBE.Infrastructure.Persistence.Repositories
{
    public class USR_USERRepositoryAsync : GenericRepositoryAsync<USR_USER>, IUSR_USERRepositoryAsync
    {
        private readonly ApplicationDbContext _dbContext;
        private IDataShapeHelper<USR_USER> _dataShaper;
        private readonly IModelHelper _modelHelper;
        private readonly IMockService _mockData;

        public USR_USERRepositoryAsync(ApplicationDbContext dbContext,
            IDataShapeHelper<USR_USER> dataShaper, IModelHelper modelHelper, IMockService mockData) : base(dbContext)
        {
            _dbContext = dbContext;
            _dataShaper = dataShaper;
            _modelHelper = modelHelper;
            _mockData = mockData;
        }

 
        public async Task<bool> IsUniqueNameAsync(string USR_NAME, long? ID = null)
        {
            return !await _dbContext.USR_USER.AnyAsync(p => p.USR_NAME == USR_NAME && p.USR_ACTIVE && (ID == null || p.ID != ID.Value));
         }


        public async Task<bool> SeedDataAsync(int rowCount)
        {
            /* MOCK
            foreach (Position position in _mockData.GetPositions(rowCount))
            {
                await this.AddAsync(position);
            }
            */
            return true;
        }
     
        public async Task<Entity> GetUSR_USERAsync(GetUSR_USER requestParameter)
        {
          
            var ID = requestParameter.ID;

            var user = await GetByIdAsync(ID);
      
            var fields = requestParameter.Fields;

            if (user == null)
            {
                throw new ResourceNotFoundException(string.Format(bbxBEConsts.ERR_USERNOTFOUND, ID));
            }

            // shape data
            var shapeData = _dataShaper.ShapeData(user, fields);

            return shapeData;
        }
        public async Task<(IEnumerable<Entity> data, RecordsCount recordsCount)> QueryPagedUSR_USERAsync(QueryUSR_USER requestParameter)
        {

            var SearchString = requestParameter.SearchString;

            var pageNumber = requestParameter.PageNumber;
            var pageSize = requestParameter.PageSize;
            var orderBy = requestParameter.OrderBy;
            //var fields = requestParameter.Fields;
            var fields = _modelHelper.GetQueryableFields<GetUSR_USERViewModel, USR_USER>();
            int recordsTotal, recordsFiltered;

            // Setup IQueryable
            var result = _dbContext.USR_USER
                .AsNoTracking()
                .AsExpandable();

            // Count records total
            recordsTotal = await result.CountAsync();

            // filter data
            FilterByColumns(ref result, SearchString);

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
                result = result.Select<USR_USER>("new(" + fields + ")");
            }
            // paging
            result = result
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize);

            // retrieve data to list
            var resultData = await result.ToListAsync();
            // shape data
            var shapeData = _dataShaper.ShapeData(resultData, fields);

            return (shapeData, recordsCount);
        }

        private void FilterByColumns(ref IQueryable<USR_USER> p_USR, string SearchString)
        {
            if (!p_USR.Any())
                return;

            if ( string.IsNullOrEmpty(SearchString) )
                return;

            var predicate = PredicateBuilder.New<USR_USER>();

    
            predicate = predicate.And(p => p.USR_NAME.Contains(SearchString.Trim())|| p.USR_LOGIN.Contains(SearchString.Trim()));

            p_USR = p_USR.Where(predicate);
        }

       
    }
}