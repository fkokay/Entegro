using Entegro.Application.DTOs.Common;
using Entegro.Application.DTOs.IntegrationSystem;
using Entegro.Application.DTOs.Marketplace.Hepsiburada;
using Entegro.Application.DTOs.Marketplace.N11;
using Entegro.Application.DTOs.Marketplace.Pazarama;
using Entegro.Application.DTOs.Marketplace.Trendyol;
using Entegro.Application.DTOs.Product;
using Entegro.Application.DTOs.ProductCategory;
using Entegro.Application.DTOs.ProductIntegration;
using Entegro.Application.DTOs.ProductMediaFile;
using Entegro.Application.DTOs.ProductVariantAttribute;
using Entegro.Application.DTOs.ProductVariantAttributeCombination;
using Entegro.Application.Interfaces.Services.Base;
using Entegro.Application.Interfaces.Services.Marketplace;
using Entegro.Domain.Enums;
using Entegro.Web.Helpers;
using Entegro.Web.Models.Catalog.Attributes;
using Entegro.Web.Models.Catalog.Products;
using Entegro.Web.Models.Catalog.ProductSpecificationAttribute;
using Entegro.Web.Models.Content;
using Entegro.Web.Models.Integration;
using Entegro.Web.Models.Integration.Common;
using Entegro.Web.Models.Integration.Marketplace;
using MapsterMapper;
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
        private readonly IProductMediaFileMappingService _productMediaFileMappingService;
        private readonly IIntegrationSystemService _integrationSystemService;
        private readonly IProductIntegrationService _productIntegrationService;
        private readonly IProductAttributeFormatter _productAttributeFormatter;
        private readonly IProductVariantAttributeCombinationService _productVariantAttributeCombinationService;
        private readonly IProductVariantAttributeValueService _productVariantAttributeValueService;
        private readonly ICategoryService _categoryService;
        private readonly ITrendyolService _trenyolService;
        private readonly IN11Service _n11Service;
        private readonly IPazaramaService _pazaramaService;
        private readonly IHepsiburadaService _hepsiburadaService;
        private readonly IProductSpecificationAttributeMappingService _productSpecificationAttributeMappingService;
        private readonly IMapper _mapper;
        public ProductController(
            IProductService productService,
            IProductCategoryService productCategoryMappingService,
            IBrandService brandService,
            IProductAttributeService productAttributeService,
            IProductVariantAttributeService productVariantAttributeService,
            IProductMediaFileMappingService productMediaFileMappingService,
            IIntegrationSystemService integrationSystemService,
            IProductIntegrationService productIntegrationService,
            IProductAttributeFormatter productAttributeFormatter,
            ICategoryService categoryService,
            ITrendyolService trendyolService,
            IProductSpecificationAttributeMappingService productSpecificationAttributeMappingService,
            IProductVariantAttributeCombinationService productVariantAttributeCombinationService,
            IN11Service n11Service,
            IPazaramaService pazaramaService,
            IHepsiburadaService hepsiburadaService,
            IMapper mapper,
            IProductVariantAttributeValueService productVariantAttributeValueService)
        {
            _productService = productService ?? throw new ArgumentNullException(nameof(productService));
            _productCategoryMappingService = productCategoryMappingService ?? throw new ArgumentNullException(nameof(productCategoryMappingService));
            _brandService = brandService ?? throw new ArgumentNullException(nameof(brandService));
            _productAttributeService = productAttributeService ?? throw new ArgumentNullException(nameof(productAttributeService));
            _productVariantAttributeService = productVariantAttributeService ?? throw new ArgumentNullException(nameof(productVariantAttributeService));
            _productMediaFileMappingService = productMediaFileMappingService ?? throw new ArgumentNullException(nameof(productMediaFileMappingService));
            _integrationSystemService = integrationSystemService ?? throw new ArgumentNullException(nameof(integrationSystemService));
            _productIntegrationService = productIntegrationService;
            _productAttributeFormatter = productAttributeFormatter;
            _trenyolService = trendyolService;
            _categoryService = categoryService;
            _productSpecificationAttributeMappingService = productSpecificationAttributeMappingService;
            _productVariantAttributeCombinationService = productVariantAttributeCombinationService;
            _n11Service = n11Service;
            _pazaramaService = pazaramaService;
            _hepsiburadaService = hepsiburadaService;
            _mapper = mapper;
            _productVariantAttributeValueService = productVariantAttributeValueService;
        }

        #region Product list / create / edit / delete
        [HttpPost]
        public async Task<IActionResult> AllProduct([FromForm] int page = 1, [FromForm] string term = "")
        {
            var products = await _productService.GetProductsAsync(page, term);

            var query = products.Items.Select(c => new
            {
                id = c.Id.ToString(),
                text = c.Name,
                code = c.Code,
            });

            var mainList = query.ToList();

            return Json(new
            {
                results = mainList,
                pagination = new { more = products.HasNextPage }
            });
        }

        [HttpPost]
        public async Task<IActionResult> GetProductVariantAttributeCombination(int productId)
        {
            var product = await _productService.GetProductByIdAsync(productId);

            var productVariantAttributeCombinations = product.ProductVariantAttributeCombinations.Select(m => new ProductVariantAttributeCombinationModel()
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

            return Json(productVariantAttributeCombinations);
        }

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
            ProductModel model = new ProductModel();
            await PrepareProductModel(model, null);
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Create(ProductModel model)
        {
            if (ModelState.IsValid)
            {
                var createDto = _mapper.Map<CreateProductDto>(model);
                await _productService.AddAsync(createDto);

                return Json(new { success = true });
            }

            await PrepareProductModel(model, null);
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {

            var product = await _productService.GetProductByIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            var model = _mapper.Map<ProductModel>(product);

            await PrepareProductModel(model, product);
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(ProductModel model)
        {
            if (ModelState.IsValid)
            {
                var updateDto = _mapper.Map<UpdateProductDto>(model);
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
                    AssignedPictureIds = m.AssignedPictureIds,
                }).ToList();

                await _productService.UpdateAsync(updateDto);

                var productVariantAttributes = await _productVariantAttributeService.GetAllAsync(model.Id);



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
                await _productService.DeleteAsync(productId);
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
            var model = _mapper.Map<List<ProductCategoryModel>>(productCategories);

            return PartialView("_CreateOrUpdate.Categories", model);
        }

        [HttpGet]
        public IActionResult ProductCategoryCreatePopup(int productId)
        {
            ProductCategoryModel model = new ProductCategoryModel();
            model.ProductId = productId;

            return PartialView("_ProductCategoryCreatePopup");
        }

        [HttpPost]
        public async Task<IActionResult> ProductCategoryInsert([FromBody] ProductCategoryModel model)
        {
            if (ModelState.IsValid)
            {
                var createDto = _mapper.Map<CreateProductCategoryDto>(model);

                await _productCategoryMappingService.AddAsync(createDto);
                return Json(new { success = true });
            }
            return Json(new { success = false, errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage) });
        }

        [HttpPost]
        public async Task<IActionResult> ProductCategoryDelete(int id)
        {
            try
            {
                await _productCategoryMappingService.DeleteAsync(id);
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
            var productSpecificationAttributes = await _productSpecificationAttributeMappingService.GetSpecificationAttributeByProductId(productId);
            var model = _mapper.Map<List<ProductSpecificationAttributeModel>>(productSpecificationAttributes);


            return PartialView("_CreateOrUpdate.SpecificationAttributes", model);
        }
        #endregion

        #region Product Pictures
        [HttpGet]
        public async Task<IActionResult> LoadTabImages(int productId)
        {
            ViewBag.ProductId = productId;
            var productMediaFiles = await _productMediaFileMappingService.GetAllAsync(productId);
            var model = _mapper.Map<List<ProductMediaFileModel>>(productMediaFiles);
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

                        var productMediaFile = await _productMediaFileMappingService.AddAsync(productPicture);


                        // İsteğe bağlı olarak frontend’e dönecek bilgi
                        var respObj = new
                        {
                            MediaFileId = mediaFileId,
                            ProductMediaFileId = productMediaFile.Id,
                            DisplayOrder = i
                        };

                        response.Add(respObj);
                    }

                    int mainPictureId = response[0].MediaFileId;
                    await _productService.UpdateProductMainPictureIdAsync(entityId, mainPictureId);
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
                await _productMediaFileMappingService.DeleteAsync(id);
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

                        var productPicture = await _productMediaFileMappingService.GetByPictureIdSortAsync(pictureId, entityId);

                        if (productPicture != null)
                        {
                            productPicture.DisplayOrder = i;

                            response.Add(new
                            {
                                productPicture.DisplayOrder,
                                productPicture.MediaFileId,
                                EntityMediaId = productPicture.Id
                            });

                            await _productMediaFileMappingService.UpdateAsync(new UpdateProductMediaFileDto
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
                    await _productIntegrationService.AddAsync(new CreateProductIntegrationDto
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
        public async Task<IActionResult> ProductIntegrationDialog(ProductIntegrationDialogModel model)
        {
            var product = await _productService.GetProductByIdAsync(model.ProductId);
            var integrationSystem = await _integrationSystemService.GetByIdAsync(model.IntegrationSystemId);
            if (integrationSystem == null)
            {
                return NotFound();
            }

            if (integrationSystem.IntegrationSystemType == Domain.Enums.IntegrationSystemType.Commerce)
            {
                var commerceType = integrationSystem.IntegrationSystemParameters
                    .FirstOrDefault(m => m.Key == "CommerceType")?.Value;

                return commerceType switch
                {
                    "Smartstore" => await ProductIntegrationSmartstoreDialog(model, product, integrationSystem),
                    _ => NotFound()
                };
            }
            if (integrationSystem.IntegrationSystemType == Domain.Enums.IntegrationSystemType.Marketplace)
            {
                var marketplaceType = integrationSystem.IntegrationSystemParameters
            .FirstOrDefault(p => p.Key == "MarketplaceType")?.Value;

                return marketplaceType switch
                {
                    "Trendyol" => await ProductIntegrationTrendyolDialog(model, product, integrationSystem),
                    "Idefix" => await ProductIntegrationIdefixDialog(model, product, integrationSystem),
                    "N11" => await ProductIntegrationN11Dialog(model, product, integrationSystem),
                    "CicekSepeti" => await ProductIntegrationCicekSepetiDialog(model, product, integrationSystem),
                    "Pazarama" => await ProductIntegrationPazaramaDialog(model, product, integrationSystem),
                    "Hepsiburada" => await ProductIntegrationHepsiburadaDialog(model, product, integrationSystem),
                    _ => NotFound()
                };
            }

            return NotFound();
        }

        #region Marketplace Dialogs
        private async Task<IActionResult> ProductIntegrationHepsiburadaDialog(ProductIntegrationDialogModel model, ProductDto? product, IntegrationSystemDto integrationSystem)
        {
            var marketplaceType = "Hepsiburada";

            if (model.ProductIntegrationId == 0)
            {
                var createModel = new HepsiburadaProductIntegrationModel
                {
                    Id = 0,
                    ProductId = product.Id,
                    IntegrationSystemId = model.IntegrationSystemId,
                    IntegrationSystemName = integrationSystem.Name,
                    MarketplaceType = marketplaceType,
                    ProductName = product.Name,
                    ProductCode = product.Code,
                    ProductMainPicture = product.ProductMediaFiles.FirstOrDefault(x => x.MediaFileId == product.MainPictureId)?.MediaFile?.Url,
                    ProductVariantAttributeCombinationId = null,
                    Price = product.Price,
                    IntegrationCode = product.Code,
                    Active = true,
                };

                var productVariantAttributeCombinations = product.ProductVariantAttributeCombinations.Select(m => new ProductVariantAttributeCombinationModel()
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
                HepsiburadaApiContext context = new HepsiburadaApiContext
                {
                    ApiUser = integrationSystem.IntegrationSystemParameters.FirstOrDefault(m => m.Key == "ApiUser")?.Value ?? "",
                    ApiPassword = integrationSystem.IntegrationSystemParameters.FirstOrDefault(m => m.Key == "ApiPassword")?.Value ?? "",
                    MerchantId = integrationSystem.IntegrationSystemParameters.FirstOrDefault(m => m.Key == "MerchantId")?.Value ?? "",
                    UserAgent = integrationSystem.IntegrationSystemParameters.FirstOrDefault(m => m.Key == "UserAgent")?.Value ?? "",
                };


                var existingProductIntegration = await _productIntegrationService.GetByIdAsync(model.ProductIntegrationId);
                var existingHepsiburadaProduct = await _hepsiburadaService.GetProductWithMerchantSkuAsync(context, existingProductIntegration.IntegrationCode);

                var createModel = new HepsiburadaProductIntegrationModel
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
                    ProductMainPicture = product.ProductMediaFiles.FirstOrDefault(x => x.MediaFileId == product.MainPictureId)?.MediaFile?.Url,
                    MarketplaceLink = "#",
                    ProductVariantAttributeCombinationId = existingProductIntegration.ProductVariantAttributeCombinationId,
                };

                if (!string.IsNullOrEmpty(existingProductIntegration.Custom))
                {
                    createModel.Custom = JsonConvert.DeserializeObject<HepsiburadaProductIntegrationCustomModel>(existingProductIntegration.Custom);
                }

                var productVariantAttributeCombinations = product.ProductVariantAttributeCombinations.Select(m => new ProductVariantAttributeCombinationModel()
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
        }

        private async Task<IActionResult> ProductIntegrationPazaramaDialog(ProductIntegrationDialogModel model, ProductDto? product, IntegrationSystemDto integrationSystem)
        {
            var marketplaceType = "Pazarama";

            if (model.ProductIntegrationId == 0)
            {
                var createModel = new PazaramaProductIntegrationModel
                {
                    Id = 0,
                    ProductId = product.Id,
                    IntegrationSystemId = model.IntegrationSystemId,
                    IntegrationSystemName = integrationSystem.Name,
                    MarketplaceType = marketplaceType,
                    ProductName = product.Name,
                    ProductCode = product.Code,
                    ProductMainPicture = product.ProductMediaFiles.FirstOrDefault(x => x.MediaFileId == product.MainPictureId)?.MediaFile?.Url,
                    ProductVariantAttributeCombinationId = null,
                    Price = product.Price,
                    IntegrationCode = product.Code,
                    Active = true,
                };

                var productVariantAttributeCombinations = product.ProductVariantAttributeCombinations.Select(m => new ProductVariantAttributeCombinationModel()
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
                PazaramaApiContext context = new PazaramaApiContext
                {
                    ClientId = integrationSystem.IntegrationSystemParameters.FirstOrDefault(m => m.Key == "ClientId")?.Value ?? "",
                    ClientSecret = integrationSystem.IntegrationSystemParameters.FirstOrDefault(m => m.Key == "ClientSecret")?.Value ?? "",

                };

                var existingProductIntegration = await _productIntegrationService.GetByIdAsync(model.ProductIntegrationId);
                var existingPazaramaProduct = await _pazaramaService.GetProductWithStockCodeAsync(context, existingProductIntegration.IntegrationCode);

                var createModel = new PazaramaProductIntegrationModel
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
                    ProductMainPicture = product.ProductMediaFiles.FirstOrDefault(x => x.MediaFileId == product.MainPictureId)?.MediaFile?.Url,
                    MarketplaceLink = "#",
                    ProductVariantAttributeCombinationId = existingProductIntegration.ProductVariantAttributeCombinationId,
                };

                if (!string.IsNullOrEmpty(existingProductIntegration.Custom))
                {
                    createModel.Custom = JsonConvert.DeserializeObject<PazaramaProductIntegrationCustomModel>(existingProductIntegration.Custom);
                }

                var productVariantAttributeCombinations = product.ProductVariantAttributeCombinations.Select(m => new ProductVariantAttributeCombinationModel()
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
        }

        private async Task<IActionResult> ProductIntegrationCicekSepetiDialog(ProductIntegrationDialogModel model, ProductDto? product, IntegrationSystemDto integrationSystem)
        {
            var marketplaceType = "CicekSepeti";

            if (model.ProductIntegrationId == 0)
            {
                var createModel = new CicekSepetiProductIntegrationModel
                {
                    Id = 0,
                    ProductId = product.Id,
                    IntegrationSystemId = model.IntegrationSystemId,
                    IntegrationSystemName = integrationSystem.Name,
                    MarketplaceType = marketplaceType,
                    ProductName = product.Name,
                    ProductCode = product.Code,
                    ProductMainPicture = product.ProductMediaFiles.FirstOrDefault(x => x.MediaFileId == product.MainPictureId)?.MediaFile?.Url,
                    ProductVariantAttributeCombinationId = null,
                    Price = product.Price,
                    IntegrationCode = product.Code,
                    Active = true,
                };

                var productVariantAttributeCombinations = product.ProductVariantAttributeCombinations.Select(m => new ProductVariantAttributeCombinationModel()
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


                var existingProductIntegration = await _productIntegrationService.GetByIdAsync(model.ProductIntegrationId);
                var existingCicekSepetiProduct = "";

                var createModel = new CicekSepetiProductIntegrationModel
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
                    ProductMainPicture = product.ProductMediaFiles.FirstOrDefault(x => x.MediaFileId == product.MainPictureId)?.MediaFile?.Url,
                    MarketplaceLink = "existingCicekSepetiProduct?.productUrl ?? #,",
                    ProductVariantAttributeCombinationId = existingProductIntegration.ProductVariantAttributeCombinationId,
                };

                if (!string.IsNullOrEmpty(existingProductIntegration.Custom))
                {
                    createModel.Custom = JsonConvert.DeserializeObject<CicekSepetiProductIntegrationCustomModel>(existingProductIntegration.Custom);
                }

                var productVariantAttributeCombinations = product.ProductVariantAttributeCombinations.Select(m => new ProductVariantAttributeCombinationModel()
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
        }

        private async Task<IActionResult> ProductIntegrationN11Dialog(ProductIntegrationDialogModel model, ProductDto? product, IntegrationSystemDto integrationSystem)
        {
            var marketplaceType = "N11";

            if (model.ProductIntegrationId == 0)
            {
                var createModel = new N11ProductIntegrationModel
                {
                    Id = 0,
                    ProductId = product.Id,
                    IntegrationSystemId = model.IntegrationSystemId,
                    IntegrationSystemName = integrationSystem.Name,
                    MarketplaceType = marketplaceType,
                    ProductName = product.Name,
                    ProductCode = product.Code,
                    ProductMainPicture = product.ProductMediaFiles.FirstOrDefault(x => x.MediaFileId == product.MainPictureId)?.MediaFile?.Url,
                    ProductVariantAttributeCombinationId = null,
                    Price = product.Price,
                    IntegrationCode = product.Code,
                    Active = true,
                };

                var productVariantAttributeCombinations = product.ProductVariantAttributeCombinations.Select(m => new ProductVariantAttributeCombinationModel()
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

                N11ApiContext context = new N11ApiContext
                {
                    AppKey = integrationSystem.IntegrationSystemParameters.FirstOrDefault(p => p.Key == "AppKey")?.Value,
                    AppSecret = integrationSystem.IntegrationSystemParameters.FirstOrDefault(p => p.Key == "AppSecret")?.Value,
                };

                var existingProductIntegration = await _productIntegrationService.GetByIdAsync(model.ProductIntegrationId);
                //var existingN11Product = await _n11Service.GetProductWithN11CodeAsync(context, existingProductIntegration.IntegrationCode);


                var createModel = new N11ProductIntegrationModel
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
                    ProductMainPicture = product.ProductMediaFiles.FirstOrDefault(x => x.MediaFileId == product.MainPictureId)?.MediaFile?.Url,
                    MarketplaceLink = "#",
                    ProductVariantAttributeCombinationId = existingProductIntegration.ProductVariantAttributeCombinationId,
                };

                if (!string.IsNullOrEmpty(existingProductIntegration.Custom))
                {
                    createModel.Custom = JsonConvert.DeserializeObject<N11ProductIntegrationCustomModel>(existingProductIntegration.Custom);
                }

                var productVariantAttributeCombinations = product.ProductVariantAttributeCombinations.Select(m => new ProductVariantAttributeCombinationModel()
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
        }

        private async Task<IActionResult> ProductIntegrationIdefixDialog(ProductIntegrationDialogModel model, ProductDto? product, IntegrationSystemDto integrationSystem)
        {
            var marketplaceType = "Idefix";

            if (model.ProductIntegrationId == 0)
            {
                var createModel = new IdefixProductIntegrationModel
                {
                    Id = 0,
                    ProductId = product.Id,
                    IntegrationSystemId = model.IntegrationSystemId,
                    IntegrationSystemName = integrationSystem.Name,
                    MarketplaceType = marketplaceType,
                    ProductName = product.Name,
                    ProductCode = product.Code,
                    ProductMainPicture = product.ProductMediaFiles.FirstOrDefault(x => x.MediaFileId == product.MainPictureId)?.MediaFile?.Url,
                    ProductVariantAttributeCombinationId = null,
                    Price = product.Price,
                    IntegrationCode = product.Code,
                    Active = true,
                };

                var productVariantAttributeCombinations = product.ProductVariantAttributeCombinations.Select(m => new ProductVariantAttributeCombinationModel()
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


                var existingProductIntegration = await _productIntegrationService.GetByIdAsync(model.ProductIntegrationId);
                var existingIdefixProduct = "";

                var createModel = new IdefixProductIntegrationModel
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
                    ProductMainPicture = product.ProductMediaFiles.FirstOrDefault(x => x.MediaFileId == product.MainPictureId)?.MediaFile?.Url,
                    MarketplaceLink = "existingIdefixProduct?.productUrl ?? #,",
                    ProductVariantAttributeCombinationId = existingProductIntegration.ProductVariantAttributeCombinationId,
                };

                if (!string.IsNullOrEmpty(existingProductIntegration.Custom))
                {
                    createModel.Custom = JsonConvert.DeserializeObject<IdefixProductIntegrationCustomModel>(existingProductIntegration.Custom);
                }

                var productVariantAttributeCombinations = product.ProductVariantAttributeCombinations.Select(m => new ProductVariantAttributeCombinationModel()
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
        }

        private async Task<IActionResult> ProductIntegrationTrendyolDialog(ProductIntegrationDialogModel model, ProductDto product, IntegrationSystemDto integrationSystem)
        {
            var marketplaceType = "Trendyol";

            if (model.ProductIntegrationId == 0)
            {
                var createModel = new TrendyolProductIntegrationModel
                {
                    Id = 0,
                    ProductId = product.Id,
                    IntegrationSystemId = model.IntegrationSystemId,
                    IntegrationSystemName = integrationSystem.Name,
                    MarketplaceType = marketplaceType,
                    ProductName = product.Name,
                    ProductCode = product.Code,
                    ProductMainPicture = product.ProductMediaFiles.FirstOrDefault(x => x.MediaFileId == product.MainPictureId)?.MediaFile?.Url,
                    ProductVariantAttributeCombinationId = null,
                    Price = product.Price,
                    IntegrationCode = product.Code,
                    Active = true,
                };

                var productVariantAttributeCombinations = product.ProductVariantAttributeCombinations.Select(m => new ProductVariantAttributeCombinationModel()
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
                    SupplierId = integrationSystem.IntegrationSystemParameters.FirstOrDefault(m => m.Key == "SupplierId")?.Value ?? "",
                    ApiUser = integrationSystem.IntegrationSystemParameters.FirstOrDefault(m => m.Key == "ApiUser")?.Value ?? "",
                    ApiPassword = integrationSystem.IntegrationSystemParameters.FirstOrDefault(m => m.Key == "ApiPassword")?.Value ?? "",
                };

                var existingProductIntegration = await _productIntegrationService.GetByIdAsync(model.ProductIntegrationId);
                var existingTrendyolProduct = await _trenyolService.GetProductWithBarcodeAsync(context, existingProductIntegration.IntegrationCode);

                var createModel = new TrendyolProductIntegrationModel
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
                    ProductMainPicture = product.ProductMediaFiles.FirstOrDefault(x => x.MediaFileId == product.MainPictureId)?.MediaFile?.Url,
                    MarketplaceLink = existingTrendyolProduct?.productUrl ?? "#",
                    ProductVariantAttributeCombinationId = existingProductIntegration.ProductVariantAttributeCombinationId,
                };

                if (!string.IsNullOrEmpty(existingProductIntegration.Custom))
                {
                    createModel.Custom = JsonConvert.DeserializeObject<TrednyolProductIntegrationCustomModel>(existingProductIntegration.Custom);
                }

                var productVariantAttributeCombinations = product.ProductVariantAttributeCombinations.Select(m => new ProductVariantAttributeCombinationModel()
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
        }

        #endregion


        #region Commerce Dialogs
        private async Task<IActionResult> ProductIntegrationSmartstoreDialog(ProductIntegrationDialogModel model, ProductDto? product, IntegrationSystemDto? integrationSystem)
        {
            var commerceType = "Smartstore";
            if (model.ProductIntegrationId == 0)
            {
                var createModel = new SmartstoreProductIntegrationModel
                {
                    Id = 0,
                    ProductId = product.Id,
                    IntegrationSystemId = model.IntegrationSystemId,
                    IntegrationSystemName = integrationSystem.Name,
                    ProductName = product.Name,
                    ProductCode = product.Code,
                    CommerceType = commerceType,
                    ProductMainPicture = product.ProductMediaFiles.FirstOrDefault(x => x.MediaFileId == product.MainPictureId)?.MediaFile?.Url,
                    Price = product.Price,
                    IntegrationCode = product.Code,
                    Active = true,
                };

                return PartialView($"_IntegrationDialog.Commerce.{commerceType}", createModel);
            }
            else
            {
                var existingProductIntegration = await _productIntegrationService.GetByIdAsync(model.ProductIntegrationId);

                var createModel = new SmartstoreProductIntegrationModel
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
                    ProductMainPicture = product.ProductMediaFiles.FirstOrDefault(x => x.MediaFileId == product.MainPictureId)?.MediaFile?.Url
                };

                if (!string.IsNullOrEmpty(existingProductIntegration.Custom))
                {
                    createModel.Custom = JsonConvert.DeserializeObject<SmartstoreProductIntegrationCustomModel>(existingProductIntegration.Custom)
                        ?? new SmartstoreProductIntegrationCustomModel();
                }

                return PartialView($"_IntegrationDialog.Commerce.{commerceType}", createModel);
            }
        }

        #endregion

        [HttpPost]
        public async Task<IActionResult> CreateOrUpdateProductIntegrationSmartstore(SmartstoreProductIntegrationModel model)
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
                    await _productIntegrationService.AddAsync(createProductIntegration);
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

                    await _productIntegrationService.UpdateAsync(updateProductIntegration);
                }

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Bir hata oluştu: " + ex.Message });
            }


        }

        [HttpPost]
        public async Task<IActionResult> CreateOrUpdateProductIntegrationTrendyol(TrendyolProductIntegrationModel model)
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
                    await _productIntegrationService.AddAsync(createProductIntegration);
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

                    await _productIntegrationService.UpdateAsync(updateProductIntegration);
                }

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Bir hata oluştu: " + ex.Message });
            }


        }

        [HttpPost]
        public async Task<IActionResult> CreateOrUpdateProductIntegrationCicekSepeti(CicekSepetiProductIntegrationModel model)
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

                //TrendyolApiContext context = new TrendyolApiContext
                //{
                //    SupplierId = integrationSystem.IntegrationSystemParameters.Where(m => m.Key == "SupplierId").Select(m => m.Value).FirstOrDefault() ?? "",
                //    ApiUser = integrationSystem.IntegrationSystemParameters.Where(m => m.Key == "ApiUser").Select(m => m.Value).FirstOrDefault() ?? "",
                //    ApiPassword = integrationSystem.IntegrationSystemParameters.Where(m => m.Key == "ApiPassword").Select(m => m.Value).FirstOrDefault() ?? "",
                //};

                //var existingTrendyolProduct = await _trenyolService.GetProductWithBarcodeAsync(context, model.IntegrationCode);
                //if (existingTrendyolProduct == null)
                //{
                //    return Json(new { success = false, message = $"Trendyol üzerinde bu barkoda sahip bir ürün bulunamadı. Barkod: {model.IntegrationCode}" });
                //}

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
                    await _productIntegrationService.AddAsync(createProductIntegration);
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

                    await _productIntegrationService.UpdateAsync(updateProductIntegration);
                }

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Bir hata oluştu: " + ex.Message });
            }


        }

        [HttpPost]
        public async Task<IActionResult> CreateOrUpdateProductIntegrationIdefix(IdefixProductIntegrationModel model)
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

                //TrendyolApiContext context = new TrendyolApiContext
                //{
                //    SupplierId = integrationSystem.IntegrationSystemParameters.Where(m => m.Key == "SupplierId").Select(m => m.Value).FirstOrDefault() ?? "",
                //    ApiUser = integrationSystem.IntegrationSystemParameters.Where(m => m.Key == "ApiUser").Select(m => m.Value).FirstOrDefault() ?? "",
                //    ApiPassword = integrationSystem.IntegrationSystemParameters.Where(m => m.Key == "ApiPassword").Select(m => m.Value).FirstOrDefault() ?? "",
                //};

                //var existingTrendyolProduct = await _trenyolService.GetProductWithBarcodeAsync(context, model.IntegrationCode);
                //if (existingTrendyolProduct == null)
                //{
                //    return Json(new { success = false, message = $"Trendyol üzerinde bu barkoda sahip bir ürün bulunamadı. Barkod: {model.IntegrationCode}" });
                //}

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
                    await _productIntegrationService.AddAsync(createProductIntegration);
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

                    await _productIntegrationService.UpdateAsync(updateProductIntegration);
                }

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Bir hata oluştu: " + ex.Message });
            }


        }

        [HttpPost]
        public async Task<IActionResult> CreateOrUpdateProductIntegrationN11(N11ProductIntegrationModel model)
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

                //TrendyolApiContext context = new TrendyolApiContext
                //{
                //    SupplierId = integrationSystem.IntegrationSystemParameters.Where(m => m.Key == "SupplierId").Select(m => m.Value).FirstOrDefault() ?? "",
                //    ApiUser = integrationSystem.IntegrationSystemParameters.Where(m => m.Key == "ApiUser").Select(m => m.Value).FirstOrDefault() ?? "",
                //    ApiPassword = integrationSystem.IntegrationSystemParameters.Where(m => m.Key == "ApiPassword").Select(m => m.Value).FirstOrDefault() ?? "",
                //};

                //var existingTrendyolProduct = await _trenyolService.GetProductWithBarcodeAsync(context, model.IntegrationCode);
                //if (existingTrendyolProduct == null)
                //{
                //    return Json(new { success = false, message = $"Trendyol üzerinde bu barkoda sahip bir ürün bulunamadı. Barkod: {model.IntegrationCode}" });
                //}

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
                    await _productIntegrationService.AddAsync(createProductIntegration);
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

                    await _productIntegrationService.UpdateAsync(updateProductIntegration);
                }

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Bir hata oluştu: " + ex.Message });
            }


        }

        [HttpPost]
        public async Task<IActionResult> CreateOrUpdateProductIntegrationPazarama(PazaramaProductIntegrationModel model)
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

                PazaramaApiContext context = new PazaramaApiContext
                {
                    ClientId = integrationSystem.IntegrationSystemParameters.FirstOrDefault(m => m.Key == "ClientId")?.Value ?? "",
                    ClientSecret = integrationSystem.IntegrationSystemParameters.FirstOrDefault(m => m.Key == "ClientSecret")?.Value ?? "",
                };


                var existingPazaramaProduct = await _pazaramaService.GetProductWithStockCodeAsync(context, model.IntegrationCode);
                if (existingPazaramaProduct == null)
                {
                    return Json(new { success = false, message = $"Pazarama üzerinde bu barkoda sahip bir ürün bulunamadı. Barkod: {model.IntegrationCode}" });
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
                    await _productIntegrationService.AddAsync(createProductIntegration);
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

                    await _productIntegrationService.UpdateAsync(updateProductIntegration);
                }

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Bir hata oluştu: " + ex.Message });
            }


        }

        [HttpPost]
        public async Task<IActionResult> CreateOrUpdateProductIntegrationHepsiburada(HepsiburadaProductIntegrationModel model)
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



                HepsiburadaApiContext context = new HepsiburadaApiContext
                {
                    ApiUser = integrationSystem.IntegrationSystemParameters.FirstOrDefault(m => m.Key == "ApiUser")?.Value ?? "",
                    ApiPassword = integrationSystem.IntegrationSystemParameters.FirstOrDefault(m => m.Key == "ApiPassword")?.Value ?? "",
                    MerchantId = integrationSystem.IntegrationSystemParameters.FirstOrDefault(m => m.Key == "MerchantId")?.Value ?? "",
                    UserAgent = integrationSystem.IntegrationSystemParameters.FirstOrDefault(m => m.Key == "UserAgent")?.Value ?? "",
                };





                var existingHepsiburadaProduct = await _hepsiburadaService.GetProductWithMerchantSkuAsync(context, model.IntegrationCode);
                if (existingHepsiburadaProduct == null)
                {
                    return Json(new { success = false, message = $"Hepsiburada üzerinde bu barkoda sahip bir ürün bulunamadı. Barkod: {model.IntegrationCode}" });
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
                    await _productIntegrationService.AddAsync(createProductIntegration);
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

                    await _productIntegrationService.UpdateAsync(updateProductIntegration);
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
                await _productIntegrationService.DeleteAsync(id);
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


        #region ProductVariantAttribute
        [HttpGet]
        public async Task<IActionResult> CreateOrUpdateProductVariantAttribute(int productId, int productVariantAttributeId)
        {
            var model = new ProductVariantAttributeModel();
            var productAttributes = await _productAttributeService.GetAllAsync();
            ViewBag.ProductAttributes = productAttributes.Select(m => new SelectListItem()
            {
                Text = m.Name,
                Value = m.Id.ToString()
            }).ToList();
            ViewBag.FormControlTypes = EnumHelper.GetEnumSelectList<FormControlType>();

            if (productVariantAttributeId == 0)
            {
                model.ProductId = productId;
                return PartialView("_CreateOrUpdate.ProductVariantAttribute", model);
            }
            var productAttribute = await _productVariantAttributeService.GetByAttibuteIdAsync(productId, productVariantAttributeId);
            var mapped = _mapper.Map(productAttribute, model);
            return PartialView("_CreateOrUpdate.ProductVariantAttribute", mapped);
        }
        [HttpPost]
        public async Task<IActionResult> CreateOrUpdateProductVariantAttribute(ProductVariantAttributeModel model)
        {
            if (model.Id > 0)
            {
                var updateDto = _mapper.Map<UpdateProductVariantAttributeDto>(model);
                await _productVariantAttributeService.UpdateAsync(updateDto);
                return Json(new { success = true });
            }

            var createDto = _mapper.Map<CreateProductVariantAttributeDto>(model);
            await _productVariantAttributeService.AddAsync(createDto);

            return Json(new { success = true });
        }
        [HttpPost]
        public async Task<IActionResult> ProductVariantAttributeList([FromBody] GridCommand gridCommand, int productId)
        {
            var result = await _productVariantAttributeService.GetPagedAsync(gridCommand, productId);

            return Json(new
            {
                draw = gridCommand.Draw,
                recordsTotal = result.TotalCount,
                recordsFiltered = result.TotalCount,
                data = result.Items
            });
        }
        [HttpGet]
        public IActionResult ProductVariantAttributeValues(int productVariantAttributeId)
        {
            ViewBag.ProductVariantAttributeId = productVariantAttributeId;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ProductVariantAttributeValueList([FromBody] GridCommand gridCommand, int productVariantAttributeId)
        {
            var result = await _productVariantAttributeValueService.GetPagedAsync(gridCommand, productVariantAttributeId);

            return Json(new
            {
                draw = gridCommand.Draw,
                recordsTotal = result.TotalCount,
                recordsFiltered = result.TotalCount,
                data = result.Items
            });
        }

        [HttpPost]
        public async Task<IActionResult> DeleteProductVariantAttribute(int id)
        {
            try
            {
                await _productVariantAttributeService.DeleteAsync(id);
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        #endregion
        private async Task PrepareProductModel(ProductModel model, ProductDto? product)
        {
            if (product != null)
            {

                model.ProductVariantAttributes = product.ProductVariantAttributes.Select(m => new ProductVariantAttributeModel()
                {
                    AttributeControlTypeId = m.AttributeControlTypeId,
                    DisplayOrder = m.DisplayOrder,
                    Id = m.Id,
                    IsRequried = m.IsRequried,
                    ProductAttribute = m.ProductAttribute.Name,
                    ProductAttributeId = m.ProductAttributeId,
                    ProductId = m.ProductId,
                    ProductVariantAttributeValues = m.ProductVariantAttributeValues.Select(x => new ProductVariantAttributeValueModel()
                    {
                        Id = x.Id,
                        Name = x.Name,
                        ProductVariantAttributeId = x.ProductVariantAttributeId,
                    }).ToList()
                }).ToList();
                model.ProductVariantAttributeCombinations = product.ProductVariantAttributeCombinations.Select(m => new ProductVariantAttributeCombinationModel()
                {
                    Id = m.Id,
                    ProductVariantAttributeSelections = JsonConvert.DeserializeObject<List<ProductVariantAttributeSelection>>(m.RawAttribute) ?? new List<ProductVariantAttributeSelection>(),
                    Gtin = m.Gtin,
                    ManufacturerPartNumber = m.ManufacturerPartNumber,
                    Price = m.Price,
                    ProductId = m.ProductId,
                    StockQuantity = m.StockQuantity,
                    StokCode = m.StokCode,
                    AssignedPictureIds = m.AssignedPictureIds
                }).ToList();
                model.ProductMediaFiles = product.ProductMediaFiles.Select(m => new ProductMediaFileModel()
                {
                    Id = m.Id,
                    DisplayOrder = m.DisplayOrder,
                    MediaFileId = m.MediaFileId,
                    ProductId = m.ProductId,
                    MediaFile = new MediaFileModel()
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
                        Folder = m.MediaFile.Folder == null ? null : new MediaFolderModel()
                        {
                            Id = m.MediaFile.Folder.Id,
                            Name = m.MediaFile.Folder.Name,
                        }
                    }
                }).ToList();

            }

            var brands = await _brandService.GetAllBrandsAsync();

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
