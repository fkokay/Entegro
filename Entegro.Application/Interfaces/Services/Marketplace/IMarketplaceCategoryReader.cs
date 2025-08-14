using Entegro.Application.DTOs.Brand;
using Entegro.Application.DTOs.Category;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entegro.Application.Interfaces.Services.Marketplace
{
    public interface IMarketplaceCategoryReader
    {
        Task<IEnumerable<CategoryDto>> GetCategoriesAsync();
    }
}
