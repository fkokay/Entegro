using AutoMapper;
using Entegro.Application.DTOs.Brand;
using Entegro.Application.DTOs.Category;
using Entegro.Application.DTOs.Common;
using Entegro.Application.DTOs.Product;
using Entegro.Application.Interfaces.Repositories;
using Entegro.Application.Interfaces.Services;
using Entegro.Domain.Entities;

namespace Entegro.Application.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;
        private readonly IBrandService _brandService;
        private readonly ICategoryService _categoryService;
        private readonly IProductAttributeService _productAttributeService;
        private readonly IProductAttributeValueService _productAttributeValueService;
        private readonly IProductVariantAttributeCombinationService _productVariantAttributeCombinationService;
        private readonly IMapper _mapper;
        public ProductService(
            IProductRepository productRepository, 
            IBrandService brandService, 
            ICategoryService categoryService,
            IProductAttributeService productAttributeService, 
            IMapper mapper)
        {
            _productRepository = productRepository;
            _brandService = brandService;
            _categoryService = categoryService;
            _mapper = mapper;
        }
        public async Task<int> CreateProductAsync(CreateProductDto createProduct)
        {
            #region Marka
            if ((createProduct.BrandId == null || createProduct.BrandId == 0) && createProduct.Brand != null)
            {
                if (await _brandService.ExistsByNameAsync(createProduct.Brand.Name))
                {
                    var brand = await _brandService.GetByNameAsync(createProduct.Brand.Name);
                    createProduct.BrandId = brand.Id;
                    createProduct.Brand = null;
                }
                else
                {
                    var createBrand = _mapper.Map<CreateBrandDto>(createProduct.Brand);

                    var brandResult = await _brandService.CreateAsync(createBrand);
                    createProduct.BrandId = brandResult.Id;
                    createProduct.Brand = null;
                }
            }
            #endregion

            #region Kategori
            foreach (var productCategory in createProduct.ProductCategories)
            {
                if (productCategory.Category != null)
                {
                    productCategory.CategoryId = await CreateCategoryWithChildrenAsync(productCategory.Category);
                    productCategory.Category = null;
                }
            }
            #endregion

            var product = _mapper.Map<Product>(createProduct);
            await _productRepository.AddAsync(product);

            return product.Id;
        }

        private async Task<int> CreateCategoryWithChildrenAsync(CategoryDto categoryDto)
        {
            if (await _categoryService.ExistsByNameAsync(categoryDto.Name))
            {
                var existing = await _categoryService.GetCategoryByNameAsync(categoryDto.Name);
                return existing.Id;
            }

            var createCategory = _mapper.Map<CreateCategoryDto>(categoryDto);
            var categoryId = await _categoryService.CreateCategoryAsync(createCategory);

            foreach (var subCategoryDto in categoryDto.SubCategories)
            {
                subCategoryDto.ParentCategoryId = categoryId;
                categoryId = await CreateCategoryWithChildrenAsync(subCategoryDto);
            }

            return categoryId;
        }


        public async Task<bool> DeleteProductAsync(int productId)
        {
            var product = await _productRepository.GetByIdAsync(productId);

            if (product == null)
            {
                throw new KeyNotFoundException($"Product with ID {productId} not found.");
            }
            await _productRepository.DeleteAsync(product);
            return true;
        }

        public async Task<bool> ExistsByCodeAsync(string productCode)
        {
            return await _productRepository.ExistsByCodeAsync(productCode);
        }

        public async Task<bool> ExistsByNameAsync(string productName)
        {
            return await _productRepository.ExistsByNameAsync(productName);
        }

        public async Task<List<int>> GetAllProductIdAsync()
        {
            var allProducts = await _productRepository.GetAllAsync();
            return allProducts.Select(x => x.Id).ToList();
        }

        public async Task<ProductDto> GetProductByCodeAsync(string productCode)
        {
            var product = await _productRepository.GetByCodeAsync(productCode);
            if (product == null)
            {
                throw new KeyNotFoundException($"Product with ID {productCode} not found.");
            }

            var productDto = _mapper.Map<ProductDto>(product);
            return productDto;
        }

        public async Task<ProductDto> GetProductByIdAsync(int productId)
        {
            var product = await _productRepository.GetByIdAsync(productId);
            if (product == null)
            {
                throw new KeyNotFoundException($"Product with ID {productId} not found.");
            }

            var productDto = _mapper.Map<ProductDto>(product);
            return productDto;
        }

        public async Task<IEnumerable<ProductDto>> GetProductsAsync()
        {
            var products = await _productRepository.GetAllAsync();
            var productDtos = _mapper.Map<IEnumerable<ProductDto>>(products);
            return productDtos;
        }

        public async Task<PagedResult<ProductDto>> GetProductsAsync(int pageNumber, int pageSize)
        {
            var products = await _productRepository.GetAllAsync(pageNumber, pageSize);
            var productDtos = _mapper.Map<PagedResult<ProductDto>>(products);
            return productDtos;
        }

        public async Task<bool> UpdateProductAsync(UpdateProductDto updateProduct)
        {
            await _productRepository.UpdateAsync(_mapper.Map<Product>(updateProduct));
            return true;
        }

        public async Task<bool> UpdateProductMainPictureIdAsync(int productId, int mainPictureId)
        {
            await _productRepository.UpdateMainPictureIdAsync(productId, mainPictureId);
            return true;
        }
    }
}
