using bbxBE.Application.BLL;
using bbxBE.Application.Helpers;
using bbxBE.Application.Interfaces;
using bbxBE.Application.Interfaces.Repositories;
using bbxBE.Application.Parameters;
using bbxBE.Application.Queries.qUser;
using bbxBE.Application.Queries.ViewModels;
using bbxBE.Common.Consts;
using bbxBE.Common.Exceptions;
using bbxBE.Domain.Entities;
using bbxBE.Infrastructure.Persistence.Repository;
using LinqKit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;

namespace bbxBE.Infrastructure.Persistence.Repositories
{
    public class UserRepositoryAsync : GenericRepositoryAsync<Users>, IUserRepositoryAsync
    {
        private readonly IApplicationDbContext _dbContext;
        private IDataShapeHelper<Users> _dataShaper;
        private readonly IModelHelper _modelHelper;
        private readonly IConfiguration _configuration;
        private readonly IMockService _mockData;

        public UserRepositoryAsync(IApplicationDbContext dbContext,
            IConfiguration configuration, IModelHelper modelHelper, IMockService mockData) : base(dbContext)
        {
            _dbContext = dbContext;
            _dataShaper = new DataShapeHelper<Users>();
            _configuration = configuration;
            _modelHelper = modelHelper;
            _mockData = mockData;
        }


        public async Task<bool> IsUniqueNameAsync(string UserName, long? ID = null)
        {
            return !await _dbContext.Users.AnyAsync(p => p.Name.ToUpper() == UserName.ToUpper() && p.Active && (ID == null || p.ID != ID.Value));
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

        public async Task<Entity> GetUserAsync(long ID, string fields)
        {

            var user = await GetByIdAsync(ID);
            if (user == null)
            {
                throw new ResourceNotFoundException(string.Format(bbxBEConsts.ERR_USERNOTFOUND, ID));
            }

            // shape data
            var shapeData = _dataShaper.ShapeData(user, fields);

            return shapeData;
        }
        public async Task<Users> GetUserRecordByNameAsync(string name)
        {
            return await _dbContext.Users
                .Include(w => w.Warehouse)
                .FirstOrDefaultAsync(p => p.Name.ToUpper() == name.ToUpper());
        }
        public async Task<Users> GetUserRecordByIDAsync(long ID)
        {
            return await _dbContext.Users
                    .Include(w => w.Warehouse)
                    .FirstOrDefaultAsync(p => p.ID == ID);
        }
        public async Task<Users> GetUserRecordByLoginNameAsync(string loginName)
        {
            return await _dbContext.Users
                .Include(w => w.Warehouse)
                .FirstOrDefaultAsync(p => p.LoginName.ToUpper() == loginName.ToUpper());

        }
        public async Task<(IEnumerable<Entity> data, RecordsCount recordsCount)> QueryPagedUserAsync(QueryUser requestParameter)
        {

            var SearchString = requestParameter.SearchString;
            var orderBy = requestParameter.OrderBy;
            //var fields = requestParameter.Fields;
            var fields = _modelHelper.GetQueryableFields<GetUsersViewModel, Users>();
            int recordsTotal, recordsFiltered;

            // Setup IQueryable
            var query = _dbContext.Users.AsNoTracking()
                        .Include(w => w.Warehouse).AsNoTracking()
                        .AsExpandable();

            // Count records total
            recordsTotal = await query.CountAsync();

            // filter data
            FilterByColumns(ref query, SearchString);

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
                query = query.Select<Users>("new(" + fields + ")");
            }

            // retrieve data to list
            var resultData = await GetPagedData(query, requestParameter);


            // shape data
            var shapeData = _dataShaper.ShapeData(resultData, fields);

            return (shapeData, recordsCount);
        }

        private void FilterByColumns(ref IQueryable<Users> p_USR, string SearchString)
        {
            if (!p_USR.Any())
                return;

            if (string.IsNullOrEmpty(SearchString))
                return;

            var predicate = PredicateBuilder.New<Users>();


            predicate = predicate.And(p => p.Name.Contains(SearchString.Trim()) || p.LoginName.Contains(SearchString.Trim()));

            p_USR = p_USR.Where(predicate);
        }

        public async Task<Entity> GetUserByLoginNameAndPwd(string LoginName, string Password)
        {

            var usr = await GetUserRecordByLoginNameAsync(LoginName);
            if (usr == null || !usr.Active)
            {
                throw new ResourceNotFoundException(string.Format(bbxBEConsts.ERR_USERNOTFOUND2, LoginName));
            }

            var salt = _configuration.GetValue<string>(bbxBEConsts.CONF_PwdSalt);
            if (BllAuth.GetPwdHash(Password, salt) != usr.PasswordHash)
            {
                throw new UnauthorizedAccessException();
            }

            // shape data
            var shapeData = _dataShaper.ShapeData(usr, "");

            return shapeData;
        }
    }
}