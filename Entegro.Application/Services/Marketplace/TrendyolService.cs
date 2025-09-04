using Entegro.Application.DTOs.Brand;
using Entegro.Application.DTOs.Category;
using Entegro.Application.DTOs.CategoryAttribute;
using Entegro.Application.DTOs.Commerce;
using Entegro.Application.DTOs.Commerce.Smartstore;
using Entegro.Application.DTOs.Marketplace.Trendyol;
using Entegro.Application.Events;
using Entegro.Application.Interfaces;
using Entegro.Application.Interfaces.Repositories;
using Entegro.Application.Interfaces.Services;
using Entegro.Application.Interfaces.Services.Marketplace;
using Entegro.Application.Mappings.Marketplace.Trendyol;
using Entegro.Application.Services.Commerce.Smartstore;
using Entegro.Domain.Enums;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Entegro.Application.Services.Marketplace
{
    public class TrendyolService : ITrendyolService, IEventHandler<ProductIntegrationRecordUpdatedEvent>
    {
        private readonly string sellerId = "474352";

        private readonly HttpClient _httpClient;
        private readonly IProductIntegrationService _productIntegrationService;
        private readonly IProductService _productService;
        private readonly ILogger<TrendyolService> _logger;
        public TrendyolService(
            HttpClient httpClient, 
            IProductIntegrationService productIntegrationService, 
            IProductService productService,
            ILogger<TrendyolService> logger)
        {


            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri($"https://apigw.trendyol.com/integration/");

            _httpClient.DefaultRequestHeaders.Accept.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));

            // Basic Auth
            var username = "9tjWr2F7zHJKnMDMbcqb";
            var password = "09WZjNvN6ZJU4Tg2z53r";
            var authToken = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{username}:{password}"));
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", authToken);
            _productIntegrationService = productIntegrationService;
            _productService = productService;
            _logger = logger;
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

                if (marketplaceType == "Trendyol")
                {
                    object? customData = string.IsNullOrEmpty(productIntegration.Custom) ? null : JsonSerializer.Deserialize<SmartstoreProductIntegrationCustomDto>(productIntegration.Custom);
                    var product = await _productService.GetProductByIdAsync(productIntegration.ProductId);
                    if (product == null)
                    {
                        _logger.LogWarning($"Product with ID {productIntegration.ProductId} not found.");
                        return;
                    }

                    product.Code = productIntegration.IntegrationCode;
                    product.Price = productIntegration.Price;

                    var request = new TrendyolPriceAndStockUpdateRequest
                    {
                        Items = new List<TrendyolPriceAndStockUpdateDto>
                        {
                            new TrendyolPriceAndStockUpdateDto
                            {
                                Barcode = productIntegration.IntegrationCode,
                                ListPrice = productIntegration.Price,
                                SalePrice = productIntegration.Price,
                                Quantity = product.StockQuantity
                            }
                        }
                    };

                    await UpdatePriceAndStockAsync(request);
                }
            }
        }



        public async Task<IEnumerable<BrandDto>> GetBrandsAsync()
        {
            var allBrands = new List<TrendyolBrandDto>();
            bool moreData = true;
            int page = 0;

            while (moreData)
            {
                var url = $"product/brands?size=2000&page={page}";
                var response = await _httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();

                var json = await response.Content.ReadAsStringAsync();
                var data = JsonSerializer.Deserialize<TrendyolBrandResponse>(json, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (data?.Brands == null || !data.Brands.Any())
                {
                    break;
                }

                allBrands.AddRange(data.Brands);

                page += 1;

                if (data.Brands.Count < 2000)
                {
                    moreData = false;
                }
            }

            TrendyolBrandMapper.ConfigureLogger(_logger);
            var brands = TrendyolBrandMapper.ToDtoList(allBrands);

            return brands;
        }

        public Task<IEnumerable<TrendyolCargoCompanyDto>> GetCargoCompaniesAsync()
        {
            List<TrendyolCargoCompanyDto> trendyolCargoCompanies = new List<TrendyolCargoCompanyDto>();
            trendyolCargoCompanies.Add(new TrendyolCargoCompanyDto()
            {
                Id = 38,
                Code = "SENDEOMP",
                Name = "Kolay Gelsin Marketplace",
                TaxNumber = "2910804196"
            });
            trendyolCargoCompanies.Add(new TrendyolCargoCompanyDto()
            {
                Id = 30,
                Code = "BORMP",
                Name = "Borusan Lojistik Marketplace",
                TaxNumber = "1800038254"
            });
            trendyolCargoCompanies.Add(new TrendyolCargoCompanyDto()
            {
                Id = 10,
                Code = "DHLECOMMP",
                Name = "DHL eCommerce Marketplace",
                TaxNumber = "6080712084"
            });
            trendyolCargoCompanies.Add(new TrendyolCargoCompanyDto()
            {
                Id = 19,
                Code = "PTTMP",
                Name = "PTT Kargo Marketplace",
                TaxNumber = "7320068060"
            });
            trendyolCargoCompanies.Add(new TrendyolCargoCompanyDto()
            {
                Id = 9,
                Code = "SURATMP",
                Name = "Sürat Kargo Marketplace",
                TaxNumber = "7870233582"
            });
            trendyolCargoCompanies.Add(new TrendyolCargoCompanyDto()
            {
                Id = 17,
                Code = "TEXMP",
                Name = "Trendyol Express Marketplace",
                TaxNumber = "8590921777"
            });
            trendyolCargoCompanies.Add(new TrendyolCargoCompanyDto()
            {
                Id = 6,
                Code = "HOROZMP",
                Name = "Horoz Kargo Marketplace",
                TaxNumber = "4630097122"
            });
            trendyolCargoCompanies.Add(new TrendyolCargoCompanyDto()
            {
                Id = 20,
                Code = "CEVAMP",
                Name = "CEVA Marketplace",
                TaxNumber = "8450298557"
            });
            trendyolCargoCompanies.Add(new TrendyolCargoCompanyDto()
            {
                Id = 4,
                Code = "YKMP",
                Name = "Yurtiçi Kargo Marketplace",
                TaxNumber = "3130557669"
            });
            trendyolCargoCompanies.Add(new TrendyolCargoCompanyDto()
            {
                Id = 7,
                Code = "ARASMP",
                Name = "Aras Kargo Marketplace",
                TaxNumber = "720039666"
            });

            return Task.FromResult(trendyolCargoCompanies.AsEnumerable());
        }

        public async Task<IEnumerable<CategoryDto>> GetCategoriesAsync()
        {
            var allCategories = new List<TrendyolCategoryDto>();

            var url = $"product/product-categories";
            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            var data = JsonSerializer.Deserialize<TrendyolCategoryResponse>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            allCategories.AddRange(data.Categories);

            TrendyolCategoryMapper.ConfigureLogger(_logger);
            var categories = TrendyolCategoryMapper.ToDtoList(allCategories);

            return categories;
        }

        public async Task<CategoryAttributeDto> GetCategoryAttibutesAsync(int categoryId)
        {
            var url = $"product/product-categories/{categoryId}/attributes";
            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            var data = JsonSerializer.Deserialize<TrendyolCategoryWithAttributeDto>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });


            TrendyolCategoryAttributeMapper.ConfigureLogger(_logger);
            var categoryAttribute = TrendyolCategoryAttributeMapper.ToDto(data);

            return categoryAttribute;
        }

        public async Task<IEnumerable<TrendyolProductDto>> GetProductsAsync(int pageSize = 50)
        {
            var allProducts = new List<TrendyolProductDto>();
            bool moreData = true;
            int page = 0;

            while (moreData)
            {
                var url = $"product/sellers/{sellerId}/products?size={pageSize}&page={page}";
                var response = await _httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();

                var json = await response.Content.ReadAsStringAsync();
                var data = JsonSerializer.Deserialize<TrendyolResponse<TrendyolProductDto>>(json, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (data?.content == null || !data.content.Any())
                {
                    break;
                }

                allProducts.AddRange(data.content);

                page += 1;

                if (page >= data.totalPages)
                {
                    moreData = false;
                }
            }

            return allProducts;
        }

        public async Task<TrendyolProductDto?> GetProductWithBarcodeAsync(string barcode)
        {
            var url = $"product/sellers/{sellerId}/products?barcode={barcode}";
            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            var data = JsonSerializer.Deserialize<TrendyolResponse<TrendyolProductDto>>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            if (data == null)
            {
                return null;
            }

            return data.content.FirstOrDefault();
        }

        public async Task<IEnumerable<TrendyolShipmentPackageDto>> GetShipmentPackagesAsync(int pageSize = 50)
        {
            var allShipmentPackages = new List<TrendyolShipmentPackageDto>();
            bool moreData = true;
            int page = 0;

            while (moreData)
            {
                var url = $"order/sellers/{sellerId}/orders?size={pageSize}&page={page}";
                var response = await _httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();

                var json = await response.Content.ReadAsStringAsync();
                var data = JsonSerializer.Deserialize<TrendyolResponse<TrendyolShipmentPackageDto>>(json, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (data?.content == null || !data.content.Any())
                {
                    break;
                }

                allShipmentPackages.AddRange(data.content);

                page += 1;

                if (page >= data.totalPages)
                {
                    moreData = false;
                }
            }

            return allShipmentPackages;
        }

        public async Task UpdatePriceAndStockAsync(TrendyolPriceAndStockUpdateRequest request)
        {
            var url = $"inventory/sellers/{sellerId}/products/price-and-inventory";
            var json = JsonSerializer.Serialize(request, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync(url, content);

            var result = await response.Content.ReadAsStringAsync();
            response.EnsureSuccessStatusCode();



        }
    }
}
