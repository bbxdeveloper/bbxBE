//using bbxBE.Application.Features.Positions.Queries.GetPositions;
using bbxBE.Application.Interfaces;
using bbxBE.Application.Interfaces.Queries;
using bbxBE.Application.Parameters;
using bbxBE.Application.Queries.qCustomer;
using bbxBE.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace bbxBE.Application.Interfaces.Repositories
{
    public interface ICustomerRepositoryAsync : IGenericRepositoryAsync<Customer>
    {
        bool IsUniqueTaxpayerId(string TaxpayerId, long? ID = null);
        bool IsUniqueIsOwnData( long? ID = null);
        bool CheckBankAccount(string bankAccountNumber);
        Task<bool> SeedDataAsync(int rowCount);
        Task<Customer> AddCustomerAsync(Customer p_customer);
        Task<Customer> UpdateCustomerAsync(Customer p_customer);
        Task<Customer> DeleteCustomerAsync(long ID);
        Customer GetOwnData();
        Customer GetCustomer(long customerID);
        Entity GetCustomer(GetCustomer requestParameters);
        Task<(IEnumerable<Entity> data, RecordsCount recordsCount)> QueryPagedCustomerAsync(QueryCustomer requestParameters);
    }
}