using Entegro.Application.DTOs.Common;
using Entegro.Domain.Entities.Checkout;

namespace Entegro.Application.Interfaces.Repositories
{
    public interface IReturnRequestRepository
    {
        Task<bool> ExistsByIdAsync(int id);
        Task<bool> ExistsByOrderNumberAsync(string orderNumber);
        Task<bool> ExistsByCustomerNameAsync(string customerName);
        Task<bool> ExistsByReturnRequestStatusAsync(int requestStatus);
        Task<ReturnRequest?> GetByIdAsync(int id);
        Task<ReturnRequest?> GetByOrderNumberAsync(string orderNumber);
        Task<ReturnRequest?> GetByCustomerNameAsync(string name);
        Task<ReturnRequest?> GetByReturnRequestStatusAsync(int requestStatus);
        Task<PagedResult<ReturnRequest>> GetPagedAsync(GridCommand gridCommand);
        Task AddAsync(ReturnRequest returnRequest);
        Task UpdateAsync(ReturnRequest returnRequest);
        Task DeleteAsync(ReturnRequest returnRequest);
    }
}
