using ClosedXML.Excel;
using Entegro.Application.DTOs.Common;
using Entegro.Application.DTOs.MediaFile;
using Entegro.Application.Interfaces.Services;
using Entegro.Web.Models.Import;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

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
                    var detail = new ImportProfileViewModel();

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
        public async Task<IActionResult> ImportData(ImportProfileViewModel detail)
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

        public IActionResult Xml()
        {
            return View();
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
    }
}