using Entegro.Application.DTOs.Brand;
using Entegro.Application.DTOs.Category;
using Entegro.Application.DTOs.ImportProfile;
using Entegro.Application.DTOs.Product;
using Entegro.Application.DTOs.ProductAttribute;
using Entegro.Application.DTOs.ProductAttributeValue;
using Entegro.Application.DTOs.ProductCategory;
using Entegro.Application.DTOs.ProductMediaFile;
using Entegro.Application.DTOs.ProductVariantAttribute;
using Entegro.Application.DTOs.ProductVariantAttributeValue;
using Entegro.Application.Interfaces.Services.Base;
using Newtonsoft.Json;
using Quartz;
using System.Collections.Concurrent;
using System.Text.Json;
using System.Xml.Linq;

namespace Entegro.Api.Jobs
{
    [DisallowConcurrentExecution]
    public class XmlJob : IJob
    {

        private readonly IImportProfileService _importProfileService;
        private readonly IProductService _productService;
        private readonly ICategoryService _categoryService;
        private readonly IBrandService _brandService;
        private readonly IProductMediaFileMappingService _productImageMappingService;
        private readonly IProductCategoryService _productCategoryService;
        private readonly IProductAttributeService _productAttributeService;
        private readonly IProductAttributeValueService _productAttributeValueService;
        private readonly IProductVariantAttributeService _productVariantAttributeService;
        private readonly IProductVariantAttributeValueService _productVariantAttributeValueService;
        private readonly IProductVariantAttributeCombinationService _productVariantAttributeCombinationService;
        private readonly ConcurrentDictionary<string, int> _attributeCache = new();
        private readonly ConcurrentDictionary<(int attributeId, string value), int> _attributeValueCache = new();
        private readonly ISettingService _settingService;
        private readonly ILogger<XmlJob> _logger;

