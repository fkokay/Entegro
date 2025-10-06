using Entegro.Application.DTOs.Common;
using Entegro.Domain.Entities.Catalog;

namespace Entegro.Application.Interfaces.Repositories
{
    public interface ISpecificationAttributeOptionRepository
    {
        Task<bool> ExistsByIdAsync(int id);
        Task<bool> ExistsByNameAsync(string name);
        Task<SpecificationAttributeOption?> GetByIdAsync(int id);
        Task<SpecificationAttributeOption?> GetByNameAsync(string name);
        Task<List<SpecificationAttributeOption>> GetAllAsync();
        Task<PagedResult<SpecificationAttributeOption>> GetAllAsync(int pageNumber, int pageSize);
        Task AddAsync(SpecificationAttributeOption specificationAttributeOption);
        Task UpdateAsync(SpecificationAttributeOption specificationAttributeOption);
        Task DeleteAsync(SpecificationAttributeOption specificationAttributeOption);
        Task<PagedResult<SpecificationAttributeOption>> GetPagedAsync(GridCommand gridCommand, int specificationAttributeId);

    }
}
