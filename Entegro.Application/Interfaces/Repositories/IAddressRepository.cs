using Entegro.Application.DTOs.Common;
using Entegro.Domain.Entities.Common;

namespace Entegro.Application.Interfaces.Repositories
{
    public interface IAddressRepository
    {
        Task<Address?> GetByIdAsync(int id);
        Task<List<Address>> GetAllAsync();
        Task<PagedResult<Address>> GetAllAsync(int pageNumber, int pageSize);
        Task<PagedResult<Address>> GetPagedAsync(GridCommand gridCommand, int customerId);
        Task AddAsync(Address address);
        Task UpdateAsync(Address address);
        Task DeleteAsync(Address address);
    }
}
