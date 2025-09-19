using Entegro.Application.DTOs.Brand;
using Entegro.Application.DTOs.Category;
using Entegro.Application.Interfaces.Services;
using Quartz;
using System.Globalization;
using System.Text.Json;
using System.Xml.Linq;
using File = System.IO.File;

namespace Entegro.Api.Jobs
{
    public class FileDownloadJob : IJob
    {
        private readonly IImportProfileService _importProfileService;
        private readonly IProductService _productService;
        private readonly ICategoryService _categoryService;
        private readonly IBrandService _brandService;
        private readonly IProductImageMappingService _productImageMappingService;
        private readonly IProductCategoryService _productCategoryService;


        public FileDownloadJob(IImportProfileService importProfileService, IProductService productService, ICategoryService categoryService, IBrandService brandService, IProductImageMappingService productImageMappingService, IProductCategoryService productCategoryService)
        {
            _importProfileService = importProfileService;
            _productService = productService;
            _categoryService = categoryService;
            _brandService = brandService;
            _productImageMappingService = productImageMappingService;
            _productCategoryService = productCategoryService;
        }
        public async Task Execute(IJobExecutionContext context)
        {
            try
            {
                var profileId = context.JobDetail.JobDataMap.GetInt("ProfileId");
                var profile = await _importProfileService.GetByIdAsync(profileId);
                if (profile == null)
                {
                    Console.WriteLine("Profil bulunamadı.");
                    return;
                }

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

                var appDataForImagePath = Path.Combine(
                    Directory.GetParent(Directory.GetCurrentDirectory()).FullName,
                    "Entegro.Web",
                    "App_Data",
                    "Media",
                    "Storage",
                    "catalog" // folderName burada sabit "images"
                );

                if (!Directory.Exists(appDataForImagePath))
                    Directory.CreateDirectory(appDataForImagePath);

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



                var products = xDoc.Descendants("Product")
                   .Select(p =>
                   {
                       // Orijinal fiyat
                       decimal price = decimal.TryParse(p.Element("psf_fiyati")?.Value, NumberStyles.Any, CultureInfo.InvariantCulture, out var parsedPrice) ? parsedPrice : 0m;
                       decimal stock = decimal.TryParse(p.Element("Stock")?.Value, NumberStyles.Any, CultureInfo.InvariantCulture, out var parsedStock) ? parsedStock : 0m;
                       decimal tax = decimal.TryParse(p.Element("Tax")?.Value, NumberStyles.Any, CultureInfo.InvariantCulture, out var parsedTax) ? parsedTax : 0m;

                       // Ayarlamalar
                       decimal priceAdjustment = profile.PriceAdjustmentAmount ?? 0;
                       decimal optionalExtra = profile.OptionalExtraAmount ?? 0;
                       decimal adjustedPrice = price * priceAdjustment / 100 + price + optionalExtra;

                       string images = string.Join(",",
                           p.Elements()
                            .Where(e => e.Name.LocalName.StartsWith("Image", StringComparison.OrdinalIgnoreCase))
                            .Select(e => e.Value?.Trim())
                            .Where(val => !string.IsNullOrWhiteSpace(val)));

                       return new
                       {
                           OriginalProductCode = p.Element("Product_code")?.Value,
                           ProductCode = $"{profileId}_" + p.Element("Product_code")?.Value,
                           Name = p.Element("Name")?.Value,
                           Brand = p.Element("Brand")?.Value,
                           Category = p.Element("subCategory")?.Value,
                           Price = adjustedPrice,
                           Stock = stock,
                           CostPrice = price,
                           CurrencyType = p.Element("CurrencyType")?.Value,
                           Tax = tax,
                           Image = images,
                           Description = p.Element("Description")?.Value,
                           Variants = p.Elements("variants").Elements("variant").Select(v => new
                           {
                               SpecName = v.Element("spec")?.Attribute("name")?.Value,
                               SpecValue = v.Element("spec")?.Value,
                               VariantPrice = decimal.TryParse(v.Element("price")?.Value, NumberStyles.Any, CultureInfo.InvariantCulture, out var variantPrice) ? variantPrice : 0m,
                           }).ToList()
                       };
                   }).ToList();






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



                    var imageUrls = p.Image?.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
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

                    // Loglama
                    Console.WriteLine($"Ürün Kodu: {p.ProductCode}");
                    Console.WriteLine($"Kar Yüzdesi: {profile.OptionalExtraAmount}");
                    Console.WriteLine($"Sabit: {profile.PriceAdjustmentAmount}");
                    Console.WriteLine($"Kategori: {p.Category}");
                    Console.WriteLine($"Adı: {p.Name}");
                    Console.WriteLine($"Marka: {p.Brand}");
                    Console.WriteLine($"Fiyatı: {p.Price} {p.CurrencyType}");
                    Console.WriteLine($"Maliyet Fiyatı: {p.CostPrice} {p.CurrencyType}");
                    Console.WriteLine($"Resimler: {p.Image}");
                    Console.WriteLine($"Stok: {p.Stock}");
                    Console.WriteLine($"KDV: {p.Tax}%");
                    Console.WriteLine($"Açıklama: {p.Description}");

                    if (p.Variants.Any())
                    {
                        Console.WriteLine("Varyantlar:");
                        p.Variants.ForEach(v =>
                        {
                            Console.WriteLine($"\tÖzellik: {v.SpecName} - Değer: {v.SpecValue} - Fiyat: {v.VariantPrice}");
                        });
                    }

                    Console.WriteLine("--------------------------------------------------");
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error downloading or saving XML file: {ex.Message}");
            }
        }



    }

}
