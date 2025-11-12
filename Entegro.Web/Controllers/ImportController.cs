using ClosedXML.Excel;
using Entegro.Application.DTOs.Common;
using Entegro.Application.DTOs.ImportProfile;
using Entegro.Application.DTOs.MediaFile;
using Entegro.Application.DTOs.Product;
using Entegro.Application.DTOs.ProductIntegration;
using Entegro.Application.Interfaces.Services.Base;
using Entegro.Web.Models.Import;
using MapsterMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using NPOI.HSSF.UserModel;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;
using System.Xml.Linq;

namespace Entegro.Web.Controllers
{
    public class ImportController : Controller
    {
        private readonly IMediaFileService _mediaFileService;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IImportProfileService _importProfileService;
        private readonly HttpClient _client;
        private readonly IProductService _productService;
        private readonly ICategoryService _categoryService;
        private readonly IBrandService _brandService;
        private readonly ISettingService _settingService;
        private readonly IMapper _mapper;
        private readonly IProductIntegrationService _productIntegrationService;
        private readonly IProductMediaFileMappingService _productMediaFileMappingService;

        public ImportController(IMediaFileService mediaFileService, IWebHostEnvironment webHostEnvironment, IImportProfileService importProfileService, HttpClient client, IProductService productService, ICategoryService categoryService, IBrandService brandService, ISettingService settingService, IMapper mapper, IProductIntegrationService productIntegrationService, IProductMediaFileMappingService productMediaFileMappingService)
        {
            _mediaFileService = mediaFileService;
            _webHostEnvironment = webHostEnvironment;
            _importProfileService = importProfileService;
            _client = client;
            _productService = productService;
            _categoryService = categoryService;
            _brandService = brandService;
            _settingService = settingService;
            _mapper = mapper;
            _productIntegrationService = productIntegrationService;
            _productMediaFileMappingService = productMediaFileMappingService;
        }

        [HttpPost]
        public async Task<IActionResult> ImportProfileList([FromBody] GridCommand gridCommand)
        {

            var result = await _importProfileService.GetPagedAsync(gridCommand);

            return Json(new
            {
                draw = gridCommand.Draw,
                recordsTotal = result.TotalCount,
                recordsFiltered = result.TotalCount,
                data = result.Items
            });

        }
        public IActionResult Index()
        {
            return List();
        }

