using Entegro.Application.DTOs.Common;
using Entegro.Domain.Entities;

namespace Entegro.Application.Interfaces.Repositories
{
    public interface IProductRepository
    {
        Task<Product?> GetByIdAsync(int id);
        Task<Product?> GetByCodeAsync(string productCode);
        Task<bool> ExistsByNameAsync(string productName);
        Task<bool> ExistsByCodeAsync(string productCode);
        Task<List<Product>> GetAllAsync();
        Task<List<Product>> GetAllAsync(List<int> productIds);
        Task<PagedResult<Product>> GetAllAsync(int pageNumber, int pageSize);
        Task AddAsync(Product product);
        Task UpdateAsync(Product product);
        Task UpdateMainPictureIdAsync(int productId, int mainPictureId);
        Task DeleteAsync(Product product);
    }
}
