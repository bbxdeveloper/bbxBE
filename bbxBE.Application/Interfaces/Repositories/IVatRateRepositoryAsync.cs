using bbxBE.Application.Interfaces;
using bbxBE.Application.Interfaces.Queries;
using bbxBE.Application.Parameters;
using bbxBE.Application.Queries.qProductGroup;
using bbxBE.Application.Queries.qVatRate;
using bbxBE.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace bbxBE.Application.Interfaces.Repositories
{
    public interface IVatRateRepositoryAsync : IGenericRepositoryAsync<VatRate>
    {
    
        Task<Entity> GetVatRateAsync(long ID);
        Task<VatRate> GetVatRateByCodeAsync(string vatRateCode);
        (IEnumerable<Entity> data, RecordsCount recordsCount) QueryPagedVatRate(QueryVatRate requestParameter);
        Task<VatRate> AddVatRateAsync(VatRate p_vatRate);
        Task<VatRate> UpdateVatRateAsync(VatRate p_vatRate);
        Task<VatRate> DeleteVatRateAsync(long ID);
        Task RefreshVatRateCache();
    }
}