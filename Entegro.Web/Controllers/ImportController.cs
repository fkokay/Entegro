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

        public ImportController(IMediaFileService mediaFileService, IWebHostEnvironment webHostEnvironment, IImportProfileService importProfileService)
        {
            _mediaFileService = mediaFileService;
            _webHostEnvironment = webHostEnvironment;
            _importProfileService = importProfileService;
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
                    var detail = new ExcelImportProfileViewModel();

                    using var stream = new FileStream(windowsFilePath, FileMode.Open, FileAccess.Read);
                    using var workbook = new XLWorkbook(stream);
                    var worksheet = workbook.Worksheet(1);

                    var headers = new List<ColumnMapping>();
                    detail.MediaFileId = mediaFileId;
                    detail.ProfileName = mediaFile.Name;
                    detail.Enable = true;
                    detail.ColumnMappings = headers;
                    foreach (var cell in worksheet.Row(1).CellsUsed())
                    {
                        headers.Add(new ColumnMapping
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
            return View(new List<ColumnMapping>());
        }

        [HttpPost]
        public async Task<IActionResult> ImportData(ExcelImportProfileViewModel detail)
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

                var mappedResult = selectedColumns.Select(col => new ColumnMappingResult
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
                await _importProfileService.CreateAsync(createDto);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Hata oluştu: {ex.Message}");
            }


            return View("ImportResult", selectedColumns);
        }
        #endregion


        #region XML Import
        public IActionResult Xml()
        {
            XmlImportProfileViewModel model = new XmlImportProfileViewModel();
            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> Xml(XmlImportProfileViewModel model)
        {
            if (string.IsNullOrWhiteSpace(model.MediaFileUrl))
                return View(new XmlImportProfileViewModel { Error = "Lütfen bir XML URL girin." });

            try
            {
                using var httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/120 Safari/537.36");
                string xmlContent = await httpClient.GetStringAsync(model.MediaFileUrl.Trim());

                var document = XDocument.Parse(xmlContent);
                if (document.Root is null)
                    return View(new XmlImportProfileViewModel { MediaFileUrl = model.MediaFileUrl, Error = "XML kök (root) bulunamadı." });


                const string StartElementName = "Product";
                var startNode = document
                    .Descendants()
                    .FirstOrDefault(x =>
                        x.NodeType == System.Xml.XmlNodeType.Element &&
                        string.Equals(x.Name.LocalName, StartElementName, StringComparison.OrdinalIgnoreCase));

                if (startNode == null)
                    return View(new XmlImportProfileViewModel { MediaFileUrl = model.MediaFileUrl, Error = $"XML içinde <{StartElementName}> bulunamadı." });

                string previewXml = startNode?.ToString(System.Xml.Linq.SaveOptions.None);
                model.PreviewXml = previewXml;

                var paths = new List<string>();
                TraverseChildrenOnly(startNode, string.Empty, paths);

                model.Paths = paths
                    .Distinct(StringComparer.OrdinalIgnoreCase)
                    .OrderBy(p => p, StringComparer.OrdinalIgnoreCase)
                    .ToList();

                return RedirectToAction("XmlMapping", model);
            }
            catch (Exception ex)
            {
                return View(new XmlImportProfileViewModel { MediaFileUrl = model.MediaFileUrl, Error = "İndirme/parse hatası: " + ex.Message });
            }
        }

        public IActionResult XmlMapping(XmlImportProfileViewModel model)
        {
            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> XmlMappingAdd(XmlImportProfileViewModel model, string SelectedImagePaths)
        {
            model.Images = SelectedImagePaths;


            var headerMaps = model.GetType().GetProperties()
                .Where(p => p.PropertyType == typeof(string) && p.Name != nameof(model.MediaFileUrl))
                .Where(p => p.PropertyType == typeof(string))
                .Select(p => new HeaderMap
                {
                    MappedName = p.Name,
                    XmlHeader = p.GetValue(model)?.ToString() ?? ""
                })
                .Where(h => !string.IsNullOrWhiteSpace(h.XmlHeader)) // boş olmayanlar
                .ToList();


            var json = System.Text.Json.JsonSerializer.Serialize(headerMaps, new System.Text.Json.JsonSerializerOptions
            {
                WriteIndented = true
            });


            using var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/120 Safari/537.36");
            string xmlContent = await httpClient.GetStringAsync(model.MediaFileUrl.Trim());

            Console.WriteLine("📄 Header Map JSON:");
            Console.WriteLine(json);


            var xmlDoc = XDocument.Parse(xmlContent);


            var dataElements = xmlDoc.Root?.Elements();

            if (dataElements != null)
            {
                Console.WriteLine("XML İçeriği:");

                foreach (var item in dataElements)
                {
                    Console.WriteLine("Yeni Kayıt -----------------------");

                    foreach (var map in headerMaps)
                    {
                        var element = item.Element(map.XmlHeader);

                        if (element != null)
                        {
                            Console.WriteLine($"Başlık: {map.XmlHeader}");
                            Console.WriteLine($"İçerik: {element.Value}");
                        }
                        else
                        {
                            Console.WriteLine($"Başlık: {map.XmlHeader}");
                            Console.WriteLine("İçerik: (Bulunamadı)");
                        }

                        Console.WriteLine("-----------------------------");
                    }
                }
            }
            else
            {
                Console.WriteLine("XML'de veri bulunamadı.");
            }

            return View(model);
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