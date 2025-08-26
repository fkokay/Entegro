using Entegro.Application.DTOs.ProductVariantAttribute;
using Entegro.Application.DTOs.ProductVariantAttributeValue;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entegro.Application.Interfaces.Services
{
    public interface IProductVariantAttributeValueService
    {
        Task<ProductVariantAttributeValueDto?> GetByNameAsync(string name);
        Task<int> AddAsync(ProductVariantAttributeValueDto data);
    }
}
