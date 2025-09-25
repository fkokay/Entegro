using ClosedXML.Excel;
using Entegro.Application.DTOs.Common;
using Entegro.Application.DTOs.MediaFile;
using Entegro.Application.Interfaces.Services;
using Entegro.Web.Models.Import;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
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
        public ImportController(IMediaFileService mediaFileService, IWebHostEnvironment webHostEnvironment, IImportProfileService importProfileService, HttpClient client, IProductService productService, ICategoryService categoryService, IBrandService brandService, ISettingService settingService)
        {
            _mediaFileService = mediaFileService;
            _webHostEnvironment = webHostEnvironment;
            _importProfileService = importProfileService;
            _client = client;
            _productService = productService;
            _categoryService = categoryService;
            _brandService = brandService;
            _settingService = settingService;
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
                        headers.Add(new ExcelColumnMapping
                        {
                            ExcelHeader = cell.Value.ToString()
                        });
                    }

                    ViewBag.DbColumns = new List<SelectListItem>
                    {
                        new SelectListItem { Text = "Ürün Adı", Value = "Name" },
                        new SelectListItem { Text = "Stok", Value = "Stock" },
                        new SelectListItem { Text = "Barkod", Value = "Barcode" },
                        new SelectListItem { Text = "Stok Miktarı", Value = "StockQuantity" },
                        new SelectListItem { Text = "Durum", Value = "Status" }
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
                using var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
                using var workbook = new XLWorkbook(stream);
                var worksheet = workbook.Worksheet(1);

                foreach (var col in selectedColumns)
                {
                    int colIndex = worksheet.Row(1).CellsUsed()
                        .FirstOrDefault(c => c.Value.ToString() == col.ExcelHeader)?.Address.ColumnNumber ?? -1;

                    if (colIndex > 0)
                    {
                        foreach (var row in worksheet.RowsUsed().Skip(1)) // Başlığı atla
                        {
                            col.Values.Add(row.Cell(colIndex).Value.ToString());
                        }
                    }
                }

                var mappedResult = selectedColumns.Select(col => new ExcelColumnMappingResult
                {
                    ExcelHeader = col.ExcelHeader,
                    MappedName = col.DbColumn,
                    DefaultValue = col.DefaultValue == null ? "" : col.DefaultValue
                }).ToList();

                var mappedJson = System.Text.Json.JsonSerializer.Serialize(mappedResult, new System.Text.Json.JsonSerializerOptions
                {
                    WriteIndented = true
                });

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
            }


            return View("ImportResult", selectedColumns);
        }
        #endregion

        #region XML Import
        public async Task<IActionResult> Xml(int? profileId)
        {
            XmlImportProfileModel model = new XmlImportProfileModel();
            model.Id = 0;

            if (profileId != null && profileId > 0)
            {
                var profileModel = await _importProfileService.GetByIdAsync(profileId.Value);
                model.Id = profileModel.Id;
                model.ProfileName = profileModel.ProfileName;
                model.Enable = profileModel.Enable;
                model.MediaFileType = profileModel.MediaFileType;
                model.MediaFileUrl = profileModel.MediaFileUrl;
                model.ApplyPriceAdjustment = profileModel.ApplyPriceAdjustment;
                model.OptionalExtraAmount = profileModel.OptionalExtraAmount;
                model.PriceAdjustmentType = profileModel.PriceAdjustmentType;
                model.PriceAdjustmentAmount = profileModel.PriceAdjustmentAmount;

                var document = await ReadXml(model.MediaFileUrl);
                if (document.Root == null)
                    return View(CreateErrorModel("XML kök (root) bulunamadı.", model.MediaFileUrl));

                var startNode = FindStartNode(document, "Product");
                if (startNode == null)
                    return View(CreateErrorModel("XML içinde <Product> bulunamadı.", model.MediaFileUrl));

                model.PreviewXml = GeneratePreviewXml(startNode);
                model.Paths = GetDistinctPaths(startNode);

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
                return View(model);
            }

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

                return RedirectToAction("Xml", model);
            }
            catch (Exception ex)
            {
                return View(CreateErrorModel("İndirme/parse hatası: " + ex.Message, model.MediaFileUrl));
            }
        }
        //public IActionResult XmlMapping(XmlImportProfileModel model)
        //{
        //    CreateXmlImportProfileModel createModel = new CreateXmlImportProfileModel()
        //    {
        //        MediaFileUrl = model.MediaFileUrl,
        //        ProfileName = model.ProfileName,
        //        MediaFileType = model.MediaFileType,
        //        Enable = model.Enable,
        //    };


        //    var variantIndices = model.Paths.SelectMany(path =>
        //    {
        //        var parts = path.Split("->", StringSplitOptions.RemoveEmptyEntries);
        //        return parts.Where(part => int.TryParse(part, out _));
        //    })
        //    .Distinct().Select(int.Parse).ToList();


        //    var variantCount = variantIndices.Count;
        //    int targetIndex = 0;
        //    var variantPaths = model.Paths.Where(path =>
        //    {
        //        var parts = path.Split("->", StringSplitOptions.RemoveEmptyEntries);
        //        return parts.Contains(targetIndex.ToString());
        //    }).ToList();

        //    ViewBag.VariantCount = variantCount;
        //    ViewBag.VariantPaths = variantPaths;
        //    ViewBag.XmlPaths = model.Paths;
        //    ViewBag.PreviewXml = model.PreviewXml;
        //    return View(createModel);
        //}

        //[HttpPost]
        //public async Task<IActionResult> XmlMapping(CreateXmlImportProfileModel model, string SelectedImagePaths, string SelectedAttributeSpecifications, int VariantCount)
        //{
        //    UpdateVariantMappedProperties(model, VariantCount);
        //    UpdateImagesAndSpecifications(model, SelectedImagePaths, SelectedAttributeSpecifications, VariantCount);

        //    var headerMaps = GetHeaderMappings(model);
        //    PrintHeaderMapsJson(headerMaps);

        //    var xmlDoc = await ReadXml(model.MediaFileUrl);
        //    Console.WriteLine("📄 Header Map JSON:");


        //    var createModel = new Application.DTOs.ImportProfile.CreateImportProfileDto
        //    {
        //        ProfileName = model.ProfileName,
        //        ApplyPriceAdjustment = model.CreateXmlProduct.ApplyPriceAdjustment,
        //        PriceAdjustmentType = model.CreateXmlProduct.PriceAdjustmentType,
        //        MediaFileType = "xml",
        //        OptionalExtraAmount = model.CreateXmlProduct.OptionalExtraAmount,
        //        PriceAdjustmentAmount = model.CreateXmlProduct.PriceAdjustmentAmount,
        //        MediaFileUrl = model.MediaFileUrl,
        //        Enable = true,
        //        ColumnMapping = System.Text.Json.JsonSerializer.Serialize(headerMaps, new System.Text.Json.JsonSerializerOptions
        //        {
        //            WriteIndented = true
        //        })
        //    };

        //    var profile = await _importProfileService.AddAsync(createModel);


        //    return RedirectToAction("List");
        //}

        //public async Task<IActionResult> ImportAllProductsFromXml(int profileId)
        //{
        //    var setting = await _settingService.GetByKeyAsync("SystemApiUrl");
        //    if (setting == null)
        //    {
        //        throw new Exception($"Ayar Bulunamadı");
        //    }
        //    var response = await _client.PostAsync($"{setting.Value}/api/Job/run?profileId={profileId}", null);
        //    if (!response.IsSuccessStatusCode)
        //    {
        //        var error = await response.Content.ReadAsStringAsync();
        //        throw new Exception($"Error: {error}");
        //    }
        //    var resultContent = await response.Content.ReadAsStringAsync();
        //    Console.WriteLine(resultContent);

        //    return View();
        //}
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
        //private void UpdateVariantMappedProperties(CreateXmlImportProfileModel model, int variantCount)
        //{
        //    var variantMappedProps = new[]
        //    {
        //        nameof(model.CreateXmlProduct.AttributePrice),
        //        nameof(model.CreateXmlProduct.AttributeStockQuantity),
        //        nameof(model.CreateXmlProduct.AttributeStockCode),
        //        nameof(model.CreateXmlProduct.AttributeGtin),
        //        nameof(model.CreateXmlProduct.AttributeManufacturerPartNumber),
        //    };

        //    foreach (var propName in variantMappedProps)
        //    {
        //        var propInfo = model.CreateXmlProduct.GetType().GetProperty(propName);
        //        if (propInfo == null || propInfo.PropertyType != typeof(string)) continue;

        //        var originalValue = propInfo.GetValue(model.CreateXmlProduct) as string;

        //        if (!string.IsNullOrWhiteSpace(originalValue) && originalValue.Contains("variants->variant->0"))
        //        {
        //            var updatedPaths = new List<string>();

        //            for (int i = 0; i < variantCount; i++)
        //            {
        //                updatedPaths.Add(originalValue.Replace("variants->variant->0", $"variants->variant->{i}"));
        //            }

        //            var newValue = string.Join(",", updatedPaths.Distinct());
        //            propInfo.SetValue(model.CreateXmlProduct, newValue);
        //        }
        //    }
        //}
        //private void UpdateImagesAndSpecifications(CreateXmlImportProfileModel model, string selectedImagePaths, string selectedAttributeSpecifications, int variantCount)
        //{
        //    model.CreateXmlProduct.Images = selectedImagePaths;
        //    model.CreateXmlProduct.AttributeSpecifications = selectedAttributeSpecifications;

        //    if (!string.IsNullOrWhiteSpace(selectedAttributeSpecifications) && variantCount > 0)
        //    {
        //        var updatedSpecifications = new List<string>();

        //        var originalSpecs = selectedAttributeSpecifications
        //            .Split(",", StringSplitOptions.RemoveEmptyEntries)
        //            .Select(s => s.Trim())
        //            .ToList();

        //        foreach (var specPath in originalSpecs)
        //        {
        //            var parts = specPath.Split("->", StringSplitOptions.RemoveEmptyEntries);
        //            var specIndexPart = parts.Last(); // Örn. "1"
        //            var specPart = $"spec->{specIndexPart}";

        //            for (int i = 0; i < variantCount; i++)
        //            {
        //                updatedSpecifications.Add($"variants->variant->{i}->{specPart}");
        //            }
        //        }

        //        model.CreateXmlProduct.AttributeSpecifications = string.Join(",", updatedSpecifications.Distinct());
        //    }
        //}
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
    }
}