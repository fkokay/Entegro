using Entegro.Application.DTOs.Category;
using Entegro.Application.DTOs.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entegro.Application.Interfaces.Services
{
    public interface ICategoryService
    {
        Task<CategoryDto> GetCategoryByIdAsync(int categoryId);
        Task<IEnumerable<CategoryDto>> GetCategoriesAsync();
        Task<PagedResult<CategoryDto>> GetCategoriesAsync(int pageNumber, int pageSize);
        Task<int> CreateCategoryAsync(CreateCategoryDto createCategory);
        Task<bool> UpdateCategoryAsync(UpdateCategoryDto updateCategory);
        Task<bool> DeleteCategoryAsync(int categoryId);
    }
}
