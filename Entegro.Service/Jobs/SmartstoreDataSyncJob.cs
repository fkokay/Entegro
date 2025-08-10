using AutoMapper;
using Entegro.Application.DTOs.Commerce.Smartstore;
using Entegro.Application.DTOs.Order;
using Entegro.Application.DTOs.Product;
using Entegro.Application.Interfaces.Services;
using Entegro.Application.Interfaces.Services.Commerce;
using Entegro.Application.Mappings.Commerce.Smartstore;
using Entegro.Domain.Entities;
using Entegro.Domain.Enums;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Retry;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Entegro.Service.Jobs
{
    public class SmartstoreDataSyncJob : IJob
    {
        private readonly ISmartstoreService _smartstoreService;
        private readonly IProductService _productService;
        private readonly IOrderService _orderService;
        private readonly ICustomerService _customerService;
        private readonly IBrandService _brandService;
        private readonly IMapper _mapper;
        private readonly ILogger<SmartstoreDataSyncJob> _logger;

        public SmartstoreDataSyncJob(
            ISmartstoreService smartstoreService,
            IProductService productService,
            IOrderService orderService,
            ICustomerService customerService,
            IBrandService brandService,
            IMapper mapper,
            ILogger<SmartstoreDataSyncJob> logger)
        {
            _smartstoreService = smartstoreService ?? throw new ArgumentNullException(nameof(smartstoreService));
            _productService = productService ?? throw new ArgumentNullException(nameof(productService));
            _orderService = orderService ?? throw new ArgumentNullException(nameof(orderService));
            _customerService = customerService ?? throw new ArgumentNullException(nameof(customerService));
            _brandService = brandService ?? throw new ArgumentNullException(nameof(brandService));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task Execute(IJobExecutionContext context)
        {
            await ProductSync();
           // await OrderSync();
        }

        private async Task OrderSync()
        {
            _logger.LogInformation("Smartstore sipariş senkronizasyonu başlatıldı. Zaman: {Time}", DateTime.UtcNow);
            IEnumerable<SmartstoreOrderDto> smartstoreOrders;
            try
            {
                smartstoreOrders = await _smartstoreService.GetOrdersAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Smartstore'dan siparişler alınırken bir hata oluştu.");
                return;
            }

            if (smartstoreOrders == null || !smartstoreOrders.Any())
            {
                _logger.LogWarning("Smartstore'dan hiç sipariş alınamadı.");
                return;
            }

            SmartstoreOrderMapper.ConfigureLogger(_logger);
            var orders = SmartstoreOrderMapper.ToDtoList(smartstoreOrders);

            var retryPolicy = Policy
                .Handle<Exception>()
                .WaitAndRetryAsync(
                    retryCount: 3,
                    sleepDurationProvider: attempt => TimeSpan.FromSeconds(2 * attempt),
                    onRetry: (exception, timeSpan, retryCount, context) =>
                    {
                        _logger.LogWarning(exception, "{RetryCount}. deneme başarısız oldu, {WaitTime} saniye bekleniyor.", retryCount, timeSpan.TotalSeconds);
                    });

            foreach (var order in orders)
            {
                if (await _orderService.ExistsByOrderNoAsync(order.OrderNo))
                {
                    _logger.LogInformation("'{OrderNo}' nolu sipariş zaten kayıtlı", order.OrderNo);
                    continue;
                }

                order.OrderSource = OrderSource.Smartstore;

                try
                {
                    await retryPolicy.ExecuteAsync(async () =>
                    {
                        var createOrder = _mapper.Map<CreateOrderDto>(order);
                        await _orderService.CreateOrderAsync(createOrder);
                        _logger.LogInformation("'{OrderNo}' nolu sipariş başarıyla kaydedildi.", order.OrderNo);
                    });
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "'{OrderNo}' nolu sipariş için tüm denemeler başarısız oldu.", order.OrderNo);
                }
            }

            _logger.LogInformation("Smartstore sipariş senkronizasyonu tamamlandı. Zaman: {Time}", DateTime.UtcNow);
        }

        private async Task ProductSync()
        {
            _logger.LogInformation("Smartstore ürün senkronizasyonu başlatıldı. Zaman: {Time}", DateTime.UtcNow);

            IEnumerable<SmartstoreProductDto> smartstoreProducts;

            try
            {
                smartstoreProducts = await _smartstoreService.GetProductsAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Smartstore'dan ürünler alınırken bir hata oluştu.");
                return;
            }

            if (smartstoreProducts == null || !smartstoreProducts.Any())
            {
                _logger.LogWarning("Smartstore'dan hiç ürün alınamadı.");
                return;
            }

            SmartstoreProductMapper.ConfigureLogger(_logger);
            var products = SmartstoreProductMapper.ToDtoList(smartstoreProducts);

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
                        await _productService.CreateProductAsync(createProduct);
                        _logger.LogInformation("'{Name}' adlı ürün başarıyla kaydedildi.", product.Name);
                    });
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "'{Name}' adlı ürün için tüm denemeler başarısız oldu.", product.Name);
                }
            }

            _logger.LogInformation("Smartstore ürün senkronizasyonu tamamlandı. Zaman: {Time}", DateTime.UtcNow);
        }
    }
}
