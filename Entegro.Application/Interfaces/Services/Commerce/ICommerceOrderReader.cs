using Entegro.Application.DTOs.Brand;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entegro.Application.Interfaces.Services.Commerce
{
    public interface ICommerceOrderReader
    {
        Task<int> CreateBrandAsync(BrandDto brand);
        Task UpdateBrandAsync(BrandDto brand);
        Task DeleteBrandAsync(int brandId);
        Task<bool> BrandExistsAsync(string brandName);
    }
}
