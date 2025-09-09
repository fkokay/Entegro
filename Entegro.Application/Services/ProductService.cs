
using Entegro.Application.DTOs.Brand;
using Entegro.Application.DTOs.Category;
using Entegro.Application.DTOs.Common;
using Entegro.Application.DTOs.Product;
using Entegro.Application.Interfaces.Repositories;
using Entegro.Application.Interfaces.Services;
using Entegro.Domain.Entities.Catalog;
using MapsterMapper;

namespace Entegro.Application.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;
        private readonly IProductVariantAttributeCombinationRepository _productVariantAttributeCombinationRepository;
        private readonly IBrandService _brandService;
        private readonly ICategoryService _categoryService;
        private readonly IMapper _mapper;
        public ProductService(
            IProductRepository productRepository,
            IProductVariantAttributeCombinationRepository productVariantAttributeCombinationRepository,
            IBrandService brandService,
            ICategoryService categoryService,
            IMapper mapper)
        {
            _productRepository = productRepository;
            _productVariantAttributeCombinationRepository = productVariantAttributeCombinationRepository;
            _brandService = brandService;
            _categoryService = categoryService;
            _mapper = mapper;
        }
        public async Task<ProductDto> CreateProductAsync(CreateProductDto createProduct)
        {

            if (createProduct == null)
                throw new ArgumentNullException(nameof(createProduct));

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

            return _mapper.Map<ProductDto>(product);
        }

        private async Task<int> CreateCategoryWithChildrenAsync(CategoryDto categoryDto)
        {
            if (await _categoryService.ExistsByNameAsync(categoryDto.Name))
            {
                var existing = await _categoryService.GetCategoryByNameAsync(categoryDto.Name);
                return existing.Id;
            }

            var createCategory = _mapper.Map<CreateCategoryDto>(categoryDto);
            var createCategoryModel = await _categoryService.CreateCategoryAsync(createCategory);

            foreach (var subCategoryDto in categoryDto.SubCategories)
            {
                subCategoryDto.ParentCategoryId = createCategoryModel.Id;
                createCategoryModel.Id = await CreateCategoryWithChildrenAsync(subCategoryDto);
            }

            return createCategoryModel.Id;
        }

        public async Task DeleteProductAsync(int productId)
        {

            if (productId <= 0)
                throw new ArgumentOutOfRangeException(nameof(productId));

            var product = await _productRepository.GetByAsync(m=>m.Id == productId);

            if (product == null)
            {
                throw new KeyNotFoundException($"Product with ID {productId} not found.");
            }

            await _productRepository.DeleteAsync(product);
        }

        public async Task<bool> ExistsByCodeAsync(string productCode)
        {
            return await _productRepository.ExistsAsync(m=>m.Code == productCode);
        }

        public async Task<bool> ExistsByNameAsync(string productName)
        {
            return await _productRepository.ExistsAsync(m=>m.Name == productName);
        }

        public async Task<ProductDto?> GetProductByCodeAsync(string productCode)
        {
            var product = await _productRepository.GetByAsync(m=>m.Code == productCode);
            if (product == null)
            {
                throw new KeyNotFoundException($"Product with ID {productCode} not found.");
            }

            var productDto = _mapper.Map<ProductDto>(product);
            return productDto;
        }

        public async Task<ProductDto?> GetProductByIdAsync(int productId)
        {
            var product = await _productRepository.GetByAsync(m=>m.Id == productId);
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

        public async Task<ProductDto> UpdateProductAsync(UpdateProductDto updateProduct)
        {
            if (updateProduct == null)
                throw new ArgumentNullException(nameof(updateProduct));

            var existingProduct = await _productRepository.GetByAsync(m=>m.Id == updateProduct.Id);
            if (existingProduct == null)
                throw new KeyNotFoundException($"ID {updateProduct.Id} ile Product bulunamadı.");


            _mapper.Map(updateProduct, existingProduct);
            await _productRepository.UpdateAsync(existingProduct);

            foreach (var item in existingProduct.ProductVariantAttributeCombinations)
            {
                if (item.Id == 0)
                {
                    await _productVariantAttributeCombinationRepository.AddAsync(item);
                }
                else
                {
                    await _productVariantAttributeCombinationRepository.UpdateAsync(item);
                }
            }

            return _mapper.Map<ProductDto>(existingProduct);
        }

        public async Task<bool> UpdateProductMainPictureIdAsync(int productId, int mainPictureId)
        {
            await _productRepository.UpdateMainPictureIdAsync(productId, mainPictureId);
            return true;
        }

        public async Task<bool> ExistsByBarcodeAsync(string productBarcode)
        {
            return await _productRepository.ExistsAsync(m=>m.Barcode == productBarcode);
        }

        public async Task<PagedResult<ProductDto>> GetPagedAsync(GridCommand gridCommand)
        {
            var products = await _productRepository.GetPagedAsync(gridCommand);
            var productDtos = _mapper.Map<PagedResult<ProductDto>>(products);
            return productDtos;
        }

        public async Task<ProductDto?> GetProductByBarcodeAsync(string productBarcode)
        {
            var product = await _productRepository.GetByAsync(m=>m.Barcode == productBarcode);
            var productDto = product == null ? null : _mapper.Map<ProductDto>(product);
            return productDto;
        }
    }
}
