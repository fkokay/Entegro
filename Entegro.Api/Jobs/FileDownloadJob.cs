using Entegro.Application.DTOs.Brand;
using Entegro.Application.DTOs.ImportProfile;
using Entegro.Application.DTOs.Product;
using Entegro.Application.DTOs.ProductAttribute;
using Entegro.Application.DTOs.ProductAttributeValue;
using Entegro.Application.DTOs.ProductVariantAttribute;
using Entegro.Application.DTOs.ProductVariantAttributeValue;
using Entegro.Application.Interfaces.Services;
using Newtonsoft.Json;
using Org.BouncyCastle.Utilities;
using Quartz;
using Quartz.Util;
using System.Collections.Concurrent;
using System.Text.Json;
using System.Xml.Linq;
using static System.Net.Mime.MediaTypeNames;
using File = System.IO.File;

namespace Entegro.Api.Jobs
{
    [DisallowConcurrentExecution]
    public class FileDownloadJob : IJob
    {

        //    private readonly IImportProfileService _importProfileService;
        //    private readonly IProductService _productService;
        //    private readonly ICategoryService _categoryService;
        //    private readonly IBrandService _brandService;
        //    private readonly IProductMediaFileMappingService _productImageMappingService;
        //    private readonly IProductCategoryService _productCategoryService;
        //    private readonly IProductAttributeService _productAttributeService;
        //    private readonly IProductAttributeValueService _productAttributeValueService;
        //    private readonly IProductVariantAttributeService _productVariantAttributeService;
        //    private readonly IProductVariantAttributeValueService _productVariantAttributeValueService;
        //    private readonly IProductVariantAttributeCombinationService _productVariantAttributeCombinationService;
        //    private readonly ConcurrentDictionary<string, int> _attributeCache = new();
        //    private readonly ConcurrentDictionary<(int attributeId, string value), int> _attributeValueCache = new();
        //    private readonly ILogger<FileDownloadJob> _logger;

        //    public FileDownloadJob(
        //        IImportProfileService importProfileService,
        //        IProductService productService,
        //        ICategoryService categoryService,
        //        IBrandService brandService,
        //        IProductMediaFileMappingService productImageMappingService,
        //        IProductCategoryService productCategoryService,
        //        IProductAttributeService productAttributeService,
        //        IProductAttributeValueService productAttributeValueService,
        //        IProductVariantAttributeService productVariantAttributeService,
        //        IProductVariantAttributeValueService productVariantAttributeValueService,
        //        IProductVariantAttributeCombinationService productVariantAttributeCombinationService,
        //        ILogger<FileDownloadJob> logger)
        //    {
        //        _importProfileService = importProfileService;
        //        _productService = productService;
        //        _categoryService = categoryService;
        //        _brandService = brandService;
        //        _productImageMappingService = productImageMappingService;
        //        _productCategoryService = productCategoryService;
        //        _productAttributeService = productAttributeService;
        //        _productAttributeValueService = productAttributeValueService;
        //        _productVariantAttributeService = productVariantAttributeService;
        //        _productVariantAttributeValueService = productVariantAttributeValueService;
        //        _productVariantAttributeCombinationService = productVariantAttributeCombinationService;
        //        _logger = logger;
        //    }
        //    public async Task Execute(IJobExecutionContext context)
        //    {
        //        try
        //        {
        //            await LoadAttributeCacheAsync();

        //            var profileId = context.JobDetail.JobDataMap.GetInt("ProfileId");
        //            var profile = await _importProfileService.GetByIdAsync(profileId);
        //            if (profile == null)
        //            {
        //                _logger.LogError("Profil bulunamadı.");
        //                return;
        //            }

        //            var mappings = JsonConvert.DeserializeObject<List<XmlColumnMappingResult>>(profile.ColumnMapping);
        //            if (mappings == null)
        //            {
        //                _logger.LogError("Xml eşleştirilmesi bulunamadı");
        //                return;
        //            }

        //            #region dosya 
        //            var xDoc = GetDocument(profile);
        //            #endregion

        //            if (xDoc == null)
        //            {
        //                Console.WriteLine("Xml dosyasu bulunamadı.");
        //                return;
        //            }

        //            if (xDoc.Result == null)
        //            {

        //                return;
        //            }

        //            int productCount = xDoc.Result.Descendants("Product").Count();

