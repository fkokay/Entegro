using Entegro.Application.DTOs.Commerce.Smartstore;
using Entegro.Application.DTOs.Product;
using Microsoft.Extensions.Logging;

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
        public static SmartstoreProductDto? ToDto(ProductDto product, SmartstoreProductIntegrationCustomDto? customData)
        {
            try
            {
                if (product == null)
                {
                    return null;
                }

                SmartstoreManufacturerMapper.ConfigureLogger(_logger);

                SmartstoreProductDto smartstoreProduct = new SmartstoreProductDto();
                smartstoreProduct.Id = product.Id;
                smartstoreProduct.ProductTypeId = 5; // Varsayılan olarak basit ürün tipi
                smartstoreProduct.ParentGroupedProductId = 0;
                smartstoreProduct.Visibility = "Full";
                smartstoreProduct.Condition = "New";
                smartstoreProduct.Name = product.Name;
                smartstoreProduct.ShortDescription = "";
                smartstoreProduct.AdminComment = "";
                smartstoreProduct.ProductTemplateId = 1;
                smartstoreProduct.ShowOnHomePage = customData == null ? false : customData.ShowOnHomePage;
                smartstoreProduct.HomePageDisplayOrder = customData == null ? 0 : customData.HomePageDisplayOrder;
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
                smartstoreProduct.Gtin = product.Gtin;
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
                smartstoreProduct.ManageInventoryMethodId = customData == null ? 0 : customData.ManageInventoryMethod;
                smartstoreProduct.StockQuantity = product.StockQuantity;
                smartstoreProduct.DisplayStockAvailability = customData == null ? false : customData.DisplayStockAvailability;
                smartstoreProduct.DisplayStockQuantity = customData == null ? false : customData.DisplayStockQuantity;
                smartstoreProduct.MinStockQuantity = customData == null ? 0 : customData.MinStockQuantity;
                smartstoreProduct.LowStockActivityId = customData == null ? 0 : customData.LowStockActivityId;
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
                smartstoreProduct.SpecialPrice = customData == null ? null : customData.SpecialPrice;
                smartstoreProduct.SpecialPriceStartDateTimeUtc = customData == null ? null : customData.SpecialPriceStartDateTime ?? customData.SpecialPriceStartDateTime.ToUniversalTime();
                smartstoreProduct.SpecialPriceEndDateTimeUtc = customData == null ? null : customData.SpecialPriceEndDateTime ?? customData.SpecialPriceEndDateTime.ToUniversalTime();
                smartstoreProduct.CustomerEntersPrice = false;
                smartstoreProduct.MinimumCustomerEnteredPrice = 0;
                smartstoreProduct.MaximumCustomerEnteredPrice = 1000;
                smartstoreProduct.HasTierPrices = false;
                smartstoreProduct.LowestAttributeCombinationPrice = null;
                smartstoreProduct.AttributeCombinationRequired = true;
                smartstoreProduct.AttributeChoiceBehaviour = "GrayOutUnavailable";
                smartstoreProduct.Weight = product.Weight;
                smartstoreProduct.Length = product.Length;
                smartstoreProduct.Width = product.Width;
                smartstoreProduct.Height = product.Height;
                smartstoreProduct.AvailableStartDateTimeUtc = null;
                smartstoreProduct.AvailableEndDateTimeUtc = null;
                smartstoreProduct.DisplayOrder = 0;
                smartstoreProduct.Published = product.Published;
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


                return smartstoreProduct;
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Product mapping sırasında hata oluştu. ProductId: {ProductId}", product.Id);
                return null;
            }
        }
        public static SmartstoreUpdateProductDto? UpdateToDto(ProductDto product, SmartstoreProductIntegrationCustomDto? customData)
        {
            try
            {
                if (product == null)
                {
                    return null;
                }

                SmartstoreManufacturerMapper.ConfigureLogger(_logger);

                SmartstoreUpdateProductDto smartstoreProduct = new SmartstoreUpdateProductDto();
                smartstoreProduct.Id = product.Id;
                smartstoreProduct.ProductTypeId = 5; // Varsayılan olarak basit ürün tipi
                smartstoreProduct.ParentGroupedProductId = 0;
                smartstoreProduct.Visibility = "Full";
                smartstoreProduct.Condition = "New";
                smartstoreProduct.Name = product.Name;
                smartstoreProduct.ShortDescription = "";
                smartstoreProduct.AdminComment = "";
                smartstoreProduct.ProductTemplateId = 1;
                smartstoreProduct.ShowOnHomePage = customData == null ? false : customData.ShowOnHomePage;
                smartstoreProduct.HomePageDisplayOrder = customData == null ? 0 : customData.HomePageDisplayOrder;
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
                smartstoreProduct.ManufacturerPartNumber = "";
                smartstoreProduct.Gtin = product.Gtin;
                smartstoreProduct.GiftCardTypeId = 0;
                smartstoreProduct.UnlimitedDownloads = true;
                smartstoreProduct.MaxNumberOfDownloads = 10;
                smartstoreProduct.DownloadActivationTypeId = 1;
                smartstoreProduct.RecurringCycleLength = 100;
                smartstoreProduct.RecurringCyclePeriodId = 0;
                smartstoreProduct.RecurringTotalCycles = 10;
                smartstoreProduct.IsShippingEnabled = true;
                smartstoreProduct.AdditionalShippingCharge = 0;
                smartstoreProduct.TaxCategoryId = 1;
                smartstoreProduct.ManageInventoryMethodId = customData == null ? 0 : customData.ManageInventoryMethod;
                smartstoreProduct.StockQuantity = product.StockQuantity;
                smartstoreProduct.DisplayStockAvailability = customData == null ? false : customData.DisplayStockAvailability;
                smartstoreProduct.DisplayStockQuantity = customData == null ? false : customData.DisplayStockQuantity;
                smartstoreProduct.MinStockQuantity = customData == null ? 0 : customData.MinStockQuantity;
                smartstoreProduct.LowStockActivityId = customData == null ? 0 : customData.LowStockActivityId;
                smartstoreProduct.NotifyAdminForQuantityBelow = 0;
                smartstoreProduct.BackorderModeId = 0;
                smartstoreProduct.OrderMinimumQuantity = 1;
                smartstoreProduct.OrderMaximumQuantity = int.MaxValue;
                smartstoreProduct.QuantityStep = 1;
                smartstoreProduct.QuantityControlType = "Spinner";
                smartstoreProduct.Price = product.Price;
                smartstoreProduct.ComparePrice = 0;
                smartstoreProduct.SpecialPrice = customData == null ? null : customData.SpecialPrice;
                smartstoreProduct.SpecialPriceStartDateTimeUtc = customData == null ? null : customData.SpecialPriceStartDateTime ?? customData.SpecialPriceStartDateTime.ToUniversalTime();
                smartstoreProduct.SpecialPriceEndDateTimeUtc = customData == null ? null : customData.SpecialPriceEndDateTime ?? customData.SpecialPriceEndDateTime.ToUniversalTime();
                smartstoreProduct.MinimumCustomerEnteredPrice = 0;
                smartstoreProduct.MaximumCustomerEnteredPrice = 1000;
                smartstoreProduct.AttributeCombinationRequired = true;
                smartstoreProduct.AttributeChoiceBehaviour = "GrayOutUnavailable";
                smartstoreProduct.Weight = product.Weight;
                smartstoreProduct.Length = product.Length;
                smartstoreProduct.Width = product.Width;
                smartstoreProduct.Height = product.Height;
                smartstoreProduct.DisplayOrder = 0;
                smartstoreProduct.Published = product.Published;
                smartstoreProduct.SystemName = "";
                smartstoreProduct.CreatedOnUtc = DateTimeOffset.UtcNow;
                smartstoreProduct.UpdatedOnUtc = DateTimeOffset.UtcNow;
                smartstoreProduct.CustomsTariffNumber = "";
                smartstoreProduct.CountryOfOriginId = 77;



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
