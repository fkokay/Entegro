using Entegro.Application.DTOs.Common;
using Entegro.Application.DTOs.OrderItem;

namespace Entegro.Application.Interfaces.Services
{
    public interface IOrderItemService
    {
        Task<List<OrderItemDto>> GetByOrderIdAsync(int orderId);
        Task<OrderItemDto?> GetByIdAsync(int id);
        Task<List<OrderItemDto>> GetAllAsync();
        Task<PagedResult<OrderItemDto>> GetAllAsync(int pageNumber, int pageSize);
        Task AddAsync(OrderItemDto orderItem);
        Task UpdateAsync(OrderItemDto orderItem);
        Task DeleteAsync(int id);
    }
}
