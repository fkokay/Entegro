using Entegro.Domain.Entities;

namespace Entegro.Application.Interfaces.Repositories
{
    public interface IProductImageMappingRepository
    {
        Task<ProductCategoryMapping?> GetByIdAsync(int id);
        Task<List<ProductCategoryMapping>> GetAllAsync();
        Task AddAsync(ProductCategoryMapping brand);
        Task UpdateAsync(ProductCategoryMapping brand);
        Task DeleteAsync(ProductCategoryMapping brand);
    }
}
