using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entegro.Application.DTOs.Commerce.Smartstore
{
    public class SmartstoreProductVariantAttributeDto
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public int ProductAttributeId { get; set; }
        public SmartstoreProductAttributeDto ProductAttribute { get; set; }
        public string? TextPrompt { get; set; }
        public string? CustomData { get; set; }
        public bool IsRequired { get; set; }
        public int AttributeControlTypeId { get; set; }
        public int DisplayOrder { get; set; }

        public List<SmartstoreProductVariantAttributeValueDto> ProductVariantAttributeValues { get; set; } = new List<SmartstoreProductVariantAttributeValueDto>();
    }
}
