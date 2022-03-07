//using bbxBE.Application.Features.Positions.Queries.GetPositions;
using bbxBE.Application.Interfaces;
using bbxBE.Application.Interfaces.Queries;
using bbxBE.Application.Parameters;
using bbxBE.Application.Queries.qCounter;
using bbxBE.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace bbxBE.Application.Interfaces.Repositories
{
    public interface ICounterRepositoryAsync : IGenericRepositoryAsync<Counter>
    {
        Task<bool> IsUniqueCounterCodeAsync(string CounterCode, long? ID = null);
        Task<bool> SeedDataAsync(int rowCount);

        Task<Entity> GetCounterAsync(GetCounter requestParameters);
        Task<(IEnumerable<Entity> data, RecordsCount recordsCount)> QueryPagedCounterAsync(QueryCounter requestParameters);
    }
}