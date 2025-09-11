using Entegro.Application.DTOs.Common;
using Entegro.Application.DTOs.Marketplace.Trendyol;
using Entegro.Application.DTOs.Product;
using Entegro.Application.DTOs.ProductCategory;
using Entegro.Application.DTOs.ProductIntegration;
using Entegro.Application.DTOs.ProductMediaFile;
using Entegro.Application.DTOs.ProductVariantAttribute;
using Entegro.Application.DTOs.ProductVariantAttributeCombination;
using Entegro.Application.Interfaces.Services;
using Entegro.Application.Interfaces.Services.Marketplace;
using Entegro.Web.Models.Catalog.Attributes;
using Entegro.Web.Models.Catalog.Products;
using Entegro.Web.Models.Catalog.ProductSpecificationAttribute;
using Entegro.Web.Models.Catalog.SpecificationAttributeOptions;
using Entegro.Web.Models.Content;
using Entegro.Web.Models.Integration;
using Entegro.Web.Models.Integration.Common;
using Entegro.Web.Models.Integration.Marketplace;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis;
using Newtonsoft.Json;
using System.Net;

namespace Entegro.Web.Controllers
{
    [Authorize]
    public class ProductController : Controller
    {
        private readonly IProductService _productService;
        private readonly IProductCategoryService _productCategoryMappingService;
        private readonly IBrandService _brandService;
        private readonly IProductAttributeService _productAttributeService;
        private readonly IProductVariantAttributeService _productVariantAttributeService;
        private readonly IProductImageMappingService _productImageMappingService;
        private readonly IIntegrationSystemService _integrationSystemService;
        private readonly IProductIntegrationService _productIntegrationService;
        private readonly IProductAttributeFormatter _productAttributeFormatter;
        private readonly IProductVariantAttributeCombinationService _productVariantAttributeCombinationService;
        private readonly ICategoryService _categoryService;
        private readonly ITrendyolService _trenyolService;
        private readonly IProductSpecificationAttributeMappingService _productSpecificationAttributeMappingService;
        public ProductController(
            IProductService productService,
            IProductCategoryService productCategoryMappingService,
            IBrandService brandService,
            IProductAttributeService productAttributeService,
            IProductVariantAttributeService productVariantAttributeService,
            IProductImageMappingService productImageMappingService,
            IIntegrationSystemService integrationSystemService,
            IProductIntegrationService productIntegrationService,
            IProductAttributeFormatter productAttributeFormatter,
            ICategoryService categoryService,
            ITrendyolService trendyolService,
            IProductSpecificationAttributeMappingService productSpecificationAttributeMappingService,
            IProductVariantAttributeCombinationService productVariantAttributeCombinationService)
        {
            _productService = productService ?? throw new ArgumentNullException(nameof(productService));
            _productCategoryMappingService = productCategoryMappingService ?? throw new ArgumentNullException(nameof(productCategoryMappingService));
            _brandService = brandService ?? throw new ArgumentNullException(nameof(brandService));
            _productAttributeService = productAttributeService ?? throw new ArgumentNullException(nameof(productAttributeService));
            _productVariantAttributeService = productVariantAttributeService ?? throw new ArgumentNullException(nameof(productVariantAttributeService));
            _productImageMappingService = productImageMappingService ?? throw new ArgumentNullException(nameof(productImageMappingService));
            _integrationSystemService = integrationSystemService ?? throw new ArgumentNullException(nameof(integrationSystemService));
            _productIntegrationService = productIntegrationService;
            _productAttributeFormatter = productAttributeFormatter;
            _trenyolService = trendyolService;
            _categoryService = categoryService;
            _productSpecificationAttributeMappingService = productSpecificationAttributeMappingService;
            _productVariantAttributeCombinationService = productVariantAttributeCombinationService;
        }

        #region Product list / create / edit / delete
        public Task<IActionResult> Index()
        {
            return List();
        }

