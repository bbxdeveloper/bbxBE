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
using bbxBE.Common.Exceptions;
using bbxBE.Common.Consts;
using EFCore.BulkExtensions;

namespace bbxBE.Infrastructure.Persistence.Repositories
{
    public class CustomerRepositoryAsync : GenericRepositoryAsync<Customer>, ICustomerRepositoryAsync
    {
        private readonly ApplicationDbContext _dbContext;
        private IDataShapeHelper<Customer> _dataShaperCustomer;
        private IDataShapeHelper<GetCustomerViewModel> _dataShaperGetCustomerViewModel;
        private readonly IMockService _mockData;
        private readonly IModelHelper _modelHelper;
        private readonly IMapper _mapper;
        private readonly ICacheService<Customer> _cacheService;

        public CustomerRepositoryAsync(ApplicationDbContext dbContext,
            IDataShapeHelper<Customer> dataShaperCustomer,
            IDataShapeHelper<GetCustomerViewModel> dataShaperGetCustomerViewModel,
            IModelHelper modelHelper, IMapper mapper, IMockService mockData,
            ICacheService<Customer> customerCacheService) : base(dbContext)
        {
            _dbContext = dbContext;
            _dataShaperCustomer = dataShaperCustomer;
            _dataShaperGetCustomerViewModel = dataShaperGetCustomerViewModel;
            _modelHelper = modelHelper;
            _mapper = mapper;
            _mockData = mockData;
            _cacheService = customerCacheService;
        }


        public bool IsUniqueTaxpayerId(string TaxpayerId, long? ID = null)
        {
            var query = _cacheService.QueryCache();
            return !query.ToList().Any(p => p.TaxpayerId == TaxpayerId && !p.Deleted && (ID == null || p.ID != ID.Value));
        }

        public bool IsUniqueIsOwnData(long? ID = null)
        {
            var query = _cacheService.QueryCache();
            return !query.ToList().Any(p => p.IsOwnData && !p.Deleted && (ID == null || p.ID != ID.Value));
        }

        public bool CheckBankAccount(string bankAccountNumber)
        {
            if (string.IsNullOrWhiteSpace(bankAccountNumber))
                return true;

            return bllCustomer.ValidateBankAccount(bankAccountNumber) || bllCustomer.ValidateIBAN(bankAccountNumber);
        }

        public async Task<Customer> AddCustomerAsync(Customer p_customer)
        {

            await _dbContext.Customer.AddAsync(p_customer);
            //            _dbContext.ChangeTracker.AcceptAllChanges();
            await _dbContext.SaveChangesAsync();

            _cacheService.AddOrUpdate(p_customer);
            return p_customer;
        }
        public async Task<int> AddCustomerRangeAsync(List<Customer> p_customerList)
        {

            _dbContext.Database.SetCommandTimeout(3600);
            await _dbContext.BulkInsertAsync(p_customerList, new BulkConfig
            {
                SetOutputIdentity = true,
                PreserveInsertOrder = true,
                BulkCopyTimeout = 0,
                WithHoldlock = false,
                BatchSize = 5000
            });
            await _dbContext.SaveChangesAsync();

            return p_customerList.Count;

        }

        public async Task<Customer> UpdateCustomerAsync(Customer p_customer)
        {
            _cacheService.AddOrUpdate(p_customer);
            _dbContext.Customer.Update(p_customer);
            await _dbContext.SaveChangesAsync();
            return p_customer;
        }

        public async Task<Customer> DeleteCustomerAsync(long ID)
        {

            Customer cust = null;
            using (var dbContextTransaction = await _dbContext.Database.BeginTransactionAsync())
            {
                cust = await _dbContext.Customer.Where(x => x.ID == ID).FirstOrDefaultAsync();

                if (cust != null)
                {


                    _cacheService.TryRemove(cust);

                    _dbContext.Customer.Remove(cust);

                    await _dbContext.SaveChangesAsync();
                    await dbContextTransaction.CommitAsync();

                }
                else
                {
                    throw new ResourceNotFoundException(string.Format(bbxBEConsts.FV_CUSTNOTFOUND, ID));
                }
            }
            return cust;
        }
        public Customer GetOwnData()
        {
            var query = _cacheService.QueryCache();
            return query.SingleOrDefault(s => s.IsOwnData);
        }

