
using bbxBE.Application.Interfaces;
using bbxBE.Application.Interfaces.Queries;
using bbxBE.Application.Parameters;
using bbxBE.Application.Queries.qOrigin;
using bbxBE.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace bbxBE.Application.Interfaces.Repositories
{
    public interface IOriginRepositoryAsync : IGenericRepositoryAsync<Origin>
    {
        bool IsUniqueOriginCode(string OriginCode, long? ID = null);
        Task<bool> SeedDataAsync(int rowCount);
        Task<Origin> AddOriginAsync(Origin p_origin);
        Task<long> AddOriginRangeAsync(List<Origin> p_originList);

        Task<Origin> UpdateOriginAsync(Origin p_origin);
        Task<long> UpdateOriginRangeAsync(List<Origin> p_originList);

        Task<Origin> DeleteOriginAsync(long ID);
        Entity GetOrigin(GetOrigin requestParameters);
        List<Entity> GetOriginList();
        Task<(IEnumerable<Entity> data, RecordsCount recordsCount)> QueryPagedOriginAsync(QueryOrigin requestParameters);
    }
}