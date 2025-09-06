using Entegro.Application.DTOs.Common;
using Entegro.Application.DTOs.OrderNote;
using Entegro.Application.Interfaces.Repositories;
using Entegro.Application.Interfaces.Services;
using Entegro.Domain.Entities;
using MapsterMapper;

namespace Entegro.Application.Services
{
    public class OrderNoteService : IOrderNoteService
    {
        private readonly IOrderNoteRepository _orderNoteRepository;
        private readonly IMapper _mapper;
        public OrderNoteService(IOrderNoteRepository orderNoteRepository, IMapper mapper)
        {
            _orderNoteRepository = orderNoteRepository ?? throw new ArgumentNullException(nameof(orderNoteRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<OrderNoteDto> CreateOrderNoteAsync(CreateOrderNoteDto orderNote)
        {
            var orderNoteDto = _mapper.Map<OrderNote>(orderNote);
            await _orderNoteRepository.AddAsync(orderNoteDto);
            return _mapper.Map<OrderNoteDto>(orderNoteDto);
        }

        public async Task DeleteOrderNoteAsync(int orderNoteId)
        {
            var orderNote = await _orderNoteRepository.GetByAsync(m => m.Id == orderNoteId);
            if (orderNote == null)
                throw new KeyNotFoundException($"Category with ID {orderNoteId} not found.");

            await _orderNoteRepository.DeleteAsync(orderNote);
        }

        public async Task<OrderNoteDto?> GetOrderNoteByIdAsync(int orderNoteId)
        {
            var orderNote = await _orderNoteRepository.GetByAsync(m => m.Id == orderNoteId);
            return orderNote == null ? null : _mapper.Map<OrderNoteDto>(orderNote);
        }

        public async Task<PagedResult<OrderNoteDto>> GetPagedAsync(int pageNumber = 1, int pageSize = 7)
        {
            var orderNotes = await _orderNoteRepository.GetAllAsync("", pageNumber, pageSize);
            return new PagedResult<OrderNoteDto>
            {
                Items = _mapper.Map<IEnumerable<OrderNoteDto>>(orderNotes.Items),
                TotalCount = orderNotes.TotalCount,
                PageNumber = orderNotes.PageNumber,
                PageSize = orderNotes.PageSize
            };
        }

        public async Task<OrderNoteDto> UpdateOrderNoteAsync(UpdateOrderNoteDto orderNote)
        {
            var orderNoteDto = _mapper.Map<OrderNote>(orderNote);
            await _orderNoteRepository.UpdateAsync(orderNoteDto);
            return _mapper.Map<OrderNoteDto>(orderNoteDto);
        }
    }
}
