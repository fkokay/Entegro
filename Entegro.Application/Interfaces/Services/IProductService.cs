using Entegro.Application.DTOs.Common;
using Entegro.Application.DTOs.Product;

namespace Entegro.Application.Interfaces.Services
{
    public interface IProductService
    {
        Task<ProductDto> GetProductByIdAsync(int productId);
        Task<bool> ExistsByNameAsync(string productName);
        Task<bool> ExistsByCodeAsync(string productCode);
        Task<IEnumerable<ProductDto>> GetProductsAsync();
        Task<List<int>> GetAllProductIdAsync();
        Task<PagedResult<ProductDto>> GetProductsAsync(int pageNumber, int pageSize);
        Task<int> CreateProductAsync(CreateProductDto createProduct);
        Task<bool> UpdateProductAsync(UpdateProductDto updateProduct);
        Task<bool> DeleteProductAsync(int productId);
    }
}
