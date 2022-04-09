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
        Task<bool> IsUniqueTaxpayerIdAsync(string TaxpayerId, long? ID = null);
        Task<bool> IsUniqueIsOwnDataAsync( long? ID = null);
        Task<bool> CheckBankAccountAsync(string bankAccountNumber);
        Task<bool> SeedDataAsync(int rowCount);

        Task<Entity> GetCustomerAsync(GetCustomer requestParameters);
        Task<(IEnumerable<Entity> data, RecordsCount recordsCount)> QueryPagedCustomerAsync(QueryCustomer requestParameters);
    }
}