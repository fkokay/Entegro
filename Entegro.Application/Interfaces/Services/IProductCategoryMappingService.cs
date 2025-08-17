using Entegro.Application.DTOs.ProductCategory;

namespace Entegro.Application.Interfaces.Services
{
    public interface IProductCategoryMappingService
    {
        Task<ProductCategoryDto> GetProductCategoryByIdAsync(int productCategoryId);
        Task<IEnumerable<ProductCategoryDto>> GetProductCategorysAsync();
        Task<int> CreateProductCategoryAsync(CreateProductCategoryDto createProductCategory);
        Task<bool> UpdateProductCategoryAsync(UpdateProductCategoryDto updateProductCategory);
        Task<bool> DeleteProductCategoryAsync(int productCategoryId);

        Task<IReadOnlyList<ProductCategoryParhDto>> GetCategoryPathsByProductAsync(int productId, CancellationToken ct = default);
        Task<IReadOnlyDictionary<int, IReadOnlyList<ProductCategoryParhDto>>> GetCategoryPathsByProductsAsync(IEnumerable<int> productIds, CancellationToken ct = default);


    }
}
