//using bbxBE.Application.Features.Positions.Queries.GetPositions;
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
        Task<bool> IsUniqueOriginCodeAsync(string OriginCode, long? ID = null);
        Task<bool> SeedDataAsync(int rowCount);

        Task<Entity> GetOriginReponseAsync(GetOrigin requestParameters);
        Task<(IEnumerable<Entity> data, RecordsCount recordsCount)> QueryPagedOriginReponseAsync(QueryOrigin requestParameters);
    }
}