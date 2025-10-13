using Entegro.Application.DTOs.Common;
using Entegro.Domain.Entities.Checkout;

namespace Entegro.Application.Interfaces.Repositories
{
    public interface ICustomerRepository
    {
        Task<bool> ExistsByEmailAsync(string email);
        Task<Customer?> GetByIdAsync(int id);
        Task<Customer?> GetByEmailAsync(string email);
        Task<List<Customer>> GetAllAsync();
        Task<PagedResult<Customer>> GetAllAsync(int pageNumber, int pageSize);
        Task<PagedResult<Customer>> GetPagedAsync(GridCommand gridCommand);
        Task AddAsync(Customer customer);
        Task UpdateAsync(Customer customer);
        Task DeleteAsync(Customer customer);
        Task<int> GetCustomerCount();
        Task<int> GetCurrentMonthCustomerCountAsync();
    }
}
