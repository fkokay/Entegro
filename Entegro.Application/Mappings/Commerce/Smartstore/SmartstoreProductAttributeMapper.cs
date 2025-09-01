using Entegro.Application.DTOs.Commerce.Smartstore;
using Entegro.Application.DTOs.ProductAttribute;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entegro.Application.Mappings.Commerce.Smartstore
{
    public class SmartstoreProductAttributeMapper
    {
        private static ILogger _logger;
        public static void ConfigureLogger(ILogger logger)
        {
            _logger = logger;
        }
        public static ProductAttributeDto? ToDto(SmartstoreProductAttributeDto smartstoreProductAttribute)
        {
            try
            {
                if (smartstoreProductAttribute == null)
                {
                    return null;
                }

                SmartstoreManufacturerMapper.ConfigureLogger(_logger);

                ProductAttributeDto productAttribute = new ProductAttributeDto();
                productAttribute.DisplayOrder = smartstoreProductAttribute.DisplayOrder;
                productAttribute.Description = smartstoreProductAttribute.Description;
                productAttribute.Name = smartstoreProductAttribute.Name;
                productAttribute.Id = smartstoreProductAttribute.Id;
              


                return productAttribute; ;
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Product attribute mapping sırasında hata oluştu. ProductId: {ProductId}", smartstoreProductAttribute.Id);
                return null;
            }
        }
        public static SmartstoreProductAttributeDto? ToDto(ProductAttributeDto  productAttribute)
        {
            try
            {
                if (productAttribute == null)
                {
                    return null;
                }

                SmartstoreManufacturerMapper.ConfigureLogger(_logger);

                SmartstoreProductAttributeDto smartstoreProductAttribute = new SmartstoreProductAttributeDto();
                smartstoreProductAttribute.DisplayOrder = productAttribute.DisplayOrder;
                smartstoreProductAttribute.Id = 0;
                smartstoreProductAttribute.AllowFiltering = true;
                smartstoreProductAttribute.ExportMappings = null;
                smartstoreProductAttribute.Alias = productAttribute.Name;
                smartstoreProductAttribute.Description= productAttribute.Description;
                smartstoreProductAttribute.Name = productAttribute.Name;
                smartstoreProductAttribute.FacetTemplateHint = "0";
                smartstoreProductAttribute.IndexOptionNames = false;
                return smartstoreProductAttribute;
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Product attibute mapping sırasında hata oluştu. ProductId: {ProductId}", productAttribute.Id);
                return null;
            }
        }

        public static IEnumerable<ProductAttributeDto> ToDtoList(IEnumerable<SmartstoreProductAttributeDto> productAttributes)
        {
            if (productAttributes == null)
                yield break;

            foreach (var product in productAttributes)
            {
                var dto = ToDto(product);
                if (dto != null)
                    yield return dto;
            }
        }
    }
}