        public XmlJob(
            IImportProfileService importProfileService,
            IProductService productService,
            ICategoryService categoryService,
            IBrandService brandService,
            IProductMediaFileMappingService productImageMappingService,
            IProductCategoryService productCategoryService,
            IProductAttributeService productAttributeService,
            IProductAttributeValueService productAttributeValueService,
            IProductVariantAttributeService productVariantAttributeService,
            IProductVariantAttributeValueService productVariantAttributeValueService,
            IProductVariantAttributeCombinationService productVariantAttributeCombinationService,
            ISettingService settingService,
            ILogger<XmlJob> logger)
        {
            _importProfileService = importProfileService;
            _productService = productService;
            _categoryService = categoryService;
            _brandService = brandService;
            _productImageMappingService = productImageMappingService;
            _productCategoryService = productCategoryService;
            _productAttributeService = productAttributeService;
            _productAttributeValueService = productAttributeValueService;
            _productVariantAttributeService = productVariantAttributeService;
            _productVariantAttributeValueService = productVariantAttributeValueService;
            _productVariantAttributeCombinationService = productVariantAttributeCombinationService;
            _settingService = settingService;
            _logger = logger;
        }
        public async Task Execute(IJobExecutionContext context)
        {
            try
            {
                await LoadAttributeCacheAsync();

                var profileId = context.JobDetail.JobDataMap.GetInt("ProfileId");
                var profile = await _importProfileService.GetByIdAsync(profileId);
                if (profile == null)
                {
                    _logger.LogError("Profil bulunamadı.");
                    return;
                }

                var mappings = JsonConvert.DeserializeObject<List<XmlColumnMappingResult>>(profile.ColumnMapping);
                if (mappings == null)
                {
                    _logger.LogError("Xml eşleştirilmesi bulunamadı");
                    return;
                }

                #region dosya 
                var xDoc = GetDocument(profile);
                #endregion

                if (xDoc == null)
                {
                    Console.WriteLine("Xml dosyasu bulunamadı.");
                    return;
                }

                if (xDoc.Result == null)
                {

                    return;
                }

                var systemUrl = await _settingService.GetByKeyAsync("SystemUrl");
                if (systemUrl == null || string.IsNullOrWhiteSpace(systemUrl.Value))
                {
                    _logger.LogError("SystemUrl tanımlı değil veya boş");
                    return;
                }

                if (!Uri.TryCreate(systemUrl.Value, UriKind.Absolute, out var baseUri))
                {
                    _logger.LogError("Geçersiz SystemUrl: {Url}", systemUrl.Value);
                    return;
                }

                using var httpClient = new HttpClient
                {
                    BaseAddress = baseUri
                };


                int productCount = xDoc.Result.Descendants("Product").Count();

                for (int i = 0; i < productCount; i += 1)
                {
                    try
                    {
                        var rootElement = xDoc.Result.Descendants("Product").ElementAtOrDefault(i);
                        if (rootElement == null)
                        {
                            _logger.LogError("XML'de Product Alanı Bulunamadı");
                            return;
                        }

                        CreateProductDto createProductDto = new CreateProductDto();
                        createProductDto.Code = GetValue(rootElement, mappings, "Code", "");
                        createProductDto.Name = GetValue(rootElement, mappings, "Name", "");
                        createProductDto.Description = GetValue(rootElement, mappings, "Description", "");
                        createProductDto.Barcode = GetValue(rootElement, mappings, "Barcode", "");
                        createProductDto.Currency = GetValue(rootElement, mappings, "Currency", "TL");
                        createProductDto.Price = GetDecimalValue(rootElement, mappings, "Price", 0);
                        createProductDto.VatRate = GetDecimalValue(rootElement, mappings, "VatRate", 0);
                        createProductDto.StockQuantity = GetIntegerValue(rootElement, mappings, "StockQuantity", 0);
                        createProductDto.BrandId = await CreateOrUpdateBrand(GetValue(rootElement, mappings, "Brand", ""));
                        createProductDto.ManufacturerPartNumber = GetValue(rootElement, mappings, "ManufacturerPartNumber", "");
                        createProductDto.Unit = GetValue(rootElement, mappings, "Unit", "Adet");
                        createProductDto.Gtin = GetValue(rootElement, mappings, "Gtin", "");
                        createProductDto.OldPrice = 0;
                        createProductDto.SpecialPrice = GetDecimalValue(rootElement, mappings, "SpecialPrice", 0);
                        createProductDto.Weight = GetDecimalValue(rootElement, mappings, "Weight", 0);
                        createProductDto.Length = GetDecimalValue(rootElement, mappings, "Length", 0);
                        createProductDto.Height = GetDecimalValue(rootElement, mappings, "Height", 0);
                        createProductDto.Width = GetDecimalValue(rootElement, mappings, "Width", 0);
                        createProductDto.MetaDescription = GetValue(rootElement, mappings, "MetaDescription", "");
                        createProductDto.MetaKeywords = GetValue(rootElement, mappings, "MetaKeywords", "");
                        createProductDto.MetaTitle = GetValue(rootElement, mappings, "MetaTitle", "");
                        createProductDto.Barcode = GetValue(rootElement, mappings, "Barcode", "");
                        createProductDto.Published = false;
                        createProductDto.Deleted = false;

                        int productId = 0;
                        if (await _productService.ExistsByCodeAsync(createProductDto.Code))
                        {

                        }
                        else
                        {
                            var createdProduct = await _productService.AddAsync(createProductDto);
                            if (createdProduct != null)
                            {
                                productId = createdProduct.Id;
                            }
                        }

                        var images = GetMultipleValues(rootElement, mappings, "Images");
                        List<int> mediaFiles = await UploadImagesAsync(images, httpClient);
                        foreach (var item in mediaFiles)
                        {
                            CreateProductMediaFileDto createProductMediaFile = new CreateProductMediaFileDto();
                            createProductMediaFile.MediaFileId = item;
                            createProductMediaFile.ProductId = productId;

                            await _productImageMappingService.AddAsync(createProductMediaFile);
                        }

                        var categories = GetMultipleValues(rootElement, mappings, "Categories");
                        foreach (var item in categories)
                        {
                            var category =await CreateOrderUpdateCategory(item);
                            if (category == null)
                            {
                                continue;
                            }

                            await CreateCategoryProduct(productId, category.Id);
                        }

                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "{i} sıralı ürün aktarımı sırasında hata oluştu", i);
                    }

                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Aktarım sırasında bir hata oluştur");
            }
        }



