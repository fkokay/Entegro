using AutoMapper;
using Entegro.Application.DTOs.Erp;
using Entegro.Application.DTOs.Marketplace.Trendyol;
using Entegro.Application.DTOs.Product;
using Entegro.Application.DTOs.ProductAttribute;
using Entegro.Application.Interfaces.Services;
using Entegro.Application.Interfaces.Services.Erp;
using Entegro.Application.Interfaces.Services.Marketplace;
using Entegro.Application.Mappings.Erp;
using Entegro.Application.Mappings.Marketplace;
using Entegro.Domain.Entities;
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
                erpProducts = (await _erpService.GetProductsAsync(erpType)).ToList();
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
                if (string.IsNullOrEmpty(product.Code))
                {
                    _logger.LogWarning("Ürün kodu boş veya null, '{Name}' adlı ürün atlanıyor.", product.Name);
                    continue;
                }
                if (await _productService.ExistsByCodeAsync(product.Code))
                {
                    _logger.LogInformation("'{Name}' adlı ürün zaten kayıtlı", product.Name);
                    continue;
                }

                try
                {
                    await retryPolicy.ExecuteAsync(async () =>
                    {
                        var createProduct = _mapper.Map<CreateProductDto>(product);
                        var productId = await _productService.CreateProductAsync(createProduct);

                        var p = await _productService.GetProductByIdAsync(productId);
                        foreach (var item in product.ProductVariantAttributeCombinations)
                        {
                            var values = p.ProductVariantAttributes.Select(m => m.ProductAttribute.ProductAttributeValues.Select(m => new { ProductAttributeId = m.ProductAttributeId, ProductAttributeValueId = m.Id }).ToList()).ToList();
                            foreach (var value in values)
                            {
                                item.AttributeXml = JsonConvert.SerializeObject(value[0]);
                                item.ProductId = productId;
                                await _productVariantAttributeCombinationService.AddAsync(item);
                            }

                          
                        }
                        _logger.LogInformation("'{Name}' adlı ürün başarıyla kaydedildi.", product.Name);
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
}