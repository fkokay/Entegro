using Entegro.Application.DTOs.Common;
using Entegro.Application.DTOs.Order;

namespace Entegro.Application.Interfaces.Services
{
    public interface IOrderService
    {
        Task<OrderDto> GetOrderByIdAsync(int orderId);
        Task<bool> ExistsByOrderNoAsync(string orderNo);
        Task<IEnumerable<OrderDto>> GetOrdersAsync();
        Task<PagedResult<OrderDto>> GetPagedAsync(int pageNumber, int pageSize);
        Task<PagedResult<OrderModel>> GetPagedAsync(GridCommand gridCommand,int orderStatus);
        Task<OrderDto> CreateOrderAsync(CreateOrderDto createOrder);
        Task<OrderDto> UpdateOrderAsync(UpdateOrderDto updateOrder);
        Task DeleteOrderAsync(int orderId);
    }
}