        //            List<CreateProductDto> createdProductList = new List<CreateProductDto>();
        //            for (int i = 0; i < productCount; i += 1)
        //            {
        //                var rootElement = xDoc.Result.Descendants("Product").ElementAtOrDefault(i);
        //                if (rootElement == null)
        //                {
        //                    _logger.LogError("XML'de Product Alanı Bulunamadı");
        //                    return;
        //                }

        //                CreateProductDto createProductDto = new CreateProductDto();
        //                createProductDto.Code = GetValue(rootElement, mappings, "Code");
        //                createProductDto.Name = GetValue(rootElement, mappings, "Name");
        //                createProductDto.Description = GetValue(rootElement, mappings, "Description");
        //                createProductDto.Barcode = GetValue(rootElement, mappings, "Barcode");
        //                createProductDto.Currency = GetValue(rootElement, mappings, "Currency");
        //                createProductDto.Price = GetDecimalValue(rootElement, mappings, "Price");
        //                createProductDto.VatRate = GetDecimalValue(rootElement, mappings, "VatRate");
        //                createProductDto.StockQuantity = GetIntegerValue(rootElement, mappings, "StockQuantity");

        //                var brandId = await CreateOrUpdateBrand(GetValue(rootElement, mappings, "Brand"));
        //                createProductDto.BrandId = brandId;



        //                //varyantlar
        //                var xmlElementAttributeStockCode = GetMappedXmlHeader(mappings, "AttributeStockCode");
        //                if (xmlElementAttributeStockCode != null)
        //                {
        //                    var deger = productElement?.Element(xmlElementAttributeStockCode)?.Value ?? "";
        //                }

        //                var xmlElementAttributePrice = GetMappedXmlHeader(mappings, "AttributePrice");
        //                if (xmlElementAttributePrice != null)
        //                {
        //                    var deger = productElement?.Element(xmlElementAttributePrice)?.Value ?? "";
        //                }

        //                var xmlElementAttributeStockQuantity = GetMappedXmlHeader(mappings, "AttributeStockQuantity");
        //                if (xmlElementAttributeStockQuantity != null)
        //                {
        //                    var deger = productElement?.Element(xmlElementAttributeStockQuantity)?.Value ?? "";
        //                }

        //                var xmlElementAttributeManufacturerPartNumber = GetMappedXmlHeader(mappings, "AttributeManufacturerPartNumber");
        //                if (xmlElementAttributeManufacturerPartNumber != null)
        //                {
        //                    var deger = productElement?.Element(xmlElementAttributeManufacturerPartNumber)?.Value ?? "";
        //                }

        //                var xmlElementAttributeSpecifications = GetMappedXmlHeader(mappings, "AttributeSpecifications");
        //                if (xmlElementAttributeSpecifications != null)
        //                {
        //                    var deger = productElement?.Element(xmlElementAttributeSpecifications)?.Value ?? "";
        //                }


        //                createProductDto.ManufacturerPartNumber = "";
        //                createProductDto.Unit = "Adet";
        //                createProductDto.Gtin = "";
        //                createProductDto.OldPrice = 0;
        //                createProductDto.SpecialPrice = 0;
        //                createProductDto.Weight = 0;
        //                createProductDto.Length = 0;
        //                createProductDto.Height = 0;
        //                createProductDto.Width = 0;
        //                createProductDto.MetaDescription = "";
        //                createProductDto.MetaKeywords = "";
        //                createProductDto.MetaTitle = "";
        //                createProductDto.Barcode = "";
        //                createProductDto.Published = false;
        //                createProductDto.Deleted = false;



        //                #region Images
        //                List<string> images = new();
        //                HttpClient httpClient = new HttpClient();
        //                string imageMap = mappings.FirstOrDefault(x => x.MappedName == "Images")?.XmlHeader ?? "";

        //                if (imageMap.Contains(","))
        //                {
        //                    var imageFields = imageMap.Split(",");
        //                    foreach (var item in imageFields)
        //                    {
        //                        var value = xDoc.Result.Descendants("Product").ElementAt(i)
        //                                     .Descendants(item)
        //                                     .Select(x => x.Value)
        //                                     .FirstOrDefault();

        //                        if (!string.IsNullOrEmpty(value))
        //                        {
        //                            images.Add(value);
        //                        }
        //                    }
        //                }
        //                else
        //                {
        //                    images = xDoc.Result.Descendants("Product").ElementAt(i)
        //                             .Descendants(imageMap)
        //                             .Select(x => x.Value)
        //                             .ToList();
        //                }

