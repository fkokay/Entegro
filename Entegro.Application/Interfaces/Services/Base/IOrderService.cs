using Entegro.Application.DTOs.Common;
using Entegro.Application.DTOs.Order;

namespace Entegro.Application.Interfaces.Services.Base
{
    public interface IOrderService
    {
        Task<OrderDto> GetOrderByIdAsync(int orderId);
        Task<OrderPrintDto> GetOrderPrintByIdAsync(int orderId, string packageNo);
        Task<bool> ExistsByOrderNoAsync(string orderNo);
        Task<IEnumerable<OrderDto>> GetOrdersAsync();
        Task<PagedResult<OrderDto>> GetPagedAsync(int pageNumber, int pageSize);
        Task<PagedResult<OrderListDto>> GetPagedAsync(GridCommand gridCommand, int orderStatus);
        Task<OrderDto> AddAsync(CreateOrderDto createOrder);
        Task<OrderDto> UpdateAsync(UpdateOrderDto updateOrder);
        Task DeleteAsync(int orderId);
        Task<OrderListPageDto> GetOrderPageAsync();
        Task<decimal> GetTotalSalesAsync();
        Task<int> CompleteOrderStatusCount();
        Task<List<(int Month, decimal TotalAmount)>> GetMonthlySalesByYearAsync(int year);
        Task<List<OrderDto>> GetLast10OrdersWithItemsAsync();
    }
}
