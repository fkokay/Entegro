using Entegro.Application.DTOs.Common;
using Entegro.Domain.Entities.Catalog;

namespace Entegro.Application.Interfaces.Repositories
{
    public interface ISpecificationAttributeRepository
    {
        Task<bool> ExistsByIdAsync(int id);
        Task<bool> ExistsByNameAsync(string name);
        Task<SpecificationAttribute?> GetByIdAsync(int id);
        Task<SpecificationAttribute?> GetByNameAsync(string name);
        Task<List<SpecificationAttribute>> GetAllAsync();
        Task<PagedResult<SpecificationAttribute>> GetAllAsync(int pageNumber, int pageSize);
        Task<PagedResult<SpecificationAttribute>> GetPagedAsync(GridCommand gridCommand);
        Task AddAsync(SpecificationAttribute specificationAttribute);
        Task UpdateAsync(SpecificationAttribute specificationAttribute);
        Task DeleteAsync(SpecificationAttribute specificationAttribute);
    }
}
