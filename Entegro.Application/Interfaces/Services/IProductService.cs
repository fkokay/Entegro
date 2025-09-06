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
        Task<ProductDto?> GetProductByBarcodeAsync(string productBarcode);
        Task<IEnumerable<ProductDto>> GetProductsAsync();
        Task<PagedResult<ProductDto>> GetPagedAsync(GridCommand gridCommand);
        Task<ProductDto> CreateProductAsync(CreateProductDto createProduct);
        Task<ProductDto> UpdateProductAsync(UpdateProductDto updateProduct);
        Task<bool> UpdateProductMainPictureIdAsync(int productId, int mainPictureId);
        Task DeleteProductAsync(int productId);
    }
}
