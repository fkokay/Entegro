using Entegro.Api.Models;
using Entegro.Application.DTOs.ImportProfile;
using Entegro.Application.DTOs.Product;
using Entegro.Application.DTOs.ProductMediaFile;
using Entegro.Application.Interfaces.Services.Base;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using Quartz;
using System.Text.Json;

namespace Entegro.Api.Jobs
{
    [DisallowConcurrentExecution]
    public class ImportJob : IJob
    {
        private readonly IImportProfileService _importProfileService;
        private readonly IMediaFileService _mediaFileService;
        private readonly ISettingService _settingService;
        private readonly IProductService _productService;
        private readonly ILogger<ImportJob> _logger;
        private readonly IProductMediaFileMappingService _productMediaFileMappingService;
        public ImportJob(IImportProfileService importProfileService, IMediaFileService mediaFileService, ILogger<ImportJob> logger, ISettingService settingService, IProductService productService, IProductMediaFileMappingService productMediaFileMappingService)
        {
            _importProfileService = importProfileService;
            _mediaFileService = mediaFileService;
            _settingService = settingService;
            _productService = productService;
            _logger = logger;
            _productMediaFileMappingService = productMediaFileMappingService;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            _logger.LogInformation("İçe aktarım servisi başladı.");
            var profiles = await _importProfileService.GetAllAsync();

            foreach (var profile in profiles)
            {
                if (profile.MediaFileType == "xml")
                {

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
            throw new NotImplementedException();
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
