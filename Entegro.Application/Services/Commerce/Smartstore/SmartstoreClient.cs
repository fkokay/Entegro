using Entegro.Application.DTOs.Brand;
using Entegro.Application.DTOs.Category;
using Entegro.Application.DTOs.Commerce;
using Entegro.Application.DTOs.Commerce.Smartstore;
using Entegro.Application.DTOs.Common;
using Entegro.Application.DTOs.Product;
using Entegro.Application.DTOs.ProductAttribute;
using Entegro.Application.DTOs.ProductBrand;
using Entegro.Application.DTOs.ProductCategory;
using Entegro.Application.DTOs.ProductMediaFile;
using Entegro.Application.DTOs.ProductVariantAttribute;
using Entegro.Application.DTOs.ProductVariantAttributeCombination;
using Entegro.Application.DTOs.ProductVariantAttributeValue;
using Entegro.Application.Interfaces.Services.Base;
using Entegro.Application.Mappings.Commerce.Smartstore;
using Microsoft.Extensions.Logging;
using System.Globalization;
using System.IO.Hashing;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Xml.Linq;

namespace Entegro.Application.Services.Commerce.Smartstore
{
    public class SmartstoreClient
    {
        private readonly ISettingService _settingService;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<SmartstoreClient> _logger;
        
        private readonly JsonSerializerOptions _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
        };

