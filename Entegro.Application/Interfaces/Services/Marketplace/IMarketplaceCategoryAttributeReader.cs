using Entegro.Application.DTOs.Brand;
using Entegro.Application.DTOs.CategoryAttribute;
using Entegro.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entegro.Application.Interfaces.Services.Marketplace
{
    public interface IMarketplaceCategoryAttributeReader
    {
        Task<CategoryAttributeDto> GetCategoryAttibutesAsync(int categoryId);
    }
}
