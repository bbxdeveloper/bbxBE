using bbxBE.Application.Interfaces;
using bbxBE.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace bbxBE.Application.Commands.cmdImport
{
    public interface ICustomerRepositoryAsync : IGenericRepositoryAsync<Customer>
    {
        bool IsUniqueProductCode(string CustomerCode, long? ID = null);
        Task<int> AddProductRangeAsync(List<Customer> p_customerList);
        Task<int> UpdateProductRangeAsync(List<Customer> p_customerList);
    }
}