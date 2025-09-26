using Entegro.Application.DTOs.Brand;
using Entegro.Application.DTOs.Category;
using Entegro.Application.DTOs.CategoryAttribute;
using Entegro.Application.DTOs.Commerce.Smartstore;
using Entegro.Application.DTOs.Marketplace.N11;
using Entegro.Application.DTOs.Marketplace.Trendyol;
using Entegro.Application.Events;
using Entegro.Application.Interfaces;
using Entegro.Application.Interfaces.Services;
using Entegro.Application.Interfaces.Services.Marketplace;
using Entegro.Application.Notifications;
using Entegro.Domain.Enums;
using Mapster;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Entegro.Application.Services.Marketplace
{
    public class N11Service : IN11Service, IEventHandler<ProductIntegrationRecordUpdatedEvent>
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IProductIntegrationService _productIntegrationService;
        private readonly IProductService _productService;
        private readonly IProductVariantAttributeCombinationService _productVariantAttributeCombinationService;
        public N11Service(IHttpClientFactory httpClientFactory,IProductIntegrationService productIntegrationService, IProductService productService, IProductVariantAttributeCombinationService productVariantAttributeCombinationService)
        {
            _httpClientFactory = httpClientFactory;
            _productIntegrationService = productIntegrationService;
            _productService = productService;
            _productVariantAttributeCombinationService = productVariantAttributeCombinationService;
        }

        private HttpClient CreateHttpClient(N11ApiContext context)
        {
            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(context.BaseUrl);

            client.DefaultRequestHeaders.Add("appKey", context.AppKey);
            client.DefaultRequestHeaders.Add("appSecret", context.AppSecret);

            return client;
        }

        public async Task HandleAsync(ProductIntegrationRecordUpdatedEvent recordUpdatedEvent)
        {
            var productIntegration = await _productIntegrationService.GetByIdAsync(recordUpdatedEvent.ProductIntegrationId);
            if (productIntegration == null)
            {
                return;
            }

            if (productIntegration.IntegrationSystem.IntegrationSystemType == IntegrationSystemType.Marketplace)
            {
                string marketplaceType = productIntegration.IntegrationSystem.IntegrationSystemParameters.Where(m => m.Key == "MarketplaceType").Select(m => m.Value).FirstOrDefault();

                if (marketplaceType == "N11")
                {
                    var apiContext = new N11ApiContext
                    {
                        AppSecret = productIntegration.IntegrationSystem.IntegrationSystemParameters.First(p => p.Key == "AppSecret").Value,
                        AppKey = productIntegration.IntegrationSystem.IntegrationSystemParameters.First(p => p.Key == "AppKey").Value
                    };

                    object? customData = string.IsNullOrEmpty(productIntegration.Custom) ? null : JsonSerializer.Deserialize<SmartstoreProductIntegrationCustomDto>(productIntegration.Custom);
                    var product = await _productService.GetProductByIdAsync(productIntegration.ProductId);
                    if (product == null)
                    {
                        return;
                    }

                    product.Code = productIntegration.IntegrationCode;
                    product.Price = productIntegration.Price;


                    int stockQuantity = 0;
                    if (productIntegration.ProductVariantAttributeCombinationId.HasValue)
                    {
                        var productVariantAttributeCombination = await _productVariantAttributeCombinationService.GetByIdAsync(productIntegration.ProductVariantAttributeCombinationId.Value);
                        stockQuantity = productVariantAttributeCombination.StockQuantity;
                    }
                    else
                    {
                        stockQuantity = product.StockQuantity;
                    }

                    var request = new N11PriceAndStockUpdatePayload
                    {
                        Payload = new N11PriceAndStockUpdateRequest()
                        {
                            Integrator = "ÖZGÜR TEKNOLOJİ",
                             Skus = new List<N11PriceAndStockUpdateDto>()
                             {
                                 new N11PriceAndStockUpdateDto()
                                 {
                                     StockCode = productIntegration.IntegrationCode,
                                     CurrencyType = "TL",
                                     ListPrice = product.Price,
                                     Quantity = stockQuantity,
                                     SalePrice = product.Price,
                                 }
                             }
                        }
                    };

                    await UpdatePriceAndStockAsync(apiContext, request);

                    await EntegroNotification.SendNotification(NotificationType.Info, "Bildirim", $"N11 {product.Name} stok ve fiyat güncellendi");
                }
            }
        }


        public Task<IEnumerable<BrandDto>> GetBrandsAsync(N11ApiContext context)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<CategoryDto>> GetCategoriesAsync(N11ApiContext context)
        {
            throw new NotImplementedException();
        }

        public Task<CategoryAttributeDto> GetCategoryAttibutesAsync(N11ApiContext context,int categoryId)
        {
            throw new NotImplementedException();
        }

        public async Task UpdatePriceAndStockAsync(N11ApiContext context,N11PriceAndStockUpdatePayload payload)
        {
            using var client = CreateHttpClient(context);
            var url = $"ms/product/tasks/price-stock-update";
            var json = JsonSerializer.Serialize(payload, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await client.PostAsync(url, content);

            var result = await response.Content.ReadAsStringAsync();
            response.EnsureSuccessStatusCode();
        }

        public async Task<N11ProductDto?> GetProductWithN11CodeAsync(N11ApiContext context,string n11Code)
        {
            using var client = CreateHttpClient(context);
            var url = $"ms/product-query?id={n11Code}";
            var response = await client.GetAsync(url);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            var data = JsonSerializer.Deserialize<N11Response<N11ProductDto>>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            if (data == null)
            {
                return null;
            }

            return data.Content.FirstOrDefault();
        }

        public async Task<N11ProductDto?> GetProductWithStockCodeAsync(N11ApiContext context,string stockCode)
        {
            using var client = CreateHttpClient(context);
            var url = $"ms/product-query?stockCode={stockCode}";
            var response = await client.GetAsync(url);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            var data = JsonSerializer.Deserialize<N11Response<N11ProductDto>>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            if (data == null)
            {
                return null;
            }

            return data.Content.FirstOrDefault();
        }

        public async Task<IEnumerable<N11ProductDto>> GetProductsAsync(N11ApiContext context,int pageSize = 50)
        {
            using var client = CreateHttpClient(context);
            var url = $"ms/product-query";
            var response = await client.GetAsync(url);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            var data = JsonSerializer.Deserialize<N11Response<N11ProductDto>>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            if (data == null)
            {
                return null;
            }

            return data.Content.ToList();
        }

        public async Task<IEnumerable<N11ShipmentPackageDto>> GetShipmentPackagesAsync(N11ApiContext context,int pageSize = 50)
        {
            using var client = CreateHttpClient(context);
            var url = $"rest/delivery/v1/shipmentPackages";
            var response = await client.GetAsync(url);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            var data = JsonSerializer.Deserialize<N11Response<N11ShipmentPackageDto>>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            if (data == null)
            {
                return null;
            }

            return data.Content.ToList();
        }
    }
}
