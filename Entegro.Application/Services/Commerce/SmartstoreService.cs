using Entegro.Application.DTOs.Category;
using Entegro.Application.DTOs.Commerce.Smartstore;
using Entegro.Application.DTOs.Marketplace.CicekSepeti;
using Entegro.Application.DTOs.Product;
using Entegro.Application.Interfaces.Services.Commerce;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Entegro.Application.Services.Commerce
{
    public class SmartstoreService : ISmartstoreService
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public SmartstoreService(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        private HttpClient CreateHttpClient(SmartstoreApiContext context)
        {
            var client = _httpClientFactory.CreateClient();

            client.BaseAddress = new Uri(context.BaseUrl);

            var authToken = Encoding.ASCII.GetBytes($"{context.ApiUser}:{context.ApiPassword}");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(authToken));
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));


            return client;
        }

        public async Task<IEnumerable<SmartstoreProductDto>> GetProductsAsync(SmartstoreApiContext context, int pageSize = 50)
        {
            var httpClient = CreateHttpClient(context);

            var allProducts = new List<SmartstoreProductDto>();
            int skip = 0;
            bool moreData = true;

            while (moreData)
            {
                var url = $"products?$top={pageSize}&$skip={skip}&$count=true&expand=ProductManufacturers";
                var response = await httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();

                var json = await response.Content.ReadAsStringAsync();
                var data = JsonSerializer.Deserialize<ODataListResponse<SmartstoreProductDto>>(json, new JsonSerializerOptions
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

        public async Task<IEnumerable<SmartstoreCategoryDto>> GetCategoriesAsync(SmartstoreApiContext context)
        {
            var httpClient = CreateHttpClient(context);

            var response = await httpClient.GetAsync("categories?$count=true");
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();

            var categoryResponse = JsonSerializer.Deserialize<ODataListResponse<SmartstoreCategoryDto>>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            return categoryResponse?.Value ?? Enumerable.Empty<SmartstoreCategoryDto>();
        }

        public async Task<IEnumerable<SmartstoreManufacturerDto>> GetManufacturersAsync(SmartstoreApiContext context)
        {
            var httpClient = CreateHttpClient(context);

            var response = await httpClient.GetAsync("manufacturers?$count=true");
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();

            var manufacturers = JsonSerializer.Deserialize<ODataListResponse<SmartstoreManufacturerDto>>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            return manufacturers?.Value ?? Enumerable.Empty<SmartstoreManufacturerDto>();
        }

        public async Task<SmartstoreManufacturerDto?> GetManufacturerAsync(SmartstoreApiContext context, int id)
        {
            var httpClient = CreateHttpClient(context);

            var response = await httpClient.GetAsync($"manufacturers({id})");
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();

            var manufacturer = JsonSerializer.Deserialize<SmartstoreManufacturerDto>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            return manufacturer;
        }

        public async Task<IEnumerable<SmartstoreOrderDto>> GetOrdersAsync(SmartstoreApiContext context, int pageSize = 50)
        {
            var httpClient = CreateHttpClient(context);

            var allOrders = new List<SmartstoreOrderDto>();
            int skip = 0;
            bool moreData = true;

            while (moreData)
            {
                var url = $"orders?$top={pageSize}&$skip={skip}&$count=true&expand=Customer,OrderItems($expand=Product),ShippingAddress,OrderNotes";
                var response = await httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();

                var json = await response.Content.ReadAsStringAsync();
                var data = JsonSerializer.Deserialize<ODataListResponse<SmartstoreOrderDto>>(json, new JsonSerializerOptions
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
