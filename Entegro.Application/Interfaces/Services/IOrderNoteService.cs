using Entegro.Application.DTOs.Common;
using Entegro.Application.DTOs.OrderNote;

namespace Entegro.Application.Interfaces.Services
{
    public interface IOrderNoteService
    {
        Task<OrderNoteDto?> GetOrderNoteByIdAsync(int orderNoteId);
        Task<PagedResult<OrderNoteDto>> GetPagedAsync(int pageNumber = 1, int pageSize = 7);
        Task<OrderNoteDto> CreateOrderNoteAsync(CreateOrderNoteDto orderNote);
        Task<OrderNoteDto> UpdateOrderNoteAsync(UpdateOrderNoteDto orderNote);
        Task DeleteOrderNoteAsync(int orderNoteId);
    }
}
