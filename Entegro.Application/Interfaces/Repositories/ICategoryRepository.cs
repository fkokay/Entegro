using Entegro.Application.DTOs.Common;
using Entegro.Domain.Entities;

namespace Entegro.Application.Interfaces.Repositories
{
    public interface ICategoryRepository
    {
        Task<bool> ExistsByNameAsync(string name);
        Task<Category?> GetByIdAsync(int id);
        Task<Category?> GetByNameAsync(string name);
        Task<List<Category>> GetAllAsync();
        Task<PagedResult<Category>> GetAllAsync(int pageNumber, int pageSize);
        Task AddAsync(Category category);
        Task UpdateAsync(Category category);
        Task DeleteAsync(Category category);
        Task<List<Category>> GetByParentIdAsync(int parentCategoryId);


        Task<PagedResult2<CategorySlim>> SearchPagedAsync(string? term, int page, int pageSize, CancellationToken ct = default);
        Task<Dictionary<int, string>> GetNamesByIdsAsync(IEnumerable<int> ids, CancellationToken ct = default);
        Task<Category?> GetByIdWithMediaAsync(int id);

    }
}
