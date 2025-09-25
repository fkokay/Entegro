using Entegro.Application.DTOs.Address;
using Entegro.Application.DTOs.Common;

namespace Entegro.Application.Interfaces.Services
{
    public interface IAddressService
    {
        Task<AddressDto> GetByIdAsync(int id);
        Task<List<AddressDto>> GetAllAsync();
        Task<PagedResult<AddressDto>> GetPagedAsync(int pageNumber = 1, int pageSize = 7);
        Task<PagedResult<AddressDto>> GetPagedAsync(GridCommand gridCommand, int customerId);
        Task<AddressDto> AddAsync(CreateAddressDto address);
        Task<AddressDto> UpdateAsync(UpdateAddressDto address);
        Task DeleteAsync(int id);
    }
}
