using Entegro.Application.DTOs.Brand;
using Entegro.Application.DTOs.Category;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entegro.Application.Interfaces.Services.Commerce
{
    public interface ICommerceCategoryWriter
    {
        Task<int> CreateCategoryAsync(CategoryDto category);
        Task UpdateCategoryAsync(CategoryDto category, int id);
        Task DeleteCategoryAsync(int categoryId);
        Task<CategoryDto?> CategoryExistsAsync(string categoryName);
    }
}
