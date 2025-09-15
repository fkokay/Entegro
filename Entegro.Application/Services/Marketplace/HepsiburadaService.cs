using Entegro.Application.DTOs.Brand;
using Entegro.Application.DTOs.Category;
using Entegro.Application.DTOs.CategoryAttribute;
using Entegro.Application.DTOs.Commerce.Smartstore;
using Entegro.Application.DTOs.Marketplace.Hepsiburada;
using Entegro.Application.DTOs.Marketplace.N11;
using Entegro.Application.DTOs.Marketplace.Pazarama;
using Entegro.Application.DTOs.Marketplace.Trendyol;
using Entegro.Application.Events;
using Entegro.Application.Interfaces;
using Entegro.Application.Interfaces.Services;
using Entegro.Application.Interfaces.Services.Marketplace;
using Entegro.Domain.Enums;
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
    public class HepsiburadaService : IHepsiburadaService, IEventHandler<ProductIntegrationRecordUpdatedEvent>
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IProductService _productService;
        private readonly IProductIntegrationService _productIntegrationService;
        private readonly IProductVariantAttributeCombinationService _productVariantAttributeCombinationService;
        public HepsiburadaService(IHttpClientFactory httpClientFactory, IProductService productService, IProductIntegrationService productIntegrationService, IProductVariantAttributeCombinationService productVariantAttributeCombinationService)
        {
            _httpClientFactory = httpClientFactory;
            _productService = productService;
            _productIntegrationService = productIntegrationService;
            _productVariantAttributeCombinationService = productVariantAttributeCombinationService;
        }

        private HttpClient CreateHttpClient(HepsiburadaApiContext context,string clientType)
        {
            var client = _httpClientFactory.CreateClient();
            switch (clientType)
            {
                case "Listing":
                    client.BaseAddress = new Uri(context.ListingBaseUrl);
                    break;
                case "Order":
                    client.BaseAddress = new Uri(context.OrderBaseUrl);
                    break;
                default:
                    break;
            }


            var authToken = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{context.ApiUser}:{context.ApiPassword}"));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", authToken);
            client.DefaultRequestHeaders.Add("User-Agent", context.UserAgent);
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
                string? marketplaceType = productIntegration.IntegrationSystem.IntegrationSystemParameters.Where(m => m.Key == "MarketplaceType").Select(m => m.Value).FirstOrDefault();

                if (marketplaceType == "Hepsiburada")
                {
                    var apiContext = new HepsiburadaApiContext
                    {
                        MerchantId = productIntegration.IntegrationSystem.IntegrationSystemParameters.First(p => p.Key == "MerchantId").Value,
                        ApiUser = productIntegration.IntegrationSystem.IntegrationSystemParameters.First(p => p.Key == "ApiUser").Value,
                        ApiPassword = productIntegration.IntegrationSystem.IntegrationSystemParameters.First(p => p.Key == "ApiPassword").Value,
                        UserAgent = productIntegration.IntegrationSystem.IntegrationSystemParameters.First(p => p.Key == "UserAgent").Value
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

                    var prices = new List<HepsiburadaPriceUpdateDto>()
                        {
                            new HepsiburadaPriceUpdateDto()
                            {
                                HepsiburadaSku = product.Code,
                                Price = product.Price,
                            }
                        };

                    await UpdatePriceAsync(apiContext, prices);

                    var stocks = new List<HepsiburadaStockUpdateDto>()
                        {
                            new HepsiburadaStockUpdateDto()
                            {
                                HepsiburadaSku = product.Code,
                                AvailableStock = stockQuantity,
                            }
                        };

                    await UpdateStockAsync(apiContext, stocks);
                }
            }
        }

        public Task<IEnumerable<BrandDto>> GetBrandsAsync(HepsiburadaApiContext context)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<CategoryDto>> GetCategoriesAsync(HepsiburadaApiContext context)
        {
            throw new NotImplementedException();
        }

        public Task<CategoryAttributeDto> GetCategoryAttibutesAsync(HepsiburadaApiContext context, int categoryId)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<HepsiburadaProductDto>> GetProductsAsync(HepsiburadaApiContext context, int pageSize = 50)
        {
            using var client = CreateHttpClient(context, "Listing");
            var response = await client.GetAsync($"listings/merchantid/{context.MerchantId}?offset=0&limit=10");
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            var data = JsonSerializer.Deserialize<HepsibuaradaResponse<HepsiburadaProductDto>>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            if (data == null)
            {
                return null;
            }

            return data.Listings.ToList();
        }

        public async Task<HepsiburadaProductDto?> GetProductWitHbSkuAsync(HepsiburadaApiContext context, string hbSku)
        {
            using var client = CreateHttpClient(context, "Listing");
            var response = await client.GetAsync($"listings/merchantid/{context.MerchantId}?offset=0&limit=10&hbSkuList={hbSku}");
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            var data = JsonSerializer.Deserialize<HepsibuaradaResponse<HepsiburadaProductDto>>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            if (data == null)
            {
                return null;
            }

            return data.Listings.FirstOrDefault();
        }

        public async Task<HepsiburadaProductDto?> GetProductWithMerchantSkuAsync(HepsiburadaApiContext context, string merchantSku)
        {
            using var client = CreateHttpClient(context, "Listing");
            var response = await client.GetAsync($"listings/merchantid/{context.MerchantId}?offset=0&limit=10&merchantSkuList={merchantSku}");
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            var data = JsonSerializer.Deserialize<HepsibuaradaResponse<HepsiburadaProductDto>>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            if (data == null)
            {
                return null;
            }

            return data.Listings.FirstOrDefault();
        }

        public async Task<IEnumerable<HepsiburadaShipmentPackageDto>> GetShipmentPackagesAsync(HepsiburadaApiContext context, int pageSize = 50)
        {
            using var client = CreateHttpClient(context, "Order");
            var response = await client.GetAsync($"packages/merchantid/{context.MerchantId}?offset=0&limit=10");
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            var data = JsonSerializer.Deserialize<IEnumerable<HepsiburadaShipmentPackageDto>>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            if (data == null)
            {
                return null;
            }

            return data.ToList();
        }

        public async Task UpdatePriceAsync(HepsiburadaApiContext context, List<HepsiburadaPriceUpdateDto> hepsiburadaPriceUpdates)
        {

            using var client = CreateHttpClient(context, "Listing");
            var url = $"listings/merchantid/{context.MerchantId}/price-uploads";
            var json = JsonSerializer.Serialize(hepsiburadaPriceUpdates, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await client.PostAsync(url, content);

            var result = await response.Content.ReadAsStringAsync();
            response.EnsureSuccessStatusCode();
        }

        public async Task UpdateStockAsync(HepsiburadaApiContext context, List<HepsiburadaStockUpdateDto> hepsiburadaStockUpdates)
        {
            using var client = CreateHttpClient(context, "Listing");
            var url = $"listings/merchantid/{context.MerchantId}/stock-uploads";
            var json = JsonSerializer.Serialize(hepsiburadaStockUpdates, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await client.PostAsync(url, content);

            var result = await response.Content.ReadAsStringAsync();
            response.EnsureSuccessStatusCode();
        }
    }
}
