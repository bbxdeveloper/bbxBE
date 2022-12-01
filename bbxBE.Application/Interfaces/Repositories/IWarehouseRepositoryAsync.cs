using bbxBE.Application.Interfaces;
using bbxBE.Application.Interfaces.Queries;
using bbxBE.Application.Parameters;
using bbxBE.Application.Queries.qWarehouse;
using bbxBE.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace bbxBE.Application.Interfaces.Repositories
{
    public interface IWarehouseRepositoryAsync : IGenericRepositoryAsync<Warehouse>
    {
        Task<bool> IsUniqueWarehouseCodeAsync(string WarehouseCode, long? ID = null);
        Task<Warehouse> AddWarehouseAsync(Warehouse p_Warehouse);
        Task<Warehouse> UpdateWarehouseAsync(Warehouse p_Warehouse);

        Task<Warehouse> DeleteWarehouseAsync(long ID);

        Task<bool> SeedDataAsync(int rowCount);
        Task<Warehouse> GetWarehouseByCodeAsync(string WarehouseCode);

        Task<Entity> GetWarehouseAsync(long ID);
        Task<(IEnumerable<Entity> data, RecordsCount recordsCount)> QueryPagedWarehouseAsync(QueryWarehouse requestParameters);
    }
}