using Entegro.Application.DTOs.Common;
using Entegro.Application.DTOs.Customer;

namespace Entegro.Application.Interfaces.Services.Base
{
    public interface ICustomerService
    {
        Task<CustomerDto?> GetCustomerByIdAsync(int customerId);
        Task<CustomerDto?> GetCustomerByEmailAsync(string email);
        Task<IEnumerable<CustomerDto>> GetCustomersAsync();
        Task<PagedResult<CustomerDto>> GetCustomersAsync(int pageNumber, int pageSize);
        Task<PagedResult<CustomerDto>> GetPagedAsync(GridCommand gridCommand);
        Task<CustomerDto> AddAsync(CreateCustomerDto createCustomer);
        Task<bool> UpdateAsync(UpdateCustomerDto updateCustomer);
        Task<bool> DeleteAsync(int customerId);
        Task<bool> ExistsByEmailAsync(string email);
        Task<int> GetCustomerCount();
        Task<int> GetCurrentMonthCustomerCountAsync();
    }
}
