
using Entegro.Application.DTOs.Common;
using Entegro.Application.DTOs.Customer;
using Entegro.Application.DTOs.Order;
using Entegro.Application.Interfaces.Repositories;
using Entegro.Application.Interfaces.Services;
using MapsterMapper;
using Microsoft.Extensions.Logging;
using Order = Entegro.Domain.Entities.Checkout.Order;

namespace Entegro.Application.Services
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly ICustomerService _customerService;
        private readonly IMapper _mapper;
        private readonly ILogger<OrderService> _logger;
        public OrderService(IOrderRepository orderRepository, ICustomerService customerService, IMapper mapper, ILogger<OrderService> logger)
        {
            _orderRepository = orderRepository ?? throw new ArgumentNullException(nameof(orderRepository));
            _customerService = customerService ?? throw new ArgumentNullException(nameof(customerService));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<OrderDto> CreateOrderAsync(CreateOrderDto createOrder)
        {
            var order = _mapper.Map<Order>(createOrder);

            await _orderRepository.AddAsync(order);

            return _mapper.Map<OrderDto>(order);
        }

        public async Task DeleteOrderAsync(int orderId)
        {
            var order = await _orderRepository.GetByIdAsync(orderId);

            if (order == null)
            {
                throw new KeyNotFoundException($"Order with ID {orderId} not found.");
            }
            await _orderRepository.DeleteAsync(order);

        }

        public async Task<bool> ExistsByOrderNoAsync(string orderNo) => await _orderRepository.ExistsByOrderNoAsync(orderNo);

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

        public async Task<PagedResult<OrderDto>> GetPagedAsync(int pageNumber, int pageSize)
        {
            var orders = await _orderRepository.GetAllAsync(pageNumber, pageSize);
            var orderDtos = _mapper.Map<PagedResult<OrderDto>>(orders);
            return orderDtos;
        }

        public async Task<OrderDto> UpdateOrderAsync(UpdateOrderDto updateOrder)
        {
            await _orderRepository.UpdateAsync(_mapper.Map<Order>(updateOrder));
            return _mapper.Map<OrderDto>(updateOrder);
        }
    }
}