        //                List<int> uploadedIds = await UploadImagesAsync(images, httpClient);

        //                foreach (var id in uploadedIds)
        //                {
        //                    Console.WriteLine(id);
        //                }
        //                #endregion

        //                createdProductList.Add(createProductDto);

        //            }

        //            foreach (var item in createdProductList)
        //            {


        //                Console.WriteLine($"---------  {item.Code} Kodlu Ürün Ürün ---------");
        //                Console.WriteLine($"ProductCode: {item.Code}");
        //                Console.WriteLine($"Name: {item.Name}");
        //                Console.WriteLine($"Brand: {item.Brand.Name}");
        //                Console.WriteLine($"Price: {item.Price}");
        //                Console.WriteLine($"Stock: {item.StockQuantity}");
        //                Console.WriteLine($"CurrencyType: {item.Currency}");
        //                Console.WriteLine($"Vergi: {item.VatRate}");
        //                Console.WriteLine($"Description: {item.Description}");


        //                Console.WriteLine("-", 40);
        //            }


        //            //job kodu.txt kodların yeri



        //            Console.WriteLine($"Aktarım tamaMlandı");
        //        }
        //        catch (Exception ex)
        //        {
        //            Console.WriteLine($"Error downloading or saving XML file: {ex.Message}");
        //        }
        //    }



        //    private async Task<XDocument?> GetDocument(ImportProfileDto profile)
        //    {
        //        try
        //        {
        //            var url = profile.MediaFileUrl;

        //            if (string.IsNullOrEmpty(url))
        //            {
        //                throw new Exception("Dosya url boş");
        //            }

        //            using var httpClient = new HttpClient();
        //            httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/120 Safari/537.36");
        //            var xmlContent = await httpClient.GetStringAsync(url);
        //            using TextReader reader = new StringReader(xmlContent);
        //            XDocument xDoc = XDocument.Load(reader);

        //            return xDoc;
        //        }
        //        catch (Exception ex)
        //        {
        //            _logger.LogError(ex, "Dosya indirlirken bir hata oluştu");
        //            return null;
        //        }
        //    }
        //    public string GetValue(XElement xElement, List<XmlColumnMappingResult> mappingList, string mappedName)
        //    {
        //        if (mappingList == null)
        //        {
        //            _logger.LogError("Eşleştirlme yapılmamış");
        //            return "";
        //        }

        //        if (string.IsNullOrEmpty(mappedName))
        //        {
        //            _logger.LogError("Alan adı boş");
        //            return "";
        //        }

        //        if (!mappingList.Any(x => x?.MappedName == mappedName))
        //        {
        //            _logger.LogError("{MappedName} Eşleştirlme yapılmamış", mappedName);
        //            return "";
        //        }

        //        var xName = mappingList.First(x => x?.MappedName == mappedName).XmlHeader;
        //        var xFindElement = xElement.Element(xName);

        //        if (xFindElement == null)
        //        {
        //            _logger.LogError("Xml'de {MappedName} alanı bulunamadı.", xName);
        //            return "";
        //        }

        //        return xFindElement.Value;
        //    }
        //    public decimal GetDecimalValue(XElement xElement, List<XmlColumnMappingResult> mappingList, string mappedName)
        //    {
        //        if (mappingList == null)
        //        {
        //            _logger.LogError("Eşleştirlme yapılmamış");
        //            return 0;
        //        }

        //        if (string.IsNullOrEmpty(mappedName))
        //        {
        //            _logger.LogError("Alan adı boş");
        //            return 0;
        //        }

        //        if (!mappingList.Any(x => x?.MappedName == mappedName))
        //        {
        //            _logger.LogError("{MappedName} Eşleştirlme yapılmamış", mappedName);
        //            return 0;
        //        }

        //        var xName = mappingList.First(x => x?.MappedName == mappedName).XmlHeader;
        //        var xFindElement = xElement.Element(xName);

        //        if (xFindElement == null)
        //        {
        //            _logger.LogError("Xml'de {MappedName} alanı bulunamadı.", xName);
        //            return 0;
        //        }

        //        return decimal.TryParse(xFindElement.Value.Replace(".", ","), out var vatRate) ? vatRate : 0m;
        //    }
        //    public int GetIntegerValue(XElement xElement, List<XmlColumnMappingResult> mappingList, string mappedName)
        //    {
        //        if (mappingList == null)
        //        {
        //            _logger.LogError("Eşleştirlme yapılmamış");
        //            return 0;
        //        }