        public IActionResult List()
        {
            return View();
        }
        #region Excel Import
        [HttpGet]
        public IActionResult Excel()
        {
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> MapColumns(int mediaFileId)
        {
            MediaFileDto? mediaFile = await _mediaFileService.GetByIdAsync(mediaFileId);
            if (mediaFile is not null)
            {
                string fileUrl = mediaFile.Url;
                string webRootPath = _webHostEnvironment.WebRootPath;
                string filePathFromUrl = Path.Combine(webRootPath, fileUrl.TrimStart('/'));
                string folderName = mediaFile.Folder == null ? "" : mediaFile.Folder.Name;
                string uploadedFileName = mediaFile.Name;
                string appDataPath = Path.Combine(
                     Directory.GetCurrentDirectory(),
                     "App_Data",
                     "Media",
                     "Storage",
                     folderName,
                     uploadedFileName
                 );
                string finalPath = System.IO.File.Exists(filePathFromUrl) ? filePathFromUrl : appDataPath;
                string windowsFilePath = finalPath.Replace('/', '\\');
                TempData["UploadedFilePath"] = windowsFilePath;
                if (System.IO.File.Exists(windowsFilePath))
                {
                    var detail = new ExcelImportProfileModel();

                    using var stream = new FileStream(windowsFilePath, FileMode.Open, FileAccess.Read);
                    using var workbook = new XLWorkbook(stream);
                    var worksheet = workbook.Worksheet(1);

                    var headers = new List<ExcelColumnMapping>();
                    detail.MediaFileId = mediaFileId;
                    detail.ProfileName = mediaFile.Name;
                    detail.Enable = true;
                    detail.ColumnMappings = headers;
                    foreach (var cell in worksheet.Row(1).CellsUsed())
                    {
                        var columnValues = new List<string>();
                        int columnNumber = cell.Address.ColumnNumber;

                        foreach (var row in worksheet.RowsUsed().Skip(1))
                        {
                            columnValues.Add(row.Cell(columnNumber).Value.ToString());
                        }

                        headers.Add(new ExcelColumnMapping
                        {
                            ExcelHeader = cell.Value.ToString()
                        });
                    }

                    ViewBag.DbColumns = new List<SelectListItem>
                    {
                        new SelectListItem { Text = "Ürün Kodu", Value = "Code" },
                        new SelectListItem { Text = "Ürün Adı", Value = "Name" },
                        new SelectListItem { Text = "Açıklama", Value = "Description" },
                        new SelectListItem { Text = "Üretici Parça Numarası", Value = "ManufacturerPartNumber" },
                        new SelectListItem { Text = "GTIN", Value = "Gtin" },
                        new SelectListItem { Text = "Fiyat", Value = "Price" },
                        new SelectListItem { Text = "Eski Fiyat", Value = "OldPrice" },
                        new SelectListItem { Text = "İndirimli Fiyat", Value = "SalePrice" },
                        new SelectListItem { Text = "KDV Oranı", Value = "VatRate" },
                        new SelectListItem { Text = "Stok Miktarı", Value = "StockQuantity" },
                        new SelectListItem { Text = "Ağırlık", Value = "Weight" },
                        new SelectListItem { Text = "Uzunluk", Value = "Length" },
                        new SelectListItem { Text = "Genişlik", Value = "Width" },
                        new SelectListItem { Text = "Yükseklik", Value = "Height" },
                        new SelectListItem { Text = "Meta Anahtar Kelimeler", Value = "MetaKeywords" },
                        new SelectListItem { Text = "Meta Açıklama", Value = "MetaDescription" },
                        new SelectListItem { Text = "Meta Başlık", Value = "MetaTitle" },
                        new SelectListItem { Text = "Barkod", Value = "Barcode" },
                        new SelectListItem { Text = "Maliyet Fiyatı", Value = "CostPrice" },
                        new SelectListItem { Text = "Resimler", Value = "Images" }
                    };
                    return View(detail);
                }
            }
            return View(new List<ExcelColumnMapping>());
        }



        [HttpPost]
        public async Task<IActionResult> ImportData(ExcelImportProfileModel detail)
        {
            var filePath = TempData["UploadedFilePath"] as string;
            if (string.IsNullOrEmpty(filePath) || !System.IO.File.Exists(filePath))
            {
                TempData["Error"] = "Dosya bulunamadı, tekrar yükleyin.";
                return RedirectToAction("Index");
            }

            ViewBag.ProfileName = detail.ProfileName;
            var selectedColumns = detail.ColumnMappings
                .Where(x => x.IsImport && !string.IsNullOrEmpty(x.DbColumn))
                .ToList();

            if (!selectedColumns.Any())
            {
                TempData["Error"] = "En az bir kolon seçmelisiniz.";
                return RedirectToAction("Index");
            }

            try
            {
                var mappedResult = selectedColumns.Select(col => new ExcelColumnMappingResult
                {
                    ExcelHeader = col.ExcelHeader,
                    MappedName = col.DbColumn,
                    DefaultValue = col.DefaultValue ?? ""
                }).ToList();


                var options = new JsonSerializerOptions
                {
                    WriteIndented = true,
                    Encoder = JavaScriptEncoder.Create(UnicodeRanges.All)
                };
                var mappedJson = System.Text.Json.JsonSerializer.Serialize(mappedResult, options);

                ViewBag.MappedJson = mappedJson;

                var createDto = new Application.DTOs.ImportProfile.CreateImportProfileDto
                {
                    ProfileName = detail.ProfileName,
                    Enable = detail.Enable,
                    MediaFileId = detail.MediaFileId,
                    MediaFileType = "excel",
                    ColumnMapping = mappedJson,
                };
                await _importProfileService.AddAsync(createDto);

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Hata oluştu: {ex.Message}");
                TempData["Error"] = "Excel verisi işlenirken hata oluştu.";
                return RedirectToAction("Index");
            }

            return View("ImportResult", selectedColumns);
        }




        #endregion

        #region XML Import

        public async Task<IActionResult> Xml(int profileId = 0)
        {

            var importProfile = new XmlImportProfileModel();
            if (profileId == 0)
            {
                return View(new XmlImportProfileModel());
            }

            var profileDto = await _importProfileService.GetByIdAsync(profileId);
            if (profileDto != null)
            {
                importProfile = _mapper.Map<XmlImportProfileModel>(profileDto);

            }
            return View(importProfile);
        }


        [HttpPost]
        public async Task<IActionResult> Analyze(XmlImportProfileModel model)
        {
            if (string.IsNullOrWhiteSpace(model.MediaFileUrl))
                return View(CreateErrorModel("Lütfen bir XML URL girin."));

            try
            {
                var (xmlContent, profileId) = await DownloadAndSaveXmlAsync(model);
                var structure = AnalyzeXml(xmlContent);

                structure.Tags = structure.Tags
                    .Where(t => !string.Equals(t, structure.RootName, StringComparison.OrdinalIgnoreCase)
                             && !new[] { "Product", "spec", "variant", "variants" }
                                 .Contains(t, StringComparer.OrdinalIgnoreCase))
                    .ToList();

                var xDoc = XDocument.Parse(xmlContent);


                ImportProfileDto importProfile = null;
                if (model.Id > 0)
                {
                    importProfile = await _importProfileService.GetByIdAsync(model.Id);
                }


                if (importProfile != null && !string.IsNullOrEmpty(importProfile.ColumnMapping))
                {
                    try
                    {
                        var mappingData = JsonConvert.DeserializeObject<MappingSaveRequest>(importProfile.ColumnMapping);

                        ViewBag.SavedMappings = mappingData?.Mappings ?? new List<XmlMapping>();
                        ViewBag.IncludeVariants = mappingData?.IncludeVariants ?? false;
                        ViewBag.SelectedProducts = mappingData?.SelectedProducts ?? new List<string>();
                        ViewBag.XmlUrl = mappingData?.XmlUrl ?? importProfile.MediaFileUrl;
                    }
                    catch (Exception ex)
                    {
                        return View(CreateErrorModel($"Mapping JSON parse hatası (ProfileId: {{ProfileId}}) {ex.Message}", importProfile?.Id.ToString()));

                    }
                }


                ViewBag.HasVariants = xDoc.Descendants("variant").Any();
                ViewBag.ImageTags = GetImageTags(xDoc);
                ViewBag.DbColumnDisplayNames = DbColumnDisplayNames;
                ViewBag.XmlTags = structure.Tags;
                ViewBag.ProfileId = importProfile?.Id ?? profileId;
                ViewBag.XmlUrl ??= model.MediaFileUrl;

                return View(structure);
            }
            catch (Exception ex)
            {
                return View(CreateErrorModel($"İndirme/parse hatası: {ex.Message}", model.MediaFileUrl));
            }
        }


        [HttpGet]
        public async Task<IActionResult> GetFirstVariantPreview(string xmlUrl)
        {
            if (string.IsNullOrWhiteSpace(xmlUrl))
                return Content("<em>XML URL belirtilmedi.</em>", "text/html");

            try
            {
                var xmlContent = await DownloadXmlAsync(xmlUrl);
                var doc = XDocument.Parse(xmlContent);

                var variants = doc.Descendants("Product")
                                  .FirstOrDefault()?
                                  .Descendants("variant")
                                  .ToList();

                if (variants == null || variants.Count == 0)
                    return Content("<em>Bu üründe varyant yok</em>", "text/html");

                var html = string.Join("<hr/>", variants.Select(v =>
                    "<li>" + string.Join("<br/>", v.Elements()
                        .Select(el => $"<strong>{el.Name.LocalName}:</strong> {System.Net.WebUtility.HtmlEncode(el.Value)}")) + "</li>"));

                return Content($"<ul>{html}</ul>", "text/html");
            }
            catch (Exception ex)
            {
                return Content($"<em>Hata oluştu: {System.Net.WebUtility.HtmlEncode(ex.Message)}</em>", "text/html");
            }
        }

        [HttpPost]
        public async Task<IActionResult> SaveMapping([FromBody] MappingSaveRequest request)
        {
            if (request?.Mappings == null || request.Mappings.Count == 0)
                return Json(new { success = false, message = "Mapping listesi boş geldi." });

            if (request.ProfileId <= 0)
                return Json(new { success = false, message = "Geçersiz ProfileId gönderildi." });

            try
            {
                var importProfile = await _importProfileService.GetByIdAsync(request.ProfileId);
                if (importProfile == null)
                    return Json(new { success = false, message = "İlgili import profili bulunamadı." });

                var updateDto = _mapper.Map<UpdateImportProfileDto>(importProfile);
                updateDto.ColumnMapping = JsonConvert.SerializeObject(new
                {
                    request.XmlUrl,
                    request.Mappings,
                    request.SelectedProducts,
                    request.IncludeVariants
                }, Formatting.Indented);

                await _importProfileService.UpdateAsync(updateDto);
                return Json(new { success = true, message = "Eşleştirme JSON olarak kaydedildi." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Hata oluştu: {ex.Message}" });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetProductList(string xmlUrl)
        {
            if (string.IsNullOrWhiteSpace(xmlUrl))
                return Json(new { success = false, message = "XML URL bulunamadı." });

            try
            {
                var xmlContent = await DownloadXmlAsync(xmlUrl);
                var doc = XDocument.Parse(xmlContent);

                var products = doc.Descendants("Product")
                    .Select(p => new
                    {
                        Code = (p.Element("Product_code")?.Value ??
                                p.Element("ProductCode")?.Value ??
                                p.Element("productCode")?.Value ??
                                p.Element("Code")?.Value ?? "").Trim(),
                        Name = (p.Element("Name")?.Value ??
                                p.Element("ProductName")?.Value ??
                                p.Element("name")?.Value ?? "(İsimsiz Ürün)").Trim()
                    })
                    .Where(p => !string.IsNullOrEmpty(p.Code))
                    .Distinct()
                    .ToList();

                return products.Any()
                    ? Json(new { success = true, data = products })
                    : Json(new { success = false, message = "XML'de ürün bulunamadı." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"XML okunamadı: {ex.Message}" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> DeleteProfile(int id)
        {
            try
            {
                await _importProfileService.DeleteAsync(id);
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        #region Private Helpers

        private async Task<(string XmlContent, int ProfileId)> DownloadAndSaveXmlAsync(XmlImportProfileModel model)
        {
            if (model.Id > 0)
            {
                var importProfile = await _importProfileService.GetByIdAsync(model.Id);
                if (importProfile != null)
                {
                    var map = _mapper.Map<UpdateImportProfileDto>(importProfile);
                    var updated = await _importProfileService.UpdateAsync(map);
                    return (await DownloadXmlAsync(model.MediaFileUrl), updated.Id);
                }
            }
            var createModel = new CreateXmlImportProfileModel
            {
                MediaFileUrl = model.MediaFileUrl,
                ProfileName = model.ProfileName,
                MediaFileType = "xml",
                Enable = false,
            };

            var dto = _mapper.Map<CreateImportProfileDto>(createModel);
            var profile = await _importProfileService.AddAsync(dto);
            return (await DownloadXmlAsync(model.MediaFileUrl), profile.Id);
        }

        private static async Task<string> DownloadXmlAsync(string url)
        {
            using var client = new HttpClient();
            client.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0");
            return await client.GetStringAsync(url);
        }

        private static XmlStructure AnalyzeXml(string xmlContent)
        {
            var doc = XDocument.Parse(xmlContent);
            return new XmlStructure
            {
                RootName = doc.Root?.Name.LocalName ?? "Unknown",
                Tags = doc.Descendants().Select(x => x.Name.LocalName).Distinct().OrderBy(x => x).ToList()
            };
        }

        private static List<string> GetImageTags(XDocument doc) =>
            doc.Descendants()
               .Where(x => x.Name.LocalName.StartsWith("Image", StringComparison.OrdinalIgnoreCase))
               .Select(x => x.Name.LocalName)
               .Distinct()
               .ToList();

        private static readonly Dictionary<string, string> DbColumnDisplayNames = new()
        {
            { "Code", "Ürün Kodu" },
            { "Name", "Ürün Adı" },
            { "Category", "Kategori" },
            { "Description", "Açıklama" },
            { "ManufacturerPartNumber", "Üretici Parça No" },
            { "Gtin", "GTIN" },
            { "Price", "Fiyat" },
            { "OldPrice", "Eski Fiyat" },
            { "SalePrice", "Satış Fiyatı" },
            { "Currency", "Para Birimi" },
            { "Unit", "Birim" },
            { "VatRate", "KDV Oranı" },
            { "VatInc", "KDV Dahil" },
            { "Brand", "Marka" },
            { "StockQuantity", "Stok Miktarı" },
            { "Weight", "Ağırlık" },
            { "Length", "Uzunluk" },
            { "Width", "Genişlik" },
            { "Height", "Yükseklik" },
            { "MetaKeywords", "Meta Anahtar Kelimeler" },
            { "MetaDescription", "Meta Açıklama" },
            { "MetaTitle", "Meta Başlık" },
            { "Barcode", "Barkod" },
            { "Images", "Görseller" },
            { "IntegrationSku", "Entegrasyon SKU" },
            { "ShortDescription", "Kısa Açıklama" },
            { "MinStockQuantity", "Min. Stok Miktarı" },
            { "CostPrice", "Maliyet Fiyatı" }
        };


        private static XmlImportProfileModel CreateErrorModel(string error, string? url = null) =>
            new() { MediaFileUrl = url, Error = error };

        #endregion

        #endregion


        #region BulkUpdatePrices
        public async Task<IActionResult> BulkUpdatePrices(int page = 1, int brandId = -1)
        {
            int pageSize = 20;
            int totalCount = 0;
            List<ProductIntegrationViewModel> viewModel = null;

            var brands = await _brandService.GetAllBrandsAsync();

            ViewBag.Brands = brands.Select(m => new SelectListItem()
            {
                Text = m.Name,
                Value = m.Id.ToString()
            }).ToList();

            if (brandId == -1)
            {
                ViewBag.CurrentPage = page;
                ViewBag.TotalPages = 0;
                ViewBag.SelectedBrandId = brandId;
                return View(null);
            }


            var integrations = await _productService.GetProductIntegrationMatrixAsync(page, pageSize, brandId);

            viewModel = integrations.Select(p => new ProductIntegrationViewModel
            {
                ProductId = p.Id,
                ProductName = p.Name,
                ProductBrand = p.Brand?.Name ?? "Marka Yok",
                ProductCode = p.Code ?? "Kod Yok",
                ProductCategory = p.ProductCategories != null && p.ProductCategories.Any() ? string.Join(", ", p.ProductCategories.Select(pc => pc.Category != null ? pc.Category.Name : "Kategori Yok")) : "Kategori Yok",
                Price = p.Price,
                SalePrice = p.SalePrice,
                CostPrice = p.CostPrice,
                IntegrationPrices = p.ProductIntegrations.ToDictionary(
                    pi =>
                        pi.IntegrationSystem.IntegrationSystemParameters
                            .FirstOrDefault(x => x.Key == "MarketplaceType")?.Value +
                        pi.IntegrationSystem.IntegrationSystemParameters
                            .FirstOrDefault(x => x.Key == "CommerceType")?.Value +
                        " > " +
                        pi.IntegrationSystem.Name,
                    pi => new IntegrationPriceInfo
                    {
                        IntegrationSystemId = pi.IntegrationSystem.Id,
                        Price = pi.Price
                    }
                )
            }).ToList();

            if (brandId > 0)
                totalCount = integrations.Count();
            else
                totalCount = await _productService.GetProductCountAsync();

            var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;
            ViewBag.SelectedBrandId = brandId;

            return View(viewModel);
        }
        [HttpPost]
        public async Task<IActionResult> SaveIntegrations(List<ProductIntegrationViewModel> model)
        {
            var changedProducts = model.Where(m => m.IsChanged).ToList();

            foreach (var product in changedProducts)
            {
                var productDto = await _productService.GetProductByIdAsync(product.ProductId);
                if (productDto == null) continue;

                var mapped = _mapper.Map<UpdateProductDto>(productDto);
                mapped.Price = product.Price;
                mapped.SalePrice = product.SalePrice;
                await _productService.UpdateAsync(mapped);

                foreach (var integrationEntry in product.IntegrationPrices.Values)
                {
                    int integrationSystemId = integrationEntry.IntegrationSystemId;
                    decimal? newPrice = integrationEntry.Price;
                    if (!newPrice.HasValue) continue;

                    var integration = await _productIntegrationService
                        .GetByProductAndIntegrationSystemAsync(product.ProductId, integrationSystemId);

                    if (integration != null)
                    {
                        var mappedIntegration = _mapper.Map<UpdateProductIntegrationDto>(integration);
                        mappedIntegration.ProductId = product.ProductId;
                        mappedIntegration.Price = newPrice.Value;
                        await _productIntegrationService.UpdateAsync(mappedIntegration);
                    }
                }
            }

            return Json(new { success = true });
        }
        [HttpGet]
        public async Task<IActionResult> ExcelExport(int brandId)
        {
            var integrations = await _productService.GetProductIntegrationMatrixAsync(brandId);

            List<ProductIntegrationViewModel> viewModel = integrations.Select(p => new ProductIntegrationViewModel
            {
                ProductId = p.Id,
                ProductName = p.Name,
                ProductBrand = p.Brand?.Name ?? "Marka Yok",
                ProductCode = p.Code ?? "Kod Yok",
                ProductCategory = p.ProductCategories != null && p.ProductCategories.Any()
                    ? string.Join(", ", p.ProductCategories.Select(pc => pc.Category != null ? pc.Category.Name : "Kategori Yok"))
                    : "Kategori Yok",
                Price = p.Price,
                SalePrice = p.SalePrice,
                CostPrice = p.CostPrice,
                IntegrationPrices = p.ProductIntegrations.ToDictionary(
                    pi =>
                        (pi.IntegrationSystem.IntegrationSystemParameters
                            .FirstOrDefault(x => x.Key == "MarketplaceType")?.Value ?? "") +
                        (pi.IntegrationSystem.IntegrationSystemParameters
                            .FirstOrDefault(x => x.Key == "CommerceType")?.Value ?? "") +
                        " > " +
                        (pi.IntegrationSystem.Name ?? ""),
                    pi => new IntegrationPriceInfo
                    {
                        IntegrationSystemId = pi.IntegrationSystem.Id,
                        Price = pi.Price
                    }
                )
            }).ToList();

            var workbook = new HSSFWorkbook();
            var sheet = workbook.CreateSheet("Ürün Listesi");


            var headerRow = sheet.CreateRow(0);
            int col = 0;
            headerRow.CreateCell(col++).SetCellValue("Id");
            headerRow.CreateCell(col++).SetCellValue("Ürün");
            headerRow.CreateCell(col++).SetCellValue("Kodu");
            headerRow.CreateCell(col++).SetCellValue("Marka");
            headerRow.CreateCell(col++).SetCellValue("Kategori");
            headerRow.CreateCell(col++).SetCellValue("Maliyet Fiyatı");
            headerRow.CreateCell(col++).SetCellValue("Liste Fiyatı");
            headerRow.CreateCell(col++).SetCellValue("Satış Fiyatı");


            var integrationKeyMap = viewModel
                .SelectMany(v => v.IntegrationPrices)
                .GroupBy(x => x.Key)
                .Select(g => new
                {
                    Key = g.Key,
                    IntegrationSystemId = g.First().Value.IntegrationSystemId
                })
                .ToList();


            foreach (var item in integrationKeyMap)
            {
                headerRow.CreateCell(col++).SetCellValue($"{item.Key}_Fiyat_IntegrationSystemId_{item.IntegrationSystemId}");
            }

            int rowNumber = 1;
            foreach (var item in viewModel)
            {
                var row = sheet.CreateRow(rowNumber++);
                col = 0;

                row.CreateCell(col++).SetCellValue(item.ProductId);
                row.CreateCell(col++).SetCellValue(item.ProductName);
                row.CreateCell(col++).SetCellValue(item.ProductCode);
                row.CreateCell(col++).SetCellValue(item.ProductBrand);
                row.CreateCell(col++).SetCellValue(item.ProductCategory);
                row.CreateCell(col++).SetCellValue(Convert.ToDouble(item.CostPrice));
                row.CreateCell(col++).SetCellValue(Convert.ToDouble(item.Price));
                row.CreateCell(col++).SetCellValue(Convert.ToDouble(item.SalePrice));

                foreach (var integration in integrationKeyMap)
                {
                    if (item.IntegrationPrices.ContainsKey(integration.Key))
                    {
                        // sadece fiyat yazılacak
                        row.CreateCell(col++).SetCellValue(Convert.ToDouble(item.IntegrationPrices[integration.Key].Price));
                    }
                    else
                    {
                        row.CreateCell(col++).SetCellValue(0);
                    }
                }
            }

            // Freeze header
            sheet.CreateFreezePane(0, 1, 0, 1);

            using var exportData = new MemoryStream();
            workbook.Write(exportData);

            return File(exportData.ToArray(), "application/vnd.ms-excel", "urunler.xls");
        }
        [HttpPost]
        public async Task<IActionResult> ExcelImport([FromForm] ExcelImportRequest request)
        {
            if (request.File == null || request.File.Length == 0)
                return BadRequest("Dosya seçilmedi.");

            var ext = Path.GetExtension(request.File.FileName).ToLower();
            if (ext != ".xls")
                return BadRequest("Lütfen yalnızca .xls dosyası yükleyin.");

            List<ProductIntegrationViewModel> importedData = new List<ProductIntegrationViewModel>();

            using (var stream = request.File.OpenReadStream())
            {
                var workbook = new HSSFWorkbook(stream);
                var sheet = workbook.GetSheetAt(0);

                for (int rowIndex = 1; rowIndex <= sheet.LastRowNum; rowIndex++)
                {
                    var row = sheet.GetRow(rowIndex);
                    if (row == null) continue;

                    try
                    {
                        int col = 0;
                        var model = new ProductIntegrationViewModel();

                        model.ProductId = (int)row.GetCell(col++).NumericCellValue;
                        model.ProductName = row.GetCell(col++)?.ToString();
                        model.ProductCode = row.GetCell(col++)?.ToString();
                        model.ProductBrand = row.GetCell(col++)?.ToString();
                        model.ProductCategory = row.GetCell(col++)?.ToString();
                        model.CostPrice = row.GetCell(col) != null ? Convert.ToDecimal(row.GetCell(col++).NumericCellValue) : 0;
                        model.Price = row.GetCell(col) != null ? Convert.ToDecimal(row.GetCell(col++).NumericCellValue) : 0;
                        model.SalePrice = row.GetCell(col) != null ? Convert.ToDecimal(row.GetCell(col++).NumericCellValue) : 0;

                        // Integration sütunlarını oku
                        model.IntegrationPrices = new Dictionary<string, IntegrationPriceInfo>();

                        while (col < row.LastCellNum)
                        {
                            string integrationHeader = sheet.GetRow(0).GetCell(col).ToString(); // başlıktan al
                            double integrationPrice = row.GetCell(col)?.NumericCellValue ?? 0;
                            col++;

                            int integrationId = 0;
                            var match = System.Text.RegularExpressions.Regex.Match(integrationHeader, @"IntegrationSystemId_(\d+)$");
                            if (match.Success)
                                integrationId = int.Parse(match.Groups[1].Value);

                            string key = integrationHeader;

                            if (integrationId > 0)
                            {
                                model.IntegrationPrices[key] = new IntegrationPriceInfo
                                {
                                    IntegrationSystemId = integrationId,
                                    Price = Convert.ToDecimal(integrationPrice)
                                };
                            }
                        }

                        importedData.Add(model);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Row {rowIndex} okunamadı: {ex.Message}");
                    }
                }
            }
            int notUpdatedProductCount = 0;
            foreach (var item in importedData)
            {
                var product = await _productService.GetProductByIdAsync(item.ProductId);
                if (product != null)
                {
                    product.Price = item.Price;
                    product.SalePrice = item.SalePrice;
                    product.CostPrice = item.CostPrice;

                    var mapped = _mapper.Map<UpdateProductDto>(product);
                    mapped.Price = item.Price;
                    mapped.SalePrice = item.SalePrice;
                    mapped.CostPrice = item.CostPrice;
                    await _productService.UpdateAsync(mapped);

                    foreach (var kvp in item.IntegrationPrices)
                    {
                        var integration = await _productIntegrationService.GetByProductAndIntegrationSystemAsync(item.ProductId, kvp.Value.IntegrationSystemId);

                        if (integration != null)
                        {
                            integration.Price = kvp.Value.Price ?? 0;
                            var mappedIntegration = _mapper.Map<UpdateProductIntegrationDto>(integration);
                            mappedIntegration.IntegrationSystemId = kvp.Value.IntegrationSystemId;
                            mappedIntegration.Price = kvp.Value.Price.Value;
                            await _productIntegrationService.UpdateAsync(mappedIntegration);
                        }
                        else
                        {
                            Console.WriteLine($"{product.Id} - {product.Name} ürünün entegrasyonu bulunmuyor.");
                        }
                    }
                }
                else
                {
                    notUpdatedProductCount++;
                }
            }

            return Json(new
            {
                success = true,
                count = importedData.Count,
                notUpdatedProductCount = notUpdatedProductCount
            });

        }

        #endregion

    }
}