using Entegro.Application.DTOs.Common;
using Entegro.Application.DTOs.Order;
using Entegro.Application.DTOs.ReturnRequest;
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
        Task<PagedResult<ReturnRequestListDto>> GetPagedAsync(GridCommand gridCommand, ReturnRequestListFilterDto filters, int returnrequestStatusId);
        Task AddAsync(ReturnRequest returnRequest);
        Task UpdateAsync(ReturnRequest returnRequest);
        Task DeleteAsync(ReturnRequest returnRequest);
        Task<ReturnListPageDto> GetReturnRequestPageAsync();
    }
}