        //        if (string.IsNullOrEmpty(mappedName))
        //        {
        //            _logger.LogError("Alan adı boş");
        //            return 0;
        //        }

        //        if (!mappingList.Any(x => x?.MappedName == mappedName))
        //        {
        //            _logger.LogError("{MappedName} Eşleştirlme yapılmamış", mappedName);
        //            return 0;
        //        }

        //        var xName = mappingList.First(x => x?.MappedName == mappedName).XmlHeader;
        //        var xFindElement = xElement.Element(xName);

        //        if (xFindElement == null)
        //        {
        //            _logger.LogError("Xml'de {MappedName} alanı bulunamadı.", xName);
        //            return 0;
        //        }

        //        var value = int.TryParse(xFindElement.Value, out var price) ? price : 0;
        //        return value;
        //    }

        //    private async Task AddVariantAttributeAsync(int productId, string attributeName, string attributeValue, List<ProductVariantAttributeModel> variantAttributes)
        //    {
        //        if (string.IsNullOrEmpty(attributeName) || string.IsNullOrEmpty(attributeValue)) return;

        //        int attrId = await EnsureProductAttributeAsync(attributeName);
        //        int attrValueId = await EnsureProductAttributeValueAsync(attrId, attributeValue);

        //        var existingVariant = await _productVariantAttributeService.GetByAttibuteIdAsync(productId, attrId);
        //        int variantId = existingVariant?.Id ?? (await _productVariantAttributeService.AddAsync(new CreateProductVariantAttributeDto
        //        {
        //            ProductId = productId,
        //            ProductAttributeId = attrId,
        //            DisplayOrder = 0,
        //            AttributeControlTypeId = 0
        //        })).Id;

        //        var existingVariantValue = await _productVariantAttributeValueService.GetByNameAsync(variantId, attributeValue);
        //        int variantValueId = existingVariantValue?.Id ?? (await _productVariantAttributeValueService.AddAsync(new CreateProductVariantAttributeValueDto()
        //        {
        //            Name = attributeValue,
        //            ProductVariantAttributeId = variantId
        //        })).Id;

        //        variantAttributes.Add(new ProductVariantAttributeModel
        //        {
        //            ProductVariantAttributeId = variantId,
        //            ProductVariantAttributeValueId = variantValueId
        //        });
        //    }
        //    private async Task<int> EnsureProductAttributeAsync(string name)
        //    {
        //        if (_attributeCache.TryGetValue(name, out int id)) return id;

        //        var created = await _productAttributeService.AddAsync(new CreateProductAttributeDto
        //        {
        //            Name = name,
        //            Description = "",
        //            DisplayOrder = 0
        //        });

        //        _attributeCache.TryAdd(name, created.Id);
        //        return created.Id;
        //    }
        //    private async Task<int> EnsureProductAttributeValueAsync(int attributeId, string value)
        //    {
        //        if (_attributeValueCache.TryGetValue((attributeId, value), out int id)) return id;

        //        var created = await _productAttributeValueService.AddAsync(new CreateProductAttributeValueDto
        //        {
        //            Name = value,
        //            DisplayOrder = 0,
        //            ProductAttributeId = attributeId
        //        });

        //        _attributeValueCache.TryAdd((attributeId, value), created.Id);
        //        return created.Id;
        //    }
        //    private async Task LoadAttributeCacheAsync()
        //    {
        //        var allAttributes = await _productAttributeService.GetAllAsync();
        //        foreach (var attr in allAttributes)
        //            _attributeCache.TryAdd(attr.Name, attr.Id);

        //        var allValues = await _productAttributeValueService.GetAllAsync();
        //        foreach (var val in allValues)
        //            _attributeValueCache.TryAdd((val.ProductAttributeId, val.Name), val.Id);

        //    }
        //    private async Task<int?> CreateOrUpdateBrand(string brand)
        //    {
        //        if (!string.IsNullOrWhiteSpace(brand))
        //        {
        //            var existingBrand = await _brandService.GetByNameAsync(brand);

        //            if (existingBrand == null)
        //            {
        //                var createdBrand = await _brandService.CreateAsync(new Application.DTOs.Brand.CreateBrandDto
        //                {
        //                    DisplayOrder = 0,
        //                    Name = brand,
        //                    Published = false
        //                });

