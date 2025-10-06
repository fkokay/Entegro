using Entegro.Application.DTOs.Common;
using Entegro.Application.DTOs.ProductCategory;
using Entegro.Application.Interfaces.Repositories;
using Entegro.Application.Interfaces.Services.Base;
using Entegro.Domain.Entities.Catalog;
using MapsterMapper;

namespace Entegro.Application.Services.Base
{
    public class ProductCategoryService : IProductCategoryService
    {
        private readonly IProductCategoryRepository _productCategoryMappingRepository;
        private readonly ICategoryService _categoryService;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IMapper _mapper;
        public ProductCategoryService(
            IProductCategoryRepository productCategoryMappingRepository,
            IMapper mapper,
            ICategoryService categoryService,
            ICategoryRepository categoryRepository)
        {
            _productCategoryMappingRepository = productCategoryMappingRepository ?? throw new ArgumentNullException(nameof(productCategoryMappingRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _categoryService = categoryService;
            _categoryRepository = categoryRepository;
        }
        public async Task<ProductCategoryDto> AddAsync(CreateProductCategoryDto createProductCategoryDto)
        {
            var createProductCategory = _mapper.Map<ProductCategory>(createProductCategoryDto);
            await _productCategoryMappingRepository.AddAsync(createProductCategory);
            return _mapper.Map<ProductCategoryDto>(createProductCategory);
        }

        public async Task DeleteAsync(int productCategoryId)
        {
            var productCategory = await _productCategoryMappingRepository.GetByIdAsync(productCategoryId);

            if (productCategory == null)
            {
                throw new KeyNotFoundException($"ProductCategory with ID {productCategoryId} not found.");
            }
            await _productCategoryMappingRepository.DeleteAsync(productCategory);
        }

        public async Task<ProductCategoryDto> GetProductCategoryByIdAsync(int productCategoryId)
        {
            var productCategory = await _productCategoryMappingRepository.GetByIdAsync(productCategoryId);
            if (productCategory == null)
            {
                throw new KeyNotFoundException($"ProductCategory with ID {productCategoryId} not found.");
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

        public async Task<ProductCategoryDto> UpdateAsync(UpdateProductCategoryDto updateProductCategory)
        {
            await _productCategoryMappingRepository.UpdateAsync(_mapper.Map<ProductCategory>(updateProductCategory));
            return _mapper.Map<ProductCategoryDto>(updateProductCategory);
        }

        public async Task<List<ProductCategoryDto>> GetByProductWithCategoryAsync(int productId)
        {
            var productCategories = await _productCategoryMappingRepository.GetByProductWithCategoryAsync(productId);

            var productCategoryDtos = await productCategories.SelectAwait(async c =>
            {
                var model = _mapper.Map<ProductCategoryDto>(productCategories);
                model.Id = c.Id;
                model.CategoryId = c.CategoryId;
                model.ProductId = c.ProductId;
                model.DisplayOrder = c.DisplayOrder;
                model.CategoryBreadcrumb = await _categoryService.GetCategoryPathAsync(c.Category);

                return model;
            }).ToListAsync();

            return productCategoryDtos;
        }

        public async Task<PagedResult<ProductCategoryDto>> GetPagedAsync(GridCommand gridCommand, int productId)
        {
            var productCategories = await _productCategoryMappingRepository.GetPagedAsync(gridCommand, productId);

            var items = await productCategories.Items.SelectAwait(async x =>
            {
                var model = _mapper.Map<ProductCategoryDto>(x);
                model.Id = x.Id;
                model.CategoryId = x.CategoryId;
                model.ProductId = x.ProductId;
                model.DisplayOrder = x.DisplayOrder;
                model.CategoryBreadcrumb = await _categoryService.GetCategoryPathAsync(x.Category);
                return model;
            }).AsyncToList();
            return new PagedResult<ProductCategoryDto>
            {
                Items = items,
                TotalCount = productCategories.TotalCount,
                PageNumber = productCategories.PageNumber,
                PageSize = productCategories.PageSize
            };
        }
    }
}
