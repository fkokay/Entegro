using Entegro.Application.DTOs.Commerce.Smartstore;
using Entegro.Application.DTOs.Product;
using Entegro.Domain.Entities;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entegro.Application.Mappings.Commerce.Smartstore
{
    public static class SmartstoreProductMapper
    {
        private static ILogger? _logger;
        public static void ConfigureLogger(ILogger logger)
        {
            _logger = logger;
        }
        public static ProductDto? ToDto(SmartstoreProductDto smartstoreProduct)
        {
            try
            {
                if (smartstoreProduct == null)
                {
                    return null;
                }

                SmartstoreManufacturerMapper.ConfigureLogger(_logger);

                ProductDto productDto = new ProductDto();
                productDto.Name = smartstoreProduct.Name;
                productDto.Code = smartstoreProduct.Sku;
                productDto.Description = smartstoreProduct.FullDescription;
                productDto.Price = smartstoreProduct.Price;
                productDto.MetaKeywords = smartstoreProduct.MetaKeywords;
                productDto.MetaDescription = smartstoreProduct.MetaDescription;
                productDto.MetaTitle = smartstoreProduct.MetaTitle;
                productDto.StockQuantity = smartstoreProduct.StockQuantity;
                productDto.CreatedOn = DateTime.Now;
                productDto.UpdatedOn = DateTime.Now;
                productDto.Id = smartstoreProduct.Id;

                if (smartstoreProduct.ProductManufacturers.Any())
                {
                    var productManufacturer = smartstoreProduct.ProductManufacturers.First(); 
                    var brand = SmartstoreManufacturerMapper.ToDto(productManufacturer.Manufacturer);
                    productDto.Brand = brand;
                }
                else
                {
                    productDto.Brand = null;
                }


                return productDto; ;
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Product mapping sırasında hata oluştu. ProductId: {ProductId}", smartstoreProduct.Id);
                return null;
            }
        }
        public static SmartstoreProductDto? ToDto(ProductDto product)
        {
            try
            {
                if (product == null)
                {
                    return null;
                }

                SmartstoreManufacturerMapper.ConfigureLogger(_logger);

                SmartstoreProductDto smartstoreProduct = new SmartstoreProductDto();
                smartstoreProduct.ProductTypeId = 5; // Varsayılan olarak basit ürün tipi
                smartstoreProduct.ParentGroupedProductId = 0;
                smartstoreProduct.Visibility = "Full";
                smartstoreProduct.Condition = "New";
                smartstoreProduct.Name = product.Name;
                smartstoreProduct.ShortDescription = "";
                smartstoreProduct.AdminComment = "";
                smartstoreProduct.ProductTemplateId = 1;
                smartstoreProduct.ShowOnHomePage = false;
                smartstoreProduct.HomePageDisplayOrder = 0;
                smartstoreProduct.MetaTitle = "";
                smartstoreProduct.MetaDescription= "";
                smartstoreProduct.MetaKeywords = "";
                smartstoreProduct.AllowCustomerReviews = true;
                smartstoreProduct.Sku = product.Code;
                smartstoreProduct.FullDescription = product.Description;
                smartstoreProduct.Price = product.Price;
                smartstoreProduct.MetaKeywords = product.MetaKeywords;
                smartstoreProduct.MetaDescription = product.MetaDescription;
                smartstoreProduct.MetaTitle = product.MetaTitle;
                smartstoreProduct.StockQuantity = product.StockQuantity;
                smartstoreProduct.ApprovedRatingSum = 0;
                smartstoreProduct.NotApprovedRatingSum = 0;
                smartstoreProduct.ApprovedTotalReviews = 0;
                smartstoreProduct.NotApprovedTotalReviews = 0;
                smartstoreProduct.SubjectToAcl = false;
                smartstoreProduct.LimitedToStores = false;
                smartstoreProduct.ManufacturerPartNumber = "";
                smartstoreProduct.Gtin = "";
                smartstoreProduct.IsGiftCard = false;
                smartstoreProduct.GiftCardTypeId = 0;
                smartstoreProduct.RequireOtherProducts = false;
                smartstoreProduct.RequiredProductIds = null;
                smartstoreProduct.AutomaticallyAddRequiredProducts = false;
                smartstoreProduct.IsDownload = false;
                smartstoreProduct.UnlimitedDownloads = true;
                smartstoreProduct.MaxNumberOfDownloads = 10;
                smartstoreProduct.DownloadExpirationDays = null;
                smartstoreProduct.DownloadActivationTypeId = 1;
                smartstoreProduct.HasSampleDownload = false;
                smartstoreProduct.SampleDownloadId = null;
                smartstoreProduct.HasUserAgreement = false;
                smartstoreProduct.UserAgreementText = null;
                smartstoreProduct.IsRecurring = false;
                smartstoreProduct.RecurringCycleLength = 100;
                smartstoreProduct.RecurringCyclePeriodId = 0;
                smartstoreProduct.RecurringTotalCycles = 10;
                smartstoreProduct.IsShippingEnabled = true;
                smartstoreProduct.IsFreeShipping = false;
                smartstoreProduct.AdditionalShippingCharge = 0;
                smartstoreProduct.IsTaxExempt = false;
                smartstoreProduct.IsEsd = false;
                smartstoreProduct.TaxCategoryId = 1;
                smartstoreProduct.ManageInventoryMethodId = 0;
                smartstoreProduct.StockQuantity = 10000;
                smartstoreProduct.DisplayStockAvailability = false;
                smartstoreProduct.DisplayStockQuantity = false;
                smartstoreProduct.MinStockQuantity = 0;
                smartstoreProduct.LowStockActivityId = 0;
                smartstoreProduct.NotifyAdminForQuantityBelow = 0;
                smartstoreProduct.BackorderModeId = 0;
                smartstoreProduct.AllowBackInStockSubscriptions = false;
                smartstoreProduct.OrderMinimumQuantity = 1;
                smartstoreProduct.OrderMaximumQuantity = int.MaxValue;
                smartstoreProduct.QuantityStep = 1;
                smartstoreProduct.QuantityControlType = "Spinner";
                smartstoreProduct.HideQuantityControl = false;
                smartstoreProduct.AllowedQuantities = null;
                smartstoreProduct.DisableBuyButton = false;
                smartstoreProduct.DisableWishlistButton = false;
                smartstoreProduct.AvailableForPreOrder = false;
                smartstoreProduct.CallForPrice = false;
                smartstoreProduct.Price = product.Price;
                smartstoreProduct.ComparePrice = 0;
                smartstoreProduct.ComparePriceLabelId = null;
                smartstoreProduct.SpecialPrice = null;
                smartstoreProduct.SpecialPriceStartDateTimeUtc = null;
                smartstoreProduct.SpecialPriceEndDateTimeUtc = null;
                smartstoreProduct.CustomerEntersPrice = false;
                smartstoreProduct.MinimumCustomerEnteredPrice = 0;
                smartstoreProduct.MaximumCustomerEnteredPrice = 1000;
                smartstoreProduct.HasTierPrices = false;
                smartstoreProduct.LowestAttributeCombinationPrice = null;
                smartstoreProduct.AttributeCombinationRequired = false;
                smartstoreProduct.AttributeChoiceBehaviour = "GrayOutUnavailable";
                smartstoreProduct.Weight = 0;
                smartstoreProduct.Length = 0;
                smartstoreProduct.Width = 0;
                smartstoreProduct.Height = 0;
                smartstoreProduct.AvailableStartDateTimeUtc = null;
                smartstoreProduct.AvailableEndDateTimeUtc = null;
                smartstoreProduct.DisplayOrder = 0;
                smartstoreProduct.Published = true;
                smartstoreProduct.IsSystemProduct = false;
                smartstoreProduct.SystemName = "";
                smartstoreProduct.CreatedOnUtc = DateTimeOffset.UtcNow;
                smartstoreProduct.UpdatedOnUtc = DateTimeOffset.UtcNow;
                smartstoreProduct.DeliveryTimeId = null;
                smartstoreProduct.QuantityUnitId = null;
                smartstoreProduct.CustomsTariffNumber = "";
                smartstoreProduct.CountryOfOriginId = 77;
                smartstoreProduct.BasePriceEnabled = false;
                smartstoreProduct.BasePriceMeasureUnit = null;
                smartstoreProduct.BasePriceAmount = null;
                smartstoreProduct.BasePriceBaseAmount = null;
                smartstoreProduct.BundleTitleText = null;
                smartstoreProduct.BundlePerItemShipping = false;
                smartstoreProduct.BundlePerItemPricing = false;
                smartstoreProduct.BundlePerItemShoppingCart = false;
                smartstoreProduct.MainPictureId = null;
                smartstoreProduct.HasPreviewPicture = false;
                smartstoreProduct.HasDiscountsApplied = false;
                smartstoreProduct.Id = 0;


                smartstoreProduct.ProductManufacturers = new List<SmartstoreProductManufacturerDto>();
                if (product.BrandId.HasValue && product.BrandId > 0)
                {
                    var productManufacturer = new SmartstoreProductManufacturerDto
                    {
                        ManufacturerId = product.BrandId.Value,
                        DisplayOrder = 0
                    };
                    smartstoreProduct.ProductManufacturers.Add(productManufacturer);
                }

                return smartstoreProduct; 
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Product mapping sırasında hata oluştu. ProductId: {ProductId}", product.Id);
                return null;
            }
        }

        public static IEnumerable<ProductDto> ToDtoList(IEnumerable<SmartstoreProductDto> products)
        {
            if (products == null)
                yield break;

            foreach (var product in products)
            {
                var dto = ToDto(product);
                if (dto != null)
                    yield return dto;
            }
        }
    }
}
