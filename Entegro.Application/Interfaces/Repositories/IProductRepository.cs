using Entegro.Application.DTOs.Common;
using Entegro.Domain.Entities;

namespace Entegro.Application.Interfaces.Repositories
{
    public interface IProductRepository
    {
        Task<bool> ExistsByNameAsync(string productName);
        Task<bool> ExistsByCodeAsync(string productCode);
        Task<bool> ExistsByBarcodeAsync(string productBarcode);
        Task<Product?> GetByIdAsync(int id);
        Task<Product?> GetByCodeAsync(string productCode);
        Task<Product?> GetByBarcodeAsync(string productBarcode);
        Task<List<Product>> GetAllAsync();
        Task<List<Product>> GetAllAsync(List<int> productIds);
        Task<PagedResult<Product>> GetAllAsync(int pageNumber, int pageSize);
        Task AddAsync(Product product);
        Task UpdateAsync(Product product);
        Task UpdateMainPictureIdAsync(int productId, int mainPictureId);
        Task DeleteAsync(Product product);
    }
}
