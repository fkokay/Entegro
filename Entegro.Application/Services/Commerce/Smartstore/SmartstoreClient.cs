using Entegro.Application.DTOs.Brand;
using Entegro.Application.DTOs.Category;
using Entegro.Application.DTOs.Commerce.Smartstore;
using Entegro.Application.DTOs.Product;
using Entegro.Application.DTOs.ProductAttribute;
using Entegro.Application.DTOs.ProductAttributeValue;
using Entegro.Application.DTOs.ProductVariantAttribute;
using Entegro.Application.DTOs.ProductVariantAttributeCombination;
using Entegro.Application.DTOs.ProductVariantAttributeValue;
using Entegro.Application.Mappings.Commerce.Smartstore;
using Entegro.Domain.Entities;
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
        private readonly HttpClient _httpClient;
        private readonly JsonSerializerOptions _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
        };

        public SmartstoreClient(HttpClient httpClient)
        {
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
                var data = System.Text.Json.JsonSerializer.Deserialize<ODataListResponse<SmartstoreProductDto>>(json, _jsonOptions);

                return data?.Value?.FirstOrDefault() is SmartstoreProductDto dto ? SmartstoreProductMapper.ToDto(dto) : null;
            }
            catch (Exception)
            {
                return null;
            }
        }
        public async Task UpsertProductAsync(ProductDto product)
        {
            try
            {
                var existing = await GetProductBySkuAsync(product.Code);

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
                        product.BrandId = manufacturer.Id;
                    }
                }

                foreach (var item in product.ProductCategories)
                {
                    int parenCategoryId = 0;
                    if (item.Category.ParentCategory != null)
                    {
                        var parenCategory = await CategoryExistsAsync(item.Category.ParentCategory.Name);
                        if (parenCategory == null)
                        {
                            parenCategoryId = await CreateCategoryAsync(item.Category.ParentCategory);
                        }
                        else
                        {
                            parenCategoryId = parenCategory.Id;
                        }
                    }
                    var category = await CategoryExistsAsync(item.Category.Name);
                    if (category == null)
                    {
                        item.Category.ParentCategoryId = parenCategoryId;
                        int categoryId = await CreateCategoryAsync(item.Category);
                        item.CategoryId = categoryId;
                        item.ProductId = existing == null ? 0 : existing.Id;
                    }
                    else
                    {
                        item.CategoryId = category.Id;
                        item.ProductId = existing == null ? 0 : existing.Id;
                    }
                }

                foreach (var item in product.ProductMediaFiles)
                {
                    var fileExists = await FileExists("catalog/" + item.MediaFile.Name);
                    if (fileExists.Value)
                    {
                        var smartstoreFile = await GetFileByPath("catalog/" + item.MediaFile.Name);
                        item.MediaFileId = smartstoreFile.Id;
                        item.ProductId = existing == null ? 0 : existing.Id;
                    }
                    else
                    {
                        SmartstoreFileDto smartstoreFile = new SmartstoreFileDto();
                        smartstoreFile.File = await getFileAsync("https://localhost:7230" + item.MediaFile.Url);
                        smartstoreFile.FileName = "catalog/" + item.MediaFile.Name;
                        smartstoreFile.MimeType = item.MediaFile.MimeType;

                        item.MediaFileId = await UpsertMediaFile(smartstoreFile) ?? 0;
                        item.ProductId = existing == null ? 0 : existing.Id;
                    }
                }

                foreach (var item in product.ProductVariantAttributes)
                {

                    var attribute = await ProductAttributeExistsAsync(item.ProductAttribute.Name);
                    if (attribute == null)
                    {
                        int attributeId = await CreateProductAttributeAsync(item.ProductAttribute);
                        item.ProductAttributeId = attributeId;
                        item.ProductId = existing == null ? 0 : existing.Id;
                    }
                    else
                    {
                        item.ProductAttributeId = attribute.Id;
                        item.ProductId = existing == null ? 0 : existing.Id;

                        if (existing != null)
                        {
                            var productVariantAttribute = await ProductVariantAttributeExistsAsync(existing.Id, attribute.Id);
                            if (productVariantAttribute != null)
                            {
                                item.EntityId = item.Id;
                                item.Id = productVariantAttribute.Id;

                                foreach (var value in item.ProductVariantAttributeValues)
                                {
                                    var attributeValue = await ProductVariantAttributeValueExistsAsync(productVariantAttribute.Id, value.Name);
                                    if (attributeValue != null)
                                    {
                                        value.ProductVariantAttributeId = productVariantAttribute.Id;
                                        value.EntityId = value.Id;
                                        value.Id = attributeValue.Id;

                                    }
                                    else
                                    {
                                        value.ProductVariantAttributeId = productVariantAttribute.Id;
                                        value.Id = 0;
                                    }
                                }
                            }
                            else
                            {
                                item.Id = 0;

                                foreach (var value in item.ProductVariantAttributeValues)
                                {
                                    value.Id = 0;
                                }
                            }
                        }
                        else
                        {
                            item.Id = 0;
                        }
                    }


                }

                foreach (var item in product.ProductVariantAttributeCombinations)
                {
                    item.ProductId = existing == null ? 0 : existing.Id;
                }



                if (existing == null)
                {
                    product.Id = 0;
                    var payload = SmartstoreProductMapper.ToDto(product);
                    var json = JsonSerializer.Serialize(payload, _jsonOptions);
                    var content = new StringContent(json, Encoding.UTF8, "application/json");

                    var response = await _httpClient.PostAsync("products", content);
                    var result = await response.Content.ReadAsStringAsync();
                    response.EnsureSuccessStatusCode();
                }
                else
                {
                    product.Id = existing.Id;
                    var payload = SmartstoreProductMapper.ToDto(product);
                    if (payload != null)
                    {
                        var json = JsonSerializer.Serialize(payload, _jsonOptions);
                        var content = new StringContent(json, Encoding.UTF8, "application/json");

                        var response = await _httpClient.PatchAsync($"products({product.Id})", content);
                        var result = await response.Content.ReadAsStringAsync();
                        response.EnsureSuccessStatusCode();
                    }
                }

                var data = await GetProductBySkuAsync(product.Code);
                var productVariantAttributes = await ProductVariantAttributeExistsAsync(data.Id);

                foreach (var item in product.ProductVariantAttributeCombinations)
                {
                    List<KeyValuePair<int, ICollection<object>>> attributes = new List<KeyValuePair<int, ICollection<object>>>();

                    var attr = JsonSerializer.Deserialize<List<ProductVariantAttributeModel>>(item.AttributeXml);

                    foreach (var at in attr)
                    {
                        var productVariantAttribute = product.ProductVariantAttributes.Where(m => m.EntityId == at.ProductAttributeId).First();
                        var productVariantAttributeValue = productVariantAttribute.ProductVariantAttributeValues.Where(m => m.EntityId == at.ProductAttributeValueId).ToList();

                        attributes.Add(new KeyValuePair<int, ICollection<object>>(productVariantAttribute.Id, productVariantAttributeValue.Select(m => m.Id as object).ToList()));
                    }

                    RawAttribute rawAttribute = new RawAttribute();
                    rawAttribute.Attributes = attributes;

                    item.AttributeXml = JsonSerializer.Serialize(rawAttribute);
                    item.HashCode = GetHashCode(rawAttribute);


                    var ex = await GetProductVariantAttributeCombination(data.Id, item.HashCode);
                    if (ex == null)
                    {
                        await CreateProductVariantAttributeCombination(item);
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
        public async Task UpdateBrandAsync(BrandDto brand, int id)
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
                var data = JsonSerializer.Deserialize<ODataListResponse<SmartstoreManufacturerDto>>(json, _jsonOptions);

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
            return created == null ? 0 : created.Id;
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
                var data = JsonSerializer.Deserialize<ODataListResponse<SmartstoreCategoryDto>>(json, _jsonOptions);

                return data?.Value?.FirstOrDefault() is SmartstoreCategoryDto dto ? SmartstoreCategoryMapper.ToDto(dto) : null;
            }
            catch (Exception)
            {
                return null;
            }
        }
        #endregion

        #region Media File
        public async Task<int?> UpsertMediaFile(SmartstoreFileDto smartstoreFile)
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
                var data = System.Text.Json.JsonSerializer.Deserialize<ODataListResponse<SmartstoreProductAttributeDto>>(json, _jsonOptions);

                return data?.Value?.FirstOrDefault() is SmartstoreProductAttributeDto dto ? SmartstoreProductAttributeMapper.ToDto(dto) : null;
            }
            catch (Exception)
            {
                return null;
            }
        }
        public async Task<int> CreateProductAttributeAsync(ProductAttributeDto productAttribute)
        {
            var payload = SmartstoreProductAttributeMapper.ToDto(productAttribute);
            var json = JsonSerializer.Serialize(payload, _jsonOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("productattributes", content);
            var result = await response.Content.ReadAsStringAsync();
            response.EnsureSuccessStatusCode();

            var created = await response.Content.ReadFromJsonAsync<SmartstoreProductAttributeDto>();
            return created == null ? 0 : created.Id;
        }
        public async Task DeleteProductAttributeAsync(int productAttributeId)
        {
            var response = await _httpClient.DeleteAsync($"productattributes({productAttributeId})");
            response.EnsureSuccessStatusCode();
        }
        public async Task<List<ProductVariantAttributeDto>?> ProductVariantAttributeExistsAsync(int productId)
        {

            try
            {
                var url = $"productvariantattributes?$expand=ProductVariantAttributeValues,ProductAttribute&$filter=ProductId eq {productId}";
                var response = await _httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();

                var json = await response.Content.ReadAsStringAsync();
                var data = System.Text.Json.JsonSerializer.Deserialize<ODataListResponse<SmartstoreProductVariantAttributeDto>>(json, _jsonOptions);

                return SmartstoreProductVariantAttributeMapper.ToDtoList(data.Value).ToList();
            }
            catch (Exception)
            {
                return null;
            }
        }
        public async Task<ProductAttributeDto?> ProductVariantAttributeExistsAsync(int productId, int productAttributeId)
        {

            try
            {
                var url = $"productvariantattributes?$filter=ProductId eq {productId} and ProductAttributeId eq {productAttributeId}";
                var response = await _httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();

                var json = await response.Content.ReadAsStringAsync();
                var data = System.Text.Json.JsonSerializer.Deserialize<ODataListResponse<SmartstoreProductAttributeDto>>(json, _jsonOptions);

                return data?.Value?.FirstOrDefault() is SmartstoreProductAttributeDto dto ? SmartstoreProductAttributeMapper.ToDto(dto) : null;
            }
            catch (Exception)
            {
                return null;
            }
        }
        public async Task<ProductVariantAttributeValueDto?> ProductVariantAttributeValueExistsAsync(int productVariantAttributeId, string name)
        {

            try
            {
                var url = $"productvariantattributevalues?$filter= ProductVariantAttributeId eq {productVariantAttributeId} and Name eq '{name}'";
                var response = await _httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();

                var json = await response.Content.ReadAsStringAsync();
                var data = System.Text.Json.JsonSerializer.Deserialize<ODataListResponse<SmartstoreProductVariantAttributeValueDto>>(json, _jsonOptions);

                return data?.Value?.FirstOrDefault() is SmartstoreProductVariantAttributeValueDto dto ? SmartstoreProductVariantAttributeValueMapper.ToDto(dto) : null;

            }
            catch (Exception)
            {
                return null;
            }
        }
        public async Task<int> CreateProductVariantAttributeValueAsync(ProductVariantAttributeValueDto productVariantAttributeValue)
        {
            var payload = SmartstoreProductVariantAttributeValueMapper.ToDto(productVariantAttributeValue);
            var json = JsonSerializer.Serialize(payload, _jsonOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("productvariantattributevalues", content);
            var result = await response.Content.ReadAsStringAsync();
            response.EnsureSuccessStatusCode();

            var created = await response.Content.ReadFromJsonAsync<SmartstoreProductVariantAttributeValueDto>();
            return created == null ? 0 : created.Id;
        }
        public async Task<ProductVariantAttributeCombinationDto?> GetProductVariantAttributeCombination(int productId, int hashCode)
        {
            try
            {

                var url = $"productvariantattributecombinations?$filter=ProductId eq {productId} and HashCode eq {hashCode}";
                var response = await _httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();

                var json = await response.Content.ReadAsStringAsync();
                var data = System.Text.Json.JsonSerializer.Deserialize<ODataListResponse<SmartstoreProductVariantAttributeCombinationDto>>(json, _jsonOptions);

                return data?.Value?.FirstOrDefault() is SmartstoreProductVariantAttributeCombinationDto dto ? SmartstoreProductVariantAttributeCombinationMapper.ToDto(dto) : null;
            }
            catch (Exception)
            {
                return null;
            }
        }
        public async Task<int?> CreateProductVariantAttributeCombination(ProductVariantAttributeCombinationDto productVariantAttributeCombination)
        {

            var payload = SmartstoreProductVariantAttributeCombinationMapper.ToDto(productVariantAttributeCombination);
            var json = JsonSerializer.Serialize(payload, _jsonOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("productvariantattributecombinations", content);
            var result = await response.Content.ReadAsStringAsync();
            response.EnsureSuccessStatusCode();

            var created = await response.Content.ReadFromJsonAsync<SmartstoreProductVariantAttributeCombinationDto>();
            return created == null ? 0 : created.Id;
        }
        public async Task<int?> UpdateProductVariantAttributeCombination(ProductVariantAttributeCombinationDto productVariantAttributeCombination)
        {
            var payload = SmartstoreProductVariantAttributeCombinationMapper.ToDto(productVariantAttributeCombination);
            var json = JsonSerializer.Serialize(payload, _jsonOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PatchAsync("productvariantattributecombinations", content);
            var result = await response.Content.ReadAsStringAsync();
            response.EnsureSuccessStatusCode();

            var created = await response.Content.ReadFromJsonAsync<SmartstoreProductVariantAttributeCombinationDto>();
            return created == null ? 0 : created.Id;
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
