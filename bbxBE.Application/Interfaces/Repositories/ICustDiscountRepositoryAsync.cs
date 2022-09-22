
using bbxBE.Application.Interfaces;
using bbxBE.Application.Interfaces.Queries;
using bbxBE.Application.Parameters;
using bbxBE.Application.Queries.qCustDiscount;
using bbxBE.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace bbxBE.Application.Interfaces.Repositories
{
    public interface ICustDiscountRepositoryAsync : IGenericRepositoryAsync<CustDiscount>
    {
        Task<long> MaintanenceCustDiscountRangeAsync(List<CustDiscount> p_CustDiscountList);
        Task<Entity> GetCustDiscountAsync(long ID);
        Task<List<Entity>> GetCustDiscountForCustomerListAsync(long customerID);
     Task<bool> SeedDataAsync(int rowCount);
          }
}