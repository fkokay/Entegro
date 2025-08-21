using Entegro.Application.DTOs.Common;
using Entegro.Domain.Entities;

namespace Entegro.Application.Interfaces.Repositories
{
    public interface IProductAttributeMappingRepository
    {
        Task<ProductAttributeMapping?> GetByIdAsync(int id);
        Task<ProductAttributeMapping?> GetByAttributeIdAsync(int id);
        Task<List<ProductAttributeMapping>> GetAllAsync();
        Task<PagedResult<ProductAttributeMapping>> GetAllAsync(int pageNumber, int pageSize);
        Task AddAsync(ProductAttributeMapping productAttributeMapping);
        Task UpdateAsync(ProductAttributeMapping productAttributeMapping);
        Task DeleteAsync(ProductAttributeMapping productAttributeMapping);
    }
}
