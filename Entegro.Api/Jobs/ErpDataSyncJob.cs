
using Entegro.Application.DTOs.Brand;
using Entegro.Application.DTOs.Category;
using Entegro.Application.DTOs.Erp;
using Entegro.Application.DTOs.Product;
using Entegro.Application.DTOs.ProductAttribute;
using Entegro.Application.DTOs.ProductAttributeValue;
using Entegro.Application.DTOs.ProductVariantAttribute;
using Entegro.Application.DTOs.ProductVariantAttributeCombination;
using Entegro.Application.DTOs.ProductVariantAttributeValue;
using Entegro.Application.Interfaces.Services;
using Entegro.Application.Interfaces.Services.Erp;
using Entegro.Application.Mappings.Erp;
using Entegro.Application.Services;
using MapsterMapper;
using Newtonsoft.Json;
using Polly;
using Quartz;
using System.Collections.Concurrent;

namespace Entegro.Api.Jobs
{
    public class ErpDataSyncJob : IJob
    {
        private readonly IErpService _erpService;
        private readonly IProductService _productService;
        private readonly IOrderService _orderService;
        private readonly ICustomerService _customerService;
        private readonly IBrandService _brandService;
        private readonly ICategoryService _categoryService;
        private readonly IProductAttributeService _productAttributeService;
        private readonly IProductAttributeValueService _productAttributeValueService;
        private readonly IProductVariantAttributeService _productVariantAttributeService;
        private readonly IProductVariantAttributeValueService _productVariantAttributeValueService;
        private readonly IProductVariantAttributeCombinationService _productVariantAttributeCombinationService;
        private readonly IMapper _mapper;
        private readonly ILogger<ErpDataSyncJob> _logger;

        private readonly ConcurrentDictionary<string, int> _attributeCache = new();
        private readonly ConcurrentDictionary<(int attributeId, string value), int> _attributeValueCache = new();