        public SmartstoreClient(ISettingService settingService, IHttpClientFactory httpClientFactory, ILogger<SmartstoreClient> logger)
        {
            _settingService = settingService;
            _httpClientFactory = httpClientFactory;
            _logger = logger;
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

        #region Product
        public async Task<ProductDto?> GetProductBySkuAsync(SmartstoreApiContext context,string sku)
        {
            try
            {
                var httpClient = CreateHttpClient(context);

                var url = $"products?$filter=Sku eq '{sku}'";
                var response = await httpClient.GetAsync(url);
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
        public async Task UpsertProductAsync(SmartstoreApiContext context,UpsertProductRequest request)
        {
            // 1. Ürün var mı kontrolü
            var existingProduct = await GetProductBySkuAsync(context,request.Product.Code);
            int productId;
            if (existingProduct != null)
            {
                request.Product.Id = existingProduct.Id;
                productId = existingProduct.Id;
                await UpdateProductAsync(context,request.Product, request.CustomData as SmartstoreProductIntegrationCustomDto);
            }
            else
            {
                request.Product.Id = 0;
                productId = await CreateProductAsync(context,request.Product, request.CustomData as SmartstoreProductIntegrationCustomDto) ?? 0;
            }

            // 2. Kategoriler
            await HandleCategoriesAsync(context,productId, request.Product);

            // 3. Marka
            await HandleBrandAsync(context, productId, request.Product);

            // 4. Resimler
            await HandleMediaFilesAsync(context, productId, request.Product);

            // 5. Özellikler (Attributes)
            await HandleAttributesAsync(context, productId, request.Product);

            // 6. Variant Combinations
            await HandleVariantCombinationsAsync(context, productId, request.Product);
        }
        public async Task UpsertProductsAsync(SmartstoreApiContext context,IEnumerable<UpsertProductRequest> requests)
        {
            foreach (var request in requests)
            {
                if (request == null)
                {
                    throw new ArgumentNullException(nameof(request), "Request cannot be null.");
                }

                await UpsertProductAsync(context,request);
            }
        }
        public async Task<int?> CreateProductAsync(SmartstoreApiContext context,ProductDto product, SmartstoreProductIntegrationCustomDto? customData)
        {
            var httpClient = CreateHttpClient(context);
            var payload = SmartstoreProductMapper.ToDto(product, customData);
            var json = JsonSerializer.Serialize(payload, _jsonOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await httpClient.PostAsync("products", content);
            response.EnsureSuccessStatusCode();

            var created = await response.Content.ReadFromJsonAsync<SmartstoreProductDto>();
            return created?.Id ?? 0;
        }
        public async Task UpdateProductAsync(SmartstoreApiContext context,ProductDto product, SmartstoreProductIntegrationCustomDto? customData)
        {
            var httpClient = CreateHttpClient(context);
            var payload = SmartstoreProductMapper.ToDto(product, customData);
            if (payload == null)
                throw new Exception("SmartstoreProductMapper returned null");

            var json = JsonSerializer.Serialize(payload, _jsonOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await httpClient.PutAsync($"products({product.Id})", content);
            var result = await response.Content.ReadAsStringAsync();
            response.EnsureSuccessStatusCode();
        }
        public async Task DeleteProductAsync(SmartstoreApiContext context,string sku)
        {
            var httpClient = CreateHttpClient(context);
            var product = await GetProductBySkuAsync(context,sku);
            if (product == null)
            {
                throw new InvalidOperationException($"Product with SKU '{sku}' not found.");
            }
            var response = await httpClient.DeleteAsync($"products({product.Id})");
            response.EnsureSuccessStatusCode();
        }
        public async Task DeleteProductsAsync(SmartstoreApiContext context,IEnumerable<string> skus)
        {
            foreach (var sku in skus)
            {
                await DeleteProductAsync(context, sku);
            }
        }
        private async Task HandleBrandAsync(SmartstoreApiContext context,int productId, ProductDto product)
        {
            if (string.IsNullOrEmpty(product.Brand?.Name))
                return;

            _logger.LogInformation("Checking brand: {Brand}", product.Brand.Name);

            var existingBrand = await BrandExistsAsync(context,product.Brand.Name);
            if (existingBrand != null)
            {
                product.BrandId = existingBrand.Id;
                _logger.LogInformation("Brand exists. Using BrandId={BrandId}", product.BrandId);
            }
            else
            {
                product.BrandId = await CreateBrandAsync(context, product.Brand);
                _logger.LogInformation("Brand created. BrandId={BrandId}", product.BrandId);
            }

            var existingProductBrand = await GetProductBrand(context,productId, product.BrandId.Value);
            if (existingProductBrand == null)
            {
                ProductBrandDto productBrand = new ProductBrandDto();
                productBrand.Id = 0;
                productBrand.ProductId = productId;
                productBrand.ManufacturerId = product.BrandId.Value;
                productBrand.IsFeaturedProduct = true;
                productBrand.DisplayOrder = 0;
                await CreateProductBrandAsync(context, productBrand);
            }
            else
            {
                ProductBrandDto productBrand = new ProductBrandDto();
                productBrand.Id = existingProductBrand.Id;
                productBrand.ProductId = productId;
                productBrand.ManufacturerId = product.BrandId.Value;
                productBrand.IsFeaturedProduct = true;
                productBrand.DisplayOrder = 0;
                await UpdateProductBrandAsync(context, productBrand);
            }
        }
        private async Task HandleCategoriesAsync(SmartstoreApiContext context,int productId, ProductDto product)
        {
            if (product.ProductCategories == null || !product.ProductCategories.Any())
                return;

            foreach (var productCategory in product.ProductCategories)
            {
                var existing = await CategoryExistsAsync(context, productCategory.Category.Name);
                productCategory.CategoryId = existing != null
                    ? existing.Id
                    : await EnsureCategoryHierarchyAsync(context, productCategory.Category);

                var existingProductCategory = await GetProductCategory(context, productId, productCategory.CategoryId);
                if (existingProductCategory == null)
                {
                    productCategory.Id = 0;
                    productCategory.ProductId = productId;
                    await CreateProductCategoryAsync(context, productCategory);
                }
                else
                {
                    productCategory.Id = existingProductCategory.Id;
                    productCategory.ProductId = productId;
                    await UpdateProductCategoryAsync(context, productCategory);
                }
            }
        }
        private async Task<int> EnsureCategoryHierarchyAsync(SmartstoreApiContext context,CategoryDto category)
        {
            if (category.Parent != null)
            {
                category.Parent.Id = await EnsureCategoryHierarchyAsync(context, category.Parent);
                category.ParentId = category.Parent.Id;
            }

            var existing = await CategoryExistsAsync(context, category.Name);
            if (existing != null)
                return existing.Id;

            return await CreateCategoryAsync(context, category);
        }
        private async Task HandleMediaFilesAsync(SmartstoreApiContext context,int productId, ProductDto product)
        {
            if (product.ProductMediaFiles == null || !product.ProductMediaFiles.Any())
                return;

            foreach (var productMediaFile in product.ProductMediaFiles)
            {
                var fileExists = await FileExists(context,$"catalog/{productMediaFile.MediaFile.Name}");
                if (fileExists != null && fileExists.Value)
                {
                    var smartstoreFile = await GetFileByPath(context,$"catalog/{productMediaFile.MediaFile.Name}");
                    productMediaFile.MediaFileId = smartstoreFile.Id;
                    productMediaFile.ProductId = productId;
                }
                else
                {
                    var systemurlSetting = await _settingService.GetByKeyAsync("SystemUrl");
                    if (systemurlSetting == null)
                    {
                        _logger.LogError("SystemUrl tanımlı değil");
                        continue;
                    }

                    SmartstoreFileDto smartstoreFile = new SmartstoreFileDto();
                    smartstoreFile.File = await GetFileAsync($"{systemurlSetting.Value}{productMediaFile.MediaFile.Url}");
                    smartstoreFile.FileName = string.Format($"catalog/{productMediaFile.MediaFile.Name}");
                    smartstoreFile.MimeType = productMediaFile.MediaFile.MimeType;

                    productMediaFile.MediaFileId = await CreateMediaFileAsync(context, smartstoreFile) ?? 0;
                    productMediaFile.ProductId = productId;
                }

                var existingProductMediaFile = await GetProductMediaFile(context, productId, productMediaFile.MediaFileId);
                if (existingProductMediaFile == null)
                {
                    var productMediaFileId =await CreateProductMediaFileAsync(context, productMediaFile);
                    productMediaFile.IntegrationId = productMediaFileId;
                }
                else
                {
                    productMediaFile.IntegrationId = existingProductMediaFile.Id;
                    await UpdateProductMediaFileAsync(context, productMediaFile);
                }
            }

            var deletedProductMediaFiles = await GetProductMediaFiles(context,productId);
            if (deletedProductMediaFiles != null)
            {
                foreach (var item in deletedProductMediaFiles)
                {
                    if (!product.ProductMediaFiles.Any(m => m.MediaFileId == item.MediaFileId))
                    {
                        await DeleteProductMediaFileAsync(context,item.Id);
                    }
                }
            }
        }
        private async Task HandleAttributesAsync(SmartstoreApiContext context,int productId, ProductDto product)
        {
            if (product.ProductVariantAttributes == null || !product.ProductVariantAttributes.Any())
                return;

            foreach (var productVariantAttribute in product.ProductVariantAttributes)
            {
                productVariantAttribute.EntityId = productVariantAttribute.Id;
                // ProductAttribute kontrol / ekle
                var existingAttr = await ProductAttributeExistsAsync(context, productVariantAttribute.ProductAttribute.Name);
                int productAttributeId = existingAttr != null
                    ? existingAttr.Id
                    : await CreateProductAttributeAsync(context, productVariantAttribute.ProductAttribute);

                // ProductVariantAttribute kontrol / ekle
                var existingVariantAttr = await ProductVariantAttributeExistsAsync(context, productId, productVariantAttribute.Id);
                productVariantAttribute.Id = existingVariantAttr?.Id ?? await CreateProductVariantAttributeAsync(context, new ProductVariantAttributeDto
                {
                    ProductId = productId,
                    ProductAttributeId = productAttributeId,
                    IsRequried = productVariantAttribute.IsRequried,
                    DisplayOrder = productVariantAttribute.DisplayOrder,
                    AttributeControlTypeId = productVariantAttribute.AttributeControlTypeId,
                });

                // AttributeValue kontrol / ekle
                foreach (var productVariantAttributeValue in productVariantAttribute.ProductVariantAttributeValues)
                {
                    productVariantAttributeValue.EntityId = productVariantAttributeValue.Id;
                    productVariantAttributeValue.ProductVariantAttributeId = productVariantAttribute.Id;

                    var existingValue = await ProductVariantAttributeValueExistsAsync(context, productVariantAttribute.Id, productVariantAttributeValue.Name);
                    productVariantAttributeValue.Id = existingValue != null
                        ? existingValue.Id
                        : await CreateProductVariantAttributeValueAsync(context, productVariantAttributeValue);
                }
            }
        }
        private async Task HandleVariantCombinationsAsync(SmartstoreApiContext context,int productId, ProductDto product)
        {
            if (product.ProductVariantAttributeCombinations == null || !product.ProductVariantAttributeCombinations.Any())
                return;

            foreach (var combination in product.ProductVariantAttributeCombinations)
            {
                combination.ProductId = productId;
                combination.AssignedPictureIds = product.ProductMediaFiles.Where(m=> combination.AssignedPictureIds.Contains(m.Id)).Select(m=>m.MediaFileId).ToArray();


                List<KeyValuePair<int, ICollection<object>>> attributes = new List<KeyValuePair<int, ICollection<object>>>();

                var rawAttributes = JsonSerializer.Deserialize<List<ProductVariantAttributeSelection>>(combination.RawAttribute);

                foreach (var rawAttibute in rawAttributes)
                {
                    var productVariantAttribute = product.ProductVariantAttributes.Where(m => m.EntityId == rawAttibute.ProductVariantAttributeId).First();
                    var productVariantAttributeValue = productVariantAttribute.ProductVariantAttributeValues.Where(m => m.EntityId == rawAttibute.ProductVariantAttributeValueId).ToList();

                    attributes.Add(new KeyValuePair<int, ICollection<object>>(productVariantAttribute.Id, productVariantAttributeValue.Select(m => m.Id as object).ToList()));
                }

                RawAttribute rawAttribute = new RawAttribute();
                rawAttribute.Attributes = attributes;

                combination.RawAttribute = JsonSerializer.Serialize(rawAttribute);
                combination.HashCode = GetHashCode(rawAttribute);

                var existingCombination = await GetProductVariantAttributeCombination(context,productId, combination.HashCode);

                if (existingCombination != null)
                {
                    combination.Id = existingCombination.Id;
                    await UpdateProductVariantAttributeCombination(context,combination);
                }
                else
                {
                    combination.Id = 0;
                    await CreateProductVariantAttributeCombination(context,combination);
                }
            }
        }
        #endregion

        #region Brand
        public async Task<int> CreateBrandAsync(SmartstoreApiContext context,BrandDto brand)
        {
            var httpClient = CreateHttpClient(context);
            var payload = SmartstoreManufacturerMapper.ToDto(brand);
            var json = JsonSerializer.Serialize(payload, _jsonOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await httpClient.PostAsync("manufacturers", content);
            response.EnsureSuccessStatusCode();

            var created = await response.Content.ReadFromJsonAsync<SmartstoreManufacturerDto>();
            return created?.Id ?? 0;
        }
        public async Task UpdateBrandAsync(SmartstoreApiContext context,BrandDto brand, int id)
        {
            var httpClient = CreateHttpClient(context);
            var payload = SmartstoreManufacturerMapper.ToDto(brand);
            if (payload == null)
                throw new Exception("SmartstoreManufacturerMapper returned null");

            payload.Id = id;
            var json = JsonSerializer.Serialize(payload, _jsonOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await httpClient.PutAsync($"manufacturers({id})", content);
            response.EnsureSuccessStatusCode();
        }
        public async Task DeleteBrandAsync(SmartstoreApiContext context, int brandId)
        {
            var httpClient = CreateHttpClient(context);
            var response = await httpClient.DeleteAsync($"manufacturers({brandId})");
            response.EnsureSuccessStatusCode();
        }
        public async Task<BrandDto?> BrandExistsAsync(SmartstoreApiContext context,string brandName)
        {
            try
            {
                var httpClient = CreateHttpClient(context);
                var url = $"manufacturers?$filter=Name eq '{Uri.EscapeDataString(brandName)}'";
                var response = await httpClient.GetAsync(url);
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
        public async Task<ProductBrandDto?> GetProductBrand(SmartstoreApiContext context,int productId, int brandId)
        {
            try
            {
                var httpClient = CreateHttpClient(context);
                var url = $"productmanufacturers?$filter=ProductId eq {productId} and ManufacturerId eq {brandId}";
                var response = await httpClient.GetAsync(url);
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
        public async Task<int> CreateProductBrandAsync(SmartstoreApiContext context,ProductBrandDto productBrand)
        {
            var httpClient = CreateHttpClient(context);
            var payload = SmartstoreProductManufacturerMapper.ToDto(productBrand);
            var json = JsonSerializer.Serialize(payload, _jsonOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await httpClient.PostAsync("productmanufacturers", content);
            response.EnsureSuccessStatusCode();

            var created = await response.Content.ReadFromJsonAsync<SmartstoreProductCategoryDto>();
            return created?.Id ?? 0;
        }
        public async Task UpdateProductBrandAsync(SmartstoreApiContext context,ProductBrandDto productBrand)
        {
            var httpClient = CreateHttpClient(context);
            var payload = SmartstoreProductManufacturerMapper.ToDto(productBrand);
            var json = JsonSerializer.Serialize(payload, _jsonOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await httpClient.PatchAsync("productmanufacturers", content);
            response.EnsureSuccessStatusCode();
        }
        #endregion

        #region Category
        public async Task<int> CreateCategoryAsync(SmartstoreApiContext context,CategoryDto category)
        {
            var httpClient = CreateHttpClient(context);
            var payload = SmartstoreCategoryMapper.ToDto(category);
            var json = JsonSerializer.Serialize(payload, _jsonOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await httpClient.PostAsync("categories", content);
            response.EnsureSuccessStatusCode();

            var created = await response.Content.ReadFromJsonAsync<SmartstoreCategoryDto>();
            return created?.Id ?? 0;
        }
        public async Task UpdateCategoryAsync(SmartstoreApiContext context,CategoryDto category, int id)
        {
            var httpClient = CreateHttpClient(context);
            var payload = SmartstoreCategoryMapper.ToDto(category);
            if (payload == null)
                throw new Exception("SmartstoreCategoryMapper returned null");

            payload.Id = id;
            var json = JsonSerializer.Serialize(payload, _jsonOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await httpClient.PutAsync($"categories({id})", content);
            response.EnsureSuccessStatusCode();
        }
        public async Task DeleteCategoryAsync(SmartstoreApiContext context,int categoryId)
        {
            var httpClient = CreateHttpClient(context);
            var response = await httpClient.DeleteAsync($"categories({categoryId})");
            response.EnsureSuccessStatusCode();
        }
        public async Task<CategoryDto?> CategoryExistsAsync(SmartstoreApiContext context,string categoryName)
        {
            try
            {
                var httpClient = CreateHttpClient(context);
                var url = $"categories?$filter=Name eq '{Uri.EscapeDataString(categoryName)}'";
                var response = await httpClient.GetAsync(url);
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
        public async Task<ProductCategoryDto?> GetProductCategory(SmartstoreApiContext context,int productId, int categoryId)
        {
            try
            {
                var httpClient = CreateHttpClient(context);
                var url = $"productcategories?$filter=ProductId eq {productId} and CategoryId eq {categoryId}";
                var response = await httpClient.GetAsync(url);
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
        public async Task<int> CreateProductCategoryAsync(SmartstoreApiContext context,ProductCategoryDto productCategory)
        {
            var httpClient = CreateHttpClient(context);
            var payload = SmartstoreProductCategoryMapper.ToDto(productCategory);
            var json = JsonSerializer.Serialize(payload, _jsonOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await httpClient.PostAsync("productcategories", content);
            response.EnsureSuccessStatusCode();

            var created = await response.Content.ReadFromJsonAsync<SmartstoreProductCategoryDto>();
            return created?.Id ?? 0;
        }
        public async Task UpdateProductCategoryAsync(SmartstoreApiContext context,ProductCategoryDto productCategory)
        {
            var httpClient = CreateHttpClient(context);
            var payload = SmartstoreProductCategoryMapper.ToDto(productCategory);
            var json = JsonSerializer.Serialize(payload, _jsonOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await httpClient.PatchAsync("productcategories", content);
            response.EnsureSuccessStatusCode();
        }
        #endregion

        #region Media File
        public async Task<int?> CreateMediaFileAsync(SmartstoreApiContext context,SmartstoreFileDto smartstoreFile)
        {
            var httpClient = CreateHttpClient(context);
            MultipartFormDataContent multipartContent = new MultipartFormDataContent();

            var fileContent = new ByteArrayContent(smartstoreFile.File);
            fileContent.Headers.ContentType = MediaTypeHeaderValue.Parse(smartstoreFile.MimeType);
            multipartContent.Add(fileContent, "file", smartstoreFile.FileName);
            multipartContent.Add(new StringContent(smartstoreFile.FileName, Encoding.UTF8), "path");

            var request = new HttpRequestMessage(HttpMethod.Post, "mediafiles/savefile");
            request.Content = multipartContent;

            var response = await httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            var data = JsonSerializer.Deserialize<SmartstoreFileItemInfoDto>(json, _jsonOptions);

            return data?.Id;
        }
        public async Task<ODataResponse<bool>?> FileExists(SmartstoreApiContext context,string filePath)
        {
            try
            {
                var httpClient = CreateHttpClient(context);
                var jsonContent = new
                {
                    path = filePath
                };

                var request = new HttpRequestMessage(HttpMethod.Post, "mediafiles/fileexists");
                request.Content = new StringContent(JsonSerializer.Serialize(jsonContent), Encoding.UTF8, "application/json");
                var response = await httpClient.SendAsync(request);
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
        public async Task<SmartstoreFileItemInfoDto?> GetFileByPath(SmartstoreApiContext context,string filePath)
        {
            try
            {
                var httpClient = CreateHttpClient(context);
                var jsonContent = new
                {
                    path = filePath
                };

                var request = new HttpRequestMessage(HttpMethod.Post, "mediafiles/getfilebypath");
                request.Content = new StringContent(JsonSerializer.Serialize(jsonContent), Encoding.UTF8, "application/json");
                var response = await httpClient.SendAsync(request);
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

        public async Task<List<ProductMediaFileDto>?> GetProductMediaFiles(SmartstoreApiContext context,int productId)
        {
            try
            {
                var httpClient = CreateHttpClient(context);
                var url = $"productmediafiles?$filter=ProductId eq {productId}";
                var response = await httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();

                var json = await response.Content.ReadAsStringAsync();
                var data = JsonSerializer.Deserialize<ODataListResponse<SmartstoreProductMediaFileDto>>(json, _jsonOptions);

                return SmartstoreProductMediaFileMapper.ToDtoList(data.Value).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "");
                return null;
            }
        }
        public async Task<ProductMediaFileDto?> GetProductMediaFile(SmartstoreApiContext context,int productId, int mediaFileId)
        {
            try
            {
                var httpClient = CreateHttpClient(context);
                var url = $"productmediafiles?$filter=ProductId eq {productId} and MediaFileId eq {mediaFileId}";
                var response = await httpClient.GetAsync(url);
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
        public async Task<int> CreateProductMediaFileAsync(SmartstoreApiContext context,ProductMediaFileDto productMediaFile)
        {
            var httpClient = CreateHttpClient(context);
            var payload = SmartstoreProductMediaFileMapper.ToDto(productMediaFile);
            var json = JsonSerializer.Serialize(payload, _jsonOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await httpClient.PostAsync("productmediafiles", content);
            response.EnsureSuccessStatusCode();

            var created = await response.Content.ReadFromJsonAsync<SmartstoreProductMediaFileDto>();
            return created?.Id ?? 0;
        }
        public async Task UpdateProductMediaFileAsync(SmartstoreApiContext context,ProductMediaFileDto productMediaFile)
        {
            var httpClient = CreateHttpClient(context);
            var payload = SmartstoreProductMediaFileMapper.ToDto(productMediaFile);
            var json = JsonSerializer.Serialize(payload, _jsonOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await httpClient.PatchAsync("productmediafiles", content);
            response.EnsureSuccessStatusCode();
        }
        public async Task DeleteProductMediaFileAsync(SmartstoreApiContext context,int id)
        {
            var httpClient = CreateHttpClient(context);
            var response = await httpClient.DeleteAsync("productmediafiles({id})");
            response.EnsureSuccessStatusCode();
        }
        #endregion

        #region Product Atribute
        public async Task<ProductAttributeDto?> ProductAttributeExistsAsync(SmartstoreApiContext context,string name)
        {
            try
            {
                var httpClient = CreateHttpClient(context);
                var url = $"productattributes?$filter=Name eq '{Uri.EscapeDataString(name)}'";
                var response = await httpClient.GetAsync(url);
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
        public async Task<int> CreateProductAttributeAsync(SmartstoreApiContext context,ProductAttributeDto productAttribute)
        {
            var httpClient = CreateHttpClient(context);
            var payload = SmartstoreProductAttributeMapper.ToDto(productAttribute);
            var json = JsonSerializer.Serialize(payload, _jsonOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await httpClient.PostAsync("productattributes", content);
            response.EnsureSuccessStatusCode();

            var created = await response.Content.ReadFromJsonAsync<SmartstoreProductAttributeDto>();
            return created?.Id ?? 0;
        }
        public async Task DeleteProductAttributeAsync(SmartstoreApiContext context,int productAttributeId)
        {
            var httpClient = CreateHttpClient(context);
            var response = await httpClient.DeleteAsync($"productattributes({productAttributeId})");
            response.EnsureSuccessStatusCode();
        }
        #endregion

        #region Product Variant Attribute
        public async Task<ProductVariantAttributeDto?> ProductVariantAttributeExistsAsync(SmartstoreApiContext context,int productId, int productAttributeId)
        {
            try
            {
                var httpClient = CreateHttpClient(context);
                var url = $"productvariantattributes?$expand=ProductAttribute&$filter=ProductId eq {productId} and ProductAttributeId eq {productAttributeId}";
                var response = await httpClient.GetAsync(url);
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
        public async Task<ProductVariantAttributeValueDto?> ProductVariantAttributeValueExistsAsync(SmartstoreApiContext context,int productVariantAttributeId, string name)
        {
            try
            {
                var httpClient = CreateHttpClient(context);
                var url = $"productvariantattributevalues?$filter=ProductVariantAttributeId eq {productVariantAttributeId} and Name eq '{Uri.EscapeDataString(name)}'";
                var response = await httpClient.GetAsync(url);
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
        public async Task<int> CreateProductVariantAttributeAsync(SmartstoreApiContext context,ProductVariantAttributeDto productVariantAttribute)
        {
            var httpClient = CreateHttpClient(context);
            var payload = SmartstoreProductVariantAttributeMapper.ToDto(productVariantAttribute);
            var json = JsonSerializer.Serialize(payload, _jsonOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await httpClient.PostAsync("productvariantattributes", content);
            response.EnsureSuccessStatusCode();

            var created = await response.Content.ReadFromJsonAsync<SmartstoreProductVariantAttributeDto>();
            return created?.Id ?? 0;
        }
        public async Task<int> CreateProductVariantAttributeValueAsync(SmartstoreApiContext context,ProductVariantAttributeValueDto productVariantAttributeValue)
        {
            var httpClient = CreateHttpClient(context);
            var payload = SmartstoreProductVariantAttributeValueMapper.ToDto(productVariantAttributeValue);
            var json = JsonSerializer.Serialize(payload, _jsonOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await httpClient.PostAsync("productvariantattributevalues", content);
            response.EnsureSuccessStatusCode();

            var created = await response.Content.ReadFromJsonAsync<SmartstoreProductVariantAttributeValueDto>();
            return created?.Id ?? 0;
        }
        #endregion

        #region Product Variant Attribute Combination
        public async Task<ProductVariantAttributeCombinationDto?> GetProductVariantAttributeCombination(SmartstoreApiContext context,int productId, int hashCode)
        {
            try
            {
                var httpClient = CreateHttpClient(context);
                var url = $"productvariantattributecombinations?$filter=ProductId eq {productId} and HashCode eq {hashCode}";
                var response = await httpClient.GetAsync(url);
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
        public async Task<int?> CreateProductVariantAttributeCombination(SmartstoreApiContext context,ProductVariantAttributeCombinationDto productVariantAttributeCombination)
        {
            var httpClient = CreateHttpClient(context);
            var payload = SmartstoreProductVariantAttributeCombinationMapper.ToDto(productVariantAttributeCombination);
            var json = JsonSerializer.Serialize(payload, _jsonOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await httpClient.PostAsync("productvariantattributecombinations", content);
            response.EnsureSuccessStatusCode();

            var created = await response.Content.ReadFromJsonAsync<SmartstoreProductVariantAttributeCombinationDto>();
            return created?.Id ?? 0;
        }
        public async Task UpdateProductVariantAttributeCombination(SmartstoreApiContext context,ProductVariantAttributeCombinationDto productVariantAttributeCombination)
        {
            var httpClient = CreateHttpClient(context);
            var payload = SmartstoreProductVariantAttributeCombinationMapper.ToDto(productVariantAttributeCombination);
            var json = JsonSerializer.Serialize(payload, _jsonOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await httpClient.PatchAsync($"productvariantattributecombinations({productVariantAttributeCombination.Id})", content);
            response.EnsureSuccessStatusCode();
        }
        #endregion

        #region Other
        private async Task<byte[]> GetFileAsync(string url)
        {
            using var client = new HttpClient();
            using var response = await client.GetAsync(url);

            response.EnsureSuccessStatusCode(); // 404/500 varsa exception atar

            var contentType = response.Content.Headers.ContentType?.MediaType;
            if (contentType == null || !contentType.StartsWith("image", StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException($"Resim bekleniyordu ama {contentType} geldi.");
            }

            return await response.Content.ReadAsByteArrayAsync();
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
}
