using Entegro.Application.DTOs.Common;
using Entegro.Domain.Entities.Catalog;
using System.Linq.Expressions;

namespace Entegro.Application.Interfaces.Repositories
{
    public interface ICategoryRepository
    {
        Task<bool> ExistsAsync(Expression<Func<Category, bool>> predicate);
        Task<Category?> GetByAsync(Expression<Func<Category, bool>> predicate);
        Task<List<Category>> GetManyAsync(IEnumerable<int> ids, bool tracked = false);
        Task<List<Category>> GetAllAsync(bool includeHidden = false);
        Task<PagedResult<Category>> GetAllAsync(string term, int pageNumber, int pageSize);
        Task AddAsync(Category category);
        Task UpdateAsync(Category category);
        Task DeleteAsync(Category category);
    }
}