        public ErpDataSyncJob(
            IErpService erpService,
            IProductService productService,
            IOrderService orderService,
            ICustomerService customerService,
            IBrandService brandService,
            ICategoryService categoryService,
            IProductAttributeService productAttributeService,
            IProductAttributeValueService productAttributeValueService,
            IProductVariantAttributeService productVariantAttributeService,
            IProductVariantAttributeValueService productVariantAttributeValueService,
            IProductVariantAttributeCombinationService productVariantAttributeCombinationService,
            IMapper mapper,
            ILogger<ErpDataSyncJob> logger)
        {
            _erpService = erpService ?? throw new ArgumentNullException(nameof(erpService));
            _productService = productService ?? throw new ArgumentNullException(nameof(productService));
            _orderService = orderService ?? throw new ArgumentNullException(nameof(orderService));
            _customerService = customerService ?? throw new ArgumentNullException(nameof(customerService));
            _brandService = brandService ?? throw new ArgumentNullException(nameof(brandService));
            _categoryService = categoryService;
            _productAttributeService = productAttributeService ?? throw new ArgumentNullException(nameof(productAttributeService));
            _productAttributeValueService = productAttributeValueService ?? throw new ArgumentNullException(nameof(productAttributeValueService));
            _productVariantAttributeService = productVariantAttributeService ?? throw new ArgumentNullException(nameof(productVariantAttributeService));
            _productVariantAttributeValueService = productVariantAttributeValueService ?? throw new ArgumentNullException(nameof(productVariantAttributeValueService));
            _productVariantAttributeCombinationService = productVariantAttributeCombinationService ?? throw new ArgumentNullException(nameof(productVariantAttributeCombinationService));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task Execute(IJobExecutionContext context)
        {
            string erpType = "netsis";
            await ProductSync(erpType);
        }

        private async Task ProductSync(string erpType)
        {
            _logger.LogInformation("{erpType} ürün senkronizasyonu başlatıldı. Zaman: {Time}", erpType, DateTime.UtcNow);

            _logger.LogInformation("Cache yükleme başlatılıyor...");
            await LoadAttributeCacheAsync();
            _logger.LogInformation("Cache yükleme tamamlandı.");

            List<ErpProductDto> erpProducts;
            try
            {
                erpProducts = (await _erpService.GetProductsAsync(erpType, 500)).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{erpType}'dan ürünler alınırken bir hata oluştu.", erpType);
                return;
            }

            if (!erpProducts.Any())
            {
                _logger.LogWarning("{erpType}'dan hiç ürün alınamadı.", erpType);
                return;
            }

            ErpProductMapper.ConfigureLogger(_logger);
            var products = ErpProductMapper.ToDtoList(erpProducts);

            var retryPolicy = Policy
                .Handle<Exception>()
                .WaitAndRetryAsync(3, attempt => TimeSpan.FromSeconds(2 * attempt),
                    (ex, ts, retryCount, ctx) =>
                    {
                        _logger.LogWarning(ex, "{RetryCount}. deneme başarısız oldu, {WaitTime} saniye bekleniyor.", retryCount, ts.TotalSeconds);
                    });

            foreach (var product in products)
            {
                if (string.IsNullOrEmpty(product.Name) || string.IsNullOrEmpty(product.Code))
                {
                    _logger.LogWarning("Ürün adı veya kodu boş, ürün atlanıyor: {Name} / {Code}", product.Name, product.Code);
                    continue;
                }

                try
                {
                    await retryPolicy.ExecuteAsync(async () =>
                    {
                        if (await _productService.ExistsByCodeAsync(product.Code)) return;

                        if ((product.BrandId == null || product.BrandId == 0) && product.Brand != null)
                        {
                            if (await _brandService.ExistsByNameAsync(product.Brand.Name))
                            {
                                var brand = await _brandService.GetByNameAsync(product.Brand.Name);
                                product.BrandId = brand.Id;
                                product.Brand = null;
                            }
                            else
                            {
                                var createBrand = _mapper.Map<CreateBrandDto>(product.Brand);

                                var brandResult = await _brandService.CreateAsync(createBrand);
                                product.BrandId = brandResult.Id;
                                product.Brand = null;
                            }
                        }

                        foreach (var productCategory in product.ProductCategories)
                        {
                            if (productCategory.Category != null)
                            {
                                productCategory.CategoryId = await CreateCategoryWithChildrenAsync(productCategory.Category);
                                productCategory.Category = null;
                            }
                        }

                        var createProduct = _mapper.Map<CreateProductDto>(product);
                        var productDTO = await _productService.CreateProductAsync(createProduct);

                        var erpProduct = erpProducts.First(m => m.Code == product.Code);
                        foreach (var variant in erpProduct.ProductVariantAttributes)
                        {
                            var variantAttributes = new List<ProductVariantAttributeModel>();

                            await AddVariantAttributeAsync(productDTO.Id, variant.Variant1Name, variant.Variant1Value, variantAttributes);
                            await AddVariantAttributeAsync(productDTO.Id, variant.Variant2Name, variant.Variant2Value, variantAttributes);

                            var combinationDto = new CreateProductVariantAttributeCombinationDto
                            {
                                ProductId = productDTO.Id,
                                Gtin = "",
                                ManufacturerPartNumber = "",
                                Price = variant.Price,
                                StockQuantity = Convert.ToInt32(variant.StockQuantity),
                                StokCode = variant.VariantCode,
                                RawAttribute = JsonConvert.SerializeObject(variantAttributes)
                            };

                            await _productVariantAttributeCombinationService.AddAsync(combinationDto);
                        }

                        _logger.LogInformation("'{Name}' ürünü kaydedildi.", product.Name);
                    });
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "'{Name}' ürünü için tüm denemeler başarısız oldu.", product.Name);
                }
            }

            _logger.LogInformation("{erpType} ürün senkronizasyonu tamamlandı. Zaman: {Time}", erpType, DateTime.UtcNow);
        }

