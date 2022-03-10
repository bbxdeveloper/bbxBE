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
        Task<bool> CheckProductGroupCodeAsync(string ProductGroupCode);
        Task<bool> CheckOriginCodeAsync(string OriginCode);
        Task<Product> AddProductAsync(Product p_product, ProductCode p_productCode, ProductCode p_VTSZ, ProductCode p_EAN, string p_ProductGroupCode, string p_OriginCode);
        Task<Product> UpdateProductAsync(Product p_product, ProductCode p_productCode, ProductCode p_VTSZ, ProductCode p_EAN, string p_ProductGroupCode, string p_OriginCode);
        Task<Product> DeleteProductAsync(long ID);

        Task<bool> SeedDataAsync(int rowCount);
        Task<Entity> GetProductAsync(GetProduct requestParameters);
        Task<(IEnumerable<Entity> data, RecordsCount recordsCount)> QueryPagedProductAsync(QueryProduct requestParameters);
    }
}