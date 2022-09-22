
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
        Task<bool> SeedDataAsync(int rowCount);
        Task<CustDiscount> AddCustDiscountAsync(CustDiscount p_CustDiscount);
        Task<long> AddCustDiscountRangeAsync(List<CustDiscount> p_CustDiscountList);

        Task<CustDiscount> UpdateCustDiscountAsync(CustDiscount p_CustDiscount);
        Task<long> UpdateCustDiscountRangeAsync(List<CustDiscount> p_CustDiscountList);

        Task<CustDiscount> DeleteCustDiscountAsync(long ID);
        Entity GetCustDiscount(GetCustDiscount requestParameters);
        List<Entity> GetCustDiscountList();
        Task<(IEnumerable<Entity> data, RecordsCount recordsCount)> QueryPagedCustDiscountAsync(QueryCustDiscount requestParameters);
    }
}