using Entegro.Api.Models;
using Entegro.Application.DTOs.Category;
using Entegro.Application.DTOs.ImportProfile;
using Entegro.Application.DTOs.Product;
using Entegro.Application.DTOs.ProductAttribute;
using Entegro.Application.DTOs.ProductAttributeValue;
using Entegro.Application.DTOs.ProductCategory;
using Entegro.Application.DTOs.ProductMediaFile;
using Entegro.Application.DTOs.ProductVariantAttribute;
using Entegro.Application.DTOs.ProductVariantAttributeCombination;
using Entegro.Application.DTOs.ProductVariantAttributeValue;
using Entegro.Application.Interfaces.Services.Base;
using MapsterMapper;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using Quartz;
using System.Collections.Concurrent;
using System.Globalization;
using System.Text.Json;
using System.Xml.Linq;
using JsonSerializer = System.Text.Json.JsonSerializer;

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
        private readonly ISpecificationAttributeService _specificationAttributeService;
        private readonly ISpecificationAttributeOptionService _specificationAttributeOptionService;
        private readonly IProductSpecificationAttributeMappingService _productSpecificationAttributeMappingService;
        private readonly ConcurrentDictionary<string, int> _attributeCache = new();
        private readonly ConcurrentDictionary<(int attributeId, string value), int> _attributeValueCache = new();
        private readonly IProductVariantAttributeCombinationService _productVariantAttributeCombinationService;
        private readonly IProductVariantAttributeValueService _productVariantAttributeValueService;
        private readonly IProductVariantAttributeService _productVariantAttributeService;
        private readonly IProductAttributeService _productAttributeService;
        private readonly IProductAttributeValueService _productAttributeValueService;
        private readonly IMapper _mapper;
        public ImportJob(IImportProfileService importProfileService, IMediaFileService mediaFileService, ILogger<ImportJob> logger, ISettingService settingService, IProductService productService, IProductMediaFileMappingService productMediaFileMappingService, IBrandService brandService, ICategoryService categoryService, IProductCategoryService productCategoryService, ISpecificationAttributeService specificationAttributeService, ISpecificationAttributeOptionService specificationAttributeOptionService, IProductSpecificationAttributeMappingService productSpecificationAttributeMappingService, IProductVariantAttributeCombinationService productVariantAttributeCombinationService, IProductVariantAttributeValueService productVariantAttributeValueService, IProductVariantAttributeService productVariantAttributeService, IProductAttributeService productAttributeService, IProductAttributeValueService productAttributeValueService, IMapper mapper)
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
            _specificationAttributeService = specificationAttributeService;
            _specificationAttributeOptionService = specificationAttributeOptionService;
            _productSpecificationAttributeMappingService = productSpecificationAttributeMappingService;
            _productVariantAttributeCombinationService = productVariantAttributeCombinationService;
            _productVariantAttributeValueService = productVariantAttributeValueService;
            _productVariantAttributeService = productVariantAttributeService;
            _productAttributeService = productAttributeService;
            _productAttributeValueService = productAttributeValueService;
            _mapper = mapper;
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
                    await ExcelJob(profile);
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


            var xmlProductCodes = filtered
                .Select(x => x.Element("Product_code")?.Value)
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .ToList();


            foreach (var product in filtered)
            {
                var dict = new Dictionary<string, object>();
                string code = "";
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


                code = dict.ContainsKey("Code") ? dict["Code"]?.ToString() + $"_xml_{profile.Id}" : "";
                var existProduct = await _productService.ExistsByCodeAsync(code);

                if (!existProduct)
                {

                    var productDto = new CreateProductDto
                    {
                        Code = code,
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
                                string vCode = variant.Element("productCode")?.Value?.Trim() ?? "";
                                string vBarcode = variant.Element("barcode")?.Value?.Trim() ?? "";
                                string vPriceStr = variant.Element("price")?.Value ?? "";
                                string vStockStr = variant.Element("quantity")?.Value ?? "";

                                decimal vPrice = ParsePrice(vPriceStr);
                                int vStock = ParseStock(vStockStr);

                                var specs = variant.Elements("spec")
                                                   .ToDictionary(
                                                       x => x.Attribute("name")?.Value ?? "",
                                                       x => x.Value?.Trim() ?? ""
                                                   );

                                await AddVariantCombinationAsync(
                                    createdProduct.Id,
                                    specs,
                                    new ProductDto
                                    {
                                        Code = vCode,
                                        Name = createdProduct.Name,
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
                                    },
                                    mediaFiles
                                );
                            }
                        }
                    }




                    #endregion


                }


                else
                {

                    var existingProduct = await _productService.GetProductByCodeAsync(code);
                    var updateDto = _mapper.Map<UpdateProductDto>(existingProduct);

                    // XML’den gelen değerlerle güncelle
                    updateDto.Name = dict["Name"]?.ToString() ?? updateDto.Name;
                    updateDto.Description = dict["Description"]?.ToString() ?? updateDto.Description;
                    updateDto.Barcode = dict["Barcode"]?.ToString() ?? updateDto.Barcode;
                    updateDto.Price = price;
                    updateDto.StockQuantity = stock;
                    updateDto.BrandId = brandId > 0 ? brandId : updateDto.BrandId;
                    updateDto.Published = true;                   // XML’de varsa aktif tut
                    updateDto.UpdatedOn = DateTime.UtcNow;

                    await _productService.UpdateAsync(updateDto);
                    // RESİMLERİ GÜNCELLE
                    if (Images.Any())
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
                        var newFiles = await UploadImagesAsync(Images, httpClient);

                        // mevcut resim eşleştirmelerini temizle (opsiyonel)
                        await _productMediaFileMappingService.DeleteByProductIdAsync(existingProduct.Id);

                        foreach (var fileId in newFiles)
                        {
                            await _productMediaFileMappingService.AddAsync(new CreateProductMediaFileDto
                            {
                                ProductId = existingProduct.Id,
                                MediaFileId = fileId
                            });
                        }

                        if (categoryId > 0)
                        {
                            await _productCategoryService.DeleteByProductIdAsync(existingProduct.Id);

                            await _productCategoryService.AddAsync(new CreateProductCategoryDto
                            {
                                ProductId = existingProduct.Id,
                                CategoryId = categoryId
                            });
                        }
                        if (mapping.IncludeVariants)
                        {
                            var variantNodes = product.Element("variants")?.Elements("variant");

                            if (variantNodes != null)
                            {

                                await _productVariantAttributeCombinationService.DeleteAsync(existingProduct.Id);

                                foreach (var variant in variantNodes)
                                {
                                    string vCode = variant.Element("productCode")?.Value?.Trim() ?? "";
                                    string vBarcode = variant.Element("barcode")?.Value?.Trim() ?? "";
                                    string vPriceStr = variant.Element("price")?.Value ?? "";
                                    string vStockStr = variant.Element("quantity")?.Value ?? "";

                                    decimal vPrice = ParsePrice(vPriceStr);
                                    int vStock = ParseStock(vStockStr);

                                    var specs = variant.Elements("spec")
                                                       .ToDictionary(
                                                            x => x.Attribute("name")?.Value ?? "",
                                                            x => x.Value?.Trim() ?? ""
                                                       );

                                    await AddVariantCombinationAsync(
                                         existingProduct.Id,
                                         specs,
                                         new ProductDto
                                         {
                                             Code = vCode,
                                             Name = updateDto.Name,
                                             Description = updateDto.Description,
                                             Price = vPrice,
                                             Barcode = vBarcode,
                                             CostPrice = updateDto.CostPrice,
                                             Unit = "Adet",
                                             Currency = "TL",
                                             BrandId = updateDto.BrandId,
                                             VatRate = 0,
                                             StockQuantity = vStock,
                                             Published = true,
                                             Deleted = false,
                                             CreatedOn = existingProduct.CreatedOn,
                                             UpdatedOn = DateTime.UtcNow
                                         },
                                         new List<int>() // resimler yukarıda zaten güncellendi
                                    );
                                }
                            }
                        }
                    }

                    _logger.LogInformation($"{code} kodlu ürün zaten mevcut.");
                    continue;
                }
            }



            //veritabanında olup xmlde olmayan ürünleri pasif yapma
            var xmlMissingCodes = mapping.SelectedProducts.Where(selectedCode => !xmlProductCodes.Contains(selectedCode)).ToList();
            foreach (var originalCode in xmlMissingCodes)
            {
                var productCode = $"{originalCode}_xml_{profile.Id}";
                var existsInDb = await _productService.ExistsByCodeAsync(productCode);

                if (existsInDb)
                {
                    var existingProduct = await _productService.GetProductByCodeAsync(productCode);
                    var updateDto = _mapper.Map<UpdateProductDto>(existingProduct);
                    updateDto.StockQuantity = 0;
                    updateDto.Published = false;
                    await _productService.UpdateAsync(updateDto);
                    _logger.LogInformation($"{productCode} XML’de bulunmadığı için stok 0’a çekildi ve pasif duruma alındı.");
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
        private async Task<int> EnsureProductAttributeAsync(string name)
        {
            if (_attributeCache.TryGetValue(name, out int id)) return id;

            var productAttribute = await _productAttributeService.GetByNameAsync(name);
            if (productAttribute is null)
            {
                var created = await _productAttributeService.AddAsync(new CreateProductAttributeDto
                {
                    Name = name,
                    Description = "",
                    DisplayOrder = 0
                });

                _attributeCache.TryAdd(name, created.Id);
                return created.Id;
            }

            _attributeCache.TryAdd(name, productAttribute.Id);
            return productAttribute.Id;
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
        private async Task AddVariantCombinationAsync(int productId, Dictionary<string, string> specs, ProductDto product, List<int> mediaFiles)
        {
            if (specs == null || !specs.Any())
                return;

            List<ProductVariantAttributeModel> variantAttributes = new();
            List<int> mediaFileMappingIds = new();

            // Tüm özellikleri tek listeye ekliyoruz
            foreach (var spec in specs)
            {
                string attributeName = spec.Key;
                string attributeValue = spec.Value;

                if (string.IsNullOrEmpty(attributeName) || string.IsNullOrEmpty(attributeValue))
                    continue;

                // Attribute ve Value ID'lerini garanti altına al
                int attrId = await EnsureProductAttributeAsync(attributeName);
                int attrValueId = await EnsureProductAttributeValueAsync(attrId, attributeValue);

                // Varyant Attribute (üst grup)
                var existingVariant = await _productVariantAttributeService.GetByAttibuteIdAsync(productId, attrId);
                int variantId = existingVariant?.Id ??
                    (await _productVariantAttributeService.AddAsync(new CreateProductVariantAttributeDto
                    {
                        ProductId = productId,
                        ProductAttributeId = attrId,
                        DisplayOrder = 0,
                        AttributeControlTypeId = 0
                    })).Id;

                // Varyant Attribute Value
                var existingVariantValue = await _productVariantAttributeValueService.GetByNameAsync(variantId, attributeValue);
                int variantValueId = existingVariantValue?.Id ??
                    (await _productVariantAttributeValueService.AddAsync(new CreateProductVariantAttributeValueDto
                    {
                        Name = attributeValue,
                        ProductVariantAttributeId = variantId
                    })).Id;

                // JSON içine eklenecek
                variantAttributes.Add(new ProductVariantAttributeModel
                {
                    ProductVariantAttributeId = variantId,
                    ProductVariantAttributeValueId = variantValueId
                });
            }

            // JSON serialize
            string rawAttributeJson = JsonSerializer.Serialize(variantAttributes,
                new JsonSerializerOptions { PropertyNamingPolicy = null, WriteIndented = false });

            // Aynı Product + StokCode (Barcode) var mı?
            bool exists = await _productVariantAttributeCombinationService.ExistsAsync(productId, product.Barcode);

            if (exists)
                return;

            // Media dosyaları
            foreach (var item in mediaFiles)
            {
                var created = await _productMediaFileMappingService.GetByPictureIdProductIdAsync(item, productId);
                if (created != null)
                    mediaFileMappingIds.Add(created.Id);
            }

            // Tek seferde kombinasyon ekleniyor
            await _productVariantAttributeCombinationService.AddAsync(
                new CreateProductVariantAttributeCombinationDto
                {
                    ProductId = productId,
                    StokCode = product.Barcode,
                    StockQuantity = product.StockQuantity,
                    Price = product.Price,
                    RawAttribute = rawAttributeJson,
                    AssignedMediaFileIds = string.Join(",", mediaFileMappingIds)
                });
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
