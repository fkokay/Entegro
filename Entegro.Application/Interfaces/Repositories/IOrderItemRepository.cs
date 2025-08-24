using Entegro.Application.DTOs.Common;
using Entegro.Domain.Entities;

namespace Entegro.Application.Interfaces.Repositories
{
    public interface IOrderItemRepository
    {
        Task<List<OrderItem>> GetByOrderIdAsync(int orderId);
        Task<OrderItem?> GetByIdAsync(int id);
        Task<List<OrderItem>> GetAllAsync();
        Task<PagedResult<OrderItem>> GetAllAsync(int pageNumber, int pageSize);
        Task AddAsync(OrderItem orderItem);
        Task UpdateAsync(OrderItem orderItem);
        Task DeleteAsync(OrderItem orderItem);
    }
}
