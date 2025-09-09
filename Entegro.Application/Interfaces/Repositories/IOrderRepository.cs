using Entegro.Application.DTOs.Common;
using Order = Entegro.Domain.Entities.Checkout.Order;

namespace Entegro.Application.Interfaces.Repositories
{
    public interface IOrderRepository
    {
        Task<Order?> GetByIdAsync(int id);
        Task<List<Order>> GetAllAsync();
        Task<PagedResult<Order>> GetAllAsync(int pageNumber, int pageSize);
        Task<PagedResult<Order>> GetPagedAsync(GridCommand gridCommand);
        Task AddAsync(Order order);
        Task UpdateAsync(Order order);
        Task DeleteAsync(Order order);
        Task<bool> ExistsByOrderNoAsync(string orderNo);
    }
}