        public async Task<IActionResult> List()
        {
            var allIntegrationSystems = await _integrationSystemService.GetAllAsync(null);
            ViewBag.Commerces = allIntegrationSystems.Where(m => m.IntegrationSystemType == Domain.Enums.IntegrationSystemType.Commerce).Select(
                m => new { m.Id, m.Name }
                ).ToList();

            ViewBag.Marketplaces = allIntegrationSystems.Where(m => m.IntegrationSystemType == Domain.Enums.IntegrationSystemType.Marketplace).Select(
                m => new { m.Id, m.Name, Value = m.IntegrationSystemParameters.Select(x => x.Value).FirstOrDefault() }
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
                    RawAttribute = JsonConvert.SerializeObject(m.ProductVariantAttributeSelections),
                    Gtin = m.Gtin,
                    Id = m.Id,
                    ManufacturerPartNumber = m.ManufacturerPartNumber,
                    Price = m.Price,
                    StockQuantity = m.StockQuantity,
                    ProductId = model.Id,
                    StokCode = m.StokCode,
                }).ToList();

                await _productService.UpdateProductAsync(updateDto);

                var productVariantAttributes = await _productVariantAttributeService.GetAllAsync(model.Id);

                var deletedProductVariantAttributes = productVariantAttributes.Where(m => !model.SelectedProductAttributeIds.Contains(m.ProductAttributeId)).ToList();
                foreach (var item in deletedProductVariantAttributes)
                {
                    await _productVariantAttributeService.DeleteAsync(item.Id);
                }

                foreach (var item in model.SelectedProductAttributeIds)
                {
                    var exist = productVariantAttributes.Where(m => m.ProductAttributeId == item).FirstOrDefault();

                    if (exist == null)
                    {
                        CreateProductVariantAttributeDto createProductAttributeMappingDto = new CreateProductVariantAttributeDto
                        {
                            ProductId = model.Id,
                            ProductAttributeId = item,
                            DisplayOrder = 0,
                            AttributeControlTypeId = 0
                        };

                        await _productVariantAttributeService.AddAsync(createProductAttributeMappingDto);
                    }
                }

                return Json(new { success = true });
            }

