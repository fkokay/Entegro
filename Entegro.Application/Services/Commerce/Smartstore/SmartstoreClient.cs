using Entegro.Application.DTOs.Brand;
using Entegro.Application.DTOs.Category;
using Entegro.Application.DTOs.Commerce.Smartstore;
using Entegro.Application.DTOs.Product;
using Entegro.Application.Mappings.Commerce.Smartstore;
using Entegro.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http.Json;
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

        #region Product
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
                if (product.Brand != null)
                {
                    var manufacturer = await BrandExistsAsync(product.Brand.Name);
                    if (manufacturer == null)
                    {
                        int brandId = await CreateBrandAsync(product.Brand);
                        product.BrandId = brandId;
                    }
                    else
                    {
                        //await UpdateBrandAsync(product.Brand, manufacturer.Id);
                        product.BrandId = manufacturer.Id;
                    }
                }

                foreach (var item in product.ProductCategories)
                {
                    var category = await CategoryExistsAsync(item.Category.Name);
                    if (category == null)
                    {
                        int categoryId = await CreateCategoryAsync(item.Category);
                        item.CategoryId = categoryId;
                    }
                    else
                    {
                        //await UpdateCategoryAsync(item.Category, category.Id);
                        item.CategoryId = category.Id;
                    }
                }

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
                        foreach (var manufacturer in payload.ProductManufacturers)
                        {
                            manufacturer.ProductId = id;
                        }

                        foreach (var category in payload.ProductCategories)
                        {
                            category.ProductId = id;
                        }

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
                if (product == null)
                {
                    throw new ArgumentNullException(nameof(product), "Product cannot be null.");
                }

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
        #endregion

        #region Brand
        public async Task<int> CreateBrandAsync(BrandDto brand)
        {
            var payload = SmartstoreManufacturerMapper.ToDto(brand);
            var json = JsonSerializer.Serialize(payload, _jsonOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("manufacturers", content);
            var result = await response.Content.ReadAsStringAsync();
            response.EnsureSuccessStatusCode();

            var created = await response.Content.ReadFromJsonAsync<SmartstoreProductDto>();
            return created.Id;
        }

        public async Task UpdateBrandAsync(BrandDto brand,int id)
        {
            var payload = SmartstoreManufacturerMapper.ToDto(brand);
            payload.Id = id;
            var json = JsonSerializer.Serialize(payload, _jsonOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PutAsJsonAsync("manufacturers", content);
            var result = await response.Content.ReadAsStringAsync();
            response.EnsureSuccessStatusCode();
        }

        public async Task DeleteBrandAsync(int brandId)
        {
            var response = await _httpClient.DeleteAsync($"manufacturers({brandId})");
            response.EnsureSuccessStatusCode();
        }

        public async Task<BrandDto?> BrandExistsAsync(string brandName)
        {
            try
            {
                var url = $"manufacturers?$filter=name eq '{brandName}'";
                var response = await _httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();

                var json = await response.Content.ReadAsStringAsync();
                var data = JsonSerializer.Deserialize<ODataResponse<SmartstoreManufacturerDto>>(json, _jsonOptions);

                return data?.Value?.FirstOrDefault() is SmartstoreManufacturerDto dto ? SmartstoreManufacturerMapper.ToDto(dto) : null;
            }
            catch (Exception)
            {
                return null;
            }
        }
        #endregion

        #region Category
        public async Task<int> CreateCategoryAsync(CategoryDto category)
        {
            var payload = SmartstoreCategoryMapper.ToDto(category);
            var json = JsonSerializer.Serialize(payload, _jsonOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("categories", content);
            var result = await response.Content.ReadAsStringAsync();
            response.EnsureSuccessStatusCode();

            var created = await response.Content.ReadFromJsonAsync<SmartstoreProductDto>();
            return created.Id;
        }

        public async Task UpdateCategoryAsync(CategoryDto category, int id)
        {
            var payload = SmartstoreCategoryMapper.ToDto(category);
            payload.Id = id;
            var json = JsonSerializer.Serialize(payload, _jsonOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PutAsJsonAsync("categories", content);
            var result = await response.Content.ReadAsStringAsync();
            response.EnsureSuccessStatusCode();
        }

        public async Task DeleteCategoryAsync(int categoryId)
        {
            var response = await _httpClient.DeleteAsync($"categories({categoryId})");
            response.EnsureSuccessStatusCode();
        }

        public async Task<CategoryDto?> CategoryExistsAsync(string categoryName)
        {
            try
            {
                var url = $"categories?$filter=name eq '{categoryName}'";
                var response = await _httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();

                var json = await response.Content.ReadAsStringAsync();
                var data = JsonSerializer.Deserialize<ODataResponse<SmartstoreCategoryDto>>(json, _jsonOptions);

                return data?.Value?.FirstOrDefault() is SmartstoreCategoryDto dto ? SmartstoreCategoryMapper.ToDto(dto) : null;
            }
            catch (Exception)
            {
                return null;
            }
        }
        #endregion
    }
}