        public Customer GetCustomerRecord(long customerID)
        {
            Customer cust = null;
            if (!_cacheService.TryGetValue(customerID, out cust))
                throw new ResourceNotFoundException(string.Format(bbxBEConsts.FV_CUSTNOTFOUND, customerID));
            return cust;
        }

        public Entity GetCustomer(long customerID)
        {
            var cust = GetCustomerRecord(customerID);

            var itemModel = _mapper.Map<Customer, GetCustomerViewModel>(cust);
            var listFieldsModel = _modelHelper.GetModelFields<GetCustomerViewModel>();

            // shape data
            var shapeData = _dataShaperGetCustomerViewModel.ShapeData(itemModel, String.Join(",", listFieldsModel));

            return shapeData;
        }

        public async Task<(IEnumerable<Entity> data, RecordsCount recordsCount)> QueryPagedCustomerAsync(QueryCustomer requestParameter)
        {

            var searchString = requestParameter.SearchString;
            var IsOwnData = requestParameter.IsOwnData;

            var pageNumber = requestParameter.PageNumber;
            var pageSize = requestParameter.PageSize;
            var orderBy = requestParameter.OrderBy;
            //      var fields = requestParameter.Fields;
            var fields = _modelHelper.GetQueryableFields<GetCustomerViewModel, Customer>();


            int recordsTotal, recordsFiltered;

            // Setup IQueryable

            var query = _cacheService.QueryCache();

            // Count records total
            recordsTotal = query.Count();

            // filter data
            FilterBySearchString(ref query, searchString, IsOwnData);

            // Count records after filter
            recordsFiltered = query.Count();

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
                query = query.Select<Customer>("new(" + fields + ")");
            }
            // paging
            query = query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize);

            // retrieve data to list
            var resultData = query.ToList();

            //TODO: szebben megoldani
            var resultDataModel = new List<GetCustomerViewModel>();
            resultData.ForEach(i => resultDataModel.Add(
               _mapper.Map<Customer, GetCustomerViewModel>(i))
            );


            var listFieldsModel = _modelHelper.GetModelFields<GetCustomerViewModel>();

            var shapeData = _dataShaperGetCustomerViewModel.ShapeData(resultDataModel, String.Join(",", listFieldsModel));

            return (shapeData, recordsCount);
        }

        private void FilterBySearchString(ref IQueryable<Customer> p_item, string p_searchString, bool? IsOwnData = null)
        {
            if (!p_item.Any())
                return;

            if (string.IsNullOrWhiteSpace(p_searchString) && IsOwnData == null)
                return;

            var predicate = PredicateBuilder.New<Customer>();


            var srcFor = "";
            if (p_searchString != null)
            {
                srcFor = p_searchString.ToUpper().Trim();
            }

            if (IsOwnData == null)
            {
                predicate = predicate.And(p => p.CustomerName != null && p.TaxpayerId != null && (p.CustomerName.ToUpper().Contains(srcFor) || p.TaxpayerId.ToUpper().Contains(srcFor)));
            }
            else if (IsOwnData.Value)
            {
                predicate = predicate.And(p => (p.CustomerName != null && p.TaxpayerId != null && (p.CustomerName.ToUpper().Contains(srcFor) || p.TaxpayerId.ToUpper().Contains(srcFor))) && p.IsOwnData);

            }
            else
            {
                predicate = predicate.And(p => (p.CustomerName != null && p.TaxpayerId != null && (p.CustomerName.ToUpper().Contains(srcFor) || p.TaxpayerId.ToUpper().Contains(srcFor))) && !p.IsOwnData);
            }

            p_item = p_item.Where(predicate);
        }

        public Task<bool> SeedDataAsync(int rowCount)
        {
            throw new System.NotImplementedException();
        }
    
    }
}