using Entegro.Application.DTOs.Common;
using Entegro.Application.DTOs.Product;

namespace Entegro.Application.Interfaces.Services
{
    public interface IProductService
    {
        Task<bool> ExistsByNameAsync(string productName);
        Task<bool> ExistsByCodeAsync(string productCode);
        Task<bool> ExistsByBarcodeAsync(string productBarcode);
        Task<ProductDto?> GetProductByIdAsync(int productId);
        Task<ProductDto?> GetProductByCodeAsync(string productCode);
        Task<ProductDto?> GetByBarcodeAsync(string productBarcode);
        Task<IEnumerable<ProductDto>> GetProductsAsync();
        Task<List<int>> GetAllProductIdAsync();
        Task<PagedResult<ProductDto>> GetPagedAsync(int pageNumber = 1, int pageSize = 7);
        Task<ProductDto> CreateProductAsync(CreateProductDto createProduct);
        Task<ProductDto> UpdateProductAsync(UpdateProductDto updateProduct);
        Task<bool> UpdateProductMainPictureIdAsync(int productId, int mainPictureId);
        Task DeleteProductAsync(int productId);
    }
}
