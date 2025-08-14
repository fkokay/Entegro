using Entegro.Application.DTOs.Brand;
using Entegro.Application.DTOs.Category;
using Entegro.Application.DTOs.CategoryAttribute;
using Entegro.Application.DTOs.Commerce.Smartstore;
using Entegro.Application.DTOs.Marketplace.Trendyol;
using Entegro.Application.Interfaces.Services.Marketplace;
using Entegro.Application.Mappings.Marketplace.Trendyol;
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
    public class TrendyolService : ITrendyolService
    {
        private readonly HttpClient _httpClient;
        private readonly string sellerId = "474352";
        private readonly ILogger<TrendyolService> _logger;

        public TrendyolService(HttpClient httpClient, ILogger<TrendyolService> logger)
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
            _logger = logger;
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
            List<TrendyolCargoCompanyDto> trendyolCargoCompanyDtos = new List<TrendyolCargoCompanyDto>();
            trendyolCargoCompanyDtos.Add(new TrendyolCargoCompanyDto()
            {
                Id = 38,
                Code = "SENDEOMP",
                Name = "Kolay Gelsin Marketplace",
                TaxNumber = "2910804196"
            });
            trendyolCargoCompanyDtos.Add(new TrendyolCargoCompanyDto()
            {
                Id = 30,
                Code = "BORMP",
                Name = "Borusan Lojistik Marketplace",
                TaxNumber = "1800038254"
            });
            trendyolCargoCompanyDtos.Add(new TrendyolCargoCompanyDto()
            {
                Id = 10,
                Code = "DHLECOMMP",
                Name = "DHL eCommerce Marketplace",
                TaxNumber = "6080712084"
            });
            trendyolCargoCompanyDtos.Add(new TrendyolCargoCompanyDto()
            {
                Id = 19,
                Code = "PTTMP",
                Name = "PTT Kargo Marketplace",
                TaxNumber = "7320068060"
            });
            trendyolCargoCompanyDtos.Add(new TrendyolCargoCompanyDto()
            {
                Id = 9,
                Code = "SURATMP",
                Name = "Sürat Kargo Marketplace",
                TaxNumber = "7870233582"
            });
            trendyolCargoCompanyDtos.Add(new TrendyolCargoCompanyDto()
            {
                Id = 17,
                Code = "TEXMP",
                Name = "Trendyol Express Marketplace",
                TaxNumber = "8590921777"
            });
            trendyolCargoCompanyDtos.Add(new TrendyolCargoCompanyDto()
            {
                Id = 6,
                Code = "HOROZMP",
                Name = "Horoz Kargo Marketplace",
                TaxNumber = "4630097122"
            });
            trendyolCargoCompanyDtos.Add(new TrendyolCargoCompanyDto()
            {
                Id = 20,
                Code = "CEVAMP",
                Name = "CEVA Marketplace",
                TaxNumber = "8450298557"
            });
            trendyolCargoCompanyDtos.Add(new TrendyolCargoCompanyDto()
            {
                Id = 4,
                Code = "YKMP",
                Name = "Yurtiçi Kargo Marketplace",
                TaxNumber = "3130557669"
            });
            trendyolCargoCompanyDtos.Add(new TrendyolCargoCompanyDto()
            {
                Id = 7,
                Code = "ARASMP",
                Name = "Aras Kargo Marketplace",
                TaxNumber = "720039666"
            });

            return trendyolCargoCompanyDtos.ToList();
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
    }
}
