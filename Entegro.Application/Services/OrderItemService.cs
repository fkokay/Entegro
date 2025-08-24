using AutoMapper;
using Entegro.Application.DTOs.Common;
using Entegro.Application.DTOs.OrderItem;
using Entegro.Application.Interfaces.Repositories;
using Entegro.Application.Interfaces.Services;
using Entegro.Domain.Entities;

namespace Entegro.Application.Services
{
    public class OrderItemService : IOrderItemService
    {

        private readonly IOrderItemRepository _orderItemRepository;
        private readonly IMapper _mapper;
        public OrderItemService(IOrderItemRepository orderItemRepository, IMapper mapper)
        {
            _orderItemRepository = orderItemRepository ?? throw new ArgumentNullException(nameof(orderItemRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }
        public async Task AddAsync(OrderItemDto orderItem)
        {
            var orderItemDto = _mapper.Map<OrderItem>(orderItem);
            await _orderItemRepository.AddAsync(orderItemDto);

        }

        public async Task DeleteAsync(int id)
        {
            var orderItem = await _orderItemRepository.GetByIdAsync(id);
            await _orderItemRepository.DeleteAsync(orderItem);
        }

        public async Task<List<OrderItemDto>> GetAllAsync()
        {
            var orderItems = await _orderItemRepository.GetAllAsync();
            var orderItemDtos = _mapper.Map<IEnumerable<OrderItemDto>>(orderItems);
            return orderItemDtos.ToList();
        }

        public async Task<PagedResult<OrderItemDto>> GetAllAsync(int pageNumber, int pageSize)
        {
            var orderItems = await _orderItemRepository.GetAllAsync(pageNumber, pageSize);
            var orderItemDtos = _mapper.Map<PagedResult<OrderItemDto>>(orderItems);
            return orderItemDtos;
        }

        public async Task<OrderItemDto?> GetByIdAsync(int id)
        {
            return await _orderItemRepository.GetByIdAsync(id) is OrderItem orderItem ? _mapper.Map<OrderItemDto>(orderItem) : null;
        }

        public async Task<List<OrderItemDto>> GetByOrderIdAsync(int orderId)
        {
            var orderItems = await _orderItemRepository.GetByOrderIdAsync(orderId);
            var orderItemDtos = _mapper.Map<List<OrderItemDto>>(orderItems);
            return orderItemDtos;
        }

        public async Task UpdateAsync(OrderItemDto orderItem)
        {
            var orderItemDto = _mapper.Map<OrderItem>(orderItem);
            await _orderItemRepository.UpdateAsync(orderItemDto);
        }
    }
}
