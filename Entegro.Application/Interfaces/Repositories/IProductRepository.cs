using Entegro.Application.DTOs.Common;
using Entegro.Domain.Entities;
using System.Linq.Expressions;

namespace Entegro.Application.Interfaces.Repositories
{
    public interface IProductRepository
    {
        Task<bool> ExistsAsync(Expression<Func<Product, bool>> predicate);
        Task<Product?> GetByIdAsync(int id);
        Task<Product?> GetByCodeAsync(string productCode);
        Task<Product?> GetByBarcodeAsync(string productBarcode);
        Task<List<Product>> GetAllAsync();
        Task<PagedResult<Product>> GetAllAsync(int pageNumber, int pageSize);
        Task AddAsync(Product product);
        Task UpdateAsync(Product product);
        Task UpdateMainPictureIdAsync(int productId, int mainPictureId);
        Task DeleteAsync(Product product);
    }
}
