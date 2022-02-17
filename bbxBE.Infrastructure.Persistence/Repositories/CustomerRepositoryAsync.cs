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
using bbxBE.Application.Features.Positions.Queries.GetPositions;
using bbxBE.Application.Interfaces.Queries;
using bbxBE.Application.BLL;
using System;
using AutoMapper;

namespace bbxBE.Infrastructure.Persistence.Repositories
{
    public class CustomerRepositoryAsync : GenericRepositoryAsync<Customer>, ICustomerRepositoryAsync
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly DbSet<Customer> _users;
        private IDataShapeHelper<Customer> _dataShaper;
        private readonly IMockService _mockData;
        private readonly IModelHelper _modelHelper;
        private readonly IMapper _mapper;

        public CustomerRepositoryAsync(ApplicationDbContext dbContext,
            IDataShapeHelper<Customer> dataShaper, IModelHelper modelHelper, IMapper mapper, IMockService mockData) : base(dbContext)
        {
            _dbContext = dbContext;
            _users = dbContext.Set<Customer>();
            _dataShaper = dataShaper;
            _modelHelper = modelHelper;
            _mapper = mapper;
            _mockData = mockData;
        }


        public async Task<bool> IsUniqueTaxpayerIdAsync(string TaxpayerId, long? ID = null)
        {
            return !await _users.AnyAsync(p => p.TaxpayerId == TaxpayerId && !p.Deleted && (ID == null || p.ID != ID.Value));
         }


   
        public async Task<Entity> GetCustomerReponseAsync(object requestParametersX)
        {
            var requestParameter = (GetCustomer)requestParametersX;

            var ID = requestParameter.ID;

            var item = await GetByIdAsync(ID);
      
            var fields = requestParameter.Fields;

            // shape data
            var shapeData = _dataShaper.ShapeData(item, fields);

            return shapeData;
        }
        public async Task<(IEnumerable<Entity> data, RecordsCount recordsCount)> QueryPagedCustomerReponseAsync<ModelType>(object requestParameterX)
        {

            var requestParameter = (QueryCustomer)requestParameterX;

            var customerName = requestParameter.CustomerName;
 
            var pageNumber = requestParameter.PageNumber;
            var pageSize = requestParameter.PageSize;
            var orderBy = requestParameter.OrderBy;
            var fields = requestParameter.Fields;

            int recordsTotal, recordsFiltered;

            // Setup IQueryable
            var result = _users
                .AsNoTracking()
                .AsExpandable();

            // Count records total
            recordsTotal = await result.CountAsync();

            // filter data
            FilterByColumns(ref result, customerName);

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
                result = result.Select<Customer>("new(" + fields + ")");
            }
            // paging
            result = result
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize);

            // retrieve data to list
            var resultData = await result.ToListAsync();


            var resultDataModel = _mapper.Map<List<ModelType>>(resultData);

            // shape data
         //   var shapeData = _dataShaper.ShapeData(resultDataModel, fields);
            var shapeData = _dataShaper.ShapeData(resultData, fields);

            return (shapeData, recordsCount);
        }

        private void FilterByColumns(ref IQueryable<Customer> p_item, string CustomerName)
        {
            if (!p_item.Any())
                return;

            if ( string.IsNullOrEmpty(CustomerName))
                return;

            var predicate = PredicateBuilder.New<Customer>();

            var srcFor = CustomerName.ToUpper().Trim();
            if (!string.IsNullOrEmpty(CustomerName))
                predicate = predicate.And(p => p.CustomerName.ToUpper().Contains(srcFor));

            p_item = p_item.Where(predicate);
        }

        public Task<bool> SeedDataAsync(int rowCount)
        {
            throw new System.NotImplementedException();
        }
    }
}