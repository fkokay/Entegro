using Entegro.Domain.Entities.Checkout;

namespace Entegro.Application.Interfaces.Repositories
{
    public interface ICustomerAddressMappingRepository
    {
        Task<CustomerAddressMapping?> GetAsync(int customerId, int addressId);
        Task<IEnumerable<CustomerAddressMapping>> GetByCustomerIdAsync(int customerId);
        Task AddAsync(CustomerAddressMapping mapping);
        Task UpdateAsync(CustomerAddressMapping mapping);
        Task DeleteAsync(CustomerAddressMapping mapping);
        Task<bool> ExistsAsync(int customerId, int addressId);

    }
}
