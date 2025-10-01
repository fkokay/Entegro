using Entegro.Application.DTOs.Common;
using Entegro.Application.DTOs.OrderNote;

namespace Entegro.Application.Interfaces.Services.Base
{
    public interface IOrderNoteService
    {
        Task<OrderNoteDto?> GetOrderNoteByIdAsync(int orderNoteId);
        Task<PagedResult<OrderNoteDto>> GetPagedAsync(int pageNumber = 1, int pageSize = 7);
        Task<OrderNoteDto> AddAsync(CreateOrderNoteDto orderNote);
        Task<OrderNoteDto> UpdateAsync(UpdateOrderNoteDto orderNote);
        Task DeleteAsync(int orderNoteId);
    }
}
