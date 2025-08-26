using Entegro.Application.DTOs.ProductVariantAttribute;
using Microsoft.EntityFrameworkCore.Infrastructure.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entegro.Application.DTOs.ProductVariantAttributeValue
{
    public class ProductVariantAttributeValueDto
    {
        public int ProductVariantAttributeId { get; set; }
        public ProductVariantAttributeDto ProductVariantAttribute { get; set; }
        public string Name { get; set; }
    }
}
