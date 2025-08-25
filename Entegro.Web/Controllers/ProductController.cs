using Entegro.Application.DTOs.Product;
using Entegro.Application.DTOs.ProductCategory;
using Entegro.Application.DTOs.ProductIntegration;
using Entegro.Application.DTOs.ProductMediaFile;
using Entegro.Application.DTOs.ProductVariantAttribute;
using Entegro.Application.DTOs.ProductVariantAttributeCombination;
using Entegro.Application.Interfaces.Services;
using Entegro.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using System.Net;
using static Entegro.Web.Models.ProductViewModel;

namespace Entegro.Web.Controllers
{
    public class ProductController : Controller
    {
        private readonly IProductService _productService;
        private readonly IProductCategoryMappingService _productCategoryMappingService;
        private readonly IBrandService _brandService;
        private readonly IProductAttributeService _productAttributeService;
        private readonly IProductAttributeMappingService _productAttributeMappingService;
        private readonly IProductImageMappingService _productImageMappingService;
        private readonly IIntegrationSystemService _integrationSystemService;
        private readonly IProductIntegrationService _productIntegrationService;
        public ProductController(
            IProductService productService,
            IProductCategoryMappingService productCategoryMappingService,
            IBrandService brandService,
            IProductAttributeService productAttributeService,
            IProductAttributeMappingService productAttributeMappingService,
            IProductImageMappingService productImageMappingService,
            IIntegrationSystemService integrationSystemService,
            IProductIntegrationService productIntegrationService)
        {
            _productService = productService ?? throw new ArgumentNullException(nameof(productService));
            _productCategoryMappingService = productCategoryMappingService ?? throw new ArgumentNullException(nameof(productCategoryMappingService));
            _brandService = brandService ?? throw new ArgumentNullException(nameof(brandService));
            _productAttributeService = productAttributeService ?? throw new ArgumentNullException(nameof(productAttributeService));
            _productAttributeMappingService = productAttributeMappingService ?? throw new ArgumentNullException(nameof(productAttributeMappingService));
            _productImageMappingService = productImageMappingService ?? throw new ArgumentNullException(nameof(productImageMappingService));
            _integrationSystemService = integrationSystemService ?? throw new ArgumentNullException(nameof(integrationSystemService));
            _productIntegrationService = productIntegrationService;
        }

        #region Product list / create / edit / delete
        public Task<IActionResult> Index()
        {
            return List();
        }

