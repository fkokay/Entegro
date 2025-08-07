using Entegro.Application.DTOs.Category;
using Entegro.Application.DTOs.Product;
using Entegro.Application.DTOs.Smartstore;
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

        public async Task<IEnumerable<SmartstoreProductDto>> GetProductsAsync()
        {
            var response = await _httpClient.GetAsync("products?$count=true");

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"API hatası: {response.StatusCode}");
            }

            var jsonString = await response.Content.ReadAsStringAsync();

            var odataResponse = JsonSerializer.Deserialize<ODataResponse<SmartstoreProductDto>>(jsonString, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            return odataResponse?.Value ?? new List<SmartstoreProductDto>();
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
        public async Task<IEnumerable<SmartstoreOrderDto>> GetOrdersAsync(int top = 10, int skip = 0)
        {
            var url = $"orders?$top={top}&$skip={skip}&$count=true";
            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            var data = JsonSerializer.Deserialize<ODataResponse<SmartstoreOrderDto>>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            return data?.Value ?? Enumerable.Empty<SmartstoreOrderDto>();
        }
    }
}
