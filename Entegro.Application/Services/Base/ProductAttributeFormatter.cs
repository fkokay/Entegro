using Entegro.Application.DTOs.Common;
using Entegro.Application.Interfaces.Services.Base;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entegro.Application.Services.Base
{
    public class ProductAttributeFormatter : IProductAttributeFormatter
    {
        private readonly IProductVariantAttributeService _productVariantAttributeService;
        private readonly IProductVariantAttributeValueService _productVariantAttributeValueService;
        public ProductAttributeFormatter(IProductVariantAttributeService productVariantAttributeService,IProductVariantAttributeValueService productVariantAttributeValueService) 
        {
            _productVariantAttributeService = productVariantAttributeService;
            _productVariantAttributeValueService = productVariantAttributeValueService;
        }
        public async Task<string> FormatAttributesAsync(List<ProductVariantAttributeSelection> selections)
        {
            string formattedValue = "";
            foreach (var selection in selections)
            {
                var attribute = await _productVariantAttributeService.GetByIdAsync(selection.ProductVariantAttributeId);
                var attributevalue = await _productVariantAttributeValueService.GetByIdAsync(selection.ProductVariantAttributeValueId);

                if (attribute == null || attributevalue == null)
                {
                    continue;
                }

                formattedValue = string.IsNullOrEmpty(formattedValue) ? "" : ",";
                formattedValue += string.Format("{0}:{1}", attribute.ProductAttribute.Name, attributevalue.Name);
            }
     

            return formattedValue;
        }

    }
}
