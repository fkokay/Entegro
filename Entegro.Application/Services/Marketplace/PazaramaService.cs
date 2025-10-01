using Entegro.Application.DTOs.Brand;
using Entegro.Application.DTOs.Category;
using Entegro.Application.DTOs.CategoryAttribute;
using Entegro.Application.DTOs.Commerce.Smartstore;
using Entegro.Application.DTOs.Marketplace.N11;
using Entegro.Application.DTOs.Marketplace.Pazarama;
using Entegro.Application.Events;
using Entegro.Application.Interfaces.Event;
using Entegro.Application.Interfaces.Services.Base;
using Entegro.Application.Interfaces.Services.Marketplace;
using Entegro.Domain.Enums;
using Entegro.Imaging.Barcodes;
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
    public class PazaramaService : IPazaramaService, IEventHandler<ProductIntegrationRecordUpdatedEvent>
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IProductService _productService;
        private readonly IProductIntegrationService _productIntegrationService;
        private readonly IProductVariantAttributeCombinationService _productVariantAttributeCombinationService;
        private readonly INotificationService _notificationService;
        public PazaramaService(
            IHttpClientFactory httpClientFactory, 
            IProductService productService, 
            IProductIntegrationService productIntegrationService, 
            IProductVariantAttributeCombinationService productVariantAttributeCombinationService,
            INotificationService notificationService
            )
        {
            _httpClientFactory = httpClientFactory;
            _productService = productService;
            _productIntegrationService = productIntegrationService;
            _productVariantAttributeCombinationService = productVariantAttributeCombinationService;
            _notificationService = notificationService;
        }

        private HttpClient CreateHttpClient(PazaramaApiContext context)
        {
            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(context.BaseUrlToken);
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(Encoding.ASCII.GetBytes($"{context.ClientId}:{context.ClientSecret}")));

            return client;
        }

        private HttpClient CreateHttpClientWithToken(PazaramaApiContext context, string token)
        {
            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(context.BaseUrl);
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

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
                string? marketplaceType = productIntegration.IntegrationSystem.IntegrationSystemParameters.Where(m => m.Key == "MarketplaceType").Select(m => m.Value).FirstOrDefault();

                if (marketplaceType == "Pazarama")
                {
                    var apiContext = new PazaramaApiContext
                    {
                        ClientId = productIntegration.IntegrationSystem.IntegrationSystemParameters.First(p => p.Key == "ClientId").Value,
                        ClientSecret = productIntegration.IntegrationSystem.IntegrationSystemParameters.First(p => p.Key == "ClientSecret").Value
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

                    var priceRequest = new PazaramaPriceUpdateRequest
                    {
                        Items = new List<PazaramaPriceUpdateDto>()
                        {
                            new PazaramaPriceUpdateDto()
                            {
                                Code = product.Code,
                                ListPrice = product.Price,
                                SalePrice = product.Price
                            }
                        }
                    };

                    await UpdatePriceAsync(apiContext, priceRequest);

                    var stockRequest = new PazaramaStockUpdateRequest
                    {
                        Items = new List<PazaramaStockUpdateDto>()
                        {
                            new PazaramaStockUpdateDto()
                            {
                                Code = product.Code,
                                StockCount = stockQuantity,
                            }
                        }
                    };

                    await UpdateStockAsync(apiContext, stockRequest);

                    await _notificationService.SendNotification(NotificationType.Info, "Bildirim", $"Pazarama {product.Name} stok ve fiyat güncellendi");
                }
            }
        }

        public Task<IEnumerable<BrandDto>> GetBrandsAsync(PazaramaApiContext context)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<CategoryDto>> GetCategoriesAsync(PazaramaApiContext context)
        {
            throw new NotImplementedException();
        }

        public Task<CategoryAttributeDto> GetCategoryAttibutesAsync(PazaramaApiContext context, int categoryId)
        {
            throw new NotImplementedException();
        }

        public async Task<PazaramaProductDto?> GetProductWithStockCodeAsync(PazaramaApiContext context, string stockCode)
        {
            var token = await GetToken(context);

            using var client = CreateHttpClientWithToken(context, token.AccessToken);
            var url = $"product/products?Approved=true&Code=" + stockCode;
            var response = await client.GetAsync(url);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            var jsonData = JsonSerializer.Deserialize<PazaramaResponse<List<PazaramaProductDto>>>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            if (jsonData == null)
            {
                return null;
            }

            return jsonData.Data.FirstOrDefault();
        }

        public async Task<PazaramaTokenDto> GetToken(PazaramaApiContext context)
        {
            using var client = CreateHttpClient(context);
            var request = new HttpRequestMessage(HttpMethod.Post, "connect/token");
            request.Content = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("grant_type", "client_credentials"),
                new KeyValuePair<string, string>("scope", "merchantgatewayapi.fullaccess"),
            });

            var response = await client.SendAsync(request);
            var json = await response.Content.ReadAsStringAsync();

            response.EnsureSuccessStatusCode();
            var jsonData = JsonSerializer.Deserialize<PazaramaResponse<PazaramaTokenDto>>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            if (jsonData == null)
            {
                return null;
            }

            return jsonData.Data;
        }

        public async Task<IEnumerable<PazaramaProductDto>> GetProductsAsync(PazaramaApiContext context, int pageSize = 50)
        {
            var token = await GetToken(context);

            using var client = CreateHttpClientWithToken(context, token.AccessToken);
            var url = $"product/products?Approved=true";
            var response = await client.GetAsync(url);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            var jsonData = JsonSerializer.Deserialize<PazaramaResponse<List<PazaramaProductDto>>>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            if (jsonData == null)
            {
                return null;
            }

            return jsonData.Data.ToList();
        }

        public async Task UpdatePriceAsync(PazaramaApiContext context, PazaramaPriceUpdateRequest pazaramaPriceUpdateRequest)
        {
            var token = await GetToken(context);
            using var client = CreateHttpClientWithToken(context, token.AccessToken);
            var url = $"product/updatePrice-v2";
            var json = JsonSerializer.Serialize(pazaramaPriceUpdateRequest, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await client.PostAsync(url, content);

            var result = await response.Content.ReadAsStringAsync();
            response.EnsureSuccessStatusCode();
        }

        public async Task UpdateStockAsync(PazaramaApiContext context, PazaramaStockUpdateRequest pazaramaStockUpdateRequest)
        {
            var token = await GetToken(context);
            using var client = CreateHttpClientWithToken(context, token.AccessToken);
            var url = $"product/updateStock-v2";
            var json = JsonSerializer.Serialize(pazaramaStockUpdateRequest, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await client.PostAsync(url, content);

            var result = await response.Content.ReadAsStringAsync();
            response.EnsureSuccessStatusCode();
        }

        public async Task<IEnumerable<PazaramaOrderDto>> GetOrdersAsync(PazaramaApiContext context)
        {
            var token = await GetToken(context);

            using var client = CreateHttpClientWithToken(context, token.AccessToken);
            var url = $"order/getOrdersForApi";
            var request = new
            {
                pageSize = 100,
                pageNumber = 1,
                startDate = DateTime.Now.AddDays(-30).ToString("yyyy-MM-dd"),
                endDate = DateTime.Now.ToString("yyyy-MM-dd")
            };

            var content = new StringContent(JsonSerializer.Serialize(request),Encoding.UTF8,"application/json");
            var response = await client.PostAsync(url,content);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            var jsonData = JsonSerializer.Deserialize<PazaramaResponse<List<PazaramaOrderDto>>>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            if (jsonData == null)
            {
                return null;
            }

            return jsonData.Data.ToList();
        }
    }
}
