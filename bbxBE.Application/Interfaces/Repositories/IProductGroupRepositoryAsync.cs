//using bbxBE.Application.Features.Positions.Queries.GetPositions;
using bbxBE.Application.Interfaces;
using bbxBE.Application.Interfaces.Queries;
using bbxBE.Application.Parameters;
using bbxBE.Application.Queries.qProductGroup;
using bbxBE.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace bbxBE.Application.Interfaces.Repositories
{
    public interface IProductGroupRepositoryAsync : IGenericRepositoryAsync<ProductGroup>
    {
        Task<bool> IsUniqueProductGroupCodeAsync(string ProductGroupCode, long? ID = null);
        Task<bool> SeedDataAsync(int rowCount);

        Task<Entity> GetProductGroupAsync(GetProductGroup requestParameters);
        Task<(IEnumerable<Entity> data, RecordsCount recordsCount)> QueryPagedProductGroupAsync(QueryProductGroup requestParameters);
    }
}