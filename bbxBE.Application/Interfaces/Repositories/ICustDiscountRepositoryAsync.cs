using bbxBE.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace bbxBE.Application.Interfaces.Repositories
{
    public interface ICustDiscountRepositoryAsync : IGenericRepositoryAsync<CustDiscount>
    {
        Task<long> MaintanenceCustDiscountRangeAsync(List<CustDiscount> p_CustDiscountList, long customerID);
        Task<Entity> GetCustDiscountAsync(long ID);
        Task<List<Entity>> GetCustDiscountForCustomerListAsync(long customerID);
        Task<bool> SeedDataAsync(int rowCount);
    }
}