        public async Task<IActionResult> List()
        {
            var allIntegrationSystems = await _integrationSystemService.GetAllAsync();
            ViewBag.Commerces = allIntegrationSystems.Where(m => m.IntegrationSystemType == Domain.Enums.IntegrationSystemType.Commerce).Select(
                m => new { m.Id, m.Name }
                ).ToList();
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            ProductViewModel model = new ProductViewModel();
            await PrepareProductModel(model, null);
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Create(ProductViewModel model)
        {
            if (ModelState.IsValid)
            {
                var createDto = new CreateProductDto();
                createDto.Barcode = model.Barcode;
                createDto.BrandId = model.BrandId;
                createDto.Code = model.Code;
                createDto.Currency = model.Currency;
                createDto.Description = model.Description;
                createDto.Height = model.Height;
                createDto.Length = model.Length;
                createDto.MetaDescription = model.MetaDescription;
                createDto.MetaTitle = model.MetaTitle;
                createDto.MetaKeywords = model.MetaKeywords;
                createDto.Name = model.Name;
                createDto.Price = model.Price;
                createDto.StockQuantity = model.StockQuantity;
                createDto.Unit = model.Unit;
                createDto.VatInc = model.VatInc;
                createDto.VatRate = model.VatRate;
                createDto.Weight = model.Weight;
                createDto.Width = model.Width;

                await _productService.CreateProductAsync(createDto);

                return Json(new { success = true });
            }

            await PrepareProductModel(model, null);
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            ProductViewModel model = new ProductViewModel();

            var product = await _productService.GetProductByIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            await PrepareProductModel(model, product);
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(ProductViewModel model)
        {
            if (ModelState.IsValid)
            {
                var updateDto = new UpdateProductDto();
                updateDto.Id = model.Id;
                updateDto.Barcode = model.Barcode;
                updateDto.BrandId = model.BrandId;
                updateDto.Code = model.Code;
                updateDto.Currency = model.Currency;
                updateDto.Description = model.Description;
                updateDto.Height = model.Height;
                updateDto.Length = model.Length;
                updateDto.MetaDescription = model.MetaDescription;
                updateDto.MetaTitle = model.MetaTitle;
                updateDto.MetaKeywords = model.MetaKeywords;
                updateDto.Name = model.Name;
                updateDto.Price = model.Price;
                updateDto.StockQuantity = model.StockQuantity;
                updateDto.Unit = model.Unit;
                updateDto.VatInc = model.VatInc;
                updateDto.VatRate = model.VatRate;
                updateDto.Weight = model.Weight;
                updateDto.Width = model.Width;
                updateDto.ManufacturerPartNumber = model.ManufacturerPartNumber;
                updateDto.Gtin = model.Gtin;
                updateDto.Published = model.Published;
                updateDto.ProductVariantAttributeCombinations = model.ProductVariantAttributeCombinations.Select(m => new ProductVariantAttributeCombinationDto()
                {
                    AttributeXml = JsonConvert.SerializeObject(m.Attributes),
                    Gtin = m.Gtin,
                    Id = m.Id,
                    ManufacturerPartNumber = m.ManufacturerPartNumber,
                    Price = m.Price,
                    StockQuantity = m.StockQuantity,
                    ProductId = m.ProductId,
                    StokCode = m.StokCode,
                }).ToList();

                await _productService.UpdateProductAsync(updateDto);

                foreach (var item in model.SelectedProductAttributeIds)
                {
                    var exist = await _productAttributeMappingService.GetByAttibuteIdAsync(item);

                    if (exist == null)
                    {
                        CreateProductVariantAttributeDto createProductAttributeMappingDto = new CreateProductVariantAttributeDto
                        {
                            ProductId = model.Id,
                            ProductAttributeId = item,
                            DisplayOrder = 0,
                            AttributeControlTypeId = 0
                        };

                        await _productAttributeMappingService.AddAsync(createProductAttributeMappingDto);
                    }
                }

                return Json(new { success = true });
            }

            await PrepareProductModel(model, null);
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ProductList([FromBody] GridCommand model)
        {
            int pageNumber = model.Start / model.Length;
            int pageSize = model.Length;


            var result = await _productService.GetProductsAsync(pageNumber, model.Length);

            return Json(new
            {
                draw = model.Draw,
                recordsTotal = result.TotalCount,
                recordsFiltered = result.TotalCount,
                data = result.Items
            });
        }
        #endregion

        #region Product Categories
        [HttpPost]
        public async Task<IActionResult> ProductCategoryList(int productId, CancellationToken ct)
        {
            var data = await _productCategoryMappingService.GetCategoryPathsByProductAsync(productId, ct);
            var results = data.Select(d => new { id = d.Id, text = d.CategoryPath, displayOrder = d.DisplayOrder });
            return Json(new { results });
        }

        [HttpPost]
        public async Task<IActionResult> ProductCategoryInsert([FromBody] CreateProductCategoryDto createProductCategoryDto)
        {
            if (ModelState.IsValid)
            {
                int id = await _productCategoryMappingService.CreateProductCategoryAsync(createProductCategoryDto);
                return Json(new { success = true, id });
            }
            return Json(new { success = false, errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage) });
        }

        [HttpPost]
        public async Task<IActionResult> ProductCategoryDelete(int id)
        {
            bool isSuccess = await _productCategoryMappingService.DeleteProductCategoryAsync(id);
            return Json(new { success = isSuccess });
        }

        #endregion

        #region Product Pictures
        [HttpPost]
        public async Task<IActionResult> ProductMediaFilesAdd(string mediaFileIds, int entityId)
        {

            bool success = true;
            var response = new List<dynamic>();

            try
            {
                if (!string.IsNullOrWhiteSpace(mediaFileIds))
                {
                    var mediaIdList = mediaFileIds
                        .Split(',', StringSplitOptions.RemoveEmptyEntries)
                        .Select(id => int.Parse(id.Trim()))
                        .ToList();

                    for (int i = 0; i < mediaIdList.Count; i++)
                    {
                        int mediaFileId = mediaIdList[i];

                        // Yeni ProductMediaFile nesnesi oluştur
                        var productPicture = new CreateProductMediaFileDto
                        {
                            MediaFileId = mediaFileId,
                            ProductId = entityId,
                            DisplayOrder = i // Sıralamayı kaydediyoruz
                        };

                        var id = await _productImageMappingService.AddAsync(productPicture);

                        // İsteğe bağlı olarak frontend’e dönecek bilgi
                        var respObj = new
                        {
                            MediaFileId = mediaFileId,
                            ProductMediaFileId = id,
                            DisplayOrder = i
                        };

                        response.Add(respObj);
                    }

                }
                else
                {
                    success = false;
                }
            }
            catch (Exception ex)
            {
                success = false;
                return Json(new
                {
                    success,
                    message = "Bir hata oluştu: " + ex.Message
                });
            }

            return Json(new
            {
                success,
                response,
                message = "Resimler başarıyla eklendi"
            });

        }

        [HttpPost]
        public async Task<IActionResult> ProductPictureDelete(int id)
        {
            var isSuccess = await _productImageMappingService.DeleteAsync(id);
            if (!isSuccess)
            {
                return Json(new { success = false, message = "Resim silinirken bir hata oluştu." });
            }
            return StatusCode((int)HttpStatusCode.OK);
        }

        [HttpPost]
        public async Task<IActionResult> SortPictures(string pictures, int entityId)
        {
            var response = new List<dynamic>();
            try
            {
                if (!string.IsNullOrWhiteSpace(pictures))
                {
                    var pictureIds = pictures
                        .Split(',', StringSplitOptions.RemoveEmptyEntries)
                        .Select(id => int.Parse(id.Trim()))
                        .ToList();

                    for (int i = 0; i < pictureIds.Count; i++)
                    {
                        int pictureId = pictureIds[i];

                        var productPicture = await _productImageMappingService.GetByPictureIdProductIdAsync(pictureId, entityId);

                        if (productPicture != null)
                        {
                            productPicture.DisplayOrder = i;

                            response.Add(new
                            {
                                productPicture.DisplayOrder,
                                productPicture.MediaFileId,
                                EntityMediaId = productPicture.Id
                            });

                            await _productImageMappingService.UpdateAsync(new UpdateProductMediaFileeDto
                            {
                                Id = pictureId,
                                DisplayOrder = i,
                                MediaFileId = productPicture.MediaFileId,
                                ProductId = entityId
                            });
                        }
                    }

                    int mainPictureId = response[0].MediaFileId;
                    await _productService.UpdateProductMainPictureIdAsync(entityId, mainPictureId);

                    return Json(new
                    {
                        success = true,
                        response,
                        message = "Sıralama güncellendi."
                    });
                }

                return Json(new { success = false, message = "Sıralama verisi boş." });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    message = "Hata oluştu: " + ex.Message
                });
            }

        }

