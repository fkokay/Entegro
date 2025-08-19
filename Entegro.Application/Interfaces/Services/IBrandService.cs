using Entegro.Application.DTOs.Brand;
using Entegro.Application.DTOs.Common;

namespace Entegro.Application.Interfaces.Services
{
    public interface IBrandService
    {
        Task<BrandDto> GetBrandByIdAsync(int brandId);
        Task<BrandDto> GetBrandByNameAsync(string brandName);
        Task<IEnumerable<BrandDto>> GetBrandsAsync();
        Task<PagedResult<BrandDto>> GetBrandsAsync(int pageNumber, int pageSize);
        Task<int> CreateBrandAsync(CreateBrandDto createBrand);
        Task<bool> UpdateBrandAsync(UpdateBrandDto updateBrand);
        Task<bool> DeleteBrandAsync(int brandId);

        Task<bool> ExistsByNameAsync(string brandName);
        Task UpdateBrandImageAsync(int brandId, int mediaFileId);
        Task DeleteBrandImageAsync(int brandId);
        Task<BrandDto?> GetByIdWithMediaAsync(int id);
    }
}
