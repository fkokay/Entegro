using Entegro.Application.DTOs.ProductCategory;

namespace Entegro.Application.Interfaces.Services.Base
{
    public interface IProductCategoryService
    {
        Task<ProductCategoryDto> GetProductCategoryByIdAsync(int productCategoryId);
        Task<IEnumerable<ProductCategoryDto>> GetProductCategorysAsync();
        Task<ProductCategoryDto> AddAsync(CreateProductCategoryDto createProductCategory);
        Task<ProductCategoryDto> UpdateAsync(UpdateProductCategoryDto updateProductCategory);
        Task<List<ProductCategoryDto>> GetByProductWithCategoryAsync(int productId);
        Task DeleteAsync(int productCategoryId);
    }
}