        #endregion

        #region Product Integration

        [HttpPost]
        public async Task<IActionResult> CreateOrUpdateIntegrationAll(int integrationSystemId)
        {
            var allProduct = await _productService.GetProductsAsync();
            foreach (var product in allProduct)
            {
                var productIntegration = await _productIntegrationService.GetByProductIdandIntegrationSystemIdAsync(product.Id, integrationSystemId);
                if (productIntegration != null)
                {
                    await _productIntegrationService.UpdateProductIntegrationAsync(new UpdateProductIntegrationDto
                    {
                        Id = productIntegration.Id,
                        Active = productIntegration.Active,
                        IntegrationSystemId = integrationSystemId,
                        IntegrationCode = product.Code,
                        Price = product.Price,
                        ProductId = productIntegration.ProductId,
                        LastSyncDate = productIntegration.LastSyncDate
                    });
                }
                else
                {
                    await _productIntegrationService.CreateProductIntegrationAsync(new CreateProductIntegrationDto
                    {
                        IntegrationCode = product.Code,
                        Price = product.Price,
                        ProductId = product.Id,
                        IntegrationSystemId = integrationSystemId,
                        Active = true,
                        LastSyncDate = null
                    });
                }

            }
            return Json(new { success = true, message = "Ürünlere entegrasyon Uygulandı." });
        }

