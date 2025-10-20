using Entegro.Application.DTOs.Common;
using Entegro.Application.DTOs.Order;
using Order = Entegro.Domain.Entities.Checkout.Order;

namespace Entegro.Application.Interfaces.Repositories
{
    public interface IOrderRepository
    {
        Task<Order?> GetByIdAsync(int id);
        Task<List<Order>> GetAllAsync();
        Task<PagedResult<Order>> GetAllAsync(int pageNumber, int pageSize);
        Task<PagedResult<OrderListDto>> GetPagedAsync(GridCommand gridCommand, OrderListFilterDto filters, int orderStatus);
        Task AddAsync(Order order);
        Task UpdateAsync(Order order);
        Task DeleteAsync(Order order);
        Task<bool> ExistsByOrderNoAsync(string orderNo);
        Task<Order?> GetByOrderNoAsync(string orderNo);
        Task<OrderListPageDto> GetOrderPageAsync();
        Task<decimal> GetTotalSalesAsync();
        Task<int> CompleteOrderStatusCount();
        Task<List<(int Month, decimal TotalAmount)>> GetMonthlySalesByYearAsync(int year);
        Task<List<Order>> GetLast10OrdersWithItemsAsync();

    }
}
