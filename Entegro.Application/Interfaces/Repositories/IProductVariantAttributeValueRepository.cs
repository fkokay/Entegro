using Entegro.Application.DTOs.ProductVariantAttributeValue;
using Entegro.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entegro.Application.Interfaces.Repositories
{
    public interface IProductVariantAttributeValueRepository
    {


        Task<ProductVariantAttributeValue?> GetByNameAsync(string name);
        Task AddAsync(ProductVariantAttributeValue data);
    }
}
