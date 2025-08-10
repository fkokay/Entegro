using Entegro.Application.DTOs.Commerce.Smartstore;
using Entegro.Application.DTOs.Marketplace.Trendyol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entegro.Application.Interfaces.Services.Marketplace
{
    public interface ITrendyolService
    {
        Task<IEnumerable<TrendyolProductDto>> GetProductsAsync(int pageSize = 50);
    }
}