        private async Task<XDocument?> GetDocument(ImportProfileDto profile)
        {
            try
            {
                var url = profile.MediaFileUrl;

                if (string.IsNullOrEmpty(url))
                {
                    throw new Exception("Dosya url boş");
                }

                using var httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/120 Safari/537.36");
                var xmlContent = await httpClient.GetStringAsync(url);
                using TextReader reader = new StringReader(xmlContent);
                XDocument xDoc = XDocument.Load(reader);

                return xDoc;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Dosya indirlirken bir hata oluştu");
                return null;
            }
        }
        public string GetValue(XElement xElement, List<XmlColumnMappingResult> mappingList, string mappedName, string defaultValue)
        {
            if (mappingList == null)
            {
                _logger.LogError("Eşleştirlme yapılmamış");
                return defaultValue;
            }

            if (string.IsNullOrEmpty(mappedName))
            {
                _logger.LogError("Alan adı boş");
                return defaultValue;
            }

            if (!mappingList.Any(x => x?.MappedName == mappedName))
            {
                _logger.LogError("{MappedName} Eşleştirlme yapılmamış", mappedName);
                return defaultValue;
            }

            var xName = mappingList.First(x => x?.MappedName == mappedName).XmlHeader;
            var xFindElement = xElement.Element(xName);

            if (xFindElement == null)
            {
                _logger.LogError("Xml'de {MappedName} alanı bulunamadı.", xName);
                return defaultValue;
            }


            return xFindElement.Value;
        }
        public decimal GetDecimalValue(XElement xElement, List<XmlColumnMappingResult> mappingList, string mappedName, decimal defaultValue)
        {
            if (mappingList == null)
            {
                _logger.LogError("Eşleştirlme yapılmamış");
                return defaultValue;
            }

            if (string.IsNullOrEmpty(mappedName))
            {
                _logger.LogError("Alan adı boş");
                return defaultValue;
            }

            if (!mappingList.Any(x => x?.MappedName == mappedName))
            {
                _logger.LogError("{MappedName} Eşleştirlme yapılmamış", mappedName);
                return defaultValue;
            }

            var xName = mappingList.First(x => x?.MappedName == mappedName).XmlHeader;
            var xFindElement = xElement.Element(xName);

            if (xFindElement == null)
            {
                _logger.LogError("Xml'de {MappedName} alanı bulunamadı.", xName);
                return defaultValue;
            }

            return decimal.TryParse(xFindElement.Value, out var vatRate) ? vatRate : defaultValue;
        }
        public int GetIntegerValue(XElement xElement, List<XmlColumnMappingResult> mappingList, string mappedName, int defaultValue)
        {
            if (mappingList == null)
            {
                _logger.LogError("Eşleştirlme yapılmamış");
                return defaultValue;
            }

            if (string.IsNullOrEmpty(mappedName))
            {
                _logger.LogError("Alan adı boş");
                return defaultValue;
            }

            if (!mappingList.Any(x => x?.MappedName == mappedName))
            {
                _logger.LogError("{MappedName} Eşleştirlme yapılmamış", mappedName);
                return defaultValue;
            }

            var xName = mappingList.First(x => x?.MappedName == mappedName).XmlHeader;
            var xFindElement = xElement.Element(xName);

            if (xFindElement == null)
            {
                _logger.LogError("Xml'de {MappedName} alanı bulunamadı.", xName);
                return defaultValue;
            }

            var value = int.TryParse(xFindElement.Value, out var price) ? price : defaultValue;
            return value;
        }
        public List<string> GetMultipleValues(XElement xElement, List<XmlColumnMappingResult> mappingList, string mappedName)
        {
            List<string> values = new List<string>();

            if (mappingList == null)
            {
                _logger.LogError("Eşleştirlme yapılmamış");
                return values;
            }

            if (string.IsNullOrEmpty(mappedName))
            {
                _logger.LogError("Alan adı boş");
                return values;
            }

            if (!mappingList.Any(x => x?.MappedName == mappedName))
            {
                _logger.LogError("{MappedName} Eşleştirlme yapılmamış", mappedName);
                return values;
            }

            var xName = mappingList.First(x => x?.MappedName == mappedName).XmlHeader;

            if (xName.Contains(","))
            {
                var xmlFields = xName.Split(",");
                foreach (var xmlField in xmlFields)
                {
                    var value = xElement.Descendants(xmlField).Select(x => x.Value).FirstOrDefault();

                    if (!string.IsNullOrEmpty(value))
                    {
                        values.Add(value);
                    }
                }
            }
            else
            {
                values = xElement.Descendants(xName).Select(x => x.Value).ToList();
            }

            return values;
        }

