using AutoMapper;
using Entegro.Application.DTOs.Brand;
using Entegro.Application.DTOs.Category;
using Entegro.Application.DTOs.Common;
using Entegro.Application.Interfaces.Repositories;
using Entegro.Application.Interfaces.Services;
using Entegro.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entegro.Application.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly IMapper _mapper;
        public CategoryService(ICategoryRepository categoryRepository, IMapper mapper)
        {
            _categoryRepository = categoryRepository ?? throw new ArgumentNullException(nameof(categoryRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }
        public async Task<int> CreateCategoryAsync(CreateCategoryDto createCategory)
        {
            var category = _mapper.Map<Category>(createCategory);
            await _categoryRepository.AddAsync(category);

            return category.Id;
        }

        public async Task<bool> DeleteCategoryAsync(int categoryId)
        {
            var brand = await _categoryRepository.GetByIdAsync(categoryId);

            if (brand == null)
            {
                throw new KeyNotFoundException($"Category with ID {categoryId} not found.");
            }
            await _categoryRepository.DeleteAsync(brand);
            return true;
        }

        public async Task<IEnumerable<CategoryDto>> GetCategoriesAsync()
        {
            var categories = await _categoryRepository.GetAllAsync();
            var categoryDtos = _mapper.Map<IEnumerable<CategoryDto>>(categories);
            return categoryDtos;
        }

        public async Task<PagedResult<CategoryDto>> GetCategoriesAsync(int pageNumber, int pageSize)
        {
            var categories = await _categoryRepository.GetAllAsync(pageNumber, pageSize);
            var categoryDtos = _mapper.Map<PagedResult<CategoryDto>>(categories);
            return categoryDtos;
        }

        public async Task<CategoryDto> GetCategoryByIdAsync(int categoryId)
        {
            var category = await _categoryRepository.GetByIdAsync(categoryId);
            if (category == null)
            {
                throw new KeyNotFoundException($"Category with ID {categoryId} not found.");
            }

            var categoryDto = _mapper.Map<CategoryDto>(category);
            return categoryDto;
        }

        public async Task<bool> UpdateCategoryAsync(UpdateCategoryDto updateCategory)
        {
            await _categoryRepository.UpdateAsync(_mapper.Map<Category>(updateCategory));
            return true;
        }
    }
}
