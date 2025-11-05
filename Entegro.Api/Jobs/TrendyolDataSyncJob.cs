
using Entegro.Application.DTOs.Marketplace.Trendyol;
using Entegro.Application.DTOs.Order;
using Entegro.Application.DTOs.Product;
using Entegro.Application.DTOs.ProductIntegration;
using Entegro.Application.Interfaces.Services.Base;
using Entegro.Application.Interfaces.Services.Marketplace;
using Entegro.Application.Mappings.Marketplace.Trendyol;
using MapsterMapper;
using Polly;
using Quartz;

namespace Entegro.Api.Jobs
{
    public class TrendyolDataSyncJob : IJob
    {
        private readonly ITrendyolService _trendyolService;
        private readonly IProductService _productService;
        private readonly IProductIntegrationService _productIntegrationService;
        private readonly IOrderService _orderService;
        private readonly ICustomerService _customerService;
        private readonly IBrandService _brandService;
        private readonly IMapper _mapper;
        private readonly ILogger<SmartstoreDataSyncJob> _logger;

        public TrendyolDataSyncJob(
            ITrendyolService trendyolService,
            IProductService productService,
            IProductIntegrationService productIntegrationService,
            IOrderService orderService,
            ICustomerService customerService,
            IBrandService brandService,
            IMapper mapper,
            ILogger<SmartstoreDataSyncJob> logger)
        {
            _trendyolService = trendyolService ?? throw new ArgumentNullException(nameof(trendyolService));
            _productService = productService ?? throw new ArgumentNullException(nameof(productService));
            _productIntegrationService = productIntegrationService ?? throw new ArgumentNullException(nameof(productIntegrationService));
            _orderService = orderService ?? throw new ArgumentNullException(nameof(orderService));
            _customerService = customerService ?? throw new ArgumentNullException(nameof(customerService));
            _brandService = brandService ?? throw new ArgumentNullException(nameof(brandService));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task Execute(IJobExecutionContext context)
        {
            await ProductSync();
            //await OrderSync();

            //await CategorySync();
            //await BrandSync();
            //await CategoryAttributeSync();
        }

        private async Task CategoryAttributeSync()
        {
            TrendyolApiContext context = new TrendyolApiContext();
            var result = await _trendyolService.GetCategoryAttibutesAsync(context, 411);
        }

        private async Task BrandSync()
        {
            TrendyolApiContext context = new TrendyolApiContext();
            var result = await _trendyolService.GetBrandsAsync(context);
        }

        private async Task CategorySync()
        {
            TrendyolApiContext context = new TrendyolApiContext();
            var result = await _trendyolService.GetCategoriesAsync(context);
        }

        private async Task OrderSync()
        {
            TrendyolApiContext context = new TrendyolApiContext();
            context.ApiUser = "9tjWr2F7zHJKnMDMbcqb";
            context.ApiPassword = "09WZjNvN6ZJU4Tg2z53r";
            context.SupplierId = "474352";
            _logger.LogInformation("Trendyol sipariş senkronizasyonu başlatıldı. Zaman: {Time}", DateTime.UtcNow);

            IEnumerable<TrendyolShipmentPackageDto> trendyolShipmentPackages;

            try
            {
                trendyolShipmentPackages = await _trendyolService.GetShipmentPackagesAsync(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Trendyol'dan siparişler alınırken bir hata oluştu.");
                return;
            }

            if (trendyolShipmentPackages == null || !trendyolShipmentPackages.Any())
            {
                _logger.LogWarning("Trendyol'dan hiç sipariş alınamadı.");
                return;
            }

            TrendyolShipmentPackageMapper.ConfigureLogger(_logger);
            var orders = TrendyolShipmentPackageMapper.ToDtoList(trendyolShipmentPackages).ToList();
            foreach (var order in orders)
            {
                foreach (var item in order.OrderItems)
                {
                    if (item.Product != null)
                    {
                        var productIntegration = await _productIntegrationService.GetByIntegrationCodeAsync(item.Product.Code);
                        if (productIntegration != null)
                        {
                            item.Product.Id = productIntegration.Id;
                            item.ProductId = productIntegration.ProductId;
                        }
                        else
                        {
                            _logger.LogWarning(
                                "Sipariş {OrderNumber} içindeki '{ProductCode}' kodlu ürün eşleştirilmesi yapılmamış",
                                order.OrderNumber, item.Product.Code);
                        }
                    }
                }
            }

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
                if (await _orderService.ExistsByOrderNoAsync(order.OrderNumber))
                {
                    _logger.LogInformation("'{OrderNumber}' nolu sipariş zaten kayıtlı", order.OrderNumber);

                    continue;
                }

                try
                {
                    await retryPolicy.ExecuteAsync(async () =>
                    {
                        var createOrder = _mapper.Map<CreateOrderDto>(order);
                        await _orderService.AddAsync(createOrder);
                        _logger.LogInformation("'{OrderNumber}' nolu sipariş başarıyla kaydedildi.", order.OrderNumber);
                    });
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "'{OrderNumber}' nolu sipariş için tüm denemeler başarısız oldu.", order.OrderNumber);
                }
            }

            _logger.LogInformation("Trendyol sipariş senkronizasyonu tamamlandı. Zaman: {Time}", DateTime.UtcNow);
        }

        private async Task ProductSync()
        {
            _logger.LogInformation("Trendyol ürün senkronizasyonu başlatıldı. Zaman: {Time}", DateTime.UtcNow);

            TrendyolApiContext context = new TrendyolApiContext();
            context.ApiUser = "9tjWr2F7zHJKnMDMbcqb";
            context.ApiPassword = "09WZjNvN6ZJU4Tg2z53r";
            context.SupplierId = "474352";

            IEnumerable<TrendyolProductDto> trendyolProducts;

            try
            {
                trendyolProducts = await _trendyolService.GetProductsAsync(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Trendyol'dan ürünler alınırken bir hata oluştu.");
                return;
            }

            if (trendyolProducts == null || !trendyolProducts.Any())
            {
                _logger.LogWarning("Trendyol'dan hiç ürün alınamadı.");
                return;
            }
            TrendyolProductMapper.ConfigureBrandService(_brandService);
            TrendyolProductMapper.ConfigureLogger(_logger);
            var products = TrendyolProductMapper.ToDtoList(trendyolProducts);

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
                        var productDto = await _productService.AddAsync(createProduct);

                        CreateProductIntegrationDto productIntegration = new CreateProductIntegrationDto();
                        productIntegration.ProductId = productDto.Id;
                        productIntegration.IntegrationSystemId = 7;
                        productIntegration.Active = true;
                        productIntegration.Price = createProduct.Price;
                        productIntegration.IntegrationCode = product.Code;

                        await _productIntegrationService.AddAsync(productIntegration);

                        _logger.LogInformation("'{Name}' adlı ürün başarıyla kaydedildi.", product.Name);
                    });
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "'{Name}' adlı ürün için tüm denemeler başarısız oldu.", product.Name);
                }
            }

            _logger.LogInformation("Trendyol ürün senkronizasyonu tamamlandı. Zaman: {Time}", DateTime.UtcNow);
        }
    }
}
