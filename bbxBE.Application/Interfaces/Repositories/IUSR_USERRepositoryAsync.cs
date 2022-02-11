//using bbxBE.Application.Features.Positions.Queries.GetPositions;
using bbxBE.Application.Interfaces;
using bbxBE.Application.Interfaces.Queries;
using bbxBE.Application.Parameters;
using bbxBE.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace bbxBE.Application.Interfaces.Repositories
{
    public interface IUSR_USERRepositoryAsync : IGenericRepositoryAsync<USR_USER>
    {
        Task<bool> IsUniqueNameAsync(string USR_NAME, long? ID = null);

        Task<bool> SeedDataAsync(int rowCount);

        Task<Entity> GetUSR_USERReponseAsync(object requestParameters);
        Task<(IEnumerable<Entity> data, RecordsCount recordsCount)> QueryPagedUSR_USERReponseAsync(object requestParameters);
    }
}