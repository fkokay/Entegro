using ClosedXML.Excel;
using Entegro.Application.DTOs.Common;
using Entegro.Application.DTOs.MediaFile;
using Entegro.Application.DTOs.Product;
using Entegro.Application.DTOs.ProductIntegration;
using Entegro.Application.Interfaces.Services.Base;
using Entegro.Web.Models.Import;
using MapsterMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using NPOI.HSSF.UserModel;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;
using System.Xml.Linq;
using System.Xml.XPath;

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
                var mappedJson = JsonSerializer.Serialize(mappedResult, options);

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
        public IActionResult Xml()
        {
            XmlImportProfileModel model = new XmlImportProfileModel();
            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> Xml(XmlImportProfileModel model)
        {
            if (string.IsNullOrWhiteSpace(model.MediaFileUrl))
                return View(CreateErrorModel("Lütfen bir XML URL girin."));

            try
            {
                var document = await ReadXml(model.MediaFileUrl);
                if (document.Root == null)
                    return View(CreateErrorModel("XML kök (root) bulunamadı.", model.MediaFileUrl));

                var startNode = FindStartNode(document, "Product");
                if (startNode == null)
                    return View(CreateErrorModel("XML içinde <Product> bulunamadı.", model.MediaFileUrl));

                model.PreviewXml = GeneratePreviewXml(startNode);
                model.Paths = GetDistinctPaths(startNode);

                return RedirectToAction("XmlMapping", model);
            }
            catch (Exception ex)
            {
                return View(CreateErrorModel("İndirme/parse hatası: " + ex.Message, model.MediaFileUrl));
            }
        }
        public IActionResult XmlMapping(XmlImportProfileModel model)
        {
            CreateXmlImportProfileModel createModel = new CreateXmlImportProfileModel()
            {
                MediaFileUrl = model.MediaFileUrl,
                ProfileName = model.ProfileName,
                MediaFileType = model.MediaFileType,
                Enable = model.Enable,
            };


            var variantIndices = model.Paths.SelectMany(path =>
            {
                var parts = path.Split("->", StringSplitOptions.RemoveEmptyEntries);
                return parts.Where(part => int.TryParse(part, out _));
            })
            .Distinct().Select(int.Parse).ToList();


            var variantCount = variantIndices.Count;
            int targetIndex = 0;
            var variantPaths = model.Paths.Where(path =>
            {
                var parts = path.Split("->", StringSplitOptions.RemoveEmptyEntries);
                return parts.Contains(targetIndex.ToString());
            }).ToList();

            ViewBag.VariantCount = variantCount;
            ViewBag.VariantPaths = variantPaths;
            ViewBag.XmlPaths = model.Paths;
            ViewBag.PreviewXml = model.PreviewXml;
            return View(createModel);
        }
        [HttpPost]
        public async Task<IActionResult> XmlMapping(CreateXmlImportProfileModel model, string SelectedImagePaths, string SelectedAttributeSpecifications, int VariantCount)
        {
            UpdateVariantMappedProperties(model, VariantCount);
            UpdateImagesAndSpecifications(model, SelectedImagePaths, SelectedAttributeSpecifications, VariantCount);

            var headerMaps = GetHeaderMappings(model);
            PrintHeaderMapsJson(headerMaps);

            var xmlDoc = await ReadXml(model.MediaFileUrl);
            Console.WriteLine("📄 Header Map JSON:");


            var createModel = new Application.DTOs.ImportProfile.CreateImportProfileDto
            {
                ProfileName = model.ProfileName,
                ApplyPriceAdjustment = model.CreateXmlProduct.ApplyPriceAdjustment,
                PriceAdjustmentType = model.CreateXmlProduct.PriceAdjustmentType,
                MediaFileType = "xml",
                OptionalExtraAmount = model.CreateXmlProduct.OptionalExtraAmount,
                PriceAdjustmentAmount = model.CreateXmlProduct.PriceAdjustmentAmount,
                MediaFileUrl = model.MediaFileUrl,
                Enable = true,
                ColumnMapping = System.Text.Json.JsonSerializer.Serialize(headerMaps, new System.Text.Json.JsonSerializerOptions
                {
                    WriteIndented = true
                })
            };

            var profile = await _importProfileService.AddAsync(createModel);
            var dataElements = xmlDoc.Root?.Elements();

            if (dataElements != null)
            {
                Console.WriteLine("XML İçeriği:");
                LogXmlDataElements(dataElements, headerMaps);

            }
            else
            {
                Console.WriteLine("XML'de veri bulunamadı.");
            }

            return RedirectToAction("List");
        }

        public async Task<IActionResult> ImportAllProductsFromXml(int profileId)
        {
            var setting = await _settingService.GetByKeyAsync("SystemApiUrl");
            if (setting == null)
            {
                throw new Exception($"Ayar Bulunamadı");
            }
            var response = await _client.PostAsync($"{setting.Value}/api/Job/run?profileId={profileId}", null);
            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new Exception($"Error: {error}");
            }
            var resultContent = await response.Content.ReadAsStringAsync();
            Console.WriteLine(resultContent);

            return View();
        }
        private async Task<XDocument> ReadXml(string url)
        {
            using var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/120 Safari/537.36");
            string xmlContent = await httpClient.GetStringAsync(url.Trim());
            return XDocument.Parse(xmlContent);
        }
        private static void TraverseChildrenOnly(XElement parent, string currentPath, List<string> output)
        {
            var groups = parent.Elements().GroupBy(e => e.Name.LocalName);
            foreach (var g in groups)
            {
                int count = g.Count();
                int i = 0;
                foreach (var child in g)
                {
                    var seg = count > 1 ? $"{g.Key}->{i}" : g.Key;
                    var next = string.IsNullOrEmpty(currentPath) ? seg : $"{currentPath}->{seg}";

                    output.Add(next);
                    TraverseChildrenOnly(child, next, output);
                    i++;
                }
            }
        }
        private void LogXmlDataElements(IEnumerable<XElement> dataElements, List<XmlColumnMappingResult> headerMaps)
        {
            foreach (var item in dataElements)
            {
                Console.WriteLine("Yeni Kayıt -----------------------");

                foreach (var map in headerMaps)
                {
                    bool foundAny = false;

                    var headers = map.XmlHeader.Split(',').Select(h => h.Trim()).ToList();

                    foreach (var header in headers)
                    {
                        var parts = header.Split("/");

                        var xpathParts = new List<string>();
                        foreach (var part in parts)
                        {
                            if (int.TryParse(part, out int index))
                            {
                                if (xpathParts.Count > 0)
                                {
                                    var last = xpathParts[xpathParts.Count - 1];
                                    xpathParts[xpathParts.Count - 1] = $"{last}[{index + 1}]";
                                }
                            }
                            else
                            {
                                xpathParts.Add(part);
                            }
                        }

                        var xpath = string.Join("/", xpathParts);
                        var element = item.XPathSelectElement(xpath);

                        if (element != null)
                        {
                            Console.WriteLine($"- {header}: {element.Value}");
                            foundAny = true;
                        }
                    }

                    Console.WriteLine("-----------------------------");
                }
            }
        }
        private void UpdateVariantMappedProperties(CreateXmlImportProfileModel model, int variantCount)
        {
            var variantMappedProps = new[]
            {
                nameof(model.CreateXmlProduct.AttributePrice),
                nameof(model.CreateXmlProduct.AttributeStockQuantity),
                nameof(model.CreateXmlProduct.AttributeStockCode),
                nameof(model.CreateXmlProduct.AttributeGtin),
                nameof(model.CreateXmlProduct.AttributeManufacturerPartNumber),
            };

            foreach (var propName in variantMappedProps)
            {
                var propInfo = model.CreateXmlProduct.GetType().GetProperty(propName);
                if (propInfo == null || propInfo.PropertyType != typeof(string)) continue;

                var originalValue = propInfo.GetValue(model.CreateXmlProduct) as string;

                if (!string.IsNullOrWhiteSpace(originalValue) && originalValue.Contains("variants->variant->0"))
                {
                    var updatedPaths = new List<string>();

                    for (int i = 0; i < variantCount; i++)
                    {
                        updatedPaths.Add(originalValue.Replace("variants->variant->0", $"variants->variant->{i}"));
                    }

                    var newValue = string.Join(",", updatedPaths.Distinct());
                    propInfo.SetValue(model.CreateXmlProduct, newValue);
                }
            }
        }
        private void UpdateImagesAndSpecifications(CreateXmlImportProfileModel model, string selectedImagePaths, string selectedAttributeSpecifications, int variantCount)
        {
            model.CreateXmlProduct.Images = selectedImagePaths;
            model.CreateXmlProduct.AttributeSpecifications = selectedAttributeSpecifications;

            if (!string.IsNullOrWhiteSpace(selectedAttributeSpecifications) && variantCount > 0)
            {
                var updatedSpecifications = new List<string>();

                var originalSpecs = selectedAttributeSpecifications
                    .Split(",", StringSplitOptions.RemoveEmptyEntries)
                    .Select(s => s.Trim())
                    .ToList();

                foreach (var specPath in originalSpecs)
                {
                    var parts = specPath.Split("->", StringSplitOptions.RemoveEmptyEntries);
                    var specIndexPart = parts.Last(); // Örn. "1"
                    var specPart = $"spec->{specIndexPart}";

                    for (int i = 0; i < variantCount; i++)
                    {
                        updatedSpecifications.Add($"variants->variant->{i}->{specPart}");
                    }
                }

                model.CreateXmlProduct.AttributeSpecifications = string.Join(",", updatedSpecifications.Distinct());
            }
        }
        private List<XmlColumnMappingResult> GetHeaderMappings(CreateXmlImportProfileModel model)
        {
            return model.CreateXmlProduct.GetType().GetProperties()
                .Where(p => p.PropertyType == typeof(string))
                .Select(p => new XmlColumnMappingResult
                {
                    MappedName = p.Name,
                    XmlHeader = p.GetValue(model.CreateXmlProduct)?.ToString().Replace("->", "/") ?? ""
                })
                .Where(h => !string.IsNullOrWhiteSpace(h.XmlHeader))
                .ToList();
        }
        private void PrintHeaderMapsJson(List<XmlColumnMappingResult> headerMaps)
        {
            var json = System.Text.Json.JsonSerializer.Serialize(headerMaps, new System.Text.Json.JsonSerializerOptions
            {
                WriteIndented = true
            });

            Console.WriteLine(json);
        }
        private XElement FindStartNode(XDocument document, string elementName)
        {
            return document
                .Descendants()
                .FirstOrDefault(x =>
                    x.NodeType == System.Xml.XmlNodeType.Element &&
                    string.Equals(x.Name.LocalName, elementName, StringComparison.OrdinalIgnoreCase));
        }
        private string GeneratePreviewXml(XElement startNode)
        {
            return startNode.ToString(System.Xml.Linq.SaveOptions.None);
        }
        private List<string> GetDistinctPaths(XElement startNode)
        {
            var paths = new List<string>();
            TraverseChildrenOnly(startNode, string.Empty, paths);

            return paths
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .OrderBy(p => p, StringComparer.OrdinalIgnoreCase)
                .ToList();
        }
        private XmlImportProfileModel CreateErrorModel(string errorMessage, string? mediaFileUrl = null)
        {
            return new XmlImportProfileModel
            {
                MediaFileUrl = mediaFileUrl,
                Error = errorMessage
            };
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