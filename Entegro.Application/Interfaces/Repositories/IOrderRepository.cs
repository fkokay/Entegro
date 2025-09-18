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
        Task<PagedResult<OrderModel>> GetPagedAsync(GridCommand gridCommand,int orderStatus);
        Task AddAsync(Order order);
        Task UpdateAsync(Order order);
        Task DeleteAsync(Order order);
        Task<bool> ExistsByOrderNoAsync(string orderNo);
    }
}
