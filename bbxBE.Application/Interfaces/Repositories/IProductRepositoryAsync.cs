using bbxBE.Application.Parameters;
using bbxBE.Application.Queries.qProduct;
using bbxBE.Application.Queries.ViewModels;
using bbxBE.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace bbxBE.Application.Interfaces.Repositories
{
    public interface IProductRepositoryAsync : IGenericRepositoryAsync<Product>
    {
        bool IsUniqueProductCode(string ProductCode, long? ID = null);
        Task<bool> CheckProductGroupCodeAsync(string ProductGroupCode);
        Task<bool> CheckOriginCodeAsync(string OriginCode);
        Task<bool> CheckVatRateCodeAsync(string VatRateCode);
        Task<Product> AddProductAsync(Product p_product, string p_ProductGroupCode, string p_OriginCode, string p_VatRateCode);
        Task<int> AddProductRangeAsync(List<Product> p_productList, List<string> p_ProductGroupCodeList, List<string> p_OriginCodeList, List<string> p_VatRateCodeList);
        Task<Product> UpdateProductAsync(Product p_product, string p_ProductGroupCode, string p_OriginCode, string p_VatRateCode);
        Task<int> UpdateProductRangeAsync(List<Product> p_productList, List<string> p_ProductGroupCodeList, List<string> p_OriginCodeList, List<string> p_VatRateCodeList);
        Task<int> UpdateProductRangeAsync(List<Product> p_productList, bool p_saveChanges);
        Task<Product> DeleteProductAsync(long ID);

        Task<bool> SeedDataAsync(int rowCount);
        Product GetProduct(long ID);
        Entity GetProductEntity(long ID);
        Product GetProductByProductCode(string productCode);
        Entity GetProductEntityByProductCode(string productCode);
        Task<(IEnumerable<Entity> data, RecordsCount recordsCount)> QueryPagedProductAsync(QueryProduct requestParameters);
        Task<List<Product>> GetAllProductsFromDBAsync();
        List<GetProductViewModel> GetAllProductsFromCache();
        Task RefreshProductCache();
    }
}