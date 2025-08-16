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

    }
}