        private async Task AddVariantAttributeAsync(int productId, string attributeName, string attributeValue, List<ProductVariantAttributeModel> variantAttributes)
        {
            if (string.IsNullOrEmpty(attributeName) || string.IsNullOrEmpty(attributeValue)) return;

            int attrId = await EnsureProductAttributeAsync(attributeName);
            int attrValueId = await EnsureProductAttributeValueAsync(attrId, attributeValue);

            var existingVariant = await _productVariantAttributeService.GetByAttibuteIdAsync(productId, attrId);
            int variantId = existingVariant?.Id ?? (await _productVariantAttributeService.AddAsync(new CreateProductVariantAttributeDto
            {
                ProductId = productId,
                ProductAttributeId = attrId,
                DisplayOrder = 0,
                AttributeControlTypeId = 0
            })).Id;

            var existingVariantValue = await _productVariantAttributeValueService.GetByNameAsync(variantId, attributeValue);
            int variantValueId = existingVariantValue?.Id ?? (await _productVariantAttributeValueService.AddAsync(new CreateProductVariantAttributeValueDto()
            {
                Name = attributeValue,
                ProductVariantAttributeId = variantId
            })).Id;

            variantAttributes.Add(new ProductVariantAttributeModel
            {
                ProductVariantAttributeId = variantId,
                ProductVariantAttributeValueId = variantValueId
            });
        }
        private async Task<int> EnsureProductAttributeAsync(string name)
        {
            if (_attributeCache.TryGetValue(name, out int id)) return id;

            var created = await _productAttributeService.AddAsync(new CreateProductAttributeDto
            {
                Name = name,
                Description = "",
                DisplayOrder = 0
            });

            _attributeCache.TryAdd(name, created.Id);
            return created.Id;
        }
        private async Task<int> EnsureProductAttributeValueAsync(int attributeId, string value)
        {
            if (_attributeValueCache.TryGetValue((attributeId, value), out int id)) return id;

            var created = await _productAttributeValueService.AddAsync(new CreateProductAttributeValueDto
            {
                Name = value,
                DisplayOrder = 0,
                ProductAttributeId = attributeId
            });

            _attributeValueCache.TryAdd((attributeId, value), created.Id);
            return created.Id;
        }
        private async Task LoadAttributeCacheAsync()
        {
            var allAttributes = await _productAttributeService.GetAllAsync();
            foreach (var attr in allAttributes)
                _attributeCache.TryAdd(attr.Name, attr.Id);

            var allValues = await _productAttributeValueService.GetAllAsync();
            foreach (var val in allValues)
                _attributeValueCache.TryAdd((val.ProductAttributeId, val.Name), val.Id);

        }
        private async Task<int?> CreateOrUpdateBrand(string brand)
        {
            if (!string.IsNullOrWhiteSpace(brand))
            {
                var existingBrand = await _brandService.GetByNameAsync(brand);

                if (existingBrand == null)
                {
                    var createBrand = new CreateBrandDto();
                    createBrand.Name = brand;
                    createBrand.DisplayOrder = 0;
                    createBrand.Published = true;
                    createBrand.Description = "";
                    createBrand.MediaFileId = null;
                    createBrand.MetaTitle = brand;
                    createBrand.MetaDescription = "";
                    createBrand.MetaKeywords = "";
                  
                    var createdBrand = await _brandService.AddAsync(createBrand);

                    return createdBrand.Id;
                }
                else
                {
                    return existingBrand.Id;
                }
            }

            return null;
        }
        private async Task<CategoryDto?> CreateOrderUpdateCategory(string? categoryName)
        {
            try
            {
                if (string.IsNullOrEmpty(categoryName))
                {
                    return null;
                }

                if (await _categoryService.ExistsByNameAsync(categoryName))
                {
                    var category = await _categoryService.GetCategoryByNameAsync(categoryName);
                    return category;
                }
                else
                {
                    CreateCategoryDto createCategory = new CreateCategoryDto();
                    createCategory.Name = categoryName;
                    createCategory.Description = "";
                    createCategory.MetaTitle = "";
                    createCategory.MetaKeywords = "";
                    createCategory.MetaDescription = "";
                    createCategory.ParentId = null;
                    createCategory.Published = true;
                    createCategory.DisplayOrder = 0;
                    createCategory.MediaFileId = null;

                    var category = await _categoryService.AddAsync(createCategory);

                    return category;
                }
            }
            catch (Exception)
            {
                return null;
            }
        }
        private async Task CreateCategoryProduct(int productId, int categoryId)
        {
            if (productId == 0)
            {
                _logger.LogError("Ürün Id 0");
            }

            if (categoryId == 0)
            {
                _logger.LogError("Category Id 0");
            }

            CreateProductCategoryDto createProductCategory = new CreateProductCategoryDto();
            createProductCategory.ProductId = productId;
            createProductCategory.CategoryId = categoryId;

            await _productCategoryService.AddAsync(createProductCategory);
        }

