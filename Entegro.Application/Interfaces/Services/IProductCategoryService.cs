using Entegro.Application.DTOs.ProductCategory;

namespace Entegro.Application.Interfaces.Services
{
    public interface IProductCategoryService
    {
        Task<ProductCategoryDto> GetProductCategoryByIdAsync(int productCategoryId);
        Task<IEnumerable<ProductCategoryDto>> GetProductCategorysAsync();
        Task<ProductCategoryDto> CreateProductCategoryAsync(CreateProductCategoryDto createProductCategory);
        Task<ProductCategoryDto> UpdateProductCategoryAsync(UpdateProductCategoryDto updateProductCategory);
        Task<List<ProductCategoryDto>> GetByProductWithCategoryAsync(int productId);
        Task DeleteProductCategoryAsync(int productCategoryId);
    }
}
