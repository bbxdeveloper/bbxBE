
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
        bool IsUniqueProductGroupCode(string ProductGroupCode, long? ID = null);

        Task<ProductGroup> AddProudctGroupAsync(ProductGroup p_productGroup);
        Task<long> AddProudctGroupRangeAsync(List<ProductGroup> p_productGroupList);
        Task<ProductGroup> UpdateProductGroupAsync(ProductGroup p_productGroup);
        Task<long> UpdateProductGroupRangeAsync(List<ProductGroup> p_productGroupList);
        Task<ProductGroup> DeleteProductGroupAsync(long ID);
        Task<bool> SeedDataAsync(int rowCount);

        Entity GetProductGroup(long ID);
        Task<(IEnumerable<Entity> data, RecordsCount recordsCount)> QueryPagedProductGroupAsync(QueryProductGroup requestParameters);


    }
}