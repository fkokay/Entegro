using Entegro.Application.DTOs.Brand;
using Entegro.Application.DTOs.Common;

namespace Entegro.Application.Interfaces.Services.Base
{
    public interface IBrandService
    {
        Task<bool> ExistsByIdAsync(int id);
        Task<bool> ExistsByNameAsync(string name);
        Task<BrandDto?> GetByIdAsync(int id);
        Task<BrandDto?> GetByNameAsync(string name);
        Task<List<BrandDto>> GetAllBrandsAsync();
        Task<PagedResult<BrandDto>> GetBrandsAsync(int page, string term);
        Task<PagedResult<BrandDto>> GetPagedAsync(GridCommand gridCommand);
        Task<BrandDto> AddAsync(CreateBrandDto model);
        Task<BrandDto> UpdateAsync(UpdateBrandDto model);
        Task DeleteAsync(int id);
    }
}
