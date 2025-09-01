using Entegro.Application.DTOs.Commerce.Smartstore;
using Entegro.Application.DTOs.ProductVariantAttributeValue;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entegro.Application.Mappings.Commerce.Smartstore
{
    public class SmartstoreProductVariantAttributeValueMapper
    {
        private static ILogger? _logger;
        public static void ConfigureLogger(ILogger logger)
        {
            _logger = logger;
        }
        public static ProductVariantAttributeValueDto? ToDto(SmartstoreProductVariantAttributeValueDto smartstoreProductVariantAttributeValue)
        {
            try
            {
                if (smartstoreProductVariantAttributeValue == null)
                {
                    return null;
                }

                ProductVariantAttributeValueDto productVariantAttributeValue = new ProductVariantAttributeValueDto();
                productVariantAttributeValue.Id = smartstoreProductVariantAttributeValue.Id;
                productVariantAttributeValue.ProductVariantAttributeId = smartstoreProductVariantAttributeValue.ProductVariantAttributeId;
                productVariantAttributeValue.Name = smartstoreProductVariantAttributeValue.Name;

                return productVariantAttributeValue;
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Product mapping sırasında hata oluştu. ProductId: {ProductId}", smartstoreProductVariantAttributeValue.Id);
                return null;
            }
        }
        public static SmartstoreProductVariantAttributeValueDto? ToDto(ProductVariantAttributeValueDto productVariantAttributeValue)
        {
            try
            {
                if (productVariantAttributeValue == null)
                {
                    return null;
                }

                SmartstoreProductVariantAttributeValueDto smartstoreProductVariantAttributeValue = new SmartstoreProductVariantAttributeValueDto();
                smartstoreProductVariantAttributeValue.Id = 0;
                smartstoreProductVariantAttributeValue.ProductVariantAttributeId = productVariantAttributeValue.ProductVariantAttributeId;
                smartstoreProductVariantAttributeValue.Name = productVariantAttributeValue.Name;
                smartstoreProductVariantAttributeValue.Alias = productVariantAttributeValue.Name;
                smartstoreProductVariantAttributeValue.MediaFileId = null;
                smartstoreProductVariantAttributeValue.Color = "";
                smartstoreProductVariantAttributeValue.PriceAdjustment = 0;
                smartstoreProductVariantAttributeValue.WeightAdjustment = 0;
                smartstoreProductVariantAttributeValue.IsPreSelected = false;
                smartstoreProductVariantAttributeValue.DisplayOrder = 0;
                smartstoreProductVariantAttributeValue.ValueTypeId = 0;
                smartstoreProductVariantAttributeValue.LinkedProductId = 0;
                smartstoreProductVariantAttributeValue.Quantity = 0;


                return smartstoreProductVariantAttributeValue;
               
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Product mapping sırasında hata oluştu. ProductId: {ProductId}", productVariantAttributeValue.Id);
                return null;
            }
        }

        public static IEnumerable<ProductVariantAttributeValueDto> ToDtoList(IEnumerable<SmartstoreProductVariantAttributeValueDto> smartstoreProductVariantAttributeValues)
        {
            if (smartstoreProductVariantAttributeValues == null)
                yield break;

            foreach (var smartstoreProductVariantAttributeValue in smartstoreProductVariantAttributeValues)
            {
                var dto = ToDto(smartstoreProductVariantAttributeValue);
                if (dto != null)
                    yield return dto;
            }
        }
    }
}
