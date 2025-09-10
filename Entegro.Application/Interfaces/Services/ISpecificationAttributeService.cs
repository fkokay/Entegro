using Entegro.Application.DTOs.Common;
using Entegro.Application.DTOs.SpecificationAttribute;

namespace Entegro.Application.Interfaces.Services
{
    public interface ISpecificationAttributeService
    {
        Task<bool> ExistsByIdAsync(int id);
        Task<bool> ExistsByNameAsync(string name);
        Task<SpecificationAttributeDto?> GetByIdAsync(int id);
        Task<SpecificationAttributeDto?> GetByNameAsync(string name);
        Task<IEnumerable<SpecificationAttributeDto>> GetAllAsync();
        Task<PagedResult<SpecificationAttributeDto>> GetPagedAsync(int pageNumber = 1, int pageSize = 7);
        Task<PagedResult<SpecificationAttributeDto>> GetPagedAsync(GridCommand gridCommand);
        Task<SpecificationAttributeDto> CreateAsync(CreateSpecificationAttributeDto model);
        Task<SpecificationAttributeDto> UpdateAsync(UpdateSpecificationAttributeDto model);
        Task DeleteAsync(int id);
    }
}
