using Entegro.Application.DTOs.Common;
using Entegro.Domain.Entities;

namespace Entegro.Application.Interfaces.Repositories
{
    public interface IOrderRepository
    {
        Task<bool> ExistsByCustomerIdAsync(int customerId);
        Task<Order?> GetByIdAsync(int id);
        Task<Order?> GetByCustomerIdAsync(int customerId);
        Task<List<Order>> GetAllAsync();
        Task<PagedResult<Order>> GetAllAsync(int pageNumber, int pageSize);
        Task AddAsync(Order order);
        Task UpdateAsync(Order order);
        Task DeleteAsync(Order order);
        Task<bool> ExistsByOrderNoAsync(string orderNo);
    }
}