            await PrepareProductModel(model, null);
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int productId)
        {
            try
            {
                await _productService.DeleteProductAsync(productId);
                return Json(new { success = true });
            }
            catch (Exception ex)
            {

                return Json(new { success = false, message = $"{ex.Message}" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> ProductList([FromBody] GridCommand gridCommand)
        {
            var result = await _productService.GetPagedAsync(gridCommand);

            return Json(new
            {
                draw = gridCommand.Draw,
                recordsTotal = result.TotalCount,
                recordsFiltered = result.TotalCount,
                data = result.Items
            });
        }
        #endregion

        #region Product Categories

        [HttpGet]
        public async Task<IActionResult> LoadTabCategories(int productId)
        {
            ViewBag.ProductId = productId;
            var productCategories = await _productCategoryMappingService.GetByProductWithCategoryAsync(productId);

            return PartialView("_CreateOrUpdate.Categories", productCategories.Select(x => new ProductCategoryViewModel
            {
                Id = x.Id,
                CategoryId = x.CategoryId,
                CategoryBreadcrumb = x.CategoryBreadcrumb,
                ProductId = x.ProductId,
                DisplayOrder = x.DisplayOrder,
            }).ToList());
        }

        [HttpGet]
        public IActionResult ProductCategoryCreatePopup(int productId)
        {
            ProductCategoryViewModel model = new ProductCategoryViewModel();
            model.ProductId = productId;

            return PartialView("_ProductCategoryCreatePopup");
        }

        [HttpPost]
        public async Task<IActionResult> ProductCategoryInsert([FromBody] ProductCategoryViewModel model)
        {
            if (ModelState.IsValid)
            {
                CreateProductCategoryDto createProductCategory = new CreateProductCategoryDto();
                createProductCategory.ProductId = model.ProductId;
                createProductCategory.CategoryId = model.CategoryId;
                createProductCategory.DisplayOrder = model.DisplayOrder;

                await _productCategoryMappingService.CreateProductCategoryAsync(createProductCategory);
                return Json(new { success = true });
            }
            return Json(new { success = false, errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage) });
        }

        [HttpPost]
        public async Task<IActionResult> ProductCategoryDelete(int id)
        {
            try
            {
                await _productCategoryMappingService.DeleteProductCategoryAsync(id);
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }

        }

        #endregion

        #region Product SpecificationAttribute

        [HttpGet]
        public async Task<IActionResult> LoadTabSpecificationAttribute(int productId)
        {
            ViewBag.ProductId = productId;
            var productSpecificationAttribute = await _productSpecificationAttributeMappingService.GetSpecificationAttributeByProductId(productId);

            return PartialView("_CreateOrUpdate.SpecificationAttributes", productSpecificationAttribute.Select(x => new ProductSpecificationAttributeViewModel
            {
                Id = x.Id,
                DisplayOrder = x.DisplayOrder,
                ProductId = x.ProductId,
                SpecificationAttributeOption = new SpecificationAttributeOptionViewModel
                {
                    Id = x.SpecificationAttributeOption.Id,
                    Name = x.SpecificationAttributeOption.Name,
                    SpecificationAttributeId = x.SpecificationAttributeOption.SpecificationAttributeId,
                    DisplayOrder = x.SpecificationAttributeOption.DisplayOrder,
                }
            }).ToList());
        }
        #endregion

        #region Product Pictures
        [HttpGet]
        public async Task<IActionResult> LoadTabImages(int productId)
        {
            ViewBag.ProductId = productId;
            var productMediaFiles = await _productImageMappingService.GetAllAsync(productId);
            var model = productMediaFiles.Select(m => new ProductMediaFileViewModel()
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

            return PartialView("_CreateOrUpdate.Pictures", model);
        }

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


                        var productPicture = new CreateProductMediaFileDto
                        {
                            MediaFileId = mediaFileId,
                            ProductId = entityId,
                            DisplayOrder = i
                        };

                        var productMediaFile = await _productImageMappingService.AddAsync(productPicture);

                        // İsteğe bağlı olarak frontend’e dönecek bilgi
                        var respObj = new
                        {
                            MediaFileId = mediaFileId,
                            ProductMediaFileId = productMediaFile.Id,
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
            try
            {
                await _productImageMappingService.DeleteAsync(id);
                return StatusCode((int)HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
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

                        var productPicture = await _productImageMappingService.GetByPictureIdSortAsync(pictureId, entityId);

                        if (productPicture != null)
                        {
                            productPicture.DisplayOrder = i;

                            response.Add(new
                            {
                                productPicture.DisplayOrder,
                                productPicture.MediaFileId,
                                EntityMediaId = productPicture.Id
                            });

                            await _productImageMappingService.UpdateAsync(new UpdateProductMediaFileDto
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
                var productIntegration = await _productIntegrationService.GetByProductAndIntegrationSystemAsync(product.Id, integrationSystemId);
                if (productIntegration == null)
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

        [HttpGet]
        public async Task<IActionResult> ProductIntegrationDialog(ProductIntegrationDialogViewModel model)
        {
            var product = await _productService.GetProductByIdAsync(model.ProductId);
            var integrationSystem = await _integrationSystemService.GetByIdAsync(model.IntegrationSystemId);
            if (integrationSystem == null)
            {
                return NotFound();
            }


            if (integrationSystem.IntegrationSystemType == Domain.Enums.IntegrationSystemType.Commerce)
            {
                var commerceType = integrationSystem.IntegrationSystemParameters.Where(m => m.Key == "CommerceType").Select(m => m.Value).FirstOrDefault();


                if (model.ProductIntegrationId == 0)
                {
                    var createModel = new SmartstoreProductIntegrationViewModel()
                    {
                        Id = 0,
                        ProductId = product.Id,
                        IntegrationSystemId = model.IntegrationSystemId,
                        IntegrationSystemName = integrationSystem.Name,
                        ProductName = product.Name,
                        ProductCode = product.Code,
                        CommerceType = commerceType,
                        ProductMainPicture = product.ProductMediaFiles.Where(x => x.MediaFileId == product.MainPictureId).Select(m => m.MediaFile).FirstOrDefault()?.Url,
                        Price = product.Price,
                        IntegrationCode = product.Code,
                        Active = true,
                    };

                    return PartialView($"_IntegrationDialog.Commerce.{commerceType}", createModel);
                }
                else
                {
                    var existingProductIntegration = await _productIntegrationService.GetByIdAsync(model.ProductIntegrationId);
                    var createModel = new SmartstoreProductIntegrationViewModel
                    {
                        Id = existingProductIntegration.Id,
                        ProductId = existingProductIntegration.ProductId,
                        IntegrationSystemId = existingProductIntegration.IntegrationSystemId,
                        IntegrationSystemName = integrationSystem.Name,
                        CommerceType = commerceType,
                        IntegrationCode = existingProductIntegration?.IntegrationCode,
                        Price = existingProductIntegration?.Price ?? 0m,
                        ProductName = product.Name,
                        ProductCode = product.Code,
                        Active = existingProductIntegration.Active,
                        ProductMainPicture = product.ProductMediaFiles.Where(x => x.MediaFileId == product.MainPictureId).Select(m => m.MediaFile).FirstOrDefault()?.Url
                    };

                    if (!string.IsNullOrEmpty(existingProductIntegration.Custom))
                    {
                        createModel.Custom = JsonConvert.DeserializeObject<SmartstoreProductIntegrationCustomViewModel>(existingProductIntegration.Custom) ?? new SmartstoreProductIntegrationCustomViewModel();
                    }

                    return PartialView($"_IntegrationDialog.Commerce.{commerceType}", createModel);
                }
            }

            if (integrationSystem.IntegrationSystemType == Domain.Enums.IntegrationSystemType.Marketplace)
            {
                var marketplaceType = integrationSystem.IntegrationSystemParameters.Where(m => m.Key == "MarketplaceType").Select(m => m.Value).FirstOrDefault();
                if (model.ProductIntegrationId == 0)
                {
                    var createModel = new TrendyolProductIntegrationViewModel()
                    {
                        Id = 0,
                        ProductId = product.Id,
                        IntegrationSystemId = model.IntegrationSystemId,
                        IntegrationSystemName = integrationSystem.Name,
                        MarketplaceType = marketplaceType,
                        ProductName = product.Name,
                        ProductCode = product.Code,
                        ProductMainPicture = product.ProductMediaFiles.Where(x => x.MediaFileId == product.MainPictureId).Select(m => m.MediaFile).FirstOrDefault()?.Url,
                        ProductVariantAttributeCombinationId = null,
                        Price = product.Price,
                        IntegrationCode = product.Code,
                        Active = true,
                    };

                    var productVariantAttributeCombinations = product.ProductVariantAttributeCombinations.Select(m => new ProductVariantAttributeCombinationViewModel()
                    {
                        Id = m.Id,
                        ProductId = m.Id,
                        Gtin = m.Gtin,
                        ManufacturerPartNumber = m.ManufacturerPartNumber,
                        Price = m.Price,
                        StockQuantity = m.StockQuantity,
                        StokCode = m.StokCode,
                        ProductVariantAttributeSelections = JsonConvert.DeserializeObject<List<ProductVariantAttributeSelection>>(m.RawAttribute) ?? new List<ProductVariantAttributeSelection>(),
                    }).ToList();

                    foreach (var item in productVariantAttributeCombinations)
                    {
                        item.Name = await _productAttributeFormatter.FormatAttributesAsync(item.ProductVariantAttributeSelections);
                    }
                    ViewBag.ProductVariantAttributeCombinations = productVariantAttributeCombinations.Select(m => new SelectListItem()
                    {
                        Text = m.Name,
                        Value = m.Id.ToString()
                    });

                    return PartialView($"_IntegrationDialog.Marketplace.{marketplaceType}", createModel);
                }
                else
                {
                    TrendyolApiContext context = new TrendyolApiContext
                    {
                        SupplierId = integrationSystem.IntegrationSystemParameters.Where(m => m.Key == "SupplierId").Select(m => m.Value).FirstOrDefault() ?? "",
                        ApiUser = integrationSystem.IntegrationSystemParameters.Where(m => m.Key == "ApiUser").Select(m => m.Value).FirstOrDefault() ?? "",
                        ApiPassword = integrationSystem.IntegrationSystemParameters.Where(m => m.Key == "ApiPassword").Select(m => m.Value).FirstOrDefault() ?? "",
                    };
                    var existingProductIntegration = await _productIntegrationService.GetByIdAsync(model.ProductIntegrationId);
                    var existingTrendyolProduct = await _trenyolService.GetProductWithBarcodeAsync(context, existingProductIntegration.IntegrationCode);

                    var createModel = new TrendyolProductIntegrationViewModel
                    {
                        Id = existingProductIntegration.Id,
                        ProductId = existingProductIntegration.ProductId,
                        IntegrationSystemId = existingProductIntegration.IntegrationSystemId,
                        IntegrationSystemName = integrationSystem.Name,
                        MarketplaceType = marketplaceType,
                        IntegrationCode = existingProductIntegration?.IntegrationCode,
                        Price = existingProductIntegration?.Price ?? 0m,
                        ProductName = product.Name,
                        ProductCode = product.Code,
                        Active = existingProductIntegration.Active,
                        ProductMainPicture = product.ProductMediaFiles.Where(x => x.MediaFileId == product.MainPictureId).Select(m => m.MediaFile).FirstOrDefault()?.Url,
                        MarketplaceLink = existingTrendyolProduct == null ? "#" : existingTrendyolProduct.productUrl,
                        ProductVariantAttributeCombinationId = existingProductIntegration.ProductVariantAttributeCombinationId,
                    };

                    var productVariantAttributeCombinations = product.ProductVariantAttributeCombinations.Select(m => new ProductVariantAttributeCombinationViewModel()
                    {
                        Id = m.Id,
                        ProductId = m.Id,
                        Gtin = m.Gtin,
                        ManufacturerPartNumber = m.ManufacturerPartNumber,
                        Price = m.Price,
                        StockQuantity = m.StockQuantity,
                        StokCode = m.StokCode,
                        ProductVariantAttributeSelections = JsonConvert.DeserializeObject<List<ProductVariantAttributeSelection>>(m.RawAttribute) ?? new List<ProductVariantAttributeSelection>(),
                    }).ToList();

                    foreach (var item in productVariantAttributeCombinations)
                    {
                        item.Name = await _productAttributeFormatter.FormatAttributesAsync(item.ProductVariantAttributeSelections);
                    }
                    ViewBag.ProductVariantAttributeCombinations = productVariantAttributeCombinations.Select(m => new SelectListItem()
                    {
                        Text = m.Name,
                        Value = m.Id.ToString()
                    });

                    if (!string.IsNullOrEmpty(existingProductIntegration.Custom))
                    {
                        createModel.Custom = JsonConvert.DeserializeObject<TrednyolProductIntegrationCustomViewModel>(existingProductIntegration.Custom) ?? new TrednyolProductIntegrationCustomViewModel();
                    }

                    return PartialView($"_IntegrationDialog.Marketplace.{marketplaceType}", createModel);
                }
            }

            return NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> CreateOrUpdateProductIntegrationSmartstore(SmartstoreProductIntegrationViewModel model)
        {

            try
            {
                var existingProductIntegration = await _productIntegrationService.GetByIntegrationSystemAndCodeAsync(model.IntegrationSystemId, model.IntegrationCode);
                if (existingProductIntegration != null)
                {
                    if (existingProductIntegration.ProductId != model.ProductId)
                    {
                        var product = await _productService.GetProductByIdAsync(existingProductIntegration.ProductId);
                        if (product == null)
                        {
                            return Json(new { success = false, message = $"Bu entegrasyon sistemi ve kod kombinasyonu zaten mevcut. Ürün Adı: Bulunamadı" });
                        }

                        return Json(new { success = false, message = $"Bu entegrasyon sistemi ve kod kombinasyonu zaten mevcut. Ürün Adı: {product.Name}" });
                    }

                }

                var productIntegration = await _productIntegrationService.GetByIdAsync(model.Id);
                if (productIntegration == null || model.Id == 0)
                {
                    var createProductIntegration = new CreateProductIntegrationDto();
                    createProductIntegration.IntegrationCode = model.IntegrationCode;
                    createProductIntegration.Price = model.Price;
                    createProductIntegration.ProductId = model.ProductId;
                    createProductIntegration.IntegrationSystemId = model.IntegrationSystemId;
                    createProductIntegration.Active = model.Active;
                    createProductIntegration.LastSyncDate = null;
                    createProductIntegration.IsSync = false;
                    createProductIntegration.Custom = JsonConvert.SerializeObject(model.Custom);
                    await _productIntegrationService.CreateProductIntegrationAsync(createProductIntegration);
                }
                else
                {
                    var updateProductIntegration = new UpdateProductIntegrationDto();
                    updateProductIntegration.Id = productIntegration.Id;
                    updateProductIntegration.IntegrationCode = model.IntegrationCode;
                    updateProductIntegration.Price = model.Price;
                    updateProductIntegration.ProductId = model.ProductId;
                    updateProductIntegration.IntegrationSystemId = model.IntegrationSystemId;
                    updateProductIntegration.Active = model.Active;
                    updateProductIntegration.LastSyncDate = null;
                    updateProductIntegration.IsSync = false;
                    updateProductIntegration.Custom = JsonConvert.SerializeObject(model.Custom);

                    await _productIntegrationService.UpdateProductIntegrationAsync(updateProductIntegration);
                }

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Bir hata oluştu: " + ex.Message });
            }


        }
        [HttpPost]
        public async Task<IActionResult> CreateOrUpdateProductIntegrationTrendyol(TrendyolProductIntegrationViewModel model)
        {

            try
            {
                var integrationSystem = await _integrationSystemService.GetByIdAsync(model.IntegrationSystemId);
                if (integrationSystem == null)
                {
                    return Json(new { success = false, message = $"Entegrasyon sistemi bulunamadı" });
                }

                var existingProductIntegration = await _productIntegrationService.GetByIntegrationSystemAndCodeAsync(model.IntegrationSystemId, model.IntegrationCode);
                if (existingProductIntegration != null)
                {
                    if (existingProductIntegration.ProductId != model.ProductId)
                    {
                        var product = await _productService.GetProductByIdAsync(existingProductIntegration.ProductId);
                        if (product == null)
                        {
                            return Json(new { success = false, message = $"Bu entegrasyon sistemi ve kod kombinasyonu zaten mevcut. Ürün Adı: Bulunamadı" });
                        }
                        return Json(new { success = false, message = $"Bu entegrasyon sistemi ve kod kombinasyonu zaten mevcut. Ürün Adı: {product.Name}" });
                    }

                }

                TrendyolApiContext context = new TrendyolApiContext
                {
                    SupplierId = integrationSystem.IntegrationSystemParameters.Where(m => m.Key == "SupplierId").Select(m => m.Value).FirstOrDefault() ?? "",
                    ApiUser = integrationSystem.IntegrationSystemParameters.Where(m => m.Key == "ApiUser").Select(m => m.Value).FirstOrDefault() ?? "",
                    ApiPassword = integrationSystem.IntegrationSystemParameters.Where(m => m.Key == "ApiPassword").Select(m => m.Value).FirstOrDefault() ?? "",
                };

                var existingTrendyolProduct = await _trenyolService.GetProductWithBarcodeAsync(context, model.IntegrationCode);
                if (existingTrendyolProduct == null)
                {
                    return Json(new { success = false, message = $"Trendyol üzerinde bu barkoda sahip bir ürün bulunamadı. Barkod: {model.IntegrationCode}" });
                }

                var productIntegration = await _productIntegrationService.GetByIdAsync(model.Id);
                if (productIntegration == null || model.Id == 0)
                {
                    var createProductIntegration = new CreateProductIntegrationDto();
                    createProductIntegration.IntegrationCode = model.IntegrationCode;
                    createProductIntegration.Price = model.Price;
                    createProductIntegration.ProductId = model.ProductId;
                    createProductIntegration.ProductVariantAttributeCombinationId = model.ProductVariantAttributeCombinationId;
                    createProductIntegration.IntegrationSystemId = model.IntegrationSystemId;
                    createProductIntegration.Active = model.Active;
                    createProductIntegration.LastSyncDate = null;
                    createProductIntegration.IsSync = false;
                    createProductIntegration.Custom = JsonConvert.SerializeObject(model.Custom);
                    await _productIntegrationService.CreateProductIntegrationAsync(createProductIntegration);
                }
                else
                {
                    var updateProductIntegration = new UpdateProductIntegrationDto();
                    updateProductIntegration.Id = productIntegration.Id;
                    updateProductIntegration.IntegrationCode = model.IntegrationCode;
                    updateProductIntegration.Price = model.Price;
                    updateProductIntegration.ProductId = model.ProductId;
                    updateProductIntegration.ProductVariantAttributeCombinationId = model.ProductVariantAttributeCombinationId;
                    updateProductIntegration.IntegrationSystemId = model.IntegrationSystemId;
                    updateProductIntegration.Active = model.Active;
                    updateProductIntegration.LastSyncDate = null;
                    updateProductIntegration.IsSync = false;
                    updateProductIntegration.Custom = JsonConvert.SerializeObject(model.Custom);

                    await _productIntegrationService.UpdateProductIntegrationAsync(updateProductIntegration);
                }

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Bir hata oluştu: " + ex.Message });
            }


        }
        [HttpPost]
        public async Task<IActionResult> DeleteProductIntegration(int id)
        {
            try
            {
                await _productIntegrationService.DeleteProductIntegrationAsync(id);
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
        #endregion

        #region ProductVariantAttributeCombination
        [HttpPost]
        public async Task<IActionResult> ProductVariantAttributeDelete(int id)
        {
            try
            {
                var isExisting = await _productVariantAttributeCombinationService.GetByIdAsync(id);
                if (isExisting is not null)
                {
                    await _productVariantAttributeCombinationService.DeleteAsync(id);
                    return Json(new { success = true });
                }
                else
                {
                    return Json(new { success = false, message = "Varyant bulunamadı." });
                }

            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
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
                model.SelectedProductAttributeIds = product.ProductVariantAttributes.Select(x => x.ProductAttributeId).ToArray();
                model.ProductVariantAttributes = product.ProductVariantAttributes.Select(m => new ProductVariantAttributeViewModel()
                {
                    AttributeControlTypeId = m.AttributeControlTypeId,
                    DisplayOrder = m.DisplayOrder,
                    Id = m.Id,
                    IsRequried = m.IsRequried,
                    ProductAttribute = m.ProductAttribute.Name,
                    ProductAttributeId = m.ProductAttributeId,
                    ProductId = m.ProductId,
                    ProductVariantAttributeValues = m.ProductVariantAttributeValues.Select(x => new ProductVariantAttributeValueViewModel()
                    {
                        Id = x.Id,
                        Name = x.Name,
                        ProductVariantAttributeId = x.ProductVariantAttributeId,
                    }).ToList()
                }).ToList();
                model.ProductVariantAttributeCombinations = product.ProductVariantAttributeCombinations.Select(m => new ProductVariantAttributeCombinationViewModel()
                {
                    Id = m.Id,
                    ProductVariantAttributeSelections = JsonConvert.DeserializeObject<List<ProductVariantAttributeSelection>>(m.RawAttribute) ?? new List<ProductVariantAttributeSelection>(),
                    Gtin = m.Gtin,
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
                    Value = x.Id.ToString(),
                    Selected = model.SelectedProductAttributeIds.Contains(x.Id)
                }).ToList();
            }

            var brands = await _brandService.GetAllAsync();

            ViewBag.Brands = brands.Select(m => new SelectListItem()
            {
                Text = m.Name,
                Value = m.Id.ToString()
            }).ToList();
            ViewBag.Currencies = new List<SelectListItem>
            {
                new SelectListItem { Text = "TL", Value = "TL" },
                new SelectListItem { Text = "USD", Value = "USD" },
                new SelectListItem { Text = "EUR", Value = "EUR" },
                new SelectListItem { Text = "GBP", Value = "GBP" },
                new SelectListItem { Text = "JPY", Value = "JPY" }
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
