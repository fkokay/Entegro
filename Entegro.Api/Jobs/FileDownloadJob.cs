using Entegro.Application.DTOs.Brand;
using Entegro.Application.DTOs.Category;
using Entegro.Application.DTOs.ProductAttribute;
using Entegro.Application.DTOs.ProductAttributeValue;
using Entegro.Application.DTOs.ProductVariantAttribute;
using Entegro.Application.DTOs.ProductVariantAttributeCombination;
using Entegro.Application.DTOs.ProductVariantAttributeValue;
using Entegro.Application.Interfaces.Services;
using Newtonsoft.Json;
using Quartz;
using System.Collections.Concurrent;
using System.Globalization;
using System.Text.Json;
using System.Text.RegularExpressions;
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
                string fileName = $"{profileId}_{profile.ProfileName}_{DateTime.UtcNow:yyyyMMdd_HHmmss}.xml";
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
                    Directory.GetParent(Directory.GetCurrentDirectory()).FullName,
                    "Entegro.Web",
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
                string filePattern = $"{profileId}_*.xml";
                var files = Directory.GetFiles(appDataPath, filePattern);
                if (files.Length == 0)
                {
                    Console.WriteLine("Dosya bulunamadı.");
                    return;
                }
                string xmlFilePath = files[0];
                XDocument xDoc = XDocument.Load(xmlFilePath);
                #endregion


                //var products = xDoc.Descendants("Product").Select(p =>
                //{
                //    string GetMappedValue(string mappedName)
                //    {
                //        var header = mappingList.FirstOrDefault(x => x.MappedName == mappedName)?.XmlHeader;
                //        if (string.IsNullOrEmpty(header))
                //            return null;

                //        if (header.Contains(","))
                //        {
                //            var parts = header.Split(",");
                //            var combined = string.Join(",", parts
                //                .Select(h => p.Element(h)?.Value?.Trim())
                //                .Where(v => !string.IsNullOrWhiteSpace(v)));
                //            return combined;
                //        }

                //        return p.Element(header)?.Value?.Trim();
                //    }


                //    decimal SafeParseDecimal(string value)
                //    {
                //        if (string.IsNullOrWhiteSpace(value))
                //            return 0m;


                //        if (decimal.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out var result))
                //            return result;

                //        if (decimal.TryParse(value, NumberStyles.Any, new CultureInfo("tr-TR"), out result))
                //            return result;

                //        return 0m;
                //    }

                //    var imageString = GetMappedValue("Images");
                //    var imageUrls = Regex.Matches(imageString ?? "", @"https:\/\/[^\s]+?(jpg|jpeg|png|gif)")
                //                         .Cast<Match>()
                //                         .Select(m => m.Value)
                //                         .ToList();


                //    decimal price = SafeParseDecimal(GetMappedValue("Price"));
                //    decimal stock = SafeParseDecimal(GetMappedValue("StockQuantity"));
                //    decimal tax = SafeParseDecimal(GetMappedValue("VatRate"));

                //    decimal priceAdjustment = profile.PriceAdjustmentAmount ?? 0;
                //    decimal optionalExtra = profile.OptionalExtraAmount ?? 0;
                //    decimal adjustedPrice = price * priceAdjustment / 100 + price + optionalExtra;

                //    return new
                //    {
                //        OriginalProductCode = GetMappedValue("Code"),
                //        ProductCode = $"{profileId}_" + GetMappedValue("Code"),
                //        Name = GetMappedValue("Name"),
                //        Brand = GetMappedValue("Brand"),
                //        Category = GetMappedValue("Categories"),
                //        Price = adjustedPrice,
                //        Stock = stock,
                //        CostPrice = price,
                //        CurrencyType = GetMappedValue("Currency"),
                //        Tax = tax,
                //        Image = imageUrls,
                //        Description = GetMappedValue("Description"),
                //        Variants = p.Elements("variants").Elements("variant").Select(v => new
                //        {
                //            Specs = v.Elements("spec").Select(s => new
                //            {
                //                SpecName = s.Attribute("name")?.Value,
                //                SpecValue = s.Value
                //            }).ToList(),

                //            VariantPrice = SafeParseDecimal(v.Element("price")?.Value),
                //            Barcode = v.Element("barcode")?.Value,
                //            Quantity = SafeParseDecimal(v.Element("quantity")?.Value),
                //            VariantProductCode = v.Element("productCode")?.Value,
                //        }).ToList()

                //    };
                //}).ToList();


                foreach (var product in products)
                {
                    Console.WriteLine("--------- Ürün ---------");
                    Console.WriteLine($"OriginalProductCode: {product.OriginalProductCode}");
                    Console.WriteLine($"ProductCode: {product.ProductCode}");
                    Console.WriteLine($"Name: {product.Name}");
                    Console.WriteLine($"Brand: {product.Brand}");
                    Console.WriteLine($"Category: {product.Category}");
                    Console.WriteLine($"Price: {product.Price}");
                    Console.WriteLine($"CostPrice: {product.CostPrice}");
                    Console.WriteLine($"Stock: {product.Stock}");
                    Console.WriteLine($"CurrencyType: {product.CurrencyType}");
                    Console.WriteLine($"Tax: {product.Tax}");
                    foreach (var image in product.Image)
                    {
                        Console.WriteLine($"Image: {image}");
                    }

                    Console.WriteLine($"Description: {product.Description}");

                    Console.WriteLine("-- Variants --");
                    foreach (var variant in product.Variants)
                    {
                        Console.WriteLine($"  VariantProductCode: {variant.VariantProductCode}");
                        Console.WriteLine($"  Barcode: {variant.Barcode}");
                        Console.WriteLine($"  Quantity: {variant.Quantity}");
                        Console.WriteLine($"  VariantPrice: {variant.VariantPrice}");

                        Console.WriteLine("  Specs:");
                        foreach (var spec in variant.Specs)
                        {
                            Console.WriteLine($"    {spec.SpecName}: {spec.SpecValue}");
                        }

                        Console.WriteLine();
                    }

                    Console.WriteLine("-------------------------\n");
                }


                //var products = xDoc.Descendants("Product")
                //   .Select(p =>
                //   {
                //       // Orijinal fiyat
                //       decimal price = decimal.TryParse(p.Element("psf_fiyati")?.Value, NumberStyles.Any, CultureInfo.InvariantCulture, out var parsedPrice) ? parsedPrice : 0m;
                //       decimal stock = decimal.TryParse(p.Element("Stock")?.Value, NumberStyles.Any, CultureInfo.InvariantCulture, out var parsedStock) ? parsedStock : 0m;
                //       decimal tax = decimal.TryParse(p.Element("Tax")?.Value, NumberStyles.Any, CultureInfo.InvariantCulture, out var parsedTax) ? parsedTax : 0m;

                //       // Ayarlamalar
                //       decimal priceAdjustment = profile.PriceAdjustmentAmount ?? 0;
                //       decimal optionalExtra = profile.OptionalExtraAmount ?? 0;
                //       decimal adjustedPrice = price * priceAdjustment / 100 + price + optionalExtra;

                //       string images = string.Join(",",
                //           p.Elements()
                //            .Where(e => e.Name.LocalName.StartsWith("Image", StringComparison.OrdinalIgnoreCase))
                //            .Select(e => e.Value?.Trim())
                //            .Where(val => !string.IsNullOrWhiteSpace(val)));

                //       return new
                //       {
                //           OriginalProductCode = p.Element("Product_code")?.Value,
                //           ProductCode = $"{profileId}_" + p.Element("Product_code")?.Value,
                //           Name = p.Element("Name")?.Value,
                //           Brand = p.Element("Brand")?.Value,
                //           Category = p.Element("subCategory")?.Value,
                //           Price = adjustedPrice,
                //           Stock = stock,
                //           CostPrice = price,
                //           CurrencyType = p.Element("CurrencyType")?.Value,
                //           Tax = tax,
                //           Image = images,
                //           Description = p.Element("Description")?.Value,
                //           Variants = p.Elements("variants").Elements("variant").Select(v => new
                //           {
                //               Specs = v.Elements("spec").Select(s => new
                //               {
                //                   SpecName = s.Attribute("name")?.Value,
                //                   SpecValue = s.Value
                //               }).ToList(),
                //               VariantPrice = decimal.TryParse(v.Element("price")?.Value.Replace(",", "."), out var variantPrice) ? variantPrice : 0m,
                //               Barcode = v.Element("barcode")?.Value,
                //               Quantity = v.Element("quantity")?.Value,
                //               VariantProductCode = v.Element("productCode")?.Value,
                //           }).ToList()
                //       };
                //   }).ToList();




                #region post
                foreach (var p in products)
                {

                    if (await _productService.ExistsByCodeAsync(p.ProductCode))
                        continue;

                    BrandDto brand;
                    if (!string.IsNullOrWhiteSpace(p.Brand))
                    {
                        var existingBrand = await _brandService.GetByNameAsync(p.Brand);
                        brand = existingBrand ?? await _brandService.CreateAsync(new Application.DTOs.Brand.CreateBrandDto
                        {
                            DisplayOrder = 0,
                            Name = p.Brand,
                            Published = false
                        });
                    }
                    else
                    {
                        Console.WriteLine($"Ürün {p.ProductCode} için marka bilgisi eksik, atlanıyor.");
                        continue;
                    }

                    CategoryDto category;
                    if (!string.IsNullOrWhiteSpace(p.Category))
                    {
                        var existingCategory = await _categoryService.GetCategoryByNameAsync(p.Category);
                        category = existingCategory ?? await _categoryService.CreateCategoryAsync(new Application.DTOs.Category.CreateCategoryDto
                        {
                            DisplayOrder = 0,
                            Name = p.Category,
                            Published = false
                        });
                    }
                    else
                    {
                        Console.WriteLine($"Ürün {p.ProductCode} için kategori bilgisi eksik, atlanıyor.");
                        continue;
                    }



                    var imageUrls = p.Image;
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


                    var model = new Application.DTOs.Product.CreateProductDto
                    {
                        Code = p.ProductCode,
                        Name = p.Name,
                        Description = p.Description,
                        ManufacturerPartNumber = null,
                        Gtin = null,
                        Price = p.Price,
                        OldPrice = 0,
                        SpecialPrice = 0,
                        Currency = p.CurrencyType,
                        Unit = "Adet",
                        VatRate = p.Tax,
                        VatInc = true,
                        BrandId = brand.Id,
                        StockQuantity = (int)p.Stock,
                        Weight = 0,
                        Length = 0,
                        Height = 0,
                        Width = 0,
                        MetaKeywords = null,
                        MetaDescription = null,
                        MetaTitle = null,
                        MainPictureId = null,
                        Barcode = null,
                        Published = false,
                        Deleted = false,
                    };

                    var createdProduct = await _productService.CreateProductAsync(model);
                    int orderIndex = 0;
                    foreach (var id in fileIds)
                    {

                        await _productImageMappingService.AddAsync(new Application.DTOs.ProductMediaFile.CreateProductMediaFileDto
                        {
                            ProductId = createdProduct.Id,
                            MediaFileId = id,
                            DisplayOrder = orderIndex
                        });
                        orderIndex++;
                    }

                    var categoryDto = await _categoryService.GetCategoryByNameAsync(p.Category);
                    if (categoryDto == null)
                    {
                        return;
                    }
                    var newProductCategory = new Application.DTOs.ProductCategory.CreateProductCategoryDto
                    {
                        ProductId = createdProduct.Id,
                        CategoryId = categoryDto.Id
                    };
                    await _productCategoryService.CreateProductCategoryAsync(newProductCategory);

                    var variantAttribute = new ProductAttributeDto();
                    if (p.Variants.Any())
                    {

                        foreach (var variant in p.Variants)
                        {
                            List<ProductVariantAttributeModel> variantAttributes = new List<ProductVariantAttributeModel>();
                            foreach (var spec in variant.Specs)
                            {
                                await AddVariantAttributeAsync(createdProduct.Id, spec.SpecName, spec.SpecValue, variantAttributes);
                            }

                            var combinationDto = new CreateProductVariantAttributeCombinationDto
                            {
                                ProductId = createdProduct.Id,
                                Gtin = "",
                                ManufacturerPartNumber = variant.Barcode,
                                Price = variant.VariantPrice,
                                StockQuantity = Convert.ToInt32(variant.Quantity),
                                StokCode = variant.VariantProductCode,
                                RawAttribute = JsonConvert.SerializeObject(variantAttributes),
                            };

                            await _productVariantAttributeCombinationService.AddAsync(combinationDto);
                        }
                    }
                }
                #endregion



                Console.WriteLine($"Aktarım tamaMlandı");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error downloading or saving XML file: {ex.Message}");
            }
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
    }
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