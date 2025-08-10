using Entegro.Application.DTOs.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entegro.Application.DTOs.Brand;

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
    }
}
