using Entegro.ERP.Abstractions.DTOs;
using Entegro.ERP.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entegro.ERP.Application.Services
{
    public class OrderService : IOrderService
    {
        public Task<ErpResponse<OrderDto>> GetOrdersAsync(int page, int pageSize)
        {
            throw new NotImplementedException();
        }
    }
}
