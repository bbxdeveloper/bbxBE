//using bbxBE.Application.Features.Positions.Queries.GetPositions;
using bbxBE.Application.Interfaces;
using bbxBE.Application.Interfaces.Queries;
using bbxBE.Application.Parameters;
using bbxBE.Application.Queries.qProduct;
using bbxBE.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace bbxBE.Application.Interfaces.Repositories
{
    public interface IProductRepositoryAsync : IGenericRepositoryAsync<Product>
    {
        Task<bool> IsUniqueProductCodeAsync(string ProductCode, long? ID = null);
        Task<bool> CheckProductGroupIDAsync(long ProductGroupID);
        Task<bool> CheckOriginIDAsync(long OriginID);
        Task<Product> AddProductAsync(Product p_product, ProductCode p_productCode, ProductCode p_VTSZ, ProductCode p_EAN);
        Task<Product> UpdateProductAsync(Product p_product, ProductCode p_productCode, ProductCode p_VTSZ, ProductCode p_EAN);
        Task<Product> DeleteProductAsync(long ID);

        Task<bool> SeedDataAsync(int rowCount);
        Task<Entity> GetProductAsync(GetProduct requestParameters);
        Task<(IEnumerable<Entity> data, RecordsCount recordsCount)> QueryPagedProductAsync(QueryProduct requestParameters);
    }
}