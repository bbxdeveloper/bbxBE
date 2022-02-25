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
using bbxBE.Application.Queries.qCustomer;
using bbxBE.Application.Queries.ViewModels;

namespace bbxBE.Infrastructure.Persistence.Repositories
{
    public class CustomerRepositoryAsync : GenericRepositoryAsync<Customer>, ICustomerRepositoryAsync
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly DbSet<Customer> _users;
        private IDataShapeHelper<Customer> _dataShaperCustomer;
        private IDataShapeHelper<GetCustomerViewModel> _dataShaperGetCustomerViewModel;
        private readonly IMockService _mockData;
        private readonly IModelHelper _modelHelper;
        private readonly IMapper _mapper;

        public CustomerRepositoryAsync(ApplicationDbContext dbContext,
            IDataShapeHelper<Customer> dataShaperCustomer, 
            IDataShapeHelper<GetCustomerViewModel> dataShaperGetCustomerViewModel, 
            IModelHelper modelHelper, IMapper mapper, IMockService mockData) : base(dbContext)
        {
            _dbContext = dbContext;
            _users = dbContext.Set<Customer>();
            _dataShaperCustomer = dataShaperCustomer;
            _dataShaperGetCustomerViewModel = dataShaperGetCustomerViewModel;
            _modelHelper = modelHelper;
            _mapper = mapper;
            _mockData = mockData;
        }


        public async Task<bool> IsUniqueTaxpayerIdAsync(string TaxpayerId, long? ID = null)
        {
            return !await _users.AnyAsync(p => p.TaxpayerId == TaxpayerId && !p.Deleted && (ID == null || p.ID != ID.Value));
         }

        public async Task<bool> CheckBankAccountAsync(string bankAccountNumber)
        {
            if (string.IsNullOrWhiteSpace(bankAccountNumber))
                return true;

            return bllCustomer.ValidateBankAccount(bankAccountNumber) || bllCustomer.ValidateIBAN(bankAccountNumber);
        }

        public async Task<Entity> GetCustomerReponseAsync(GetCustomer requestParameter)
        {
           

            var ID = requestParameter.ID;

            var item = await GetByIdAsync(ID);
      
//            var fields = requestParameter.Fields;

            var itemModel = _mapper.Map<Customer, GetCustomerViewModel>(item);
            var listFieldsModel = _modelHelper.GetModelFields<GetCustomerViewModel>();

            // shape data
            var shapeData = _dataShaperGetCustomerViewModel.ShapeData(itemModel, String.Join(",", listFieldsModel));

            return shapeData;
        }
        public async Task<(IEnumerable<Entity> data, RecordsCount recordsCount)> QueryPagedCustomerReponseAsync(QueryCustomer requestParameter)
        {

            var searchString = requestParameter.SearchString;
 
            var pageNumber = requestParameter.PageNumber;
            var pageSize = requestParameter.PageSize;
            var orderBy = requestParameter.OrderBy;
            //      var fields = requestParameter.Fields;
            var fields = _modelHelper.GetQueryableFields<GetCustomerViewModel, Customer>();


            int recordsTotal, recordsFiltered;

            // Setup IQueryable
            var result = _users
                .AsNoTracking()
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
                result = result.Select<Customer>("new(" + fields + ")");
            }
            // paging
            result = result
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize);

            // retrieve data to list
            var resultData = await result.ToListAsync();

            //TODO: szebben megoldani
            var resultDataModel = new List<GetCustomerViewModel>();
            resultData.ForEach( i => resultDataModel.Add(
                _mapper.Map<Customer, GetCustomerViewModel>(i))
            );


            var listFieldsModel = _modelHelper.GetModelFields<GetCustomerViewModel>();

            var shapeData = _dataShaperGetCustomerViewModel.ShapeData(resultDataModel, String.Join(",", listFieldsModel));

            return (shapeData, recordsCount);
        }

        private void FilterBySearchString(ref IQueryable<Customer> p_item, string p_searchString)
        {
            if (!p_item.Any())
                return;

            if ( string.IsNullOrWhiteSpace(p_searchString))
                return;

            var predicate = PredicateBuilder.New<Customer>();

            var srcFor = p_searchString.ToUpper().Trim();
            predicate = predicate.And(p => p.CustomerName.ToUpper().Contains(srcFor) || p.TaxpayerId.ToUpper().Contains(srcFor));

            p_item = p_item.Where(predicate);
        }

        public Task<bool> SeedDataAsync(int rowCount)
        {
            throw new System.NotImplementedException();
        }

 
    }
}