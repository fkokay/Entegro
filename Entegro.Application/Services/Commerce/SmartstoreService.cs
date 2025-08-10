using Entegro.Application.DTOs.Category;
using Entegro.Application.DTOs.Commerce.Smartstore;
using Entegro.Application.DTOs.Product;
using Entegro.Application.Interfaces.Services.Commerce;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Entegro.Application.Services.Commerce
{
    public class SmartstoreService : ISmartstoreService
    {
        private readonly HttpClient _httpClient;

        public SmartstoreService(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri("https://www.hunerisonline.com/odata/v1/");

            // Header ayarları
            _httpClient.DefaultRequestHeaders.Accept.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            // Basic Authentication header'ını ayarlıyoruz
            var authToken = Encoding.ASCII.GetBytes("bf0c273c90bcb045a7502daf2d9adaf1:193af61a4f6f74ef3ea77e1af9e0f099");
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(authToken));
        }

        public async Task<IEnumerable<SmartstoreProductDto>> GetProductsAsync(int pageSize = 50)
        {
            var allProducts = new List<SmartstoreProductDto>();
            int skip = 0;
            bool moreData = true;

            while (moreData)
            {
                var url = $"products?$top={pageSize}&$skip={skip}&$count=true&expand=ProductManufacturers";
                var response = await _httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();

                var json = await response.Content.ReadAsStringAsync();
                var data = JsonSerializer.Deserialize<ODataResponse<SmartstoreProductDto>>(json, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (data?.Value == null || !data.Value.Any())
                {
                    break;
                }

                allProducts.AddRange(data.Value);

                skip += pageSize;

                if (allProducts.Count() >= data.Count)
                {
                    moreData = false;
                }
            }

            return allProducts;
        }
        public async Task<IEnumerable<SmartstoreCategoryDto>> GetCategoriesAsync()
        {
            var response = await _httpClient.GetAsync("categories?$count=true");
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();

            var categoryResponse = JsonSerializer.Deserialize<ODataResponse<SmartstoreCategoryDto>>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            return categoryResponse?.Value ?? Enumerable.Empty<SmartstoreCategoryDto>();
        }
        public async Task<IEnumerable<SmartstoreManufacturerDto>> GetManufacturersAsync()
        {
            var response = await _httpClient.GetAsync("manufacturers?$count=true");
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();

            var manufacturers = JsonSerializer.Deserialize<ODataResponse<SmartstoreManufacturerDto>>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            return manufacturers?.Value ?? Enumerable.Empty<SmartstoreManufacturerDto>();
        }
        public async Task<SmartstoreManufacturerDto?> GetManufacturerAsync(int id)
        {
            var response = await _httpClient.GetAsync($"manufacturers({id})");
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();

            var manufacturer = JsonSerializer.Deserialize<SmartstoreManufacturerDto>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            return manufacturer;
        }
        public async Task<IEnumerable<SmartstoreOrderDto>> GetOrdersAsync(int pageSize = 50)
        {
            var allOrders = new List<SmartstoreOrderDto>();
            int skip = 0;
            bool moreData = true;

            while (moreData)
            {
                var url = $"orders?$top={pageSize}&$skip={skip}&$count=true&expand=Customer,OrderItems";
                var response = await _httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();

                var json = await response.Content.ReadAsStringAsync();
                var data = JsonSerializer.Deserialize<ODataResponse<SmartstoreOrderDto>>(json, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (data?.Value == null || !data.Value.Any())
                {
                    break;
                }

                allOrders.AddRange(data.Value);

                skip += pageSize;

                if (allOrders.Count() >= data.Count)
                {
                    moreData = false;
                }
            }

            return allOrders;
        }
    }
}
