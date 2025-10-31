using Entegro.Application.DTOs.Brand;
using Entegro.Application.DTOs.Category;
using Entegro.Application.DTOs.CategoryAttribute;
using Entegro.Application.DTOs.Commerce.Smartstore;
using Entegro.Application.DTOs.Marketplace.Trendyol;
using Entegro.Application.DTOs.OrderItem;
using Entegro.Application.DTOs.Product;
using Entegro.Application.DTOs.ProductAttribute;
using Entegro.Application.DTOs.ProductAttributeValue;
using Entegro.Application.DTOs.ProductMediaFile;
using Entegro.Application.DTOs.ProductVariantAttribute;
using Entegro.Application.DTOs.ProductVariantAttributeCombination;
using Entegro.Application.DTOs.ProductVariantAttributeValue;
using Entegro.Application.Events;
using Entegro.Application.Interfaces.Event;
using Entegro.Application.Interfaces.Services.Base;
using Entegro.Application.Interfaces.Services.Marketplace;
using Entegro.Application.Mappings.Marketplace.Trendyol;
using Entegro.Domain.Enums;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Collections.Concurrent;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using JsonSerializer = System.Text.Json.JsonSerializer;
using ProductVariantAttributeDto = Entegro.Application.DTOs.Marketplace.Trendyol.ProductVariantAttributeDto;

namespace Entegro.Application.Services.Marketplace
{
    public class TrendyolService : ITrendyolService, IEventHandler<ProductIntegrationRecordUpdatedEvent>
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IProductIntegrationService _productIntegrationService;
        private readonly IProductService _productService;
        private readonly IProductVariantAttributeCombinationService _productVariantAttributeCombinationService;
        private readonly INotificationService _notificationService;
        private readonly ILogger<TrendyolService> _logger;
        private readonly IProductVariantAttributeValueService _productVariantAttributeValueService;
        private readonly IProductVariantAttributeService _productVariantAttributeService;
        private readonly IProductAttributeService _productAttributeService;
        private readonly IProductAttributeValueService _productAttributeValueService;
        private readonly ISettingService _settingService;
        private readonly IOrderItemService _orderItemService;
        private readonly IProductMediaFileMappingService _productMediaFileMappingService;
        private readonly ConcurrentDictionary<string, int> _attributeCache = new();
        private readonly ConcurrentDictionary<(int attributeId, string value), int> _attributeValueCache = new();
        public TrendyolService(
            IHttpClientFactory httpClientFactory,
            IProductIntegrationService productIntegrationService,
            IProductService productService,
            IProductVariantAttributeCombinationService productVariantAttributeCombinationService,
            INotificationService notificationService,
            ILogger<TrendyolService> logger,
            IProductVariantAttributeValueService productVariantAttributeValueService,
            IProductVariantAttributeService productVariantAttributeService,
            IProductAttributeService productAttributeService,
            IProductAttributeValueService productAttributeValueService,
            ISettingService settingService,
            IProductMediaFileMappingService productMediaFileMappingService,
            IOrderItemService orderItemService)
        {
            _httpClientFactory = httpClientFactory;
            _productIntegrationService = productIntegrationService;
            _productService = productService;
            _productVariantAttributeCombinationService = productVariantAttributeCombinationService;
            _notificationService = notificationService;
            _logger = logger;
            _productVariantAttributeValueService = productVariantAttributeValueService;
            _productVariantAttributeService = productVariantAttributeService;
            _productAttributeService = productAttributeService;
            _productAttributeValueService = productAttributeValueService;
            _settingService = settingService;
            _productMediaFileMappingService = productMediaFileMappingService;
            _orderItemService = orderItemService;
        }


