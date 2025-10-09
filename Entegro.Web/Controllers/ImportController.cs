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
        public ImportController(IMediaFileService mediaFileService, IWebHostEnvironment webHostEnvironment, IImportProfileService importProfileService, HttpClient client, IProductService productService, ICategoryService categoryService, IBrandService brandService, ISettingService settingService, IMapper mapper, IProductIntegrationService productIntegrationService)
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
                List<SelectListItem> xmlPathsSelectItems = new();
                if (profileModel.ColumnMapping is not null)
                {
                    var mappings = JsonConvert.DeserializeObject<List<XmlColumnMappingResult>>(profileModel.ColumnMapping);
                    model.ProductImport.Name = mappings.FirstOrDefault(m => m.MappedName == "Name")?.XmlHeader;
                    model.ProductImport.Brand = mappings.FirstOrDefault(m => m.MappedName == "Brand")?.XmlHeader;
                    model.ProductImport.Code = mappings.FirstOrDefault(m => m.MappedName == "Code")?.XmlHeader;
                    model.ProductImport.Barcode = mappings.FirstOrDefault(m => m.MappedName == "Barcode")?.XmlHeader;
                    model.ProductImport.ManufacturerPartNumber = mappings.FirstOrDefault(m => m.MappedName == "ManufacturerPartNumber")?.XmlHeader;
                    model.ProductImport.Gtin = mappings.FirstOrDefault(m => m.MappedName == "Gtin")?.XmlHeader;
                    model.ProductImport.Price = mappings.FirstOrDefault(m => m.MappedName == "Price")?.XmlHeader;
                    model.ProductImport.Currency = mappings.FirstOrDefault(m => m.MappedName == "Currency")?.XmlHeader;
                    model.ProductImport.Unit = mappings.FirstOrDefault(m => m.MappedName == "VatRate")?.XmlHeader;
                    model.ProductImport.VatInc = mappings.FirstOrDefault(m => m.MappedName == "VatInc")?.XmlHeader;
                    model.ProductImport.StockQuantity = mappings.FirstOrDefault(m => m.MappedName == "StockQuantity")?.XmlHeader;
                    model.ProductImport.Weight = mappings.FirstOrDefault(m => m.MappedName == "Weight")?.XmlHeader;
                    model.ProductImport.Length = mappings.FirstOrDefault(m => m.MappedName == "Length")?.XmlHeader;
                    model.ProductImport.Width = mappings.FirstOrDefault(m => m.MappedName == "Width")?.XmlHeader;
                    model.ProductImport.Height = mappings.FirstOrDefault(m => m.MappedName == "Height")?.XmlHeader;
                    model.ProductImport.MetaKeywords = mappings.FirstOrDefault(m => m.MappedName == "MetaKeywords")?.XmlHeader;
                    model.ProductImport.MetaDescription = mappings.FirstOrDefault(m => m.MappedName == "MetaDescription")?.XmlHeader;
                    model.ProductImport.MetaTitle = mappings.FirstOrDefault(m => m.MappedName == "MetaTitle")?.XmlHeader;
                    model.ProductImport.Description = mappings.FirstOrDefault(m => m.MappedName == "Description")?.XmlHeader;
                    model.ProductImport.Images = mappings.FirstOrDefault(m => m.MappedName == "Images")?.XmlHeader?.Split(',');
                    model.ProductImport.Categories = mappings.FirstOrDefault(m => m.MappedName == "Categories")?.XmlHeader?.Split(',');
                    model.AttributeStockCode = mappings.FirstOrDefault(m => m.MappedName == "AttributeStockCode")?.XmlHeader.Split(",").FirstOrDefault().Replace("/", "->");
                    model.AttributePrice = mappings.FirstOrDefault(m => m.MappedName == "AttributePrice")?.XmlHeader.Split(",").FirstOrDefault().Replace("/", "->");
                    model.AttributeGtin = mappings.FirstOrDefault(m => m.MappedName == "AttributeGtin")?.XmlHeader.Split(",").FirstOrDefault().Replace("/", "->");
                    model.AttributeManufacturerPartNumber = mappings.FirstOrDefault(m => m.MappedName == "AttributeManufacturerPartNumber")?.XmlHeader.Split(",").FirstOrDefault().Replace("/", "->");
                    model.AttributeSpecifications = mappings.FirstOrDefault(m => m.MappedName == "AttributeSpecifications")?.XmlHeader;
                    model.AttributeStockQuantity = mappings.FirstOrDefault(m => m.MappedName == "AttributeStockQuantity")?.XmlHeader.Split(",").FirstOrDefault().Replace("/", "->");

                }
                ViewBag.XmlPathsSelectList = model.Paths.Select(m => new SelectListItem()
                {
                    Text = m,
                    Value = m
                }).ToList();

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

                model.VariantCount = variantCount;
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

            if (model.Id == 0)
            {

                var createProfile = new Application.DTOs.ImportProfile.CreateImportProfileDto();
                createProfile.MediaFileType = "xml";
                createProfile.MediaFileUrl = model.MediaFileUrl;
                createProfile.ProfileName = model.ProfileName;
                createProfile.Enable = model.Enable;
                var profile = await _importProfileService.AddAsync(createProfile);

                return RedirectToAction("Xml", new { profileId = profile.Id });
            }
            else
            {
                Type[] allowedTypes = { typeof(string), typeof(string[]) };
                UpdateVariantMappedProperties(model);

                var headerMaps = GetHeaderMappings(model);
                var maps = model.ProductImport.GetType()
                .GetProperties()
                .Where(p => allowedTypes.Contains(p.PropertyType))
                .SelectMany(p =>
                {
                    var value = p.GetValue(model.ProductImport);

                    if (p.PropertyType == typeof(string))
                    {
                        var str = value as string;
                        return new[]
                        {
                            new XmlColumnMappingResult
                            {
                                MappedName = p.Name,
                                XmlHeader = str?.Replace("->", "/") ?? ""
                            }
                        };
                    }
                    else if (p.PropertyType == typeof(string[]))
                    {
                        var arr = value as string[];
                        return new[]
                        {
                            new XmlColumnMappingResult
                            {
                                MappedName = p.Name,
                                XmlHeader = arr == null ? "": string.Join(",",arr)
                            }
                        };
                    }

                    return Enumerable.Empty<XmlColumnMappingResult>();
                })
                .Where(h => !string.IsNullOrWhiteSpace(h.XmlHeader))
                .ToList();


                var columnMapping = System.Text.Json.JsonSerializer.Serialize(maps);

                var updateProfile = new UpdateImportProfileDto();
                updateProfile.Id = model.Id;
                updateProfile.ProfileName = model.ProfileName;
                updateProfile.ApplyPriceAdjustment = model.ApplyPriceAdjustment;
                updateProfile.PriceAdjustmentType = model.PriceAdjustmentType;
                updateProfile.MediaFileType = "xml";
                updateProfile.OptionalExtraAmount = model.OptionalExtraAmount;
                updateProfile.PriceAdjustmentAmount = model.PriceAdjustmentAmount;
                updateProfile.MediaFileUrl = model.MediaFileUrl;
                updateProfile.Enable = true;
                updateProfile.ColumnMapping = columnMapping;

                var profile = await _importProfileService.UpdateAsync(updateProfile);
                return RedirectToAction("Xml", new { profileId = profile.Id });
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

        [HttpPost]
        public async Task<IActionResult> XmlMapping(XmlImportProfileModel model)
        {
            UpdateVariantMappedProperties(model);

            var headerMaps = GetHeaderMappings(model);
            PrintHeaderMapsJson(headerMaps);

            var xmlDoc = await ReadXml(model.MediaFileUrl);
            Console.WriteLine("📄 Header Map JSON:");

            if (model.Id == 0)
            {
                var createModel = new Application.DTOs.ImportProfile.CreateImportProfileDto
                {
                    ProfileName = model.ProfileName,
                    ApplyPriceAdjustment = model.ApplyPriceAdjustment,
                    PriceAdjustmentType = model.PriceAdjustmentType,
                    MediaFileType = "xml",
                    OptionalExtraAmount = model.OptionalExtraAmount,
                    PriceAdjustmentAmount = model.PriceAdjustmentAmount,
                    MediaFileUrl = model.MediaFileUrl,
                    Enable = true,
                    ColumnMapping = System.Text.Json.JsonSerializer.Serialize(headerMaps, new System.Text.Json.JsonSerializerOptions
                    {
                        WriteIndented = true
                    })
                };

                var profile = await _importProfileService.AddAsync(createModel);
            }

            else
            {
                var updateModel = new Application.DTOs.ImportProfile.UpdateImportProfileDto
                {
                    Id = model.Id,
                    ProfileName = model.ProfileName,
                    ApplyPriceAdjustment = model.ApplyPriceAdjustment,
                    PriceAdjustmentType = model.PriceAdjustmentType,
                    MediaFileType = "xml",
                    OptionalExtraAmount = model.OptionalExtraAmount,
                    PriceAdjustmentAmount = model.PriceAdjustmentAmount,
                    MediaFileUrl = model.MediaFileUrl,
                    Enable = true,
                    ColumnMapping = System.Text.Json.JsonSerializer.Serialize(headerMaps, new System.Text.Json.JsonSerializerOptions
                    {
                        WriteIndented = true
                    })
                };

                var profile = await _importProfileService.UpdateAsync(updateModel);
            }

            return RedirectToAction("List");
        }

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
        private void UpdateVariantMappedProperties(XmlImportProfileModel model)
        {
            var variantMappedProps = new[]
            {
                nameof(model.AttributePrice),
                nameof(model.AttributeStockQuantity),
                nameof(model.AttributeStockCode),
                nameof(model.AttributeGtin),
                nameof(model.AttributeManufacturerPartNumber),
            };

            foreach (var propName in variantMappedProps)
            {
                var propInfo = model.ProductImport.GetType().GetProperty(propName);
                if (propInfo == null || propInfo.PropertyType != typeof(string)) continue;

                var originalValue = propInfo.GetValue(model.ProductImport) as string;

                if (!string.IsNullOrWhiteSpace(originalValue) && originalValue.Contains("variants->variant->0"))
                {
                    var updatedPaths = new List<string>();

                    for (int i = 0; i < model.VariantCount; i++)
                    {
                        updatedPaths.Add(originalValue.Replace("variants->variant->0", $"variants->variant->{i}"));
                    }

                    var newValue = string.Join(",", updatedPaths.Distinct());
                    propInfo.SetValue(model.ProductImport, newValue);
                }
            }
        }
        private List<XmlColumnMappingResult> GetHeaderMappings(XmlImportProfileModel model)
        {
            return model.ProductImport.GetType().GetProperties()
                .Where(p => p.PropertyType == typeof(string))
                .Select(p => new XmlColumnMappingResult
                {
                    MappedName = p.Name,
                    XmlHeader = p.GetValue(model.ProductImport)?.ToString().Replace("->", "/") ?? ""
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

        #endregion

    }
}