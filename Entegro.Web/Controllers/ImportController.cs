using ClosedXML.Excel;
using Entegro.Web.Models.Import;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace Entegro.Web.Controllers
{
    public class ImportController : Controller
    {
        [HttpGet]
        public IActionResult Excel()
        {
            return View();
        }

        [HttpGet]
        public IActionResult MapColumns()
        {
            if (TempData["Headers"] != null)
            {
                var headersJson = TempData["Headers"] as string;
                var headers = JsonSerializer.Deserialize<List<ColumnMapping>>(headersJson);

                return View(headers);
            }

            return View(new List<ColumnMapping>());
        }

        [HttpPost]
        public IActionResult ImportData(List<ColumnMapping> excelColumns)
        {
            var filePath = TempData["UploadedFilePath"] as string;
            if (string.IsNullOrEmpty(filePath) || !System.IO.File.Exists(filePath))
            {
                TempData["Error"] = "Dosya bulunamadı, tekrar yükleyin.";
                return RedirectToAction("Index");
            }

            var selectedColumns = excelColumns
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
    }
}
