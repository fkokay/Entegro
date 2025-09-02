using Entegro.Application.DTOs.Commerce.Smartstore;
using Entegro.Application.DTOs.ProductVariantAttributeCombination;
using Entegro.Application.DTOs.ProductVariantAttributeValue;
using Entegro.Domain.Entities;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entegro.Application.Mappings.Commerce.Smartstore
{
    public class SmartstoreProductVariantAttributeCombinationMapper
    {
        private static ILogger? _logger;
        public static void ConfigureLogger(ILogger logger)
        {
            _logger = logger;
        }
        public static ProductVariantAttributeCombinationDto? ToDto(SmartstoreProductVariantAttributeCombinationDto smartstoreProductVariantAttributeCombination)
        {
            try
            {
                if (smartstoreProductVariantAttributeCombination == null)
                {
                    return null;
                }

                ProductVariantAttributeCombinationDto productVariantAttributeCombination = new ProductVariantAttributeCombinationDto();
                productVariantAttributeCombination.ManufacturerPartNumber = smartstoreProductVariantAttributeCombination.ManufacturerPartNumber;
                productVariantAttributeCombination.ProductId = smartstoreProductVariantAttributeCombination.ProductId;
                productVariantAttributeCombination.RawAttribute = smartstoreProductVariantAttributeCombination.RawAttributes;
                productVariantAttributeCombination.Price = smartstoreProductVariantAttributeCombination.Price ?? 0;
                productVariantAttributeCombination.StokCode = smartstoreProductVariantAttributeCombination.Sku;
                productVariantAttributeCombination.Gtin = smartstoreProductVariantAttributeCombination.Gtin;

                return productVariantAttributeCombination;
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Product mapping sırasında hata oluştu. ProductId: {ProductId}", smartstoreProductVariantAttributeCombination.Id);
                return null;
            }
        }
        public static SmartstoreProductVariantAttributeCombinationDto? ToDto(ProductVariantAttributeCombinationDto productVariantAttributeCombination)
        {
            try
            {
                if (productVariantAttributeCombination == null)
                {
                    return null;
                }

                SmartstoreProductVariantAttributeCombinationDto smartstoreProductVariantAttributeCombination = new SmartstoreProductVariantAttributeCombinationDto();
                smartstoreProductVariantAttributeCombination.ProductId = productVariantAttributeCombination.ProductId;
                smartstoreProductVariantAttributeCombination.ManufacturerPartNumber = productVariantAttributeCombination.ManufacturerPartNumber;
                smartstoreProductVariantAttributeCombination.Sku = productVariantAttributeCombination.StokCode;
                smartstoreProductVariantAttributeCombination.Width = 0;
                smartstoreProductVariantAttributeCombination.Weight = 0;
                smartstoreProductVariantAttributeCombination.Value = "";
                smartstoreProductVariantAttributeCombination.HashCode = productVariantAttributeCombination.HashCode;
                smartstoreProductVariantAttributeCombination.AllowOutOfStockOrders = false;
                smartstoreProductVariantAttributeCombination.StockQuantity = productVariantAttributeCombination.StockQuantity;
                smartstoreProductVariantAttributeCombination.QuantityUnitId = null;
                smartstoreProductVariantAttributeCombination.DeliveryTimeId = null;
                smartstoreProductVariantAttributeCombination.IsActive = true;
                smartstoreProductVariantAttributeCombination.AssignedMediaFileIds = "";
                smartstoreProductVariantAttributeCombination.BasePriceBaseAmount = 0;
                smartstoreProductVariantAttributeCombination.BasePriceAmount = 0;
                smartstoreProductVariantAttributeCombination.Length = 0;
                smartstoreProductVariantAttributeCombination.Weight = 0;
                smartstoreProductVariantAttributeCombination.Price = productVariantAttributeCombination.Price;
                smartstoreProductVariantAttributeCombination.Gtin = productVariantAttributeCombination.Gtin;
                smartstoreProductVariantAttributeCombination.RawAttributes = productVariantAttributeCombination.RawAttribute;


                return smartstoreProductVariantAttributeCombination;

            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Product mapping sırasında hata oluştu. ProductId: {ProductId}", productVariantAttributeCombination.Id);
                return null;
            }
        }

        public static IEnumerable<ProductVariantAttributeCombinationDto> ToDtoList(IEnumerable<SmartstoreProductVariantAttributeCombinationDto> productVariantAttributeCombinations)
        {
            if (productVariantAttributeCombinations == null)
                yield break;

            foreach (var productVariantAttributeCombination in productVariantAttributeCombinations)
            {
                var dto = ToDto(productVariantAttributeCombination);
                if (dto != null)
                    yield return dto;
            }
        }
    }
}
