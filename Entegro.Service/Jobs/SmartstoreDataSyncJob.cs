
using Entegro.Application.DTOs.Address;
using Entegro.Application.DTOs.Commerce;
using Entegro.Application.DTOs.Commerce.Smartstore;
using Entegro.Application.DTOs.Customer;
using Entegro.Application.DTOs.Order;
using Entegro.Application.DTOs.Product;
using Entegro.Application.DTOs.ProductIntegration;
using Entegro.Application.Interfaces.Services;
using Entegro.Application.Interfaces.Services.Commerce;
using Entegro.Application.Mappings.Commerce.Smartstore;
using Entegro.Application.Services.Commerce.Smartstore;
using Entegro.Domain.Entities;
using Entegro.Domain.Enums;
using MapsterMapper;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Polly;
using Polly.Retry;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Entegro.Service.Jobs
{
    public class SmartstoreDataSyncJob : IJob
    {
        private readonly ICommerceProductWriter _commerceProductWriter;
        private readonly ICommerceBrandWriter _commerceBrandWriter;
        private readonly ICommerceCategoryWriter _commerceCategoryWriter;
        private readonly ISmartstoreService _smartstoreService;
        private readonly IProductService _productService;
        private readonly IOrderService _orderService;
        private readonly ICustomerService _customerService;
        private readonly IBrandService _brandService;
        private readonly IProductIntegrationService _productIntegrationService;
        private readonly IAddressService _addressService;
        private readonly IMapper _mapper;
        private readonly ILogger<SmartstoreDataSyncJob> _logger;

        public SmartstoreDataSyncJob(
            ICommerceProductWriter commerceProductWriter,
            ICommerceBrandWriter commerceBrandWriter,
            ICommerceCategoryWriter commerceCategoryWriter,
            ISmartstoreService smartstoreService,
            IProductService productService,
            IOrderService orderService,
            ICustomerService customerService,
            IBrandService brandService,
            IProductIntegrationService productIntegrationService,
            IAddressService addressService,
            IMapper mapper,
            ILogger<SmartstoreDataSyncJob> logger)
        {
            _commerceProductWriter = commerceProductWriter ?? throw new ArgumentNullException(nameof(commerceProductWriter));
            _commerceBrandWriter = commerceBrandWriter ?? throw new ArgumentNullException(nameof(commerceBrandWriter));
            _commerceCategoryWriter = commerceCategoryWriter ?? throw new ArgumentNullException(nameof(commerceCategoryWriter));
            _smartstoreService = smartstoreService ?? throw new ArgumentNullException(nameof(smartstoreService));
            _productService = productService ?? throw new ArgumentNullException(nameof(productService));
            _orderService = orderService ?? throw new ArgumentNullException(nameof(orderService));
            _customerService = customerService ?? throw new ArgumentNullException(nameof(customerService));
            _brandService = brandService ?? throw new ArgumentNullException(nameof(brandService));
            _productIntegrationService = productIntegrationService ?? throw new ArgumentNullException(nameof(productIntegrationService));
            _addressService = addressService ?? throw new ArgumentNullException(nameof(addressService));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task Execute(IJobExecutionContext context)
        {
            //await ProductSync();
            await OrderSync();
            //await ProductWriter();
        }

        private async Task ProductWriter()
        {
            _logger.LogInformation("Smartstore ürün yazma işlemi başlatıldı. Zaman: {Time}", DateTime.UtcNow);
            var productIntegrations = await _productIntegrationService.GetProductIntegrationAsync();
            foreach (var item in productIntegrations)
            {
                var customData = getCustomData(item);
                var product = await _productService.GetProductByIdAsync(item.ProductId);
                if (product == null)
                {
                    _logger.LogWarning($"Product with ID {item.ProductId} not found.");
                    continue;
                }

                product.Code = item.IntegrationCode;
                product.Price = item.Price;

                var request = new UpsertProductRequest
                {
                    Product = product,
                    CustomData = customData
                };
                await _commerceProductWriter.UpsertProductAsync(request);
            }

            _logger.LogInformation("Smartstore ürün yazma işlemi tamamlandı. Zaman: {Time}", DateTime.UtcNow);
        }

        private SmartstoreProductIntegrationCustomDto? getCustomData(ProductIntegrationDto item)
        {
            if (item.IntegrationSystem.IntegrationSystemType == IntegrationSystemType.Commerce)
            {
                string? commerceType = item.IntegrationSystem.IntegrationSystemParameters.Where(m => m.Key == "CommerceType").Select(m => m.Value).FirstOrDefault();
                if (commerceType == "Smartstore")
                {
                    var data = string.IsNullOrEmpty(item.Custom) ? null : JsonConvert.DeserializeObject<SmartstoreProductIntegrationCustomDto>(item.Custom);

                    return data;
                }
            }

            return null;
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
                if (await _orderService.ExistsByOrderNoAsync(order.OrderNumber))
                {
                    _logger.LogInformation("'{OrderNumber}' nolu sipariş zaten kayıtlı", order.OrderNumber);
                    continue;
                }

                var customer = await _customerService.GetCustomerByEmailAsync(order.Customer.Email);
                if (customer == null)
                {
                    var createCustomer = _mapper.Map<CreateCustomerDto>(order.Customer);
                    createCustomer.Address = order.Customer.Address;
                    createCustomer.City = order.Customer.City;
                    createCustomer.Town = order.Customer.Town;
                    createCustomer.Street = order.Customer.Street;
                    createCustomer.PhoneNumber = order.Customer.PhoneNumber;
                    createCustomer.Name = order.Customer.Name;
                    createCustomer.CustomerType = 1;
                    createCustomer.Email = order.Customer.Email;
                    createCustomer.CreatedOn = DateTime.Now;
                    createCustomer.UpdatedOn = DateTime.Now;

                    var customerId = await _customerService.CreateCustomerAsync(createCustomer);
                    order.CustomerId = customerId;
                    order.Customer = null;
                }
                else
                {
                    order.CustomerId = customer.Id;
                    order.Customer = null;
                }

                if (order.ShippingAddress != null)
                {
                    var address = await _addressService.AddAsync(_mapper.Map<CreateAddressDto>(order.ShippingAddress));
                    order.ShippingAddressId = address.Id;
                    order.ShippingAddress = null;
                }

                if (order.BillingAddress != null)
                {
                    var address = await _addressService.AddAsync(_mapper.Map<CreateAddressDto>(order.BillingAddress));
                    order.BillingAddressId = address.Id;
                    order.BillingAddress = null;
                }


                foreach (var item in order.OrderItems)
                {
                    if (item.Product != null)
                    {
                        var product = await _productService.GetProductByCodeAsync(item.Product.Code);

                        if (product == null)
                        {
                            throw new Exception($"{item.Product.Code} kodlu ürün {order.OrderNumber} ' +nolu siparişte bulunamadı");
                        }

                        item.Product = null;
                        item.ProductId = product.Id;
                    }
                }

                try
                {
                    await retryPolicy.ExecuteAsync(async () =>
                    {
                        var createOrder = _mapper.Map<CreateOrderDto>(order);
                        await _orderService.CreateOrderAsync(createOrder);
                        _logger.LogInformation("'{OrderNo}' nolu sipariş başarıyla kaydedildi.", order.OrderNumber);
                    });
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "'{OrderNo}' nolu sipariş için tüm denemeler başarısız oldu.", order.OrderNumber);
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

            foreach (var item in smartstoreProducts)
            {
                foreach (var productManufacturer in item.ProductManufacturers)
                {
                    productManufacturer.Manufacturer = await _smartstoreService.GetManufacturerAsync(productManufacturer.ManufacturerId);
                }
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
