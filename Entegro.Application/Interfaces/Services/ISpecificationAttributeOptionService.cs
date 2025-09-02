using Entegro.Application.DTOs.Common;
using Entegro.Application.DTOs.SpecificationAttributeOption;

namespace Entegro.Application.Interfaces.Services
{
    public interface ISpecificationAttributeOptionService
    {
        Task<bool> ExistsByIdAsync(int id);
        Task<bool> ExistsByNameAsync(string name);
        Task<SpecificationAttributeOptionDto?> GetByIdAsync(int id);
        Task<SpecificationAttributeOptionDto?> GetByNameAsync(string name);
        Task<IEnumerable<SpecificationAttributeOptionDto>> GetAllAsync();
        Task<PagedResult<SpecificationAttributeOptionDto>> GetPagedAsync(int pageNumber = 1, int pageSize = 7);
        Task<SpecificationAttributeOptionDto> CreateAsync(CreateSpecificationAttributeOptionDto model);
        Task<SpecificationAttributeOptionDto> UpdateAsync(UpdateSpecificationAttributeOptionDto model);
        Task DeleteAsync(int id);
    }
}
