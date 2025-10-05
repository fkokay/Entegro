using Entegro.Application.DTOs.Common;
using Entegro.Application.DTOs.ReturnRequest;

namespace Entegro.Application.Interfaces.Services.Base
{
    public interface IReturnRequestService
    {
        Task<bool> ExistsByIdAsync(int id);
        Task<bool> ExistsByOrderNumberAsync(string orderNumber);
        Task<bool> ExistsByCustomerNameAsync(string customerName);
        Task<bool> ExistsByReturnRequestStatusAsync(int requestStatus);
        Task<ReturnRequestDto?> GetByIdAsync(int id);
        Task<ReturnRequestDto?> GetByOrderNumberAsync(string orderNumber);
        Task<ReturnRequestDto?> GetByCustomerNameAsync(string name);
        Task<ReturnRequestDto?> GetByReturnRequestDtoStatusAsync(int requestStatus);
        Task<PagedResult<ReturnRequestDto>> GetPagedAsync(GridCommand gridCommand);
        Task AddAsync(CreateReturnRequestDto returnReturnRequest);
        Task<ReturnRequestDto> UpdateAsync(UpdateReturnRequestDto returnReturnRequest);
        Task DeleteAsync(int id);
    }
}
