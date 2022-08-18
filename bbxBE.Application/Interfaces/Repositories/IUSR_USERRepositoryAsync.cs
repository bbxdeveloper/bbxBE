using bbxBE.Application.Interfaces;
using bbxBE.Application.Interfaces.Queries;
using bbxBE.Application.Parameters;
using bbxBE.Application.Queries.qUSR_USER;
using bbxBE.Application.Queries.ViewModels;
using bbxBE.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace bbxBE.Application.Interfaces.Repositories
{
    public interface IUSR_USERRepositoryAsync : IGenericRepositoryAsync<USR_USER>
    {
        Task<bool> IsUniqueNameAsync(string USR_NAME, long? ID = null);

        Task<bool> SeedDataAsync(int rowCount);

        Task<Entity> GetUSR_USERAsync(GetUSR_USER requestParameters);
        Task<(IEnumerable<Entity> data, RecordsCount recordsCount)> QueryPagedUSR_USERAsync(QueryUSR_USER requestParameters);
    }
}