using Entegro.ERP.Abstractions.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entegro.ERP.Application.Interfaces
{
    public interface IOrderService
    {
        Task<ErpResponse<OrderDto>> GetOrdersAsync(int page, int pageSize);
    }
}
