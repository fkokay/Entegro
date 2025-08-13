using Entegro.Application.DTOs.Commerce.Smartstore;
using Entegro.Application.DTOs.Product;
using Entegro.Application.Mappings.Commerce.Smartstore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Entegro.Application.Services.Commerce.Smartstore
{
    public class SmartstoreClient
    {
        private readonly HttpClient _httpClient;
        private readonly JsonSerializerOptions _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
        };

        public SmartstoreClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri("https://eticaret.ozgurteknolojiyazilim.com/odata/v1/");

            // Header ayarları
            _httpClient.DefaultRequestHeaders.Accept.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            // Basic Authentication header'ını ayarlıyoruz
            var authToken = Encoding.ASCII.GetBytes("c9a68396a00e4e58ccdda2fd2b653b51:6569aa8eb0afb17f37d0f63fdd98bf3a");
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(authToken));
        }

        public async Task<ProductDto?> GetProductBySkuAsync(string sku)
        {
            try
            {
                var url = $"products?$filter=Sku eq '{sku}'";
                var response = await _httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();

                var json = await response.Content.ReadAsStringAsync();
                var data = JsonSerializer.Deserialize<ODataResponse<SmartstoreProductDto>>(json, _jsonOptions);

                return data?.Value?.FirstOrDefault() is SmartstoreProductDto dto ? SmartstoreProductMapper.ToDto(dto) : null;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task UpsertProductAsync(ProductDto product)
        {
            // Öncelikle ürünün Smartstore’da var olup olmadığını kontrol et
            try
            {
                var existing = await GetProductBySkuAsync(product.Code);

                if (existing == null)
                {
                    // Yoksa POST ile yeni ürün ekle
                    var payload = SmartstoreProductMapper.ToDto(product);
                    var json = JsonSerializer.Serialize(payload, _jsonOptions);
                    var content = new StringContent(json, Encoding.UTF8, "application/json");

                    var response = await _httpClient.PostAsync("products", content);
                    var result = await response.Content.ReadAsStringAsync();
                    response.EnsureSuccessStatusCode();
                }
                else
                {
                    var id = existing.Id; // ProductDto içine Id eklemen gerekebilir
                    var payload = SmartstoreProductMapper.ToDto(product);
                    if (payload != null)
                    {
                        payload.Id = id; // Güncelleme için ID'yi ayarla
                        var json = JsonSerializer.Serialize(payload, _jsonOptions);
                        var content = new StringContent(json, Encoding.UTF8, "application/json");

                        var response = await _httpClient.PatchAsync($"products({id})", content);
                        var result = await response.Content.ReadAsStringAsync();
                        response.EnsureSuccessStatusCode();
                    }
                }
            }
            catch (Exception)
            {

            }
        }

        public async Task UpsertProductsAsync(IEnumerable<ProductDto> products)
        {
            foreach (var product in products)
            {
                await UpsertProductAsync(product);
            }
        }

        public async Task DeleteProductAsync(string sku)
        {
            var product = await GetProductBySkuAsync(sku);
            if (product == null)
            {
                throw new InvalidOperationException($"Product with SKU '{sku}' not found.");
            }
            var response = await _httpClient.DeleteAsync($"products({product.Id})");
            response.EnsureSuccessStatusCode();
        }

        public async Task DeleteProductsAsync(IEnumerable<string> skus)
        {
            foreach (var sku in skus)
            {
                await DeleteProductAsync(sku);
            }
        }


    }
}
