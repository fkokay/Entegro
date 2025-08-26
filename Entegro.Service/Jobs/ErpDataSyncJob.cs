using AutoMapper;
using Entegro.Application.DTOs.Erp;
using Entegro.Application.DTOs.Marketplace.Trendyol;
using Entegro.Application.DTOs.Product;
using Entegro.Application.DTOs.ProductAttribute;
using Entegro.Application.DTOs.ProductAttributeValue;
using Entegro.Application.DTOs.ProductVariantAttribute;
using Entegro.Application.DTOs.ProductVariantAttributeCombination;
using Entegro.Application.DTOs.ProductVariantAttributeValue;
using Entegro.Application.Interfaces.Services;
using Entegro.Application.Interfaces.Services.Erp;
using Entegro.Application.Interfaces.Services.Marketplace;
using Entegro.Application.Mappings.Erp;
using Entegro.Application.Mappings.Marketplace;
using Entegro.Application.Services;
using Entegro.Domain.Entities;
using Entegro.Infrastructure.Migrations;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Polly;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entegro.Service.Jobs
{
    public class ErpDataSyncJob : IJob
    {
        private readonly IErpService _erpService;
        private readonly IProductService _productService;
        private readonly IOrderService _orderService;
        private readonly ICustomerService _customerService;
        private readonly IBrandService _brandService;
        private readonly IProductAttributeService _productAttributeService;
        private readonly IProductAttributeValueService _productAttributeValueService;
        private readonly IProductVariantAttributeService _productVariantAttributeService;
        private readonly IProductVariantAttributeValueService _productVariantAttributeValueService;
        private readonly IProductVariantAttributeCombinationService _productVariantAttributeCombinationService;
        private readonly IMapper _mapper;
        private readonly ILogger<SmartstoreDataSyncJob> _logger;

        public ErpDataSyncJob(
            IErpService erpService,
            IProductService productService,
            IOrderService orderService,
            ICustomerService customerService,
            IBrandService brandService,
            IProductAttributeService productAttributeService,
            IProductAttributeValueService productAttributeValueService,
            IProductVariantAttributeService productVariantAttributeService,
            IProductVariantAttributeValueService productVariantAttributeValueService,
            IProductVariantAttributeCombinationService productVariantAttributeCombinationService,
            IMapper mapper,
            ILogger<SmartstoreDataSyncJob> logger)
        {
            _erpService = erpService ?? throw new ArgumentNullException(nameof(erpService));
            _productService = productService ?? throw new ArgumentNullException(nameof(productService));
            _orderService = orderService ?? throw new ArgumentNullException(nameof(orderService));
            _customerService = customerService ?? throw new ArgumentNullException(nameof(customerService));
            _brandService = brandService ?? throw new ArgumentNullException(nameof(brandService));
            _productAttributeService = productAttributeService ?? throw new ArgumentNullException(nameof(productAttributeService));
            _productAttributeValueService = productAttributeValueService;
            _productVariantAttributeService = productVariantAttributeService ?? throw new ArgumentNullException(nameof(productVariantAttributeService));
            _productVariantAttributeValueService = productVariantAttributeValueService;
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

            List<ErpProductDto> erpProducts;

            try
            {
                erpProducts = (await _erpService.GetProductsAsync(erpType,500)).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{erpType}'dan ürünler alınırken bir hata oluştu.", erpType);
                return;
            }

            if (erpProducts == null || !erpProducts.Any())
            {
                _logger.LogWarning("{erpType}'dan hiç ürün alınamadı.", erpType);
                return;
            }

            ErpProductMapper.ConfigureLogger(_logger);
            var products = ErpProductMapper.ToDtoList(erpProducts);

            var retryPolicy = Policy
                .Handle<Exception>()
                .WaitAndRetryAsync(
                    retryCount: 3,
                    sleepDurationProvider: attempt => TimeSpan.FromSeconds(2 * attempt),
                    onRetry: (exception, timeSpan, retryCount, context) =>
                    {
                        _logger.LogWarning(exception, "{RetryCount}. deneme başarısız oldu, {WaitTime} saniye bekleniyor.", retryCount, timeSpan.TotalSeconds);
                    });

            foreach (var product in products)
            {
                if (string.IsNullOrEmpty(product.Name))
                {
                    _logger.LogWarning("Ürün adı boş veya null, '{Code}' kodlu ürün atlanıyor.", product.Code);
                    continue;
                }
                if (string.IsNullOrEmpty(product.Code))
                {
                    _logger.LogWarning("Ürün kodu boş veya null, '{Name}' adlı ürün atlanıyor.", product.Name);
                    continue;
                }


                try
                {
                    await retryPolicy.ExecuteAsync(async () =>
                    {
                        if (await _productService.ExistsByCodeAsync(product.Code))
                        {
                        }
                        else
                        {
                            var createProduct = _mapper.Map<CreateProductDto>(product);
                            var productId = await _productService.CreateProductAsync(createProduct);


                            var erpProduct = erpProducts.Where(m => m.Code == product.Code).FirstOrDefault();

                            foreach (var item in erpProduct.ProductVariantAttributes)
                            {
                                List<ProductVariantAttributeModel> productVariantAttributes = new List<ProductVariantAttributeModel>();

                                if (!string.IsNullOrEmpty(item.Variant1Name) && !string.IsNullOrEmpty(item.Variant1Value))
                                {
                                    #region ProductAttribute
                                    var productAttribute = await _productAttributeService.GetByNameAsync(item.Variant1Name);
                                    int productAttributeId = 0;
                                    if (productAttribute == null)
                                    {
                                        CreateProductAttributeDto createProductAttribute = new CreateProductAttributeDto()
                                        {
                                            Name = item.Variant1Name,
                                            Description = "",
                                            DisplayOrder = 0,
                                        };

                                        productAttributeId = await _productAttributeService.AddAsync(createProductAttribute);
                                    }
                                    else
                                    {
                                        productAttributeId = productAttribute.Id;
                                    }
                                    #endregion

                                    #region ProductAttributeValue
                                    var productAttributeValue = await _productAttributeValueService.GetByNameAsync(item.Variant1Value);
                                    int productAttributeValueId = 0;
                                    if (productAttributeValue == null)
                                    {
                                        CreateProductAttributeValueDto createProductAttributeValue = new CreateProductAttributeValueDto();
                                        createProductAttributeValue.Name = item.Variant1Value;
                                        createProductAttributeValue.DisplayOrder = 0;
                                        createProductAttributeValue.ProductAttributeId = productAttributeId;

                                        productAttributeValueId = await _productAttributeValueService.AddAsync(createProductAttributeValue);
                                    }
                                    else
                                    {
                                        productAttributeValueId = productAttributeValue.Id;
                                    }
                                    #endregion

                                    #region ProductVariantAttribute
                                    var productVariantAttributeExist = await _productVariantAttributeService.GetByAttibuteIdAsync(productAttributeId);
                                    int productVariantAttributeId = 0;
                                    if (productVariantAttributeExist == null)
                                    {
                                        CreateProductVariantAttributeDto productVariantAttribute = new CreateProductVariantAttributeDto();
                                        productVariantAttribute.AttributeControlTypeId = 0;
                                        productVariantAttribute.DisplayOrder = 0;
                                        productVariantAttribute.ProductId = productId;
                                        productVariantAttribute.ProductAttributeId = productAttributeId;

                                        productVariantAttributeId = await _productVariantAttributeService.AddAsync(productVariantAttribute);
                                    }
                                    else
                                    {
                                        productVariantAttributeId = productVariantAttributeExist.Id;
                                    }
                                    #endregion

                                    #region ProductVariantAttributeValue
                                    var productVariantAttributeValue = await _productVariantAttributeValueService.GetByNameAsync(item.Variant1Value);
                                    int productVariantAttributeValueId = 0;
                                    if (productAttributeValue == null)
                                    {
                                        ProductVariantAttributeValueDto createProductVariantAttributeValue = new ProductVariantAttributeValueDto();
                                        createProductVariantAttributeValue.ProductVariantAttributeId = productVariantAttributeId;
                                        createProductVariantAttributeValue.Name = item.Variant1Value;

                                        productVariantAttributeValueId = await _productVariantAttributeValueService.AddAsync(createProductVariantAttributeValue);
                                    }
                                    else
                                    {
                                        productAttributeValueId = productAttributeValue.Id;
                                    }
                                    #endregion

                                    productVariantAttributes.Add(new ProductVariantAttributeModel() { ProductAttributeId = productVariantAttributeId, ProductAttributeValueId = productVariantAttributeValueId });

                                }

                                if (!string.IsNullOrEmpty(item.Variant2Name) && !string.IsNullOrEmpty(item.Variant2Value))
                                {
                                    #region ProductAttribute
                                    var productAttribute = await _productAttributeService.GetByNameAsync(item.Variant2Name);
                                    int productAttributeId = 0;
                                    if (productAttribute == null)
                                    {
                                        CreateProductAttributeDto createProductAttribute = new CreateProductAttributeDto()
                                        {
                                            Name = item.Variant2Name,
                                            Description = "",
                                            DisplayOrder = 0,
                                            ProductAttributeValues = new List<ProductAttributeValueDto>() { new ProductAttributeValueDto() { Name = item.Variant2Name, DisplayOrder = 0, ProductAttributeId = 0 } }
                                        };

                                        productAttributeId = await _productAttributeService.AddAsync(createProductAttribute);
                                    }
                                    else
                                    {
                                        productAttributeId = productAttribute.Id;
                                    }
                                    #endregion

                                    #region ProductAttributeValue
                                    var productAttributeValue = await _productAttributeValueService.GetByNameAsync(item.Variant2Name);
                                    int productAttributeValueId = 0;
                                    if (productAttributeValue == null)
                                    {
                                        CreateProductAttributeValueDto createProductAttributeValue = new CreateProductAttributeValueDto();
                                        createProductAttributeValue.Name = item.Variant2Value;
                                        createProductAttributeValue.DisplayOrder = 0;
                                        createProductAttributeValue.ProductAttributeId = productAttributeId;

                                        productAttributeValueId = await _productAttributeValueService.AddAsync(createProductAttributeValue);
                                    }
                                    else
                                    {
                                        productAttributeValueId = productAttributeValue.Id;
                                    }
                                    #endregion

                                    #region ProductVariantAttribute
                                    var productVariantAttributeExist = _productVariantAttributeService.GetByAttibuteIdAsync(productAttributeId);
                                    int productVariantAttributeId = 0;
                                    if (productVariantAttributeExist == null)
                                    {
                                        CreateProductVariantAttributeDto productVariantAttribute = new CreateProductVariantAttributeDto();
                                        productVariantAttribute.AttributeControlTypeId = 0;
                                        productVariantAttribute.DisplayOrder = 0;
                                        productVariantAttribute.ProductId = productId;
                                        productVariantAttribute.ProductAttributeId = productAttributeId;

                                        productVariantAttributeId = await _productVariantAttributeService.AddAsync(productVariantAttribute);
                                    }
                                    else
                                    {
                                        productVariantAttributeId = productVariantAttributeExist.Id;
                                    }
                                    #endregion

                                    #region ProductVariantAttributeValue
                                    var productVariantAttributeValue = await _productVariantAttributeValueService.GetByNameAsync(item.Variant2Value);
                                    int productVariantAttributeValueId = 0;
                                    if (productAttributeValue == null)
                                    {
                                        ProductVariantAttributeValueDto createProductVariantAttributeValue = new ProductVariantAttributeValueDto();
                                        createProductVariantAttributeValue.ProductVariantAttributeId = productVariantAttributeId;
                                        createProductVariantAttributeValue.Name = item.Variant2Value;

                                        productVariantAttributeValueId = await _productVariantAttributeValueService.AddAsync(productVariantAttributeValue);
                                    }
                                    else
                                    {
                                        productAttributeValueId = productAttributeValue.Id;
                                    }
                                    #endregion

                                    productVariantAttributes.Add(new ProductVariantAttributeModel() { ProductAttributeId = productVariantAttributeId, ProductAttributeValueId = productVariantAttributeValueId });
                                }

                                ProductVariantAttributeCombinationDto productVariantAttributeCombination = new ProductVariantAttributeCombinationDto()
                                {
                                    Id = 0,
                                    ProductId = productId,
                                    Gtin = "",
                                    ManufacturerPartNumber = "",
                                    Price = item.Price,
                                    StockQuantity = Convert.ToInt32(item.StockQuantity),
                                    StokCode = item.VariantCode,
                                    AttributeXml = JsonConvert.SerializeObject(productVariantAttributes)
                                };

                                await _productVariantAttributeCombinationService.AddAsync(productVariantAttributeCombination);
                            }
                            _logger.LogInformation("'{Name}' adlı ürün başarıyla kaydedildi.", product.Name);
                        }
                    });
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "'{Name}' adlı ürün için tüm denemeler başarısız oldu.", product.Name);
                }
            }

            _logger.LogInformation("{erpType} ürün senkronizasyonu tamamlandı. Zaman: {Time}", erpType, DateTime.UtcNow);
        }
    }

    public class ProductVariantAttributeModel
    {
        public int ProductAttributeId { get; set; }
        public int ProductAttributeValueId { get; set; }
    }
}