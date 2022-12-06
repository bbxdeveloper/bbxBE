using bbxBE.Application.Interfaces;
using bbxBE.Application.Interfaces.Queries;
using bbxBE.Application.Parameters;
using bbxBE.Application.Queries.qLocation;
using bbxBE.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace bbxBE.Application.Interfaces.Repositories
{
    public interface ILocationRepositoryAsync : IGenericRepositoryAsync<Location>
    {
        Task<bool> IsUniqueLocationCodeAsync(string LocationCode, long? ID = null);

        Task<Location> AddLocationAsync(Location p_Location);

        Task<Location> UpdateLocationAsync(Location p_Location);

        Task<Location> DeleteLocationAsync(long ID);

        Task<bool> SeedDataAsync(int rowCount);
        Task<Location> GetLocationByCodeAsync(string LocationCode);

        Task<Entity> GetLocationAsync(long ID);
        Task<(IEnumerable<Entity> data, RecordsCount recordsCount)> QueryPagedLocationAsync(QueryLocation requestParameters);
    }
}