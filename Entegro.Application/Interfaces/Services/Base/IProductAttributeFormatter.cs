using Entegro.Application.DTOs.Common;
using Entegro.Application.DTOs.Product;
using Entegro.Domain.Entities.Catalog;
using Entegro.Domain.Entities.Checkout;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entegro.Application.Interfaces.Services.Base
{
    public interface IProductAttributeFormatter
    {
        Task<string> FormatAttributesAsync(List<ProductVariantAttributeSelection> selections);
    }
}
