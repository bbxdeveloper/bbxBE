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
        Task<Counter> AddCounterAsync(Counter p_Counter, string p_WarehouseCode);
        Task<Counter> UpdateCounterAsync(Counter p_Counter, string p_WarehouseCode);
        Task<Entity> GetCounterAsync(GetCounter requestParameters);
        Task<string> GetNextValueAsync(string CounterCode, long WarehouseID, bool useCounterPool = true);
        Task<bool> FinalizeValueAsync(string CounterCode, long WarehouseID, string counterValue);
        Task<bool> RollbackValueAsync(string CounterCode, long WarehouseID, string counterValue);
        Task<(IEnumerable<Entity> data, RecordsCount recordsCount)> QueryPagedCounterAsync(QueryCounter requestParameters);
    }
}