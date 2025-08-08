using Entegro.Application.DTOs.Common;
using Entegro.Application.DTOs.Order;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entegro.Application.Interfaces.Services
{
    public interface IOrderService
    {
        Task<OrderDto> GetOrderByIdAsync(int orderId);
        Task<bool> ExistsByOrderNoAsync(string orderNo);
        Task<IEnumerable<OrderDto>> GetOrdersAsync();
        Task<PagedResult<OrderDto>> GetOrdersAsync(int pageNumber, int pageSize);
        Task<int> CreateOrderAsync(CreateOrderDto createOrder);
        Task<bool> UpdateOrderAsync(UpdateOrderDto updateOrder);
        Task<bool> DeleteOrderAsync(int orderId);
    }
}
