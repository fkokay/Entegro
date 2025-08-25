using Entegro.Application.DTOs.Erp;
using Entegro.Application.DTOs.ProductAttribute;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entegro.Application.Mappings.Erp
{
    public static class ErpProductVariantAttributeMapper
    {
        private static ILogger? _logger;
        public static void ConfigureLogger(ILogger logger)
        {
            _logger = logger;
        }

        public static List<ProductAttributeDto> ToDto(ErpProductDto erpProduct)
        {
            List<ProductAttributeDto> result = new List<ProductAttributeDto>();
            if (erpProduct == null)
            {
                return null;
            }

            var productAttribute1 = erpProduct.ProductVariantAttributes.Select(m => m.Variant1Name).Distinct().ToList();
            var productAttribute2 = erpProduct.ProductVariantAttributes.Select(m => m.Variant2Name).Distinct().ToList();
            var productAttribute3 = erpProduct.ProductVariantAttributes.Select(m => m.Variant3Name).Distinct().ToList();
            var productAttribute4 = erpProduct.ProductVariantAttributes.Select(m => m.Variant4Name).Distinct().ToList();
            var productAttribute5 = erpProduct.ProductVariantAttributes.Select(m => m.Variant5Name).Distinct().ToList();


            foreach (var item in productAttribute1)
            {
                if (!string.IsNullOrEmpty(item))
                {
                    var values = erpProduct.ProductVariantAttributes.Where(m => m.Variant1Name == item).Select(m => m.Variant1Value).Distinct().ToList();
                    ProductAttributeDto productAttribute = new ProductAttributeDto();
                    productAttribute.Name = item;
                    productAttribute.DisplayOrder = 0;
                    productAttribute.Description = "";
                    productAttribute.ProductAttributeValues = values.Select(m => new DTOs.ProductAttributeValue.ProductAttributeValueDto()
                    {
                        Name = m,
                        DisplayOrder = 0,
                    }).ToList();

                    result.Add(productAttribute); 
                }
            }
            foreach (var item in productAttribute2)
            {
                if (!string.IsNullOrEmpty(item))
                {
                    var values = erpProduct.ProductVariantAttributes.Where(m => m.Variant2Name == item).Select(m => m.Variant2Value).Distinct().ToList();
                    ProductAttributeDto productAttribute = new ProductAttributeDto();
                    productAttribute.Name = item;
                    productAttribute.DisplayOrder = 0;
                    productAttribute.Description = "";
                    productAttribute.ProductAttributeValues = values.Select(m => new DTOs.ProductAttributeValue.ProductAttributeValueDto()
                    {
                        Name = m,
                        DisplayOrder = 0,
                    }).ToList();

                    result.Add(productAttribute); 
                }
            }



            return result;
        }
    }
}
