using Entegro.Application.DTOs.Brand;
using Entegro.Application.DTOs.Category;
using Entegro.Application.DTOs.Commerce;
using Entegro.Application.DTOs.Commerce.Smartstore;
using Entegro.Application.DTOs.Product;
using Entegro.Application.DTOs.ProductAttribute;
using Entegro.Application.DTOs.ProductAttributeValue;
using Entegro.Application.DTOs.ProductBrand;
using Entegro.Application.DTOs.ProductCategory;
using Entegro.Application.DTOs.ProductMediaFile;
using Entegro.Application.DTOs.ProductVariantAttribute;
using Entegro.Application.DTOs.ProductVariantAttributeCombination;
using Entegro.Application.DTOs.ProductVariantAttributeValue;
using Entegro.Application.Mappings.Commerce.Smartstore;
using Entegro.Domain.Entities;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO.Hashing;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using static System.Net.WebRequestMethods;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Entegro.Application.Services.Commerce.Smartstore
{
    public class SmartstoreClient
    {
        private readonly string EntegroUrl = "https://localhost:7230";

        private readonly ILogger<SmartstoreClient> _logger;
        private readonly HttpClient _httpClient;
        private readonly JsonSerializerOptions _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
        };

        public SmartstoreClient(ILogger<SmartstoreClient> logger, HttpClient httpClient)
        {
            _logger = logger;

            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri("https://eticaret.ozgurteknolojiyazilim.com/odata/v1/");
            _httpClient.DefaultRequestHeaders.Accept.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

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
                var data = JsonSerializer.Deserialize<ODataListResponse<SmartstoreProductDto>>(json, _jsonOptions);

                return data?.Value?.FirstOrDefault() is SmartstoreProductDto dto
                    ? SmartstoreProductMapper.ToDto(dto)
                    : null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while fetching product by SKU: {Sku}", sku);
                throw; // sessiz geçmek yerine fırlatalım
            }
        }

        public async Task UpsertProductAsync(UpsertProductRequest request)
        {
            // 1. Ürün var mı kontrolü
            var existingProduct = await GetProductBySkuAsync(request.Product.Code);
            int productId;
            if (existingProduct != null)
            {
                request.Product.Id = existingProduct.Id;
                productId = existingProduct.Id;
                await UpdateProductAsync(request.Product, request.CustomData as SmartstoreProductIntegrationCustomDto);
            }
            else
            {
                request.Product.Id = 0;
                productId = await CreateProductAsync(request.Product,request.CustomData as SmartstoreProductIntegrationCustomDto) ?? 0;
            }

            // 2. Kategoriler
            await HandleCategoriesAsync(productId, request.Product);

            // 3. Marka
            await HandleBrandAsync(productId, request.Product);

            // 4. Resimler
            await HandleMediaFilesAsync(productId, request.Product);

            // 5. Özellikler (Attributes)
            await HandleAttributesAsync(productId, request.Product);

            // 6. Variant Combinations
            await HandleVariantCombinationsAsync(productId, request.Product);
        }
        public async Task UpsertProductsAsync(IEnumerable<UpsertProductRequest> requests)
        {
            foreach (var request in requests)
            {
                if (request == null)
                {
                    throw new ArgumentNullException(nameof(request), "Request cannot be null.");
                }

                await UpsertProductAsync(request);
            }
        }
        public async Task<int?> CreateProductAsync(ProductDto product, SmartstoreProductIntegrationCustomDto? customData)
        {
            var payload = SmartstoreProductMapper.ToDto(product, customData);
            var json = JsonSerializer.Serialize(payload, _jsonOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("products", content);
            response.EnsureSuccessStatusCode();

            var created = await response.Content.ReadFromJsonAsync<SmartstoreProductDto>();
            return created?.Id ?? 0;
        }
        public async Task UpdateProductAsync(ProductDto product, SmartstoreProductIntegrationCustomDto? customData)
        {
            var payload = SmartstoreProductMapper.ToDto(product, customData);
            if (payload == null)
                throw new Exception("SmartstoreProductMapper returned null");

            var json = JsonSerializer.Serialize(payload, _jsonOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PutAsync($"products({product.Id})", content);
            response.EnsureSuccessStatusCode();
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

        private async Task HandleBrandAsync(int productId, ProductDto product)
        {
            if (string.IsNullOrEmpty(product.Brand?.Name))
                return;

            _logger.LogInformation("Checking brand: {Brand}", product.Brand.Name);

            var existingBrand = await BrandExistsAsync(product.Brand.Name);
            if (existingBrand != null)
            {
                product.BrandId = existingBrand.Id;
                _logger.LogInformation("Brand exists. Using BrandId={BrandId}", product.BrandId);
            }
            else
            {
                product.BrandId = await CreateBrandAsync(product.Brand);
                _logger.LogInformation("Brand created. BrandId={BrandId}", product.BrandId);
            }

            var existingProductBrand = await GetProductBrand(productId, product.BrandId.Value);
            if (existingProductBrand == null)
            {
                ProductBrandDto productBrand = new ProductBrandDto();
                productBrand.Id = 0;
                productBrand.ProductId = productId;
                productBrand.ManufacturerId = product.BrandId.Value;
                productBrand.IsFeaturedProduct = true;
                productBrand.DisplayOrder = 0;
                await CreateProductBrandAsync(productBrand);
            }
            else
            {
                ProductBrandDto productBrand = new ProductBrandDto();
                productBrand.Id = existingProductBrand.Id;
                productBrand.ProductId = productId;
                productBrand.ManufacturerId = product.BrandId.Value;
                productBrand.IsFeaturedProduct = true;
                productBrand.DisplayOrder = 0;
                await UpdateProductBrandAsync(productBrand);
            }
        }
        private async Task HandleCategoriesAsync(int productId, ProductDto product)
        {
            if (product.ProductCategories == null || !product.ProductCategories.Any())
                return;

            foreach (var productCategory in product.ProductCategories)
            {
                var existing = await CategoryExistsAsync(productCategory.Category.Name);
                productCategory.CategoryId = existing != null
                    ? existing.Id
                    : await EnsureCategoryHierarchyAsync(productCategory.Category);

                var existingProductCategory = await GetProductCategory(productId, productCategory.CategoryId);
                if (existingProductCategory == null)
                {
                    productCategory.Id = 0;
                    productCategory.ProductId = productId;
                    await CreateProductCategoryAsync(productCategory);
                }
                else
                {
                    productCategory.Id = existingProductCategory.Id;
                    productCategory.ProductId = productId;
                    await UpdateProductCategoryAsync(productCategory);
                }
            }
        }
        private async Task<int> EnsureCategoryHierarchyAsync(CategoryDto category)
        {
            if (category.Parent != null)
            {
                category.Parent.Id = await EnsureCategoryHierarchyAsync(category.Parent);
                category.ParentCategoryId = category.Parent.Id;
            }

            var existing = await CategoryExistsAsync(category.Name);
            if (existing != null)
                return existing.Id;

            return await CreateCategoryAsync(category);
        }
        private async Task HandleMediaFilesAsync(int productId, ProductDto product)
        {
            if (product.ProductMediaFiles == null || !product.ProductMediaFiles.Any())
                return;

            foreach (var productMediaFile in product.ProductMediaFiles)
            {
                var fileExists = await FileExists($"catalog/{productMediaFile.MediaFile.Name}");
                if (fileExists != null && fileExists.Value)
                {
                    var smartstoreFile = await GetFileByPath($"catalog/{productMediaFile.MediaFile.Name}");
                    productMediaFile.MediaFileId = smartstoreFile.Id;
                    productMediaFile.ProductId = productId;
                }
                else
                {
                    SmartstoreFileDto smartstoreFile = new SmartstoreFileDto();
                    smartstoreFile.File = await getFileAsync($"{EntegroUrl}{productMediaFile.MediaFile.Name}");
                    smartstoreFile.FileName = string.Format($"catalog/{productMediaFile.MediaFile.Name}");
                    smartstoreFile.MimeType = productMediaFile.MediaFile.MimeType;

                    productMediaFile.MediaFileId = await CreateMediaFileAsync(smartstoreFile) ?? 0;
                    productMediaFile.ProductId = productId;
                }

                var existingProductMediaFile = await GetProductMediaFile(productId, productMediaFile.MediaFileId);
                if (existingProductMediaFile == null)
                {
                    productMediaFile.Id = 0;
                    await CreateProductMediaFileAsync(productMediaFile);
                }
                else
                {
                    productMediaFile.Id = existingProductMediaFile.Id;
                    await UpdateProductMediaFileAsync(productMediaFile);
                }
            }
        }
        private async Task HandleAttributesAsync(int productId, ProductDto product)
        {
            if (product.ProductVariantAttributes == null || !product.ProductVariantAttributes.Any())
                return;

            foreach (var productVariantAttribute in product.ProductVariantAttributes)
            {
                productVariantAttribute.EntityId = productVariantAttribute.Id;
                // ProductAttribute kontrol / ekle
                var existingAttr = await ProductAttributeExistsAsync(productVariantAttribute.ProductAttribute.Name);
                int productAttributeId = existingAttr != null
                    ? existingAttr.Id
                    : await CreateProductAttributeAsync(productVariantAttribute.ProductAttribute);

                // ProductVariantAttribute kontrol / ekle
                var existingVariantAttr = await ProductVariantAttributeExistsAsync(productId, productVariantAttribute.Id);
                productVariantAttribute.Id = existingVariantAttr?.Id ?? await CreateProductVariantAttributeAsync(new ProductVariantAttributeDto
                {
                    ProductId = productId,
                    ProductAttributeId = productAttributeId,
                    IsRequried = true,
                    DisplayOrder = 0,
                    AttributeControlTypeId = 1,
                });

                // AttributeValue kontrol / ekle
                foreach (var productVariantAttributeValue in productVariantAttribute.ProductVariantAttributeValues)
                {
                    productVariantAttributeValue.EntityId = productVariantAttributeValue.Id;
                    productVariantAttributeValue.ProductVariantAttributeId = productVariantAttribute.Id;

                    var existingValue = await ProductVariantAttributeValueExistsAsync(productVariantAttribute.Id, productVariantAttributeValue.Name);
                    productVariantAttributeValue.Id = existingValue != null
                        ? existingValue.Id
                        : await CreateProductVariantAttributeValueAsync(productVariantAttributeValue);
                }
            }
        }
        private async Task HandleVariantCombinationsAsync(int productId, ProductDto product)
        {
            if (product.ProductVariantAttributeCombinations == null || !product.ProductVariantAttributeCombinations.Any())
                return;

            foreach (var combination in product.ProductVariantAttributeCombinations)
            {
                combination.ProductId = productId;
                List<KeyValuePair<int, ICollection<object>>> attributes = new List<KeyValuePair<int, ICollection<object>>>();

                var rawAttributes = JsonSerializer.Deserialize<List<ProductVariantAttributeModel>>(combination.RawAttribute);

                foreach (var rawAttibute in rawAttributes)
                {
                    var productVariantAttribute = product.ProductVariantAttributes.Where(m => m.EntityId == rawAttibute.ProductAttributeId).First();
                    var productVariantAttributeValue = productVariantAttribute.ProductVariantAttributeValues.Where(m => m.EntityId == rawAttibute.ProductAttributeValueId).ToList();

                    attributes.Add(new KeyValuePair<int, ICollection<object>>(productVariantAttribute.Id, productVariantAttributeValue.Select(m => m.Id as object).ToList()));
                }

                RawAttribute rawAttribute = new RawAttribute();
                rawAttribute.Attributes = attributes;

                combination.RawAttribute = JsonSerializer.Serialize(rawAttribute);
                combination.HashCode = GetHashCode(rawAttribute);

                var existingCombination = await GetProductVariantAttributeCombination(productId, combination.HashCode);

                if (existingCombination != null)
                {
                    combination.Id = existingCombination.Id;
                    await UpdateProductVariantAttributeCombination(combination);
                }
                else
                {
                    combination.Id = 0;
                    await CreateProductVariantAttributeCombination(combination);
                }
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
            response.EnsureSuccessStatusCode();

            var created = await response.Content.ReadFromJsonAsync<SmartstoreManufacturerDto>();
            return created?.Id ?? 0;
        }
        public async Task UpdateBrandAsync(BrandDto brand, int id)
        {
            var payload = SmartstoreManufacturerMapper.ToDto(brand);
            if (payload == null)
                throw new Exception("SmartstoreManufacturerMapper returned null");

            payload.Id = id;
            var json = JsonSerializer.Serialize(payload, _jsonOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PutAsync($"manufacturers({id})", content);
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
                var url = $"manufacturers?$filter=Name eq '{brandName}'";
                var response = await _httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();

                var json = await response.Content.ReadAsStringAsync();
                var data = JsonSerializer.Deserialize<ODataListResponse<SmartstoreManufacturerDto>>(json, _jsonOptions);

                return data?.Value?.FirstOrDefault() is SmartstoreManufacturerDto dto
                    ? SmartstoreManufacturerMapper.ToDto(dto)
                    : null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "BrandExistsAsync");
                return null;
            }
        }
        public async Task<ProductBrandDto?> GetProductBrand(int productId, int brandId)
        {
            try
            {
                var url = $"productmanufacturers?$filter=ProductId eq {productId} and ManufacturerId eq {brandId}";
                var response = await _httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();

                var json = await response.Content.ReadAsStringAsync();
                var data = JsonSerializer.Deserialize<ODataListResponse<SmartstoreProductManufacturerDto>>(json, _jsonOptions);

                return data?.Value?.FirstOrDefault() is SmartstoreProductManufacturerDto dto
                    ? SmartstoreProductManufacturerMapper.ToDto(dto)
                    : null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "");
                return null;
            }
        }
        public async Task<int> CreateProductBrandAsync(ProductBrandDto productBrand)
        {
            var payload = SmartstoreProductManufacturerMapper.ToDto(productBrand);
            var json = JsonSerializer.Serialize(payload, _jsonOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("productmanufacturers", content);
            response.EnsureSuccessStatusCode();

            var created = await response.Content.ReadFromJsonAsync<SmartstoreProductCategoryDto>();
            return created?.Id ?? 0;
        }
        public async Task UpdateProductBrandAsync(ProductBrandDto productBrand)
        {
            var payload = SmartstoreProductManufacturerMapper.ToDto(productBrand);
            var json = JsonSerializer.Serialize(payload, _jsonOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PatchAsync("productmanufacturers", content);
            response.EnsureSuccessStatusCode();
        }
        #endregion

        #region Category
        public async Task<int> CreateCategoryAsync(CategoryDto category)
        {
            var payload = SmartstoreCategoryMapper.ToDto(category);
            var json = JsonSerializer.Serialize(payload, _jsonOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("categories", content);
            response.EnsureSuccessStatusCode();

            var created = await response.Content.ReadFromJsonAsync<SmartstoreCategoryDto>();
            return created?.Id ?? 0;
        }
        public async Task UpdateCategoryAsync(CategoryDto category, int id)
        {
            var payload = SmartstoreCategoryMapper.ToDto(category);
            if (payload == null)
                throw new Exception("SmartstoreCategoryMapper returned null");

            payload.Id = id;
            var json = JsonSerializer.Serialize(payload, _jsonOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PutAsync($"categories({id})", content);
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
                var url = $"categories?$filter=Name eq '{categoryName}'";
                var response = await _httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();

                var json = await response.Content.ReadAsStringAsync();
                var data = JsonSerializer.Deserialize<ODataListResponse<SmartstoreCategoryDto>>(json, _jsonOptions);

                return data?.Value?.FirstOrDefault() is SmartstoreCategoryDto dto
                    ? SmartstoreCategoryMapper.ToDto(dto)
                    : null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "CategoryExistsAsync");
                return null;
            }
        }
        public async Task<ProductCategoryDto?> GetProductCategory(int productId, int categoryId)
        {
            try
            {
                var url = $"productcategories?$filter=ProductId eq {productId} and CategoryId eq {categoryId}";
                var response = await _httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();

                var json = await response.Content.ReadAsStringAsync();
                var data = JsonSerializer.Deserialize<ODataListResponse<SmartstoreProductCategoryDto>>(json, _jsonOptions);

                return data?.Value?.FirstOrDefault() is SmartstoreProductCategoryDto dto
                    ? SmartstoreProductCategoryMapper.ToDto(dto)
                    : null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "");
                return null;
            }
        }
        public async Task<int> CreateProductCategoryAsync(ProductCategoryDto productCategory)
        {
            var payload = SmartstoreProductCategoryMapper.ToDto(productCategory);
            var json = JsonSerializer.Serialize(payload, _jsonOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("productcategories", content);
            response.EnsureSuccessStatusCode();

            var created = await response.Content.ReadFromJsonAsync<SmartstoreProductCategoryDto>();
            return created?.Id ?? 0;
        }
        public async Task UpdateProductCategoryAsync(ProductCategoryDto productCategory)
        {
            var payload = SmartstoreProductCategoryMapper.ToDto(productCategory);
            var json = JsonSerializer.Serialize(payload, _jsonOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PatchAsync("productcategories", content);
            response.EnsureSuccessStatusCode();
        }
        #endregion

        #region Media File
        public async Task<int?> CreateMediaFileAsync(SmartstoreFileDto smartstoreFile)
        {
            MultipartFormDataContent multipartContent = new MultipartFormDataContent();

            var fileContent = new ByteArrayContent(smartstoreFile.File);
            fileContent.Headers.ContentType = MediaTypeHeaderValue.Parse(smartstoreFile.MimeType);
            multipartContent.Add(fileContent, "file", smartstoreFile.FileName);
            multipartContent.Add(new StringContent(smartstoreFile.FileName, Encoding.UTF8), "path");

            var request = new HttpRequestMessage(HttpMethod.Post, "mediafiles/savefile");
            request.Content = multipartContent;

            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            var data = JsonSerializer.Deserialize<SmartstoreFileItemInfoDto>(json, _jsonOptions);

            return data?.Id;
        }
        public async Task<ODataResponse<bool>?> FileExists(string filePath)
        {
            try
            {
                var jsonContent = new
                {
                    path = filePath
                };

                var request = new HttpRequestMessage(HttpMethod.Post, "mediafiles/fileexists");
                request.Content = new StringContent(JsonSerializer.Serialize(jsonContent), Encoding.UTF8, "application/json");
                var response = await _httpClient.SendAsync(request);
                response.EnsureSuccessStatusCode();
                var json = await response.Content.ReadAsStringAsync();
                var data = JsonSerializer.Deserialize<ODataResponse<bool>>(json, _jsonOptions);
                return data;
            }
            catch (Exception)
            {
                return null;
            }
        }
        public async Task<SmartstoreFileItemInfoDto?> GetFileByPath(string filePath)
        {
            try
            {
                var jsonContent = new
                {
                    path = filePath
                };

                var request = new HttpRequestMessage(HttpMethod.Post, "mediafiles/getfilebypath");
                request.Content = new StringContent(JsonSerializer.Serialize(jsonContent), Encoding.UTF8, "application/json");
                var response = await _httpClient.SendAsync(request);
                response.EnsureSuccessStatusCode();
                var json = await response.Content.ReadAsStringAsync();
                var data = JsonSerializer.Deserialize<SmartstoreFileItemInfoDto>(json, _jsonOptions);
                return data;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<ProductMediaFileDto?> GetProductMediaFile(int productId, int mediaFileId)
        {
            try
            {
                var url = $"productmediafiles?$filter=ProductId eq {productId} and MediaFileId eq {mediaFileId}";
                var response = await _httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();

                var json = await response.Content.ReadAsStringAsync();
                var data = JsonSerializer.Deserialize<ODataListResponse<SmartstoreProductMediaFileDto>>(json, _jsonOptions);

                return data?.Value?.FirstOrDefault() is SmartstoreProductMediaFileDto dto
                    ? SmartstoreProductMediaFileMapper.ToDto(dto)
                    : null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "");
                return null;
            }
        }
        public async Task<int> CreateProductMediaFileAsync(ProductMediaFileDto productMediaFile)
        {
            var payload = SmartstoreProductMediaFileMapper.ToDto(productMediaFile);
            var json = JsonSerializer.Serialize(payload, _jsonOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("productmediafiles", content);
            response.EnsureSuccessStatusCode();

            var created = await response.Content.ReadFromJsonAsync<SmartstoreProductMediaFileDto>();
            return created?.Id ?? 0;
        }
        public async Task UpdateProductMediaFileAsync(ProductMediaFileDto productMediaFile)
        {
            var payload = SmartstoreProductMediaFileMapper.ToDto(productMediaFile);
            var json = JsonSerializer.Serialize(payload, _jsonOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PatchAsync("productmediafiles", content);
            response.EnsureSuccessStatusCode();
        }
        #endregion

        #region Product Atribute
        public async Task<ProductAttributeDto?> ProductAttributeExistsAsync(string name)
        {
            try
            {
                var url = $"productattributes?$filter=Name eq '{name}'";
                var response = await _httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();

                var json = await response.Content.ReadAsStringAsync();
                var data = JsonSerializer.Deserialize<ODataListResponse<SmartstoreProductAttributeDto>>(json, _jsonOptions);

                return data?.Value?.FirstOrDefault() is SmartstoreProductAttributeDto dto
                    ? SmartstoreProductAttributeMapper.ToDto(dto)
                    : null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "");
                return null;
            }
        }
        public async Task<int> CreateProductAttributeAsync(ProductAttributeDto productAttribute)
        {
            var payload = SmartstoreProductAttributeMapper.ToDto(productAttribute);
            var json = JsonSerializer.Serialize(payload, _jsonOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("productattributes", content);
            response.EnsureSuccessStatusCode();

            var created = await response.Content.ReadFromJsonAsync<SmartstoreProductAttributeDto>();
            return created?.Id ?? 0;
        }
        public async Task DeleteProductAttributeAsync(int productAttributeId)
        {
            var response = await _httpClient.DeleteAsync($"productattributes({productAttributeId})");
            response.EnsureSuccessStatusCode();
        }
        #endregion

        #region Product Variant Attribute
        public async Task<ProductVariantAttributeDto?> ProductVariantAttributeExistsAsync(int productId, int productAttributeId)
        {
            try
            {
                var url = $"productvariantattributes?$expand=ProductAttribute&$filter=ProductId eq {productId} and ProductAttributeId eq {productAttributeId}";
                var response = await _httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();

                var json = await response.Content.ReadAsStringAsync();
                var data = JsonSerializer.Deserialize<ODataListResponse<SmartstoreProductVariantAttributeDto>>(json, _jsonOptions);

                return data?.Value?.FirstOrDefault() is SmartstoreProductVariantAttributeDto dto
                    ? SmartstoreProductVariantAttributeMapper.ToDto(dto)
                    : null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "");
                return null;
            }
        }
        public async Task<ProductVariantAttributeValueDto?> ProductVariantAttributeValueExistsAsync(int productVariantAttributeId, string name)
        {
            try
            {
                var url = $"productvariantattributevalues?$filter=ProductVariantAttributeId eq {productVariantAttributeId} and Name eq '{name}'";
                var response = await _httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();

                var json = await response.Content.ReadAsStringAsync();
                var data = JsonSerializer.Deserialize<ODataListResponse<SmartstoreProductVariantAttributeValueDto>>(json, _jsonOptions);

                return data?.Value?.FirstOrDefault() is SmartstoreProductVariantAttributeValueDto dto
                    ? SmartstoreProductVariantAttributeValueMapper.ToDto(dto)
                    : null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "");
                return null;
            }
        }
        public async Task<int> CreateProductVariantAttributeAsync(ProductVariantAttributeDto productVariantAttribute)
        {
            var payload = SmartstoreProductVariantAttributeMapper.ToDto(productVariantAttribute);
            var json = JsonSerializer.Serialize(payload, _jsonOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("productvariantattributes", content);
            response.EnsureSuccessStatusCode();

            var created = await response.Content.ReadFromJsonAsync<SmartstoreProductVariantAttributeDto>();
            return created?.Id ?? 0;
        }
        public async Task<int> CreateProductVariantAttributeValueAsync(ProductVariantAttributeValueDto productVariantAttributeValue)
        {
            var payload = SmartstoreProductVariantAttributeValueMapper.ToDto(productVariantAttributeValue);
            var json = JsonSerializer.Serialize(payload, _jsonOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("productvariantattributevalues", content);
            response.EnsureSuccessStatusCode();

            var created = await response.Content.ReadFromJsonAsync<SmartstoreProductVariantAttributeValueDto>();
            return created?.Id ?? 0;
        }
        #endregion

        #region Product Variant Attribute Combination
        public async Task<ProductVariantAttributeCombinationDto?> GetProductVariantAttributeCombination(int productId, int hashCode)
        {
            try
            {
                var url = $"productvariantattributecombinations?$filter=ProductId eq {productId} and HashCode eq {hashCode}";
                var response = await _httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();

                var json = await response.Content.ReadAsStringAsync();
                var data = JsonSerializer.Deserialize<ODataListResponse<SmartstoreProductVariantAttributeCombinationDto>>(json, _jsonOptions);

                return data?.Value?.FirstOrDefault() is SmartstoreProductVariantAttributeCombinationDto dto
                    ? SmartstoreProductVariantAttributeCombinationMapper.ToDto(dto)
                    : null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "");
                return null;
            }
        }
        public async Task<int?> CreateProductVariantAttributeCombination(ProductVariantAttributeCombinationDto productVariantAttributeCombination)
        {
            var payload = SmartstoreProductVariantAttributeCombinationMapper.ToDto(productVariantAttributeCombination);
            var json = JsonSerializer.Serialize(payload, _jsonOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("productvariantattributecombinations", content);
            response.EnsureSuccessStatusCode();

            var created = await response.Content.ReadFromJsonAsync<SmartstoreProductVariantAttributeCombinationDto>();
            return created?.Id ?? 0;
        }
        public async Task UpdateProductVariantAttributeCombination(ProductVariantAttributeCombinationDto productVariantAttributeCombination)
        {
            var payload = SmartstoreProductVariantAttributeCombinationMapper.ToDto(productVariantAttributeCombination);
            var json = JsonSerializer.Serialize(payload, _jsonOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PatchAsync($"productvariantattributecombinations({productVariantAttributeCombination.Id})", content);
            response.EnsureSuccessStatusCode();
        }
        #endregion

        #region Other
        private async Task<byte[]> getFileAsync(string url)
        {
            using (var client = new HttpClient())
            {
                using (var response = await client.GetAsync(url))
                {
                    byte[] imageBytes = await response.Content.ReadAsByteArrayAsync();

                    return imageBytes;
                }
            }
        }

        private int GetHashCode(RawAttribute rawAttribute)
        {
            var combiner = HashCodeCombiner.Start();
            var attributes = rawAttribute.Attributes.OrderBy(x => x.Key).ToArray();

            for (var i = 0; i < attributes.Length; ++i)
            {
                var attribute = attributes[i];

                combiner.Add(attribute.Key);

                var values = attribute.Value
                    .Select(x => x.ToString())
                    .OrderBy(x => x)
                    .ToArray();

                for (var j = 0; j < values.Length; ++j)
                {
                    combiner.Add(values[j]);
                }
            }

            return combiner.CombinedHash;
        }
        #endregion
    }

    public class RawAttribute
    {
        public List<KeyValuePair<int, ICollection<object>>> Attributes { get; set; }
    }

    public struct HashCodeCombiner
    {
        const long _globalSeed = 0x1505L;

        private long _combinedHash64;

        /// <summary>
        /// Initializes the <see cref="HashCodeCombiner"/> with zero seed.
        /// </summary>
        public HashCodeCombiner()
        {
        }

        /// <summary>
        /// Initializes the <see cref="HashCodeCombiner"/> with the given <paramref name="seed"/>.
        /// </summary>
        public HashCodeCombiner(long seed)
        {
            _combinedHash64 = seed;
        }

        /// <summary>
        /// Initializes a deterministic <see cref="HashCodeCombiner"/>.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static HashCodeCombiner Start()
        {
            return new HashCodeCombiner(_globalSeed);
        }

        /// <summary>
        /// Initializes a non-deterministic <see cref="HashCodeCombiner"/>.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static HashCodeCombiner StartNonDeterministic()
        {
            return new HashCodeCombiner(CurrentSeed);
        }

        public int CombinedHash
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return _combinedHash64.GetHashCode(); }
        }

        public string CombinedHashString
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return _combinedHash64.GetHashCode().ToString("x", CultureInfo.InvariantCulture); }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator int(HashCodeCombiner self)
        {
            return self.CombinedHash;
        }

        internal static long GlobalSeed { get; } = _globalSeed;
        internal static long CurrentSeed { get; } = GenerateRandomInteger(min: int.MinValue);

        public static int GenerateRandomInteger(int min = 0, int max = int.MaxValue)
        {
            return Random.Shared.Next(min, max);
        }

        public HashCodeCombiner AddSequence<T>(IEnumerable<T> sequence, IEqualityComparer<T>? comparer = null)
            where T : notnull
        {
            if (sequence is not null)
            {
                var count = 0;
                foreach (var o in sequence)
                {
                    Add(o, comparer);
                    count++;
                }

                Append(count);
            }

            return this;
        }

        public HashCodeCombiner AddDictionary<TKey, TValue>(IEnumerable<KeyValuePair<TKey, TValue>> dictionary)
            where TKey : notnull
            where TValue : notnull
        {
            if (dictionary is not null)
            {
                foreach (var kvp in dictionary.OrderBy(x => x.Key))
                {
                    Add(kvp.Key);
                    Add(kvp.Value);
                }
            }

            return this;
        }

        public HashCodeCombiner Add<TStruct>(TStruct? value)
            where TStruct : struct
        {
            // Optimization: for value types, we can avoid boxing "value" by skipping the null check
            if (value.HasValue)
            {
                Append(value.GetHashCode());
            }

            return this;
        }

        public HashCodeCombiner Add<TStruct>(TStruct value)
            where TStruct : struct
        {
            // Optimization: for value types, we can avoid boxing "value" by skipping the null check
            Append(value.GetHashCode());

            return this;
        }

        public HashCodeCombiner Add<T>(T value, IEqualityComparer<T>? comparer = null)
        {
            if (value is string str)
            {
                // XxHash3 is faster than Marvin
                Append((long)XxHash3.HashToUInt64(Encoding.UTF8.GetBytes(str)));
            }
            else if (value is not null)
            {
                Append(comparer?.GetHashCode(value) ?? value.GetHashCode());
            }

            return this;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void Append(long hash)
        {
            if (hash != 0)
            {
                _combinedHash64 = (_combinedHash64 << 5) + _combinedHash64 ^ hash;
            }
        }
    }

    public class ProductVariantAttributeModel
    {
        public int ProductAttributeId { get; set; }
        public int ProductAttributeValueId { get; set; }
    }
}
