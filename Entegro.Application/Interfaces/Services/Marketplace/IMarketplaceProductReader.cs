using Entegro.Application.DTOs.Marketplace.Trendyol;
using Entegro.Application.DTOs.Product;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entegro.Application.Interfaces.Services.Marketplace
{
    public interface IMarketplaceProductReader
    {
        Task<IEnumerable<ProductDto>> GetProductsAsync(int pageSize = 50);
    }
}
