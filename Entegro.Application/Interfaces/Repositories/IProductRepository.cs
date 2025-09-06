using Entegro.Application.DTOs.Common;
using Entegro.Domain.Entities;
using System.Linq.Expressions;

namespace Entegro.Application.Interfaces.Repositories
{
    public interface IProductRepository
    {
        Task<bool> ExistsAsync(Expression<Func<Product, bool>> predicate);
        Task<Product?> GetByAsync(Expression<Func<Product, bool>> predicate);
        Task<List<Product>> GetAllAsync();
        Task<PagedResult<Product>> GetPagedAsync(GridCommand gridCommand);
        Task AddAsync(Product product);
        Task UpdateAsync(Product product);
        Task UpdateMainPictureIdAsync(int productId, int mainPictureId);
        Task DeleteAsync(Product product);
    }
}
