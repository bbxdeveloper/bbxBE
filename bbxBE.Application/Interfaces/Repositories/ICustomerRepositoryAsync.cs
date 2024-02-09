using bbxBE.Application.Parameters;
using bbxBE.Application.Queries.qCustomer;
using bbxBE.Common.Enums;
using bbxBE.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace bbxBE.Application.Interfaces.Repositories
{
    public interface ICustomerRepositoryAsync : IGenericRepositoryAsync<Customer>
    {

        bool IsUniqueTaxpayerId(string Taxpaxpayer, long? ID = null);
        bool IsUniqueIsOwnData(long? ID = null);
        enValidateBankAccountResult CheckCustomerBankAccount(string BankAccount);
        bool CheckCountryCode(string countryCode);
        bool CheckUnitPriceType(string unitPriceType);
        bool CheckTaxPayerNumber(string p_CheckTaxPayerNumber);
        Task<Customer> AddCustomerAsync(Customer p_customer);
        Task<int> AddCustomerRangeAsync(List<Customer> p_customerList);
        Task<Customer> DeleteCustomerAsync(long ID);
        Task<Customer> UpdateCustomerAsync(Customer p_customer);
        Task<Customer> UpdateCustomerLatestDiscountPercentAsync(long ID, decimal LatestDiscountPercent);
        Customer GetOwnData();
        Customer GetCustomerRecord(long customerID);
        Customer GetCustomerRecordByTaxpayerId(string taxpaxpayerID);
        List<Customer> GetCustomerRecordsByName(string name);
        Entity GetCustomer(long customerID);
        Task<(IEnumerable<Entity> data, RecordsCount recordsCount)> QueryPagedCustomerAsync(QueryCustomer requestParamter);
    }
}