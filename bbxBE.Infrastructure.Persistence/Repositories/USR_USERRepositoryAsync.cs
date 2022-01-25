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

namespace bbxBE.Infrastructure.Persistence.Repositories
{
    public class USR_USERRepositoryAsync : GenericRepositoryAsync<USR_USER>, IUSR_USERRepositoryAsync
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly DbSet<USR_USER> _users;
        private IDataShapeHelper<USR_USER> _dataShaper;
        private readonly IMockService _mockData;

        public USR_USERRepositoryAsync(ApplicationDbContext dbContext,
            IDataShapeHelper<USR_USER> dataShaper, IMockService mockData) : base(dbContext)
        {
            _dbContext = dbContext;
            _users = dbContext.Set<USR_USER>();
            _dataShaper = dataShaper;
            _mockData = mockData;
        }

 
        public async Task<bool> IsUniqueNameAsync(string USR_NAME)
        {
            return await _users
               .AllAsync(p => p.USR_NAME != USR_NAME);
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

        /*
        public async Task<(IEnumerable<Entity> data, RecordsCount recordsCount)> GetPagedPositionReponseAsync(GetPositionsQuery requestParameter)
        {
            var positionNumber = requestParameter.PositionNumber;
            var positionTitle = requestParameter.PositionTitle;

            var pageNumber = requestParameter.PageNumber;
            var pageSize = requestParameter.PageSize;
            var orderBy = requestParameter.OrderBy;
            var fields = requestParameter.Fields;

            int recordsTotal, recordsFiltered;

            // Setup IQueryable
            var result = _positions
                .AsNoTracking()
                .AsExpandable();

            // Count records total
            recordsTotal = await result.CountAsync();

            // filter data
            FilterByColumn(ref result, positionNumber, positionTitle);

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
                result = result.Select<Position>("new(" + fields + ")");
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

        private void FilterByColumn(ref IQueryable<Position> positions, string positionNumber, string positionTitle)
        {
            if (!positions.Any())
                return;

            if (string.IsNullOrEmpty(positionTitle) && string.IsNullOrEmpty(positionNumber))
                return;

            var predicate = PredicateBuilder.New<Position>();

            if (!string.IsNullOrEmpty(positionNumber))
                predicate = predicate.Or(p => p.PositionNumber.Contains(positionNumber.Trim()));

            if (!string.IsNullOrEmpty(positionTitle))
                predicate = predicate.Or(p => p.PositionTitle.Contains(positionTitle.Trim()));

            positions = positions.Where(predicate);
        }
        */

    }
}