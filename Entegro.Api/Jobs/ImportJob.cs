using Entegro.Api.Models;
using Entegro.Application.DTOs.Category;
using Entegro.Application.DTOs.ImportProfile;
using Entegro.Application.DTOs.Product;
using Entegro.Application.DTOs.ProductCategory;
using Entegro.Application.DTOs.ProductMediaFile;
using Entegro.Application.Interfaces.Services.Base;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using Quartz;
using System.Globalization;
using System.Text.Json;
using System.Xml.Linq;

namespace Entegro.Api.Jobs
{
    [DisallowConcurrentExecution]
    public class ImportJob : IJob
    {
        private readonly IImportProfileService _importProfileService;
        private readonly IMediaFileService _mediaFileService;
        private readonly ISettingService _settingService;
        private readonly IProductService _productService;
        private readonly IProductCategoryService _productCategoryService;
        private readonly IBrandService _brandService;
        private readonly ICategoryService _categoryService;
        private readonly ILogger<ImportJob> _logger;
        private readonly IProductMediaFileMappingService _productMediaFileMappingService;
        public ImportJob(IImportProfileService importProfileService, IMediaFileService mediaFileService, ILogger<ImportJob> logger, ISettingService settingService, IProductService productService, IProductMediaFileMappingService productMediaFileMappingService, IBrandService brandService, ICategoryService categoryService, IProductCategoryService productCategoryService)
        {
            _importProfileService = importProfileService;
            _mediaFileService = mediaFileService;
            _settingService = settingService;
            _productService = productService;
            _logger = logger;
            _productMediaFileMappingService = productMediaFileMappingService;
            _brandService = brandService;
            _categoryService = categoryService;
            _productCategoryService = productCategoryService;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            _logger.LogInformation("İçe aktarım servisi başladı.");
            var profiles = await _importProfileService.GetAllAsync();


            if (!profiles.Any())
            {
                _logger.LogError("Profil Bulunamadı");
                return;
            }


            foreach (var profile in profiles)
            {
                if (profile.MediaFileType == "xml")
                {
                    await XmlJob(profile);
                }

                else if (profile.MediaFileType == "excel")
                {
                    // await ExcelJob(profile);
                }

                else
                {
                    _logger.LogError("MediaFileType bulunamadı");
                }
            }
        }



        public async Task ExcelJob(ImportProfileDto profile)
        {
            var mediaFile = await _mediaFileService.GetByIdAsync(profile.MediaFileId.Value);


            if (mediaFile is null)
            {
                _logger.LogError("Dosya Bulunamadı");
                return;
            }
            #region dosya kaydet
            var systemUrl = await _settingService.GetByKeyAsync("SystemUrl");
            if (systemUrl == null || string.IsNullOrWhiteSpace(systemUrl.Value))
            {
                _logger.LogError("Sistem URL'si ayarlanmamış.");
                return;
            }

            if (!Uri.TryCreate(systemUrl.Value, UriKind.Absolute, out var baseUri))
            {
                _logger.LogError("Geçersiz SystemUrl formatı.");
                return;
            }

            using var httpClient = new HttpClient
            {
                BaseAddress = baseUri
            };

            string endpoint;
            if (mediaFile.Folder == null)
                endpoint = $"media/{mediaFile.Id}/{Uri.EscapeDataString(mediaFile.Name)}";
            else
                endpoint = $"media/{mediaFile.Id}/{Uri.EscapeDataString(mediaFile.Folder.Name)}/{Uri.EscapeDataString(mediaFile.Name)}";
            var response = await httpClient.GetAsync(endpoint);
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError($"Dosya indirilemedi: {response.StatusCode}");
                return;
            }

            var fileBytes = await response.Content.ReadAsByteArrayAsync();

            string basePath = Path.Combine(Directory.GetCurrentDirectory(), "App_Data", "Media", "Storage", "document");


            if (!Directory.Exists(basePath))
                Directory.CreateDirectory(basePath);


            string filePath = Path.Combine(basePath, mediaFile.Name);
            await File.WriteAllBytesAsync(filePath, fileBytes);


            _logger.LogInformation($"Excel dosyası kaydedildi: {filePath}");

            #endregion

            #region exceljob
            try
            {
                var selectedColumns = JsonSerializer.Deserialize<List<ExcelColumnMappingResult>>(profile.ColumnMapping);
                using var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
                IWorkbook workbook;
                if (Path.GetExtension(filePath).Equals(".xls", StringComparison.OrdinalIgnoreCase))
                    workbook = new HSSFWorkbook(stream);
                else
                    workbook = new XSSFWorkbook(stream);

                var sheet = workbook.GetSheetAt(0);
                if (sheet == null)
                {
                    _logger.LogError("Excel sayfası okunamadı.");
                    return;

                }

                var headerRow = sheet.GetRow(0);
                if (headerRow == null)
                {
                    _logger.LogError("Başlık satırı bulunamadı.");
                    return;
                }

                int totalRows = sheet.LastRowNum;


                var headerMap = new Dictionary<int, string>();
                for (int i = 0; i < headerRow.LastCellNum; i++)
                {
                    var headerValue = headerRow.GetCell(i)?.ToString()?.Trim();
                    if (!string.IsNullOrEmpty(headerValue))
                    {
                        var mapping = selectedColumns.FirstOrDefault(x => x.ExcelHeader == headerValue);
                        if (mapping != null)
                            headerMap[i] = mapping.MappedName;
                    }
                }

                int importedCount = 0;
                for (int rowIndex = 1; rowIndex <= totalRows; rowIndex++)
                {
                    List<string> Images = new List<string>();
                    var row = sheet.GetRow(rowIndex);
                    if (row == null) continue;

                    var productDto = new CreateProductDto
                    {
                        Code = "",
                        Name = "",
                        Description = "",
                        Price = 0,
                        CostPrice = 0,
                        Unit = "Adet",
                        Currency = "TL",
                        BrandId = null,
                        VatRate = 0,
                        StockQuantity = 0,
                        Published = true,
                        Deleted = false,
                        CreatedOn = DateTime.UtcNow,
                        UpdatedOn = DateTime.UtcNow
                    };

                    try
                    {
                        foreach (var kvp in headerMap)
                        {
                            int colIndex = kvp.Key;
                            string dbColumn = kvp.Value;
                            string cellValue = row.GetCell(colIndex)?.ToString()?.Trim() ?? "";

                            switch (dbColumn)
                            {
                                case "Code":
                                    productDto.Code = cellValue;
                                    break;

                                case "Name":
                                    productDto.Name = cellValue;
                                    break;

                                case "Description":
                                    productDto.Description = cellValue;
                                    break;

                                case "ManufacturerPartNumber":
                                    productDto.ManufacturerPartNumber = cellValue;
                                    break;

                                case "Gtin":
                                    productDto.Gtin = cellValue;
                                    break;

                                case "Price":
                                    if (decimal.TryParse(cellValue, out decimal price))
                                        productDto.Price = price;
                                    break;

                                case "OldPrice":
                                    if (decimal.TryParse(cellValue, out decimal oldPrice))
                                        productDto.OldPrice = oldPrice;
                                    break;

                                case "SalePrice":
                                    if (decimal.TryParse(cellValue, out decimal salePrice))
                                        productDto.SalePrice = salePrice;
                                    break;

                                case "VatRate":
                                    if (decimal.TryParse(cellValue, out decimal vatRate))
                                        productDto.VatRate = vatRate;
                                    break;


                                case "StockQuantity":
                                    if (int.TryParse(cellValue, out int stockQty))
                                        productDto.StockQuantity = stockQty;
                                    break;

                                case "Weight":
                                    if (decimal.TryParse(cellValue, out decimal weight))
                                        productDto.Weight = weight;
                                    break;

                                case "Length":
                                    if (decimal.TryParse(cellValue, out decimal length))
                                        productDto.Length = length;
                                    break;

                                case "Width":
                                    if (decimal.TryParse(cellValue, out decimal width))
                                        productDto.Width = width;
                                    break;

                                case "Height":
                                    if (decimal.TryParse(cellValue, out decimal height))
                                        productDto.Height = height;
                                    break;

                                case "MetaKeywords":
                                    productDto.MetaKeywords = cellValue;
                                    break;

                                case "MetaDescription":
                                    productDto.MetaDescription = cellValue;
                                    break;

                                case "MetaTitle":
                                    productDto.MetaTitle = cellValue;
                                    break;

                                case "Barcode":
                                    productDto.Barcode = cellValue;
                                    break;

                                case "CostPrice":
                                    if (decimal.TryParse(cellValue, out decimal costPrice))
                                        productDto.CostPrice = costPrice;
                                    break;

                                case "Images":
                                    if (!string.IsNullOrWhiteSpace(cellValue))
                                    {
                                        var separators = new[] { ";", ",", "\n", "\r" };
                                        Images = cellValue
                                            .Split(separators, StringSplitOptions.RemoveEmptyEntries)
                                            .Select(x => x.Trim())
                                            .Where(x => !string.IsNullOrEmpty(x))
                                            .ToList();
                                    }
                                    break;
                            }
                        }


                        if (string.IsNullOrWhiteSpace(productDto.Code))
                        {
                            _logger.LogInformation($"Satır {rowIndex} atlandı: 'Code' boş.");
                            continue;
                        }

                        var existProduct = await _productService.ExistsByCodeAsync(productDto.Code);

                        if (!existProduct)
                        {
                            var createdProduct = await _productService.AddAsync(productDto);

                            List<int> mediaFiles = await UploadImagesAsync(Images, httpClient);
                            foreach (var item in mediaFiles)
                            {
                                CreateProductMediaFileDto createProductMediaFile = new CreateProductMediaFileDto();
                                createProductMediaFile.MediaFileId = item;
                                createProductMediaFile.ProductId = createdProduct.Id;

                                await _productMediaFileMappingService.AddAsync(createProductMediaFile);
                            }

                            importedCount++;

                        }
                        else
                        {
                            _logger.LogInformation($"{productDto.Code} kodlu ürün zaten mevcut.");
                            continue;
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogInformation("Satır {rowIndex} okunamadı: {ex.Message}.");
                        continue;
                    }
                }
                _logger.LogInformation($"Toplam {importedCount} satır başarıyla kaydedildi.");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Hata oluştu: {ex.Message}");
            }
            #endregion

        }

        public async Task XmlJob(ImportProfileDto profile)
        {

            var mapping = JsonSerializer.Deserialize<ColumnMappingModel>(profile.ColumnMapping);

            using var client = new HttpClient();
            client.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0");
            var xmlData = await client.GetStringAsync(mapping.XmlUrl);
            var xdoc = XDocument.Parse(xmlData);
            var products = xdoc.Descendants("Product").ToList();
            var filtered = products.Where(p =>
            {
                var code = p.Element("Product_code")?.Value;
                return mapping.SelectedProducts.Contains(code);
            }).ToList();




            foreach (var product in filtered)
            {
                var dict = new Dictionary<string, object>();

                foreach (var map in mapping.Mappings)
                {
                    if (map.IsImage)
                    {

                        var imgList = map.XmlTags
                            .Select(tag => product.Element(tag)?.Value)
                            .Where(v => !string.IsNullOrWhiteSpace(v))
                            .ToList();

                        dict[map.ColumnName] = imgList;
                    }
                    else
                    {
                        var val = map.XmlTags
                            .Select(tag => product.Element(tag)?.Value)
                            .FirstOrDefault(v => v != null);
                        dict[map.ColumnName] = val;
                    }
                }

                decimal price = 0;
                if (dict.ContainsKey("Price"))
                {
                    price = ParsePrice(dict["Price"]?.ToString());
                }

                int brandId = 0;
                if (dict.ContainsKey("Brand"))
                {
                    var brandName = dict.ContainsKey("Brand") ? dict["Brand"]?.ToString() : "";
                    var brand = await _brandService.ExistsByNameAsync(brandName);
                    if (!brand)
                    {
                        var createdBrand = await _brandService.AddAsync(new Application.DTOs.Brand.CreateBrandDto
                        {
                            Name = brandName,
                            DisplayOrder = 0,
                            Published = true
                        });
                        brandId = createdBrand.Id;
                    }
                    else
                    {
                        var existBrand = await _brandService.GetByNameAsync(brandName);
                        brandId = existBrand.Id;
                    }

                }
                int categoryId = 0;
                if (dict.ContainsKey("Category"))
                {
                    var categoryName = dict.ContainsKey("Category") ? dict["Category"]?.ToString() : "";
                    var category = await _categoryService.ExistsByNameAsync(categoryName);
                    if (!category)
                    {
                        var createdCategory = await _categoryService.AddAsync(new CreateCategoryDto
                        {
                            Name = categoryName,
                            DisplayOrder = 0,
                            Published = true
                        });
                        categoryId = createdCategory.Id;
                    }
                    else
                    {
                        var existCategory = await _categoryService.GetCategoryByNameAsync(categoryName);
                        categoryId = existCategory.Id;
                    }

                }

                int stock = 0;
                if (dict.ContainsKey("StockQuantity") && dict["StockQuantity"] != null)
                {
                    var stockQuantity = dict["StockQuantity"]?.ToString();

                    stockQuantity = stockQuantity.Replace(".", ",");
                    if (decimal.TryParse(stockQuantity, new CultureInfo("tr-TR"), out var decStock))
                    {
                        stock = (int)decStock;
                    }
                }

                List<string> Images = new List<string>();

                if (dict.ContainsKey("Images") && dict["Images"] is List<string> imgList2)
                    Images = imgList2;
                else
                    Images = new List<string>();

                var productDto = new CreateProductDto
                {
                    Code = dict.ContainsKey("Code") ? dict["Code"]?.ToString() : "",
                    Name = dict.ContainsKey("Name") ? dict["Name"]?.ToString() : "",
                    Description = dict.ContainsKey("Description") ? dict["Description"]?.ToString() : "",
                    Price = price,
                    Barcode = dict.ContainsKey("Barcode") ? dict["Barcode"]?.ToString() : "",
                    CostPrice = 0,
                    Unit = "Adet",
                    Currency = "TL",
                    BrandId = brandId > 0 ? brandId : null,
                    VatRate = 0,
                    StockQuantity = stock,
                    Published = true,
                    Deleted = false,
                    CreatedOn = DateTime.UtcNow,
                    UpdatedOn = DateTime.UtcNow
                };

                var existProduct = await _productService.ExistsByCodeAsync(productDto.Code);

                if (!existProduct)
                {
                    var systemUrl = await _settingService.GetByKeyAsync("SystemUrl");
                    if (systemUrl == null || string.IsNullOrWhiteSpace(systemUrl.Value))
                    {
                        _logger.LogError("Sistem URL'si ayarlanmamış.");
                        return;
                    }

                    if (!Uri.TryCreate(systemUrl.Value, UriKind.Absolute, out var baseUri))
                    {
                        _logger.LogError("Geçersiz SystemUrl formatı.");
                        return;
                    }

                    using var httpClient = new HttpClient
                    {
                        BaseAddress = baseUri
                    };
                    var createdProduct = await _productService.AddAsync(productDto);

                    List<int> mediaFiles = await UploadImagesAsync(Images, httpClient);
                    foreach (var item in mediaFiles)
                    {
                        CreateProductMediaFileDto createProductMediaFile = new CreateProductMediaFileDto();
                        createProductMediaFile.MediaFileId = item;
                        createProductMediaFile.ProductId = createdProduct.Id;

                        await _productMediaFileMappingService.AddAsync(createProductMediaFile);
                    }


                    var productCategoryDto = new CreateProductCategoryDto
                    {
                        ProductId = createdProduct.Id,
                        CategoryId = categoryId
                    };

                    await _productCategoryService.AddAsync(productCategoryDto);

                    #region Variants
                    if (mapping.IncludeVariants)
                    {
                        var variantNodes = product.Element("variants")?.Elements("variant");

                        if (variantNodes != null)
                        {
                            foreach (var variant in variantNodes)
                            {
                                // varyant alanlarını oku
                                string vCode = variant.Element("productCode")?.Value?.Trim() ?? "";
                                string vBarcode = variant.Element("barcode")?.Value?.Trim() ?? "";
                                string vPriceStr = variant.Element("price")?.Value ?? "";
                                string vStockStr = variant.Element("quantity")?.Value ?? "";

                                decimal vPrice = ParsePrice(vPriceStr);
                                int vStock = ParseStock(vStockStr);

                                // specs -> beden / renk vs
                                var specs = variant.Elements("spec")
                                                   .ToDictionary(
                                                       x => x.Attribute("name")?.Value ?? "",
                                                       x => x.Value?.Trim() ?? ""
                                                    );

                                // Ürün adı: Ana + specs
                                string vName = productDto.Name;
                                if (specs.Any())
                                {
                                    vName += " (" + string.Join(" - ", specs.Select(s => $"{s.Key}:{s.Value}")) + ")";
                                }

                                // Varyant dto
                                var variantDto = new CreateProductDto
                                {
                                    Code = vCode,
                                    Name = vName,
                                    Description = productDto.Description,
                                    Price = vPrice,
                                    Barcode = vBarcode,
                                    CostPrice = productDto.CostPrice,
                                    Unit = "Adet",
                                    Currency = "TL",
                                    BrandId = productDto.BrandId,
                                    VatRate = productDto.VatRate,
                                    StockQuantity = vStock,
                                    Published = true,
                                    Deleted = false,
                                    CreatedOn = DateTime.UtcNow,
                                    UpdatedOn = DateTime.UtcNow
                                };

                                // exist kontrol
                                //var existVariant = await _productService.ExistsByCodeAsync(variantDto.Code);
                                //if (!existVariant)
                                //{
                                //    var createdVariant = await _productService.AddAsync(variantDto);

                                //    // varyant resimleri ana ürün ile aynıdır
                                //    List<int> vMediaFiles = await UploadImagesAsync(Images, httpClient);
                                //    foreach (var item in vMediaFiles)
                                //    {
                                //        await _productMediaFileMappingService.AddAsync(
                                //            new CreateProductMediaFileDto
                                //            {
                                //                MediaFileId = item,
                                //                ProductId = createdVariant.Id
                                //            });
                                //    }

                                //    // kategori bağla
                                //    await _productCategoryService.AddAsync(new CreateProductCategoryDto
                                //    {
                                //        ProductId = createdVariant.Id,
                                //        CategoryId = categoryId
                                //    });
                                //}
                                //else
                                //{
                                //    _logger.LogInformation($"{variantDto.Code} kodlu varyant zaten mevcut.");
                                //}
                            }
                        }
                    }
                    #endregion


                }
                else
                {
                    _logger.LogInformation($"{productDto.Code} kodlu ürün zaten mevcut.");
                    continue;
                }


            }
        }

        private decimal ParsePrice(string? input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return 0;

            var cleaned = new string(input.Where(c => char.IsDigit(c) || c == ',' || c == '.').ToArray());

            if (string.IsNullOrWhiteSpace(cleaned))
                return 0;

            var tr = new CultureInfo("tr-TR");


            if (cleaned.Contains(',') && cleaned.Contains('.'))
            {
                cleaned = cleaned.Replace(".", "");
                if (decimal.TryParse(cleaned, NumberStyles.Number, tr, out var val1))
                    return val1;
            }

            else if (cleaned.Contains(','))
            {
                if (decimal.TryParse(cleaned, NumberStyles.Number, tr, out var val2))
                    return val2;
            }

            else if (cleaned.Contains('.'))
            {
                cleaned = cleaned.Replace('.', ',');
                if (decimal.TryParse(cleaned, NumberStyles.Number, tr, out var val3))
                    return val3;
            }

            else
            {
                if (decimal.TryParse(cleaned, NumberStyles.Number, CultureInfo.InvariantCulture, out var val4))
                    return val4;
            }

            return 0;
        }
        private List<VariantModel> ReadVariants(XElement product)
        {
            var list = new List<VariantModel>();

            var variantNodes = product.Element("variants")?.Elements("variant");
            if (variantNodes == null) return list;

            foreach (var v in variantNodes)
            {
                var dto = new VariantModel
                {
                    ProductCode = v.Element("productCode")?.Value?.Trim(),
                    Barcode = v.Element("barcode")?.Value?.Trim(),
                    Quantity = ParseStock(v.Element("quantity")?.Value),
                    Price = ParsePrice(v.Element("price")?.Value),
                    Specs = v.Elements("spec")
                             .ToDictionary(
                                 x => x.Attribute("name")?.Value ?? "",
                                 x => x.Value?.Trim() ?? ""
                             )
                };

                list.Add(dto);
            }

            return list;
        }
        private int ParseStock(string? input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return 0;

            input = input.Replace(".", ",");
            if (decimal.TryParse(input, new CultureInfo("tr-TR"), out var dec))
                return (int)dec;

            return 0;
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

                        var nameWithoutExtension = Path.GetFileNameWithoutExtension(imageName);
                        var extension = Path.GetExtension(imageName);
                        var uniqueSuffix = $"excel_{Guid.NewGuid():N}";
                        imageName = $"{nameWithoutExtension}_{uniqueSuffix}{extension}";

                        var byteContent = new ByteArrayContent(imageBytes);
                        byteContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/octet-stream");

                        multipartContent.Add(byteContent, "upload-file", imageName);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError($"HATA (indirilemedi): {imageUrl} → {ex.Message}");
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
                            }
                        }
                    }
                    else if (root.ValueKind == JsonValueKind.Object)
                    {
                        if (root.TryGetProperty("id", out var idProp))
                        {
                            int imageId = idProp.GetInt32();
                            fileIds.Add(imageId);
                        }
                    }
                }
                else
                {

                    _logger.LogError($"Resim yükleme başarısız oldu. Durum kodu:" + response.StatusCode);
                }
            }

            return fileIds;
        }



    }
}
