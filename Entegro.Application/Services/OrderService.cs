using AutoMapper;
using Entegro.Application.DTOs.Common;
using Entegro.Application.DTOs.Order;
using Entegro.Application.Interfaces.Repositories;
using Entegro.Application.Interfaces.Services;
using Entegro.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entegro.Application.Services
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IMapper _mapper;
        public OrderService(IOrderRepository orderRepository,IMapper mapper)
        {
            _orderRepository = orderRepository ?? throw new ArgumentNullException(nameof(orderRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<int> CreateOrderAsync(CreateOrderDto createOrder)
        {
            var order = _mapper.Map<Order>(createOrder);
            await _orderRepository.AddAsync(order);

            return order.Id;
        }

        public async Task<bool> DeleteOrderAsync(int orderId)
        {
            var order = await _orderRepository.GetByIdAsync(orderId);

            if (order == null)
            {
                throw new KeyNotFoundException($"Order with ID {orderId} not found.");
            }
            await _orderRepository.DeleteAsync(order);
            return true;
        }

        public async Task<bool> ExistsByOrderNoAsync(string orderNo)
        {
            return await _orderRepository.ExistsByOrderNoAsync(orderNo);
        }

        public async Task<OrderDto> GetOrderByIdAsync(int orderId)
        {
            var order = await _orderRepository.GetByIdAsync(orderId);
            if (order == null)
            {
                throw new KeyNotFoundException($"Order with ID {orderId} not found.");
            }

            var orderDto = _mapper.Map<OrderDto>(order);
            return orderDto;
        }

        public async Task<IEnumerable<OrderDto>> GetOrdersAsync()
        {
            var orders = await _orderRepository.GetAllAsync();
            var orderDtos = _mapper.Map<IEnumerable<OrderDto>>(orders);
            return orderDtos;
        }

        public async Task<PagedResult<OrderDto>> GetOrdersAsync(int pageNumber, int pageSize)
        {
            var orders = await _orderRepository.GetAllAsync(pageNumber,pageSize);
            var orderDtos = _mapper.Map<PagedResult<OrderDto>>(orders);
            return orderDtos;
        }

        public async Task<bool> UpdateOrderAsync(UpdateOrderDto updateOrder)
        {
            await _orderRepository.UpdateAsync(_mapper.Map<Order>(updateOrder));
            return true;
        }
    }
}
