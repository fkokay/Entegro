using Entegro.Application.DTOs.Marketplace.Trendyol;
using Entegro.Application.DTOs.Order;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entegro.Application.Interfaces.Services.Marketplace
{
    public interface IMarketplaceOrderReader
    {
        Task<IEnumerable<OrderDto>> GetOrderAsync(int pageSize = 50);
    }
}
