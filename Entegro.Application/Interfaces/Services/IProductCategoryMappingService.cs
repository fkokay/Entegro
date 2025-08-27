using Entegro.Application.DTOs.ProductCategory;

namespace Entegro.Application.Interfaces.Services
{
    public interface IProductCategoryMappingService
    {
        Task<ProductCategoryDto> GetProductCategoryByIdAsync(int productCategoryId);
        Task<IEnumerable<ProductCategoryDto>> GetProductCategorysAsync();
        Task<ProductCategoryDto> CreateProductCategoryAsync(CreateProductCategoryDto createProductCategory);
        Task<ProductCategoryDto> UpdateProductCategoryAsync(UpdateProductCategoryDto updateProductCategory);
        Task DeleteProductCategoryAsync(int productCategoryId);
        Task<IReadOnlyList<ProductCategoryPathDto>> GetCategoryPathsByProductAsync(int productId);
        Task<IReadOnlyDictionary<int, IReadOnlyList<ProductCategoryPathDto>>> GetCategoryPathsByProductsAsync(IEnumerable<int> productIds);
    }
}