        //                return createdBrand.Id;
        //            }
        //            else
        //            {
        //                return existingBrand.Id;
        //            }
        //        }

        //        return null;
        //    }
        //    private async Task<int> GetCategoryId(string? categoryName)
        //    {
        //        if (string.IsNullOrWhiteSpace(categoryName))
        //            return 0;
        //        var existingCategory = await _categoryService.GetCategoryByNameAsync(categoryName);
        //        if (existingCategory != null)
        //            return existingCategory.Id;
        //        var newCategory = await _categoryService.CreateCategoryAsync(new Application.DTOs.Category.CreateCategoryDto
        //        {
        //            Name = categoryName,
        //            Description = "",
        //            MetaDescription = "",
        //            MetaKeywords = "",
        //            MetaTitle = "",
        //            ParentId = null,
        //            Published = false,
        //            DisplayOrder = 0,
        //            MediaFileId = null
        //        });
        //        return newCategory.Id;
        //    }
        //    private async Task<int> CreateCategoryProduct(int productId, int categoryId)
        //    {
        //        if (productId == 0 && categoryId == 0)
        //            return 0;
        //        else
        //        {
        //            var newProductCategory = new Application.DTOs.ProductCategory.CreateProductCategoryDto
        //            {
        //                ProductId = productId,
        //                CategoryId = categoryId
        //            };
        //            await _productCategoryService.CreateProductCategoryAsync(newProductCategory);
        //            return 1;
        //        }

        //    }

        //    public async Task<List<int>> UploadImagesAsync(List<string> imageUrls, HttpClient httpClient)
        //    {
        //        List<int> fileIds = new();

        //        if (imageUrls != null && imageUrls.Any())
        //        {
        //            var multipartContent = new MultipartFormDataContent();
        //            multipartContent.Add(new StringContent("catalog"), "path");
        //            multipartContent.Add(new StringContent("False"), "isTransient");

        //            foreach (var imageUrl in imageUrls)
        //            {
        //                try
        //                {
        //                    var imageBytes = await httpClient.GetByteArrayAsync(imageUrl);
        //                    var imageName = Path.GetFileName(imageUrl);

        //                    if (string.IsNullOrWhiteSpace(imageName))
        //                        imageName = "default.jpg";

        //                    var byteContent = new ByteArrayContent(imageBytes);
        //                    byteContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/octet-stream");

        //                    multipartContent.Add(byteContent, "upload-file", imageName);
        //                }
        //                catch (Exception ex)
        //                {
        //                    Console.WriteLine($"HATA (indirilemedi): {imageUrl} → {ex.Message}");
        //                }
        //            }

        //            var uploadUrl = "https://localhost:4000/media/upload";
        //            var response = await httpClient.PostAsync(uploadUrl, multipartContent);

        //            if (response.IsSuccessStatusCode)
        //            {
        //                var result = await response.Content.ReadAsStringAsync();
        //                Console.WriteLine("Resimler başarıyla yüklendi: " + result);

        //                using var document = JsonDocument.Parse(result);
        //                var root = document.RootElement;

        //                if (root.ValueKind == JsonValueKind.Array)
        //                {
        //                    foreach (var item in root.EnumerateArray())
        //                    {
        //                        if (item.TryGetProperty("id", out var idProp))
        //                        {
        //                            int imageId = idProp.GetInt32();
        //                            fileIds.Add(imageId);
        //                            Console.WriteLine($"id: {imageId}");
        //                        }
        //                    }
        //                }
        //                else if (root.ValueKind == JsonValueKind.Object)
        //                {
        //                    if (root.TryGetProperty("id", out var idProp))
        //                    {
        //                        int imageId = idProp.GetInt32();
        //                        fileIds.Add(imageId);
        //                        Console.WriteLine($"id: {imageId}");
        //                    }
        //                }
        //            }
        //            else
        //            {
        //                Console.WriteLine("Resim yükleme başarısız: " + response.StatusCode);
        //            }
        //        }

        //        return fileIds;
        //    }
        //    public class ProductVariantAttributeModel
        //    {
        //        public int ProductVariantAttributeId { get; set; }
        //        public int ProductVariantAttributeValueId { get; set; }
        //    }
        //    public class XmlColumnMappingResult
        //    {
        //        public string XmlHeader { get; set; }
        //        public string MappedName { get; set; }
        //    }
        //}
        public Task Execute(IJobExecutionContext context)
        {
            throw new NotImplementedException();
        }
    }

}