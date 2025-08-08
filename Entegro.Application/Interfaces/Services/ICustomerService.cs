using Entegro.Application.DTOs.Common;
using Entegro.Application.DTOs.Customer;
using Entegro.Application.Interfaces.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entegro.Application.Interfaces.Services
{
    public interface ICustomerService
    {
        Task<CustomerDto> GetCustomerByIdAsync(int customerId);
        Task<CustomerDto> GetCustomerByEmailAsync(string email);
        Task<IEnumerable<CustomerDto>> GetCustomersAsync();
        Task<PagedResult<CustomerDto>> GetCustomersAsync(int pageNumber, int pageSize);
        Task<int> CreateCustomerAsync(CreateCustomerDto createCustomer);
        Task<bool> UpdateCustomerAsync(UpdateCustomerDto updateCustomer);
        Task<bool> DeleteCustomerAsync(int customerId);
        Task<bool> ExistsByEmailAsync(string email);
    }
}