        [HttpPost]
        public async Task<IActionResult> CreateOrUpdateProductIntegration(CreateProductIntegrationViewModel model)
        {
            var productIntegration = await _productIntegrationService.GetByProductIdandIntegrationSystemIdAsync(model.ProductId, model.IntegrationSystemId);


            if (productIntegration == null)
            {
                await _productIntegrationService.CreateProductIntegrationAsync(new CreateProductIntegrationDto
                {
                    IntegrationCode = model.IntegrationCode,
                    Price = model.Price,
                    ProductId = model.ProductId,
                    IntegrationSystemId = model.IntegrationSystemId,
                    Active = true,
                    LastSyncDate = null
                });
            }

            else
            {
                await _productIntegrationService.UpdateProductIntegrationAsync(new UpdateProductIntegrationDto
                {
                    Id = productIntegration.Id,
                    Active = productIntegration.Active,
                    IntegrationSystemId = productIntegration.IntegrationSystemId,
                    IntegrationCode = model.IntegrationCode,
                    Price = model.Price,
                    ProductId = productIntegration.ProductId,
                    LastSyncDate = productIntegration.LastSyncDate
                });
            }

            return RedirectToAction("List");
        }

        [HttpGet]
        public async Task<IActionResult> IntegrationDialog(DialogViewModel model)
        {
            var existingProductIntegration = await _productIntegrationService.GetByIdAsync(model.productIntegrationSystemId);

            if (model.productIntegrationSystemId == 0)
            {
                return PartialView("_IntegrationDialog", new CreateProductIntegrationViewModel()
                {
                    ProductId = model.productId,
                    IntegrationSystemId = model.integrationSystemId
                });
            }
            var createModel = new CreateProductIntegrationViewModel
            {
                ProductId = existingProductIntegration.ProductId,
                IntegrationSystemId = existingProductIntegration.IntegrationSystemId,
                IntegrationCode = existingProductIntegration?.IntegrationCode,
                Price = existingProductIntegration?.Price ?? 0m
            };

            return PartialView("_IntegrationDialog", createModel);
        }

