using Entegro.Application.DTOs.Commerce.Smartstore;
using Entegro.Application.DTOs.ProductVariantAttribute;
using Microsoft.Extensions.Logging;

namespace Entegro.Application.Mappings.Commerce.Smartstore
{
    public class SmartstoreProductVariantAttributeMapper
    {
        private static ILogger? _logger;
        public static void ConfigureLogger(ILogger logger)
        {
            _logger = logger;
        }
        public static ProductVariantAttributeDto? ToDto(SmartstoreProductVariantAttributeDto smartstoreProductVariantAttribute)
        {
            try
            {
                if (smartstoreProductVariantAttribute == null)
                {
                    return null;
                }

                ProductVariantAttributeDto productVariantAttribute = new ProductVariantAttributeDto();
                productVariantAttribute.Id = smartstoreProductVariantAttribute.Id;
                productVariantAttribute.ProductAttributeId = smartstoreProductVariantAttribute.ProductAttributeId;
                productVariantAttribute.ProductAttribute = SmartstoreProductAttributeMapper.ToDto(smartstoreProductVariantAttribute.ProductAttribute);
                productVariantAttribute.AttributeControlTypeId = smartstoreProductVariantAttribute.AttributeControlTypeId;
                productVariantAttribute.ProductId = smartstoreProductVariantAttribute.ProductId;
                productVariantAttribute.IsRequried = smartstoreProductVariantAttribute.IsRequired;
                productVariantAttribute.DisplayOrder = smartstoreProductVariantAttribute.DisplayOrder;
                productVariantAttribute.ProductVariantAttributeValues = SmartstoreProductVariantAttributeValueMapper.ToDtoList(smartstoreProductVariantAttribute.ProductVariantAttributeValues).ToList();

                return productVariantAttribute;
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Product mapping sırasında hata oluştu. ProductId: {ProductId}", smartstoreProductVariantAttribute.Id);
                return null;
            }
        }
        public static SmartstoreProductVariantAttributeDto? ToDto(ProductVariantAttributeDto productVariantAttribute)
        {
            try
            {
                if (productVariantAttribute == null)
                {
                    return null;
                }

                SmartstoreProductVariantAttributeDto smartstoreProductVariantAttribute = new SmartstoreProductVariantAttributeDto();
                smartstoreProductVariantAttribute.Id = productVariantAttribute.Id;
                smartstoreProductVariantAttribute.ProductId = productVariantAttribute.ProductId;
                smartstoreProductVariantAttribute.ProductAttributeId = productVariantAttribute.ProductAttributeId;
                smartstoreProductVariantAttribute.AttributeControlTypeId = productVariantAttribute.AttributeControlTypeId;
                smartstoreProductVariantAttribute.CustomData = "";
                smartstoreProductVariantAttribute.DisplayOrder = 0;
                smartstoreProductVariantAttribute.IsRequired = true;
                smartstoreProductVariantAttribute.TextPrompt = "";


                return smartstoreProductVariantAttribute;

            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Product mapping sırasında hata oluştu. ProductId: {ProductId}", productVariantAttribute.Id);
                return null;
            }
        }

        public static IEnumerable<ProductVariantAttributeDto> ToDtoList(IEnumerable<SmartstoreProductVariantAttributeDto> smartstoreProductVariantAttributes)
        {
            if (smartstoreProductVariantAttributes == null)
                yield break;

            foreach (var smartstoreProductVariantAttribute in smartstoreProductVariantAttributes)
            {
                var dto = ToDto(smartstoreProductVariantAttribute);
                if (dto != null)
                    yield return dto;
            }
        }
    }
}
