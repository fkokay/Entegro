using Entegro.Application.DTOs.Common;
using Entegro.Application.DTOs.Product;
using Entegro.Application.DTOs.ProductVariantAttributeCombination;
using Entegro.Application.Events;
using Entegro.Application.Interfaces;
using Entegro.Application.Interfaces.Repositories;
using Entegro.Application.Interfaces.Services;
using Entegro.ComponentModel;
using Entegro.Domain.Entities.Catalog;
using MapsterMapper;

namespace Entegro.Application.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;
        private readonly IProductIntegrationService _productIntegrationService;
        private readonly IProductIntegrationRepository _productIntegrationRepository;
        private readonly IProductVariantAttributeCombinationRepository _productVariantAttributeCombinationRepository;
        private readonly IBrandService _brandService;
        private readonly ICategoryService _categoryService;
        private readonly IMapper _mapper;
        private readonly IEventPublisher _eventPublisher;
        public ProductService(
            IProductRepository productRepository,
            IProductIntegrationService productIntegrationService,
            IProductIntegrationRepository productIntegrationRepository,
            IProductVariantAttributeCombinationRepository productVariantAttributeCombinationRepository,
            IBrandService brandService,
            ICategoryService categoryService,
            IMapper mapper,
            IEventPublisher eventPublisher)
        {
            _productRepository = productRepository;
            _productIntegrationService = productIntegrationService;
            _productIntegrationRepository = productIntegrationRepository;
            _productVariantAttributeCombinationRepository = productVariantAttributeCombinationRepository;
            _brandService = brandService;
            _categoryService = categoryService;
            _mapper = mapper;
            _eventPublisher = eventPublisher;
        }
        public async Task<ProductDto> CreateProductAsync(CreateProductDto createProduct)
        {

            if (createProduct == null)
                throw new ArgumentNullException(nameof(createProduct));

            var product = _mapper.Map<Product>(createProduct);
            await _productRepository.AddAsync(product);

            var productIntegrations = await _productIntegrationService.GetProductIntegrationAllWithProductIdAsync(product.Id);
            foreach (var productIntegration in productIntegrations)
            {
                var recordUpdatedEvent = new ProductIntegrationRecordUpdatedEvent(productIntegration.Id);
                _eventPublisher.Publish(recordUpdatedEvent);
            }


            return _mapper.Map<ProductDto>(product);
        }

        public async Task<ProductDto> UpdateProductAsync(UpdateProductDto updateProduct)
        {
            if (updateProduct == null)
                throw new ArgumentNullException(nameof(updateProduct));

            var existingProduct = await _productRepository.GetByAsync(m => m.Id == updateProduct.Id);
            var mainPictureId = existingProduct?.MainPictureId ?? null;
            if (existingProduct == null)
                throw new KeyNotFoundException($"ID {updateProduct.Id} ile Product bulunamadı.");


            _mapper.Map(updateProduct, existingProduct);
            existingProduct.MainPictureId = mainPictureId;
            await _productRepository.UpdateAsync(existingProduct);

            foreach (var item in updateProduct.ProductVariantAttributeCombinations)
            {
                var combination = await MapperFactory.MapAsync<ProductVariantAttributeCombinationDto, ProductVariantAttributeCombination>(item);
                combination.SetAssignedMediaIds(item.AssignedPictureIds);

                if (item.Id == 0)
                {
                    await _productVariantAttributeCombinationRepository.AddAsync(combination);
                }
                else
                {
                    await _productVariantAttributeCombinationRepository.UpdateAsync(combination);
                }
            }

            var productIntegrations = await _productIntegrationService.GetProductIntegrationAllWithProductIdAsync(existingProduct.Id);
            foreach (var productIntegration in productIntegrations)
            {
                var recordUpdatedEvent = new ProductIntegrationRecordUpdatedEvent(productIntegration.Id);
                _eventPublisher.Publish(recordUpdatedEvent);
            }

            return _mapper.Map<ProductDto>(existingProduct);
        }

        public async Task DeleteProductAsync(int productId)
        {

            if (productId <= 0)
                throw new ArgumentOutOfRangeException(nameof(productId));

            var product = await _productRepository.GetByAsync(m => m.Id == productId);

            if (product == null)
            {
                throw new KeyNotFoundException($"Product with ID {productId} not found.");
            }

            await _productRepository.DeleteAsync(product);
        }

        public async Task<bool> ExistsByCodeAsync(string productCode)
        {
            return await _productRepository.ExistsAsync(m => m.Code == productCode);
        }

        public async Task<bool> ExistsByNameAsync(string productName)
        {
            return await _productRepository.ExistsAsync(m => m.Name == productName);
        }

        public async Task<ProductDto?> GetProductByCodeAsync(string productCode)
        {
            var product = await _productRepository.GetByAsync(m => m.Code == productCode);
            if (product == null)
            {
                throw new KeyNotFoundException($"Product with ID {productCode} not found.");
            }

            var productDto = _mapper.Map<ProductDto>(product);
            return productDto;
        }

        public async Task<ProductDto?> GetProductByIdAsync(int productId)
        {
            var product = await _productRepository.GetByAsync(m => m.Id == productId);
            if (product == null)
            {
                throw new KeyNotFoundException($"Product with ID {productId} not found.");
            }

            var productVariantAttributeCombinations = await product.ProductVariantAttributeCombinations.SelectAwait(async x =>
            {
                var productVariantAttributeCombination = _mapper.Map<ProductVariantAttributeCombinationDto>(x);

                productVariantAttributeCombination.AssignedPictureIds = x.GetAssignedMediaIds();
                return productVariantAttributeCombination;
            }).AsyncToList();

            var productDto = _mapper.Map<ProductDto>(product);
            productDto.ProductVariantAttributeCombinations = productVariantAttributeCombinations;


            return productDto;
        }

        public async Task<PagedResult<ProductDto>> GetProductsAsync(int page, string term)
        {
            var products = await _productRepository.GetAllAsync(page, term);
            var productDtos = _mapper.Map<PagedResult<ProductDto>>(products);
            return productDtos;
        }

        public async Task<IEnumerable<ProductDto>> GetProductsAsync()
        {
            var products = await _productRepository.GetAllAsync();
            var productDtos = _mapper.Map<IEnumerable<ProductDto>>(products);
            return productDtos;
        }

        public async Task<bool> UpdateProductMainPictureIdAsync(int productId, int mainPictureId)
        {
            await _productRepository.UpdateMainPictureIdAsync(productId, mainPictureId);
            return true;
        }

        public async Task<bool> ExistsByBarcodeAsync(string productBarcode)
        {
            return await _productRepository.ExistsAsync(m => m.Barcode == productBarcode);
        }

        public async Task<PagedResult<ProductDto>> GetPagedAsync(GridCommand gridCommand)
        {
            var products = await _productRepository.GetPagedAsync(gridCommand);
            var productDtos = _mapper.Map<PagedResult<ProductDto>>(products);
            return productDtos;
        }

        public async Task<ProductDto?> GetProductByBarcodeAsync(string productBarcode)
        {
            var product = await _productRepository.GetByAsync(m => m.Barcode == productBarcode);
            var productDto = product == null ? null : _mapper.Map<ProductDto>(product);
            return productDto;
        }
    }
}