        #endregion
        private async Task PrepareProductModel(ProductViewModel model, ProductDto? product)
        {
            if (product != null)
            {
                model.Id = product.Id;
                model.Barcode = product.Barcode;
                model.BrandId = product.BrandId;
                model.Code = product.Code;
                model.Currency = product.Currency;
                model.Description = product.Description;
                model.Height = product.Height;
                model.Length = product.Length;
                model.MetaDescription = product.MetaDescription;
                model.MetaTitle = product.MetaTitle;
                model.MetaKeywords = product.MetaKeywords;
                model.Name = product.Name;
                model.VatInc = product.VatInc;
                model.Price = product.Price;
                model.StockQuantity = product.StockQuantity;
                model.Unit = product.Unit;
                model.VatRate = product.VatRate;
                model.Weight = product.Weight;
                model.Width = product.Width;
                model.Gtin = product.Gtin;
                model.ManufacturerPartNumber = product.ManufacturerPartNumber;
                model.Published = product.Published;
                model.SelectedProductAttributeIds = product.ProductVariantAttributes.Select(x => x.Id).ToArray();
                model.ProductAttributeMappings = product.ProductVariantAttributes.Select(m => new ProductViewModel.ProductAttributeMappingViewModel()
                {
                    AttributeControlTypeId = m.AttributeControlTypeId,
                    DisplayOrder = m.DisplayOrder,
                    Id = m.Id,
                    IsRequried = m.IsRequried,
                    ProductAttribute = m.ProductAttribute.Name,
                    ProductAttributeId = m.ProductAttributeId,
                    ProductId = m.ProductId,
                    Attribute = new ProductAttributeViewModel()
                    {
                        Id = m.ProductAttribute.Id,
                        Values = m.ProductAttribute.Values.Select(x => new ProductAttributeValueViewModel()
                        {
                            Id = x.Id,
                            DisplayOrder = x.DisplayOrder,
                            Name = x.Name,
                            ProductAttributeId = x.ProductAttributeId,
                        }).ToList()
                    }
                }).ToList();
                model.ProductMediaFiles = product.ProductMediaFiles.Select(m => new ProductImageViewModel()
                {
                    Id = m.Id,
                    DisplayOrder = m.DisplayOrder,
                    MediaFileId = m.MediaFileId,
                    ProductId = m.ProductId,
                    MediaFile = new MediaFileViewModel()
                    {
                        Alt = m.MediaFile.Alt,
                        CreatedOn = m.MediaFile.CreatedOn,
                        Deleted = m.MediaFile.Deleted,
                        Extension = m.MediaFile.Extension,
                        FolderId = m.MediaFile.FolderId,
                        Height = m.MediaFile.Height,
                        Id = m.MediaFile.Id,
                        IsTransient = m.MediaFile.IsTransient,
                        MediaType = m.MediaFile.MediaType,
                        Metadata = m.MediaFile.Metadata,
                        MimeType = m.MediaFile.MimeType,
                        Name = m.MediaFile.Name,
                        PixelSize = m.MediaFile.PixelSize,
                        Size = m.MediaFile.Size,
                        Title = m.MediaFile.Title,
                        UpdatedOn = m.MediaFile.UpdatedOn,
                        Width = m.MediaFile.Width,
                        Folder = m.MediaFile.Folder == null ? null : new MediaFolderViewModel()
                        {
                            Id = m.MediaFile.Folder.Id,
                            Name = m.MediaFile.Folder.Name,
                        }
                    }
                }).ToList();
                model.ProductVariantAttributeCombinations = product.ProductVariantAttributeCombinations.Select(m => new ProductViewModel.ProductVariantAttributeCombinationViewModel()
                {
                    Attributes = JsonConvert.DeserializeObject<List<ProductVariantAttributeViewModel>>(m.AttributeXml) ?? new List<ProductVariantAttributeViewModel>(),
                    Gtin = m.Gtin,
                    Id = m.Id,
                    ManufacturerPartNumber = m.ManufacturerPartNumber,
                    Price = m.Price,
                    ProductId = m.ProductId,
                    StockQuantity = m.StockQuantity,
                    StokCode = m.StokCode
                }).ToList();

                var productAttributes = await _productAttributeService.GetAllAsync();
                ViewBag.ProductAttributes = productAttributes.Select(x => new SelectListItem
                {
                    Text = x.Name,
                    Value = x.Id.ToString()
                }).ToList();
            }

            var brands = await _brandService.GetBrandsAsync();

            ViewBag.Brands = brands.Select(m => new SelectListItem()
            {
                Text = m.Name,
                Value = m.Id.ToString()
            }).ToList();
            ViewBag.Currencies = new List<SelectListItem>
            {
                new SelectListItem { Text = "TRY", Value = "TRY" },
                new SelectListItem { Text = "USD", Value = "USD" },
                new SelectListItem { Text = "EUR", Value = "EUR" },
                new SelectListItem { Text = "GBP", Value = "GBP" },
                new SelectListItem { Text = "JPY", Value = "JPY" }
            };
            ViewBag.VatRates = new List<SelectListItem>
            {
                new SelectListItem { Text = "0%", Value = "0" },
                new SelectListItem { Text = "1%", Value = "1" },
                new SelectListItem { Text = "8%", Value = "8" },
                new SelectListItem { Text = "18%", Value = "18" },
                new SelectListItem { Text = "20%", Value = "20" }
            };
            ViewBag.Units = new List<SelectListItem>
            {
                new SelectListItem { Text = "Adet", Value = "Adet" },
                new SelectListItem { Text = "Kg", Value = "Kg" },
                new SelectListItem { Text = "Litre", Value = "Litre" },
                new SelectListItem { Text = "Metre", Value = "Metre" },
                new SelectListItem { Text = "Kutu", Value = "Kutu" }
            };
        }
    }
}
