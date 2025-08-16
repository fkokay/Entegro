using AutoMapper;
using Entegro.Application.DTOs.ProductCategory;
using Entegro.Application.Interfaces.Repositories;
using Entegro.Application.Interfaces.Services;
using Entegro.Domain.Entities;

namespace Entegro.Application.Services
{
    public class ProductCategoryMappingService : IProductCategoryMappingService
    {
        private readonly IProductCategoryMappingRepository _productCategoryMappingRepository;
        private readonly IMapper _mapper;
        public ProductCategoryMappingService(IProductCategoryMappingRepository productCategoryMappingRepository, IMapper mapper)
        {
            _productCategoryMappingRepository = productCategoryMappingRepository ?? throw new ArgumentNullException(nameof(productCategoryMappingRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }
        public async Task<int> CreateProductCategoryAsync(CreateProductCategoryDto createProductCategoryDto)
        {
            var createProductCategory = _mapper.Map<ProductCategoryMapping>(createProductCategoryDto);
            await _productCategoryMappingRepository.AddAsync(createProductCategory);

            return createProductCategory.Id;
        }

        public async Task<bool> DeleteProductCategoryAsync(int productCategoryId)
        {
            var productCategory = await _productCategoryMappingRepository.GetByIdAsync(productCategoryId);

            if (productCategory == null)
            {
                throw new KeyNotFoundException($"ProductCategory with ID {productCategoryId} not found.");
            }
            await _productCategoryMappingRepository.DeleteAsync(productCategory);
            return true;
        }

        public async Task<ProductCategoryDto> GetProductCategoryByIdAsync(int productCategoryId)
        {
            var productCategory = await _productCategoryMappingRepository.GetByIdAsync(productCategoryId);
            if (productCategory == null)
            {
                throw new KeyNotFoundException($"Brand with ID {productCategoryId} not found.");
            }

            var productCategoryDto = _mapper.Map<ProductCategoryDto>(productCategory);
            return productCategoryDto;
        }

        public async Task<IEnumerable<ProductCategoryDto>> GetProductCategorysAsync()
        {
            var productCategories = await _productCategoryMappingRepository.GetAllAsync();
            var productCategoryDtos = _mapper.Map<IEnumerable<ProductCategoryDto>>(productCategories);
            return productCategoryDtos;
        }

        public async Task<bool> UpdateProductCategoryAsync(UpdateProductCategoryDto updateProductCategory)
        {
            await _productCategoryMappingRepository.UpdateAsync(_mapper.Map<ProductCategoryMapping>(updateProductCategory));
            return true;
        }
    }
}