        public async Task<List<int>> UploadImagesAsync(List<string> imageUrls, HttpClient httpClient)
        {
            List<int> fileIds = new();

            if (imageUrls != null && imageUrls.Any())
            {
                var multipartContent = new MultipartFormDataContent();
                multipartContent.Add(new StringContent("catalog"), "path");
                multipartContent.Add(new StringContent("False"), "isTransient");

                foreach (var imageUrl in imageUrls)
                {
                    try
                    {
                        var imageBytes = await httpClient.GetByteArrayAsync(imageUrl);
                        var imageName = Path.GetFileName(imageUrl);

                        if (string.IsNullOrWhiteSpace(imageName))
                            imageName = "default.jpg";

                        var byteContent = new ByteArrayContent(imageBytes);
                        byteContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/octet-stream");

                        multipartContent.Add(byteContent, "upload-file", imageName);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"HATA (indirilemedi): {imageUrl} → {ex.Message}");
                    }
                }

                var uploadUrl = "media/upload";
                var response = await httpClient.PostAsync(uploadUrl, multipartContent);

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadAsStringAsync();

                    using var document = JsonDocument.Parse(result);
                    var root = document.RootElement;

                    if (root.ValueKind == JsonValueKind.Array)
                    {
                        foreach (var item in root.EnumerateArray())
                        {
                            if (item.TryGetProperty("id", out var idProp))
                            {
                                int imageId = idProp.GetInt32();
                                fileIds.Add(imageId);
                                Console.WriteLine($"id: {imageId}");
                            }
                        }
                    }
                    else if (root.ValueKind == JsonValueKind.Object)
                    {
                        if (root.TryGetProperty("id", out var idProp))
                        {
                            int imageId = idProp.GetInt32();
                            fileIds.Add(imageId);
                            Console.WriteLine($"id: {imageId}");
                        }
                    }
                }
                else
                {
                    throw new Exception("Resim yükleme başarısız oldu. Durum kodu:" + response.StatusCode);
                }
            }

            return fileIds;
        }
        public class ProductVariantAttributeModel
        {
            public int ProductVariantAttributeId { get; set; }
            public int ProductVariantAttributeValueId { get; set; }
        }
        public class XmlColumnMappingResult
        {
            public string XmlHeader { get; set; }
            public string MappedName { get; set; }
        }
    }

}