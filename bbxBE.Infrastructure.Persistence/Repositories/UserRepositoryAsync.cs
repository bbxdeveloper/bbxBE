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
using bbxBE.Application.Queries.qUser;
using bbxBE.Application.Queries.ViewModels;
using bbxBE.Common.Exceptions;
using bbxBE.Common.Consts;

namespace bbxBE.Infrastructure.Persistence.Repositories
{
    public class UserRepositoryAsync : GenericRepositoryAsync<Users>, IUserRepositoryAsync
    {
        private readonly ApplicationDbContext _dbContext;
        private IDataShapeHelper<Users> _dataShaper;
        private readonly IModelHelper _modelHelper;
        private readonly IMockService _mockData;

        public UserRepositoryAsync(ApplicationDbContext dbContext,
            IDataShapeHelper<Users> dataShaper, IModelHelper modelHelper, IMockService mockData) : base(dbContext)
        {
            _dbContext = dbContext;
            _dataShaper = dataShaper;
            _modelHelper = modelHelper;
            _mockData = mockData;
        }

 
        public async Task<bool> IsUniqueNameAsync(string UserName, long? ID = null)
        {
            return !await _dbContext.Users.AnyAsync(p => p.Name == UserName && p.Active && (ID == null || p.ID != ID.Value));
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
     
        public async Task<Entity> GetUserAsync(GetUser requestParameter)
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
        public async Task<(IEnumerable<Entity> data, RecordsCount recordsCount)> QueryPagedUserAsync(QueryUser requestParameter)
        {

            var SearchString = requestParameter.SearchString;

            var pageNumber = requestParameter.PageNumber;
            var pageSize = requestParameter.PageSize;
            var orderBy = requestParameter.OrderBy;
            //var fields = requestParameter.Fields;
            var fields = _modelHelper.GetQueryableFields<GetUsersViewModel, Users>();
            int recordsTotal, recordsFiltered;

            // Setup IQueryable
            var result = _dbContext.Users
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
                result = result.Select<Users>("new(" + fields + ")");
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

        private void FilterByColumns(ref IQueryable<Users> p_USR, string SearchString)
        {
            if (!p_USR.Any())
                return;

            if ( string.IsNullOrEmpty(SearchString) )
                return;

            var predicate = PredicateBuilder.New<Users>();

    
            predicate = predicate.And(p => p.Name.Contains(SearchString.Trim())|| p.LoginName.Contains(SearchString.Trim()));

            p_USR = p_USR.Where(predicate);
        }

       
    }
}