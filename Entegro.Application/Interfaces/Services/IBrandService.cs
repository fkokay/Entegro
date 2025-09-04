using Entegro.Application.DTOs.Brand;
using Entegro.Application.DTOs.Common;

namespace Entegro.Application.Interfaces.Services
{
    public interface IBrandService
    {
        Task<bool> ExistsByIdAsync(int id);
        Task<bool> ExistsByNameAsync(string name);
        Task<BrandDto?> GetByIdAsync(int id);
        Task<BrandDto?> GetByNameAsync(string name);
        Task<IEnumerable<BrandDto>> GetAllAsync();
        Task<PagedResult<BrandDto>> GetPagedAsync(int pageNumber = 1, int pageSize = 7);
        Task<PagedResult<BrandDto>> GetPagedAsync(GridCommand gridCommand);
        Task<BrandDto> CreateAsync(CreateBrandDto model);
        Task<BrandDto> UpdateAsync(UpdateBrandDto model);
        Task DeleteAsync(int id);
    }
}
