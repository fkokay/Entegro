using Entegro.Application.DTOs.CustomerAddressMapping;

namespace Entegro.Application.Interfaces.Services.Base
{
    public interface ICustomerAddressMappingService
    {
        Task<CustomerAddressMappingDto?> GetAsync(int customerId, int addressId);
        Task<IEnumerable<CustomerAddressMappingDto>> GetByCustomerIdAsync(int customerId);
        Task<CustomerAddressMappingDto> AddAsync(CreateCustomerAddressMappingDto mapping);
        Task<CustomerAddressMappingDto> UpdateAsync(UpdateCustomerAddressMappingDto mapping);

        Task DeleteAsync(int customerId, int addressId);
        Task<bool> ExistsAsync(int customerId, int addressId);
    }
}