        private HttpClient CreateHttpClient(TrendyolApiContext context)
        {
            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(context.BaseUrl);

            var authToken = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{context.ApiUser}:{context.ApiPassword}"));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", authToken);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            return client;
        }

        public async Task HandleAsync(ProductIntegrationRecordUpdatedEvent recordUpdatedEvent)
        {
            var productIntegration = await _productIntegrationService.GetByIdAsync(recordUpdatedEvent.ProductIntegrationId);
            if (productIntegration == null)
            {
                return;
            }

            if (productIntegration.IntegrationSystem.IntegrationSystemType == IntegrationSystemType.Marketplace)
            {
                string? marketplaceType = productIntegration.IntegrationSystem.IntegrationSystemParameters.Where(m => m.Key == "MarketplaceType").Select(m => m.Value).FirstOrDefault();

                if (marketplaceType == "Trendyol")
                {
                    var apiContext = new TrendyolApiContext
                    {
                        SupplierId = productIntegration.IntegrationSystem.IntegrationSystemParameters.First(p => p.Key == "SupplierId").Value,
                        ApiUser = productIntegration.IntegrationSystem.IntegrationSystemParameters.First(p => p.Key == "ApiUser").Value,
                        ApiPassword = productIntegration.IntegrationSystem.IntegrationSystemParameters.First(p => p.Key == "ApiPassword").Value
                    };

                    object? customData = string.IsNullOrEmpty(productIntegration.Custom) ? null : JsonSerializer.Deserialize<SmartstoreProductIntegrationCustomDto>(productIntegration.Custom);
                    var product = await _productService.GetProductByIdAsync(productIntegration.ProductId);
                    if (product == null)
                    {
                        _logger.LogWarning($"Product with ID {productIntegration.ProductId} not found.");
                        return;
                    }

                    product.Code = productIntegration.IntegrationCode;
                    product.Price = productIntegration.Price;


                    int stockQuantity = 0;
                    if (productIntegration.ProductVariantAttributeCombinationId.HasValue)
                    {
                        var productVariantAttributeCombination = await _productVariantAttributeCombinationService.GetByIdAsync(productIntegration.ProductVariantAttributeCombinationId.Value);
                        stockQuantity = productVariantAttributeCombination.StockQuantity;
                    }
                    else
                    {
                        stockQuantity = product.StockQuantity;
                    }

                    var request = new TrendyolPriceAndStockUpdateRequest
                    {
                        Items = new List<TrendyolPriceAndStockUpdateDto>
                            {
                                new TrendyolPriceAndStockUpdateDto
                                {
                                    Barcode = productIntegration.IntegrationCode,
                                    ListPrice = productIntegration.Price,
                                    SalePrice = productIntegration.Price,
                                    Quantity = stockQuantity
                                }
                            }
                    };

                    await UpdatePriceAndStockAsync(apiContext, request);


                    await _notificationService.SendNotification(NotificationType.Info, "Bildirim", $"Trendyol {product.Name} stok ve fiyat güncellendi");

                }
            }
        }

        public async Task<IEnumerable<BrandDto>> GetBrandsAsync(TrendyolApiContext context)
        {
            using var client = CreateHttpClient(context);
            var allBrands = new List<TrendyolBrandDto>();
            bool moreData = true;
            int page = 0;

            while (moreData)
            {
                var url = $"product/brands?size=2000&page={page}";
                var response = await client.GetAsync(url);
                response.EnsureSuccessStatusCode();

                var json = await response.Content.ReadAsStringAsync();
                var data = JsonSerializer.Deserialize<TrendyolBrandResponse>(json, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (data?.Brands == null || !data.Brands.Any())
                {
                    break;
                }

                allBrands.AddRange(data.Brands);

                page += 1;

                if (data.Brands.Count < 2000)
                {
                    moreData = false;
                }
            }

            TrendyolBrandMapper.ConfigureLogger(_logger);
            var brands = TrendyolBrandMapper.ToDtoList(allBrands);

            return brands;
        }

        public Task<IEnumerable<TrendyolCargoCompanyDto>> GetCargoCompaniesAsync()
        {
            List<TrendyolCargoCompanyDto> trendyolCargoCompanies = new List<TrendyolCargoCompanyDto>();
            trendyolCargoCompanies.Add(new TrendyolCargoCompanyDto()
            {
                Id = 38,
                Code = "SENDEOMP",
                Name = "Kolay Gelsin Marketplace",
                TaxNumber = "2910804196"
            });
            trendyolCargoCompanies.Add(new TrendyolCargoCompanyDto()
            {
                Id = 30,
                Code = "BORMP",
                Name = "Borusan Lojistik Marketplace",
                TaxNumber = "1800038254"
            });
            trendyolCargoCompanies.Add(new TrendyolCargoCompanyDto()
            {
                Id = 10,
                Code = "DHLECOMMP",
                Name = "DHL eCommerce Marketplace",
                TaxNumber = "6080712084"
            });
            trendyolCargoCompanies.Add(new TrendyolCargoCompanyDto()
            {
                Id = 19,
                Code = "PTTMP",
                Name = "PTT Kargo Marketplace",
                TaxNumber = "7320068060"
            });
            trendyolCargoCompanies.Add(new TrendyolCargoCompanyDto()
            {
                Id = 9,
                Code = "SURATMP",
                Name = "Sürat Kargo Marketplace",
                TaxNumber = "7870233582"
            });
            trendyolCargoCompanies.Add(new TrendyolCargoCompanyDto()
            {
                Id = 17,
                Code = "TEXMP",
                Name = "Trendyol Express Marketplace",
                TaxNumber = "8590921777"
            });
            trendyolCargoCompanies.Add(new TrendyolCargoCompanyDto()
            {
                Id = 6,
                Code = "HOROZMP",
                Name = "Horoz Kargo Marketplace",
                TaxNumber = "4630097122"
            });
            trendyolCargoCompanies.Add(new TrendyolCargoCompanyDto()
            {
                Id = 20,
                Code = "CEVAMP",
                Name = "CEVA Marketplace",
                TaxNumber = "8450298557"
            });
            trendyolCargoCompanies.Add(new TrendyolCargoCompanyDto()
            {
                Id = 4,
                Code = "YKMP",
                Name = "Yurtiçi Kargo Marketplace",
                TaxNumber = "3130557669"
            });
            trendyolCargoCompanies.Add(new TrendyolCargoCompanyDto()
            {
                Id = 7,
                Code = "ARASMP",
                Name = "Aras Kargo Marketplace",
                TaxNumber = "720039666"
            });

            return Task.FromResult(trendyolCargoCompanies.AsEnumerable());
        }

        public async Task<IEnumerable<CategoryDto>> GetCategoriesAsync(TrendyolApiContext context)
        {
            using var client = CreateHttpClient(context);
            var allCategories = new List<TrendyolCategoryDto>();

            var url = $"product/product-categories";
            var response = await client.GetAsync(url);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            var data = JsonSerializer.Deserialize<TrendyolCategoryResponse>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            allCategories.AddRange(data.Categories);

            TrendyolCategoryMapper.ConfigureLogger(_logger);
            var categories = TrendyolCategoryMapper.ToDtoList(allCategories);

            return categories;
        }

        public async Task<CategoryAttributeDto> GetCategoryAttibutesAsync(TrendyolApiContext context, int categoryId)
        {
            using var client = CreateHttpClient(context);
            var url = $"product/product-categories/{categoryId}/attributes";
            var response = await client.GetAsync(url);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            var data = JsonSerializer.Deserialize<TrendyolCategoryWithAttributeDto>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            var a = data.CategoryAttributes.Where(m => m.Slicer == true).ToList();

            TrendyolCategoryAttributeMapper.ConfigureLogger(_logger);
            var categoryAttribute = TrendyolCategoryAttributeMapper.ToDto(data);

            return categoryAttribute;
        }

        public async Task<IEnumerable<TrendyolProductDto>> GetProductsAsync(TrendyolApiContext context, int pageSize = 50)
        {
            using var client = CreateHttpClient(context);
            var allProducts = new List<TrendyolProductDto>();
            bool moreData = true;
            int page = 0;

            while (moreData)
            {
                var url = $"product/sellers/{context.SupplierId}/products?size={pageSize}&page={page}";
                var response = await client.GetAsync(url);
                response.EnsureSuccessStatusCode();

                var json = await response.Content.ReadAsStringAsync();
                var data = JsonSerializer.Deserialize<TrendyolResponse<TrendyolProductDto>>(json, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (data?.content == null || !data.content.Any())
                {
                    break;
                }

                allProducts.AddRange(data.content);

                page += 1;

                if (page >= data.totalPages)
                {
                    moreData = false;
                }
            }

            return allProducts;
        }

        public async Task<TrendyolProductDto?> GetProductWithBarcodeAsync(TrendyolApiContext context, string barcode)
        {
            using var client = CreateHttpClient(context);
            var url = $"product/sellers/{context.SupplierId}/products?barcode={barcode}";
            var response = await client.GetAsync(url);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            var data = JsonSerializer.Deserialize<TrendyolResponse<TrendyolProductDto>>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            if (data == null)
            {
                return null;
            }

            return data.content.FirstOrDefault();
        }

        public async Task<IEnumerable<TrendyolShipmentPackageDto>> GetShipmentPackagesAsync(TrendyolApiContext context, int pageSize = 50)
        {

            using var client = CreateHttpClient(context);

            var allShipmentPackages = new List<TrendyolShipmentPackageDto>();
            bool moreData = true;
            int page = 0;

            while (moreData)
            {
                var url = $"order/sellers/{context.SupplierId}/orders?size={pageSize}&page={page}";
                var response = await client.GetAsync(url);
                response.EnsureSuccessStatusCode();

                var json = await response.Content.ReadAsStringAsync();
                var data = JsonSerializer.Deserialize<TrendyolResponse<TrendyolShipmentPackageDto>>(json, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (data?.content == null || !data.content.Any())
                {
                    break;
                }

                allShipmentPackages.AddRange(data.content);

                page += 1;

                if (page >= data.totalPages)
                {
                    moreData = false;
                }
            }

            return allShipmentPackages;
        }

        public async Task UpdatePriceAndStockAsync(TrendyolApiContext context, TrendyolPriceAndStockUpdateRequest request)
        {
            using var client = CreateHttpClient(context);

            var url = $"inventory/sellers/{context.SupplierId}/products/price-and-inventory";
            var json = JsonSerializer.Serialize(request, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await client.PostAsync(url, content);

            var result = await response.Content.ReadAsStringAsync();
            response.EnsureSuccessStatusCode();
        }

        public async Task<List<TrendyolCategoryAttributeDto>?> GetCategorySlicerAttributesAsync(TrendyolApiContext context, int categoryId)
        {
            using var client = CreateHttpClient(context);
            var url = $"product/product-categories/{categoryId}/attributes";
            var response = await client.GetAsync(url);
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();

            var data = JsonSerializer.Deserialize<TrendyolCategoryDto2>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            return data?.CategoryAttributes.Where(a => a.Slicer).ToList() ?? new List<TrendyolCategoryAttributeDto>();
        }

        public async Task<VariantProcessStatusDto> GetProductVariantAsync(TrendyolApiContext context, string barcode, int integrationSystemId)
        {

            bool isSlicer = await IsSlicerProductAsync(context, barcode);
            if (!isSlicer)
            {
                return new VariantProcessStatusDto
                {
                    HasSlicer = false,
                    Message = "Slicer bulunamadı",
                    AddedCount = 0
                };
            }


            var product = await GetProductWithBarcodeAsync(context, barcode);
            List<TrendyolProductAttributeDto> attirbutes = new();
            if (product == null)
                return null;

            var slicers = await GetCategorySlicerAttributesAsync(context, product.pimCategoryId);// Kategorinin slicer attribute’larını çek
            var products = await GetProductsByProductMainIdAsync(context, product.productMainId);
            var dbProduct = await _productService.GetProductByBarcodeAsync(barcode);
            int addedCount = 0;
            foreach (var slicer in slicers)
            {
                foreach (var item in products)
                {
                    var matchedAttributes = item.attributes
                        .Where(m => m.AttributeId == slicer.Attribute.Id)
                        .ToList();

                    foreach (var attribute in matchedAttributes)
                    {
                        await AddVariantAttributeAsync(
                            dbProduct.Id,
                            attribute.AttributeName,
                            attribute.AttributeValue,
                            dbProduct,
                            item,
                            integrationSystemId
                        );

                        addedCount++;
                    }
                }
            }

            return new VariantProcessStatusDto
            {
                HasSlicer = true,
                Message = addedCount > 0
                    ? $"{addedCount} kombinasyon başarıyla eklendi."
                    : "Slicer bulundu fakat kombinasyon eklenmedi.",
                AddedCount = addedCount
            };
        }

        public async Task<IEnumerable<TrendyolProductDto>> GetProductsByProductMainIdAsync(TrendyolApiContext context, string productMainId)
        {
            using var client = CreateHttpClient(context);
            var allProducts = new List<TrendyolProductDto>();
            bool moreData = true;
            int page = 0;
            int pageSize = 50;

            while (moreData)
            {

                var url = $"product/sellers/{context.SupplierId}/products?productMainId={productMainId}&size={pageSize}&page={page}";
                var response = await client.GetAsync(url);
                response.EnsureSuccessStatusCode();

                var json = await response.Content.ReadAsStringAsync();
                var data = JsonSerializer.Deserialize<TrendyolResponse<TrendyolProductDto>>(json, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (data?.content == null || !data.content.Any())
                {
                    break;
                }

                allProducts.AddRange(data.content);

                page += 1;

                if (page >= data.totalPages)
                {
                    moreData = false;
                }
            }

            return allProducts;
        }
        private async Task AddVariantAttributeAsync(int productId, string attributeName, string attributeValue, ProductDto? product, TrendyolProductDto trendyolProduct, int integrationSystemId)
        {
            List<ProductVariantAttributeDto> variantAttributes = new List<ProductVariantAttributeDto>();
            List<int> mediaFiles = new List<int>();
            List<int> mediaFileMappingIds = new List<int>();

            if (string.IsNullOrEmpty(attributeName) || string.IsNullOrEmpty(attributeValue)) return;

            int attrId = await EnsureProductAttributeAsync(attributeName);
            int attrValueId = await EnsureProductAttributeValueAsync(attrId, attributeValue);
            var existingVariant = await _productVariantAttributeService.GetByAttibuteIdAsync(productId, attrId);
            int variantId = existingVariant?.Id ?? (await _productVariantAttributeService.AddAsync(new CreateProductVariantAttributeDto
            {
                ProductId = productId,
                ProductAttributeId = attrId,
                DisplayOrder = 0,
                AttributeControlTypeId = 0
            })).Id;

            var existingVariantValue = await _productVariantAttributeValueService.GetByNameAsync(variantId, attributeValue);
            int variantValueId = existingVariantValue?.Id ?? (await _productVariantAttributeValueService.AddAsync(new CreateProductVariantAttributeValueDto()
            {
                Name = attributeValue,
                ProductVariantAttributeId = variantId
            })).Id;

            variantAttributes.Add(new ProductVariantAttributeDto
            {
                ProductVariantAttributeId = variantId,
                ProductVariantAttributeValueId = variantValueId
            });
            var jsonOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = null,
                WriteIndented = false
            };

            string rawAttributeJson = JsonSerializer.Serialize(variantAttributes, jsonOptions);
            var existingCombination = await _productVariantAttributeCombinationService.ExistsAsync(product.Id, trendyolProduct.barcode);



            #region product image upload
            var systemUrl = await _settingService.GetByKeyAsync("SystemUrl");
            if (systemUrl == null || string.IsNullOrWhiteSpace(systemUrl.Value))
            {
                Console.WriteLine("Sistem URL'si ayarlanmamış.");
            }
            if (!Uri.TryCreate(systemUrl.Value, UriKind.Absolute, out var baseUri))
            {
                Console.WriteLine("hata");
            }

            using var httpClient = new HttpClient
            {
                BaseAddress = baseUri
            };

            try
            {
                var images = trendyolProduct.images.Select(m => m.url).ToList();
                mediaFiles = await UploadImagesAsync(images, httpClient);
                foreach (var item in mediaFiles)
                {
                    CreateProductMediaFileDto createProductMediaFile = new CreateProductMediaFileDto();
                    createProductMediaFile.MediaFileId = item;
                    createProductMediaFile.ProductId = product.Id;
                    var created = await _productMediaFileMappingService.AddAsync(createProductMediaFile);
                    mediaFileMappingIds.Add(created.Id);
                }

                if (!existingCombination)
                {
                    var createProductVariantAttributeCombinationDto = new CreateProductVariantAttributeCombinationDto
                    {
                        ProductId = product.Id,
                        StokCode = trendyolProduct.stockCode,
                        StockQuantity = trendyolProduct.quantity,
                        Gtin = trendyolProduct.barcode,
                        Price = trendyolProduct.salePrice,
                        RawAttribute = rawAttributeJson,
                        AssignedMediaFileIds = string.Join(",", mediaFileMappingIds)
                    };
                    var productVariantAttributeCombination = await _productVariantAttributeCombinationService.AddAsync(createProductVariantAttributeCombinationDto);

                    var systemId = integrationSystemId;
                    var barcode = trendyolProduct.barcode;
                    if (systemId > 0)
                    {
                        var model = await _productIntegrationService.GetByIntegrationSystemAndCodeAsync(systemId, barcode);
                        var add = new DTOs.ProductIntegration.CreateProductIntegrationDto
                        {
                            IntegrationSystemId = systemId,
                            ProductId = product.Id,
                            ProductVariantAttributeCombinationId = productVariantAttributeCombination.Id,
                            IntegrationCode = barcode,
                            Price = trendyolProduct.salePrice
                        };
                        await _productIntegrationService.AddAsync(add);

                    }

                    var orderItems = await _orderItemService.GetAllWithIntegrationSkuAsync(barcode);
                    foreach (var orderItem in orderItems)
                    {

                        var productIntegration = await _productIntegrationService.GetByIntegrationCodeAsync(orderItem.IntegrationSku);
                        var attributeCombination = productIntegration?.ProductVariantAttributeCombinationId != null
                            ? await _productVariantAttributeCombinationService
                                .GetByIdAsync(productIntegration.ProductVariantAttributeCombinationId.Value)
                            : null;

                        string attributeDescription = "";

                        if (attributeCombination != null && !string.IsNullOrEmpty(attributeCombination.RawAttribute))
                        {
                            var rawAttributes = JsonConvert.DeserializeObject<List<Dictionary<string, int>>>(attributeCombination.RawAttribute);

                            foreach (var raw in rawAttributes)
                            {
                                if (raw.TryGetValue("ProductVariantAttributeId", out var attributeId) &&
                                    raw.TryGetValue("ProductVariantAttributeValueId", out var valueId))
                                {
                                    var attribute = await _productVariantAttributeService.GetByIdAsync(attributeId);
                                    var attributeValue2 = await _productVariantAttributeValueService.GetByIdAsync(valueId);

                                    var attributeName2 = attribute?.ProductAttribute?.Name;
                                    var attributeValueName2 = attributeValue2?.Name;

                                    if (!string.IsNullOrEmpty(attributeName2) && !string.IsNullOrEmpty(attributeValueName2))
                                    {
                                        attributeDescription += $"{attributeName2}: {attributeValueName2} | ";
                                    }
                                }
                            }
                        }


                        UpdateOrderItemDto updateOrderItem = new UpdateOrderItemDto();
                        updateOrderItem.Id = orderItem.Id;
                        updateOrderItem.Sku = product.Code;
                        updateOrderItem.ProductId = product.Id;
                        updateOrderItem.ProductCost = orderItem.ProductCost;
                        updateOrderItem.AttributesXml = orderItem.AttributesXml;
                        updateOrderItem.DiscountAmount = orderItem.DiscountAmount;
                        updateOrderItem.Quantity = orderItem.Quantity;
                        updateOrderItem.Price = orderItem.Price;
                        updateOrderItem.UnitPrice = orderItem.UnitPrice;
                        updateOrderItem.IntegrationSku = orderItem.IntegrationSku;
                        updateOrderItem.IntegrationProductName = orderItem.IntegrationProductName;
                        updateOrderItem.ItemWeight = orderItem.ItemWeight;
                        updateOrderItem.OrderId = orderItem.OrderId;
                        updateOrderItem.IntegrationProductImageUrl = orderItem.IntegrationProductImageUrl;
                        updateOrderItem.AttributesXml = attributeCombination?.RawAttribute;
                        updateOrderItem.AttributesDescription = !string.IsNullOrEmpty(attributeDescription)
                        ? attributeDescription.TrimEnd(' ', '|')
                        : orderItem.AttributesDescription;
                        await _orderItemService.UpdateAsync(updateOrderItem);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            #endregion


        }
        private async Task<int> EnsureProductAttributeAsync(string name)
        {
            if (_attributeCache.TryGetValue(name, out int id)) return id;

            var productAttribute = await _productAttributeService.GetByNameAsync(name);
            if (productAttribute is null)
            {
                var created = await _productAttributeService.AddAsync(new CreateProductAttributeDto
                {
                    Name = name,
                    Description = "",
                    DisplayOrder = 0
                });

                _attributeCache.TryAdd(name, created.Id);
                return created.Id;
            }

            _attributeCache.TryAdd(name, productAttribute.Id);
            return productAttribute.Id;
        }
        private async Task<int> EnsureProductAttributeValueAsync(int attributeId, string value)
        {
            if (_attributeValueCache.TryGetValue((attributeId, value), out int id)) return id;

            var created = await _productAttributeValueService.AddAsync(new CreateProductAttributeValueDto
            {
                Name = value,
                DisplayOrder = 0,
                ProductAttributeId = attributeId
            });

            _attributeValueCache.TryAdd((attributeId, value), created.Id);
            return created.Id;
        }

        public async Task<bool> IsSlicerProductAsync(TrendyolApiContext context, string barcode)
        {
            var product = await GetProductWithBarcodeAsync(context, barcode);
            var slicers = await GetCategorySlicerAttributesAsync(context, product.pimCategoryId);// Kategorinin slicer attribute’larını çek
            if (slicers == null || !slicers.Any())
                return false;
            return true;
        }


        public async Task<List<int>> UploadImagesAsync(List<string> imageUrls, HttpClient httpClient)
        {
            List<int> fileIds = new();

            if (imageUrls != null && imageUrls.Any())
            {
                var multipartContent = new MultipartFormDataContent();
                multipartContent.Add(new StringContent("catalog"), "path");
                multipartContent.Add(new StringContent("False"), "isTransient");

                foreach (var imageUrl in imageUrls)
                {
                    try
                    {
                        var imageBytes = await httpClient.GetByteArrayAsync(imageUrl);
                        var imageName = Path.GetFileName(imageUrl);

                        if (string.IsNullOrWhiteSpace(imageName))
                            imageName = "default.jpg";

                        var nameWithoutExtension = Path.GetFileNameWithoutExtension(imageName);
                        var extension = Path.GetExtension(imageName);
                        var uniqueSuffix = $"trendyol_{Guid.NewGuid():N}";
                        imageName = $"{nameWithoutExtension}_{uniqueSuffix}{extension}";

                        var byteContent = new ByteArrayContent(imageBytes);
                        byteContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/octet-stream");

                        multipartContent.Add(byteContent, "upload-file", imageName);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"HATA (indirilemedi): {imageUrl} → {ex.Message}");
                    }
                }


                var uploadUrl = "media/upload";
                var response = await httpClient.PostAsync(uploadUrl, multipartContent);

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadAsStringAsync();

                    using var document = JsonDocument.Parse(result);
                    var root = document.RootElement;

                    if (root.ValueKind == JsonValueKind.Array)
                    {
                        foreach (var item in root.EnumerateArray())
                        {
                            if (item.TryGetProperty("id", out var idProp))
                            {
                                int imageId = idProp.GetInt32();
                                fileIds.Add(imageId);
                                Console.WriteLine($"id: {imageId}");
                            }
                        }
                    }
                    else if (root.ValueKind == JsonValueKind.Object)
                    {
                        if (root.TryGetProperty("id", out var idProp))
                        {
                            int imageId = idProp.GetInt32();
                            fileIds.Add(imageId);
                            Console.WriteLine($"id: {imageId}");
                        }
                    }
                }
                else
                {
                    throw new Exception("Resim yükleme başarısız oldu. Durum kodu:" + response.StatusCode);
                }
            }

            return fileIds;
        }
    }
}
