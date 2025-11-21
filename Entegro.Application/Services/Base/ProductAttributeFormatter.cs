using Entegro.Application.DTOs.Common;
using Entegro.Application.Interfaces.Services.Base;
using System.Text;

namespace Entegro.Application.Services.Base
{
    public class ProductAttributeFormatter : IProductAttributeFormatter
    {
        private readonly IProductVariantAttributeService _productVariantAttributeService;
        private readonly IProductVariantAttributeValueService _productVariantAttributeValueService;
        public ProductAttributeFormatter(IProductVariantAttributeService productVariantAttributeService, IProductVariantAttributeValueService productVariantAttributeValueService)
        {
            _productVariantAttributeService = productVariantAttributeService;
            _productVariantAttributeValueService = productVariantAttributeValueService;
        }
        public async Task<string> FormatAttributesAsync(List<ProductVariantAttributeSelection> selections)
        {
            var sb = new StringBuilder();

            foreach (var selection in selections)
            {
                var attribute = await _productVariantAttributeService.GetByIdAsync(selection.ProductVariantAttributeId);
                var attributevalue = await _productVariantAttributeValueService.GetByIdAsync(selection.ProductVariantAttributeValueId);

                if (attribute == null || attributevalue == null)
                    continue;

                if (sb.Length > 0)
                    sb.Append(",");

                sb.Append($"{attribute.ProductAttribute.Name}:{attributevalue.Name}");
            }

            return sb.ToString();
        }


    }
}
