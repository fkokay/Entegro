using Entegro.Application.DTOs.Brand;
using Entegro.Application.DTOs.Category;
using Entegro.Application.DTOs.CategoryAttribute;
using Entegro.Application.DTOs.Commerce.Smartstore;
using Entegro.Application.DTOs.Marketplace.CicekSepeti;
using Entegro.Application.DTOs.Marketplace.Hepsiburada;
using Entegro.Application.DTOs.Marketplace.N11;
using Entegro.Application.Events;
using Entegro.Application.Interfaces;
using Entegro.Application.Interfaces.Services;
using Entegro.Application.Interfaces.Services.Marketplace;
using Entegro.Application.Notifications;
using Entegro.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Entegro.Application.Services.Marketplace
{
    public class CicekSepetiService : ICicekSepetiService, IEventHandler<ProductIntegrationRecordUpdatedEvent>
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IProductService _productService;
        private readonly IProductIntegrationService _productIntegrationService;
        private readonly IProductVariantAttributeCombinationService _productVariantAttributeCombinationService;
        public CicekSepetiService(IHttpClientFactory httpClientFactory, IProductService productService, IProductIntegrationService productIntegrationService, IProductVariantAttributeCombinationService productVariantAttributeCombinationService)
        {
            _httpClientFactory = httpClientFactory;
            _productService = productService;
            _productIntegrationService = productIntegrationService;
            _productVariantAttributeCombinationService = productVariantAttributeCombinationService;
        }

        private HttpClient CreateHttpClient(CicekSepetiApiContext context)
        {
            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(context.BaseUrl);
            client.DefaultRequestHeaders.Add("x-api-key", context.ApiUser);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

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

                if (marketplaceType == "CicekSepeti")
                {
                    var apiContext = new CicekSepetiApiContext
                    {
                        ApiUser = productIntegration.IntegrationSystem.IntegrationSystemParameters.First(p => p.Key == "ApiUser").Value,
                        SupplierId = productIntegration.IntegrationSystem.IntegrationSystemParameters.First(p => p.Key == "SupplierId").Value
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

                    var request = new CicekSepetiPriceAndStockUpdateRequest
                    {
                        Items = new List<CicekSepetiPriceAndStockUpdateDto>()
                        {
                            new CicekSepetiPriceAndStockUpdateDto()
                            {
                                 StockCode = productIntegration.IntegrationCode,
                                 StockQuantity = stockQuantity,
                                 SalesPrice = product.Price,
                            }
                        }
                    };

                    await UpdatePriceAndStockAsync(apiContext, request);

                    await EntegroNotification.SendNotification(NotificationType.Info, "Bildirim", $"ÇiçekSepeti {product.Name} stok ve fiyat güncellendi");
                }
            }
        }

        public Task<IEnumerable<BrandDto>> GetBrandsAsync(CicekSepetiApiContext context)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<CategoryDto>> GetCategoriesAsync(CicekSepetiApiContext context)
        {
            throw new NotImplementedException();
        }

        public Task<CategoryAttributeDto> GetCategoryAttibutesAsync(CicekSepetiApiContext context, int categoryId)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<CicekSepetiProductDto>> GetProductsAsync(CicekSepetiApiContext context, int pageSize = 50)
        {
            using var client = CreateHttpClient(context);
            var response = await client.GetAsync($"Products");

            var json = await response.Content.ReadAsStringAsync();
            response.EnsureSuccessStatusCode();

            var data = JsonSerializer.Deserialize<CicekSepetiProductResponse>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            if (data == null)
            {
                return null;
            }

            return data.Products;
        }

        public async Task<CicekSepetiProductDto?> GetProductWithStockCodeAsync(CicekSepetiApiContext context, string stockCode)
        {
            using var client = CreateHttpClient(context);
            var response = await client.GetAsync($"Products?StockCode={stockCode}");
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            var data = JsonSerializer.Deserialize<CicekSepetiProductResponse>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            if (data == null)
            {
                return null;
            }

            return data.Products.FirstOrDefault();
        }

        public async Task UpdatePriceAndStockAsync(CicekSepetiApiContext context, CicekSepetiPriceAndStockUpdateRequest priceAndStockUpdateRequest)
        {
            using var client = CreateHttpClient(context);
            var url = $"Products/price-and-stock";
            var json = JsonSerializer.Serialize(priceAndStockUpdateRequest, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await client.PutAsync(url, content);

            var result = await response.Content.ReadAsStringAsync();
            response.EnsureSuccessStatusCode();
        }

        public Task<IEnumerable<CicekSepetiOrderDto>> GetOrdersAsync(CicekSepetiApiContext context)
        {
            throw new NotImplementedException();
        }
    }
}
