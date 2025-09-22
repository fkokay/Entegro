using Entegro.Application.DTOs.Brand;
using Entegro.Application.DTOs.ImportProfile;
using Entegro.Application.DTOs.Product;
using Entegro.Application.DTOs.ProductAttribute;
using Entegro.Application.DTOs.ProductAttributeValue;
using Entegro.Application.DTOs.ProductVariantAttribute;
using Entegro.Application.DTOs.ProductVariantAttributeValue;
using Entegro.Application.Interfaces.Services;
using Newtonsoft.Json;
using Quartz;
using System.Collections.Concurrent;
using System.Text.Json;
using System.Xml.Linq;
using File = System.IO.File;

namespace Entegro.Api.Jobs
{
    [DisallowConcurrentExecution]
    public class FileDownloadJob : IJob
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
        public FileDownloadJob(IImportProfileService importProfileService, IProductService productService, ICategoryService categoryService, IBrandService brandService, IProductMediaFileMappingService productImageMappingService, IProductCategoryService productCategoryService, IProductAttributeService productAttributeService, IProductAttributeValueService productAttributeValueService, IProductVariantAttributeService productVariantAttributeService, IProductVariantAttributeValueService productVariantAttributeValueService, IProductVariantAttributeCombinationService productVariantAttributeCombinationService)
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
        }
        public async Task Execute(IJobExecutionContext context)
        {
            try
            {
                await LoadAttributeCacheAsync();
                var profileId = context.JobDetail.JobDataMap.GetInt("ProfileId");
                var profile = await _importProfileService.GetByIdAsync(profileId);
                var mappingList = JsonConvert.DeserializeObject<List<XmlColumnMappingResult>>(profile.ColumnMapping);

                if (profile == null)
                {
                    Console.WriteLine("Profil bulunamadı.");
                    return;
                }

                #region dosya 
                var xDoc = GetFile(profile);
                #endregion

                int productCount = xDoc.Result.Descendants("Product").Count();



                List<CreateProductDto> createdProductList = new List<CreateProductDto>();
                for (int i = 0; i < productCount; i += 1)
                {
                    CreateProductDto createProductDto = new CreateProductDto();

                    var productElement = xDoc.Result.Descendants("Product").ElementAtOrDefault(i);


                    var xmlElementProductName = GetMappedXmlHeader(mappingList, "Name");
                    if (xmlElementProductName != null)
                    {
                        createProductDto.Name = productElement?.Element(xmlElementProductName)?.Value ?? "";
                    }

                    var xmlElementProductCode = GetMappedXmlHeader(mappingList, "Code");
                    if (xmlElementProductCode != null)
                    {
                        createProductDto.Code = $"{profileId}_" + (productElement?.Element(xmlElementProductCode)?.Value ?? "");
                    }

                    var xmlElementDescription = GetMappedXmlHeader(mappingList, "Description");
                    if (xmlElementDescription != null)
                    {
                        createProductDto.Description = productElement?.Element(xmlElementDescription)?.Value ?? "";
                    }

                    var xmlElementCurrency = GetMappedXmlHeader(mappingList, "Currency");
                    if (xmlElementCurrency != null)
                    {
                        createProductDto.Currency = productElement?.Element(xmlElementCurrency)?.Value ?? "";
                    }

                    var xmlElementBarcode = GetMappedXmlHeader(mappingList, "Barcode");
                    if (xmlElementBarcode != null)
                    {
                        createProductDto.Barcode = productElement?.Element(xmlElementBarcode)?.Value ?? "";
                    }

                    var xmlHeaderPrice = GetMappedXmlHeader(mappingList, "Price");
                    if (xmlHeaderPrice != null)
                    {
                        var priceStr = productElement?.Element(xmlHeaderPrice)?.Value?.Replace(".", ",") ?? "0";
                        createProductDto.Price = decimal.TryParse(priceStr, out var price) ? price : 0m;
                    }

                    var xmlHeaderVatRate = GetMappedXmlHeader(mappingList, "VatRate");
                    if (xmlHeaderVatRate != null)
                    {
                        var vatRateStr = productElement?.Element(xmlHeaderVatRate)?.Value?.Replace(".", ",") ?? "0";
                        createProductDto.VatRate = decimal.TryParse(vatRateStr, out var vatRate) ? vatRate : 0m;
                    }

                    var xmlHeaderStockQuantity = GetMappedXmlHeader(mappingList, "StockQuantity");
                    if (xmlHeaderStockQuantity != null)
                    {
                        var stockQuantityStr = productElement?.Element(xmlHeaderStockQuantity)?.Value?.Replace(".", ",") ?? "0";
                        createProductDto.StockQuantity = int.TryParse(stockQuantityStr, out var stockQuantity) ? stockQuantity : 0;
                    }



                    //varyantlar
                    var xmlElementAttributeStockCode = GetMappedXmlHeader(mappingList, "AttributeStockCode");
                    if (xmlElementAttributeStockCode != null)
                    {
                        var deger = productElement?.Element(xmlElementAttributeStockCode)?.Value ?? "";
                    }

                    var xmlElementAttributePrice = GetMappedXmlHeader(mappingList, "AttributePrice");
                    if (xmlElementAttributePrice != null)
                    {
                        var deger = productElement?.Element(xmlElementAttributePrice)?.Value ?? "";
                    }

                    var xmlElementAttributeStockQuantity = GetMappedXmlHeader(mappingList, "AttributeStockQuantity");
                    if (xmlElementAttributeStockQuantity != null)
                    {
                        var deger = productElement?.Element(xmlElementAttributeStockQuantity)?.Value ?? "";
                    }

                    var xmlElementAttributeManufacturerPartNumber = GetMappedXmlHeader(mappingList, "AttributeManufacturerPartNumber");
                    if (xmlElementAttributeManufacturerPartNumber != null)
                    {
                        var deger = productElement?.Element(xmlElementAttributeManufacturerPartNumber)?.Value ?? "";
                    }

                    var xmlElementAttributeSpecifications = GetMappedXmlHeader(mappingList, "AttributeSpecifications");
                    if (xmlElementAttributeSpecifications != null)
                    {
                        var deger = productElement?.Element(xmlElementAttributeSpecifications)?.Value ?? "";
                    }


                    var xmlElementBrand = GetMappedXmlHeader(mappingList, "Brand");
                    if (xmlElementBrand != null)
                    {
                        var brandValue = productElement?.Element(xmlElementBrand)?.Value ?? "";
                        if (!string.IsNullOrEmpty(brandValue))
                        {
                            var brand = await GetBrandId(brandValue);
                            createProductDto.BrandId = brand.Id;
                            createProductDto.Brand = brand;
                        }
                    }


                    createProductDto.ManufacturerPartNumber = "";
                    createProductDto.Unit = "Adet";
                    createProductDto.Gtin = "";
                    createProductDto.OldPrice = 0;
                    createProductDto.SpecialPrice = 0;
                    createProductDto.Weight = 0;
                    createProductDto.Length = 0;
                    createProductDto.Height = 0;
                    createProductDto.Width = 0;
                    createProductDto.MetaDescription = "";
                    createProductDto.MetaKeywords = "";
                    createProductDto.MetaTitle = "";
                    createProductDto.Barcode = "";
                    createProductDto.Published = false;
                    createProductDto.Deleted = false;



                    #region Images
                    List<string> images = new();
                    HttpClient httpClient = new HttpClient();
                    string imageMap = mappingList.FirstOrDefault(x => x.MappedName == "Images")?.XmlHeader ?? "";

                    if (imageMap.Contains(","))
                    {
                        var imageFields = imageMap.Split(",");
                        foreach (var item in imageFields)
                        {
                            var value = xDoc.Result.Descendants("Product").ElementAt(i)
                                         .Descendants(item)
                                         .Select(x => x.Value)
                                         .FirstOrDefault();

                            if (!string.IsNullOrEmpty(value))
                            {
                                images.Add(value);
                            }
                        }
                    }
                    else
                    {
                        images = xDoc.Result.Descendants("Product").ElementAt(i)
                                 .Descendants(imageMap)
                                 .Select(x => x.Value)
                                 .ToList();
                    }

                    List<int> uploadedIds = await UploadImagesAsync(images, httpClient);

                    foreach (var id in uploadedIds)
                    {
                        Console.WriteLine(id);
                    }
                    #endregion

                    createdProductList.Add(createProductDto);

                }

                foreach (var item in createdProductList)
                {


                    Console.WriteLine($"---------  {item.Code} Kodlu Ürün Ürün ---------");
                    Console.WriteLine($"ProductCode: {item.Code}");
                    Console.WriteLine($"Name: {item.Name}");
                    Console.WriteLine($"Brand: {item.Brand.Name}");
                    Console.WriteLine($"Price: {item.Price}");
                    Console.WriteLine($"Stock: {item.StockQuantity}");
                    Console.WriteLine($"CurrencyType: {item.Currency}");
                    Console.WriteLine($"Vergi: {item.VatRate}");
                    Console.WriteLine($"Description: {item.Description}");


                    Console.WriteLine("-", 40);
                }


                //job kodu.txt kodların yeri



                Console.WriteLine($"Aktarım tamaMlandı");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error downloading or saving XML file: {ex.Message}");
            }
        }
        public static string GetMappedXmlHeader(List<XmlColumnMappingResult> mappingList, string mappedName)
        {
            if (mappingList == null || string.IsNullOrEmpty(mappedName))
                return null;

            return mappingList.FirstOrDefault(x => x?.MappedName == mappedName)?.XmlHeader;
        }

        private async Task<XDocument?> GetFile(ImportProfileDto profile)
        {
            string fileName = $"{profile.Id}_{profile.ProfileName}_{DateTime.UtcNow:yyyyMMdd_HHmmss}.xml";
            var url = profile.MediaFileUrl;
            var folderName = "document";

            if (string.IsNullOrEmpty(url) || string.IsNullOrEmpty(fileName))
            {
                throw new ArgumentException("XmlUrl or UploadedFileName is missing.");
            }

            using var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/120 Safari/537.36");
            var xmlContent = await httpClient.GetStringAsync(url);

            var appDataPath = Path.Combine(
                Directory.GetCurrentDirectory(),
                "App_Data",
                "Media",
                "Storage",
                folderName
            );

            if (!Directory.Exists(appDataPath))
                Directory.CreateDirectory(appDataPath);

            var fullFilePath = Path.Combine(appDataPath, fileName);
            await File.WriteAllTextAsync(fullFilePath, xmlContent);
            Console.WriteLine($"XML file saved to: {fullFilePath}");

            // XML dosyasını yükle ve işle
            string filePattern = $"{profile.Id}_*.xml";
            var files = Directory.GetFiles(appDataPath, filePattern);
            if (files.Length == 0)
            {
                Console.WriteLine("Dosya bulunamadı.");
                return null;
            }
            string xmlFilePath = files[0];
            XDocument xDoc = XDocument.Load(xmlFilePath);

            return xDoc;
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
        private async Task<BrandDto> GetBrandId(string? brandTitle)
        {
            BrandDto brand;
            if (!string.IsNullOrWhiteSpace(brandTitle))
            {
                var existingBrand = await _brandService.GetByNameAsync(brandTitle);

                if (existingBrand == null)
                {
                    brand = await _brandService.CreateAsync(new Application.DTOs.Brand.CreateBrandDto
                    {
                        DisplayOrder = 0,
                        Name = brandTitle,
                        Published = false
                    });
                    return new BrandDto
                    {
                        Id = brand.Id,
                        Name = brand.Name
                    };
                }
                else
                    return existingBrand;
            }
            return new BrandDto();
        }
        private async Task<int> GetCategoryId(string? categoryName)
        {
            if (string.IsNullOrWhiteSpace(categoryName))
                return 0;
            var existingCategory = await _categoryService.GetCategoryByNameAsync(categoryName);
            if (existingCategory != null)
                return existingCategory.Id;
            var newCategory = await _categoryService.CreateCategoryAsync(new Application.DTOs.Category.CreateCategoryDto
            {
                Name = categoryName,
                Description = "",
                MetaDescription = "",
                MetaKeywords = "",
                MetaTitle = "",
                ParentId = null,
                Published = false,
                DisplayOrder = 0,
                MediaFileId = null
            });
            return newCategory.Id;
        }
        private async Task<int> CreateCategoryProduct(int productId, int categoryId)
        {
            if (productId == 0 && categoryId == 0)
                return 0;
            else
            {
                var newProductCategory = new Application.DTOs.ProductCategory.CreateProductCategoryDto
                {
                    ProductId = productId,
                    CategoryId = categoryId
                };
                await _productCategoryService.CreateProductCategoryAsync(newProductCategory);
                return 1;
            }

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

                var uploadUrl = "https://localhost:4000/media/upload";
                var response = await httpClient.PostAsync(uploadUrl, multipartContent);

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadAsStringAsync();
                    Console.WriteLine("Resimler başarıyla yüklendi: " + result);

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
                    Console.WriteLine("Resim yükleme başarısız: " + response.StatusCode);
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