        private async Task AddVariantAttributeAsync(int productId, string attributeName, string attributeValue, List<ProductVariantAttributeModel> variantAttributes)
        {
            if (string.IsNullOrEmpty(attributeName) || string.IsNullOrEmpty(attributeValue)) return;

            int attrId = await EnsureProductAttributeAsync(attributeName);
            int attrValueId = await EnsureProductAttributeValueAsync(attrId, attributeValue);

            var existingVariant = await _productVariantAttributeService.GetByAttibuteIdAsync(productId, attrId);
            int variantId = existingVariant?.Id ?? (await _productVariantAttributeService.AddAsync(new CreateProductVariantAttributeDto
            {
                ProductId = productId,
                ProductAttributeId = attrId,
                DisplayOrder = 0,
                AttributeControlTypeId = 0
            })).Id;

            var existingVariantValue = await _productVariantAttributeValueService.GetByNameAsync(variantId, attributeValue);
            int variantValueId = existingVariantValue?.Id ?? (await _productVariantAttributeValueService.AddAsync(new CreateProductVariantAttributeValueDto()
            {
                Name = attributeValue,
                ProductVariantAttributeId = variantId
            })).Id;

            variantAttributes.Add(new ProductVariantAttributeModel
            {
                ProductVariantAttributeId = variantId,
                ProductVariantAttributeValueId = variantValueId
            });
        }

        private async Task<int> EnsureProductAttributeAsync(string name)
        {
            if (_attributeCache.TryGetValue(name, out int id)) return id;

            var created = await _productAttributeService.AddAsync(new CreateProductAttributeDto
            {
                Name = name,
                Description = "",
                DisplayOrder = 0
            });

            _attributeCache.TryAdd(name, created.Id);
            return created.Id;
        }

        private async Task<int> EnsureProductAttributeValueAsync(int attributeId, string value)
        {
            if (_attributeValueCache.TryGetValue((attributeId, value), out int id)) return id;

            var created = await _productAttributeValueService.AddAsync(new CreateProductAttributeValueDto
            {
                Name = value,
                DisplayOrder = 0,
                ProductAttributeId = attributeId
            });

            _attributeValueCache.TryAdd((attributeId, value), created.Id);
            return created.Id;
        }

        private async Task LoadAttributeCacheAsync()
        {
            _logger.LogInformation("Attribute ve AttributeValue cache yükleniyor...");

            var allAttributes = await _productAttributeService.GetAllAsync();
            foreach (var attr in allAttributes)
                _attributeCache.TryAdd(attr.Name, attr.Id);

            var allValues = await _productAttributeValueService.GetAllAsync();
            foreach (var val in allValues)
                _attributeValueCache.TryAdd((val.ProductAttributeId, val.Name), val.Id);

            _logger.LogInformation("Cache yükleme tamamlandı. Attribute: {AttrCount}, Value: {ValueCount}", _attributeCache.Count, _attributeValueCache.Count);
        }

        private async Task<int> CreateCategoryWithChildrenAsync(CategoryDto categoryDto)
        {
            if (await _categoryService.ExistsByNameAsync(categoryDto.Name))
            {
                var existing = await _categoryService.GetCategoryByNameAsync(categoryDto.Name);
                var updatedCategory = _mapper.Map<UpdateCategoryDto>(existing);

                await _categoryService.UpdateCategoryAsync(updatedCategory);

                return existing.Id;
            }

            var createCategory = _mapper.Map<CreateCategoryDto>(categoryDto);
            var createCategoryModel = await _categoryService.CreateCategoryAsync(createCategory);

            foreach (var subCategoryDto in categoryDto.SubCategories)
            {
                subCategoryDto.ParentId = createCategoryModel.Id;
                createCategoryModel.Id = await CreateCategoryWithChildrenAsync(subCategoryDto);
            }

            return createCategoryModel.Id;
        }
    }

    public class ProductVariantAttributeModel
    {
        public int ProductVariantAttributeId { get; set; }
        public int ProductVariantAttributeValueId { get; set; }
    }
}
