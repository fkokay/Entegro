using Entegro.Application.DTOs.Common;
using Entegro.Application.DTOs.Product;

namespace Entegro.Application.Interfaces.Services.Base
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
        Task<PagedResult<ProductDto>> GetProductsAsync(int page, string term);
        Task<PagedResult<ProductDto>> GetPagedAsync(GridCommand gridCommand);
        Task<ProductDto> AddAsync(CreateProductDto createProduct);
        Task<ProductDto> UpdateAsync(UpdateProductDto updateProduct);
        Task<bool> UpdateProductMainPictureIdAsync(int productId, int mainPictureId);
        Task DeleteAsync(int productId);
        Task<int> GetProductCountAsync();
        Task<List<ProductDto?>> GetProductIntegrationMatrixAsync(int pageNumber, int pageSize, int brandId);
        Task<List<ProductDto?>> GetProductIntegrationMatrixAsync(int brandId);
        Task<ProductDto?> GetProductIntegrationMatrixByIdAsync(int productId);
        Task<List<ProductDto>> GetTopOrRandomProductsAsync(int topCount = 10, int orderStatusId = 30);
    }
}
