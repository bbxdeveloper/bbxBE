using bbxBE.Application.Interfaces;
using bbxBE.Application.Parameters;
using bbxBE.Application.Queries.qCustomer;
using bbxBE.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace bbxBE.Application.Interfaces.Repositories
{
    public interface ICustomerRepositoryAsync : IGenericRepositoryAsync<Customer>
    {
        bool IsUniqueCustomerCode(string CustomerCode, long? ID = null);
        bool IsUniqueTaxpayerId(string Taxpaxpayer, long? ID = null);
        bool IsUniqueIsOwnData(long? ID = null);
        bool CheckBankAccount(string BankAccount);
        Task<int> AddCustomerRangeAsync(List<Customer> p_customerList);
        Task<int> AddCustomerAsync(Customer p_customer);
        Task<int> DeleteCustomerAsync(long ID);
        Task<int> UpdateCustomerRangeAsync(List<Customer> p_customerList);
        Task<int> UpdateCustomerAsync(Customer p_customer);
        Customer GetCustomer(long customerID);
        Entity GetCustomer(GetCustomer requestParameters);
        Entity GetCustomer(GetCustomer requestParameters);
        Task<(IEnumerable<Entity> data, RecordsCount recordsCount)> QueryPagedCustomerAsync(QueryCustomer requestParamter);
    }
}