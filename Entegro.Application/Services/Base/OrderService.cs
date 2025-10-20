using Entegro.Application.DTOs.Address;
using Entegro.Application.DTOs.Common;
using Entegro.Application.DTOs.IntegrationSystem;
using Entegro.Application.DTOs.Order;
using Entegro.Application.DTOs.OrderItem;
using Entegro.Application.DTOs.Shipment;
using Entegro.Application.Interfaces.Repositories;
using Entegro.Application.Interfaces.Services.Base;
using MapsterMapper;
using Microsoft.Extensions.Logging;
using Order = Entegro.Domain.Entities.Checkout.Order;

namespace Entegro.Application.Services.Base
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly ICustomerService _customerService;
        private readonly IMediaFileService _medaFileService;
        private readonly IMapper _mapper;
        private readonly ILogger<OrderService> _logger;
        public OrderService(
            IOrderRepository orderRepository,
            ICustomerService customerService,
            IMediaFileService medaFileService,
            IMapper mapper,
            ILogger<OrderService> logger)
        {
            _orderRepository = orderRepository ?? throw new ArgumentNullException(nameof(orderRepository));
            _customerService = customerService ?? throw new ArgumentNullException(nameof(customerService));
            _medaFileService = medaFileService ?? throw new ArgumentNullException(nameof(medaFileService));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<OrderDto> AddAsync(CreateOrderDto createOrder)
        {
            var order = _mapper.Map<Order>(createOrder);

            await _orderRepository.AddAsync(order);

            return _mapper.Map<OrderDto>(order);
        }

        public async Task<int> CompleteOrderStatusCount()
        {
            return await _orderRepository.CompleteOrderStatusCount();
        }

        public async Task DeleteAsync(int orderId)
        {
            var order = await _orderRepository.GetByIdAsync(orderId);

            if (order == null)
            {
                throw new KeyNotFoundException($"Order with ID {orderId} not found.");
            }
            await _orderRepository.DeleteAsync(order);

        }

        public async Task<bool> ExistsByOrderNoAsync(string orderNo) => await _orderRepository.ExistsByOrderNoAsync(orderNo);

        public async Task<OrderDto?> GetByOrderNoAsync(string orderNo)
        {
            var order = await _orderRepository.GetByOrderNoAsync(orderNo);
            if (order == null)
            {
                throw new KeyNotFoundException($"Order with No {orderNo} not found.");
            }

            var orderDto = _mapper.Map<OrderDto>(order);
            return orderDto;
        }

        public async Task<List<OrderDto>> GetLast10OrdersWithItemsAsync()
        {
            var orders = await _orderRepository.GetLast10OrdersWithItemsAsync();
            var orderDtos = _mapper.Map<List<OrderDto>>(orders);
            return orderDtos;
        }

        public Task<List<(int Month, decimal TotalAmount)>> GetMonthlySalesByYearAsync(int year)
        {
            return _orderRepository.GetMonthlySalesByYearAsync(year);
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

        public async Task<OrderListPageDto> GetOrderPageAsync()
        {
            var orderPage = await _orderRepository.GetOrderPageAsync();
            return orderPage;
        }

        public async Task<OrderPrintDto> GetOrderPrintByIdAsync(int orderId, string packageNo)
        {
            var order = await _orderRepository.GetByIdAsync(orderId);
            if (order == null)
            {
                throw new KeyNotFoundException($"Order with ID {orderId} not found.");
            }

            var shipment = order.Shipments.Where(m => m.PackageNo == packageNo).FirstOrDefault();
            if (shipment == null)
            {
                throw new KeyNotFoundException($"Order with Shipment {packageNo} not found.");
            }

            OrderPrintDto orderPrint = new OrderPrintDto();
            orderPrint.OrderId = orderId;
            orderPrint.OrderNumber = order.OrderNumber;
            orderPrint.IntegrationSystem = _mapper.Map<IntegrationSystemDto>(order.IntegrationSystem);
            orderPrint.ShippingAddress = _mapper.Map<AddressDto>(order.ShippingAddress);
            orderPrint.BillingAddress = _mapper.Map<AddressDto>(order.BillingAddress);
            orderPrint.Shipment = _mapper.Map<ShipmentDto>(shipment);
            orderPrint.OrderItems = orderPrint.Shipment.ShipmentItems.Select(m => new OrderItemDto()
            {
                Quantity = m.Quantity,
                IntegrationSku = m.OrderItem.IntegrationSku,
                IntegrationProductName = m.OrderItem.IntegrationProductName,
            }).ToList();

            return orderPrint;
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

        public async Task<PagedResult<OrderListDto>> GetPagedAsync(GridCommand gridCommand, OrderListFilterDto filters, int orderStatus)
        {
            var orders = await _orderRepository.GetPagedAsync(gridCommand, filters, orderStatus);

            var items = await orders.Items.SelectAwait(async x =>
            {
                foreach (var orderItem in x.OrderItems)
                {
                    orderItem.ProductMainPicture = orderItem.ProductMainPictureId.HasValue ? await _medaFileService.GetUrl(orderItem.ProductMainPictureId.Value) : "/assets/img/products/empty.jpg";
                }
                return x;
            }).AsyncToList();

            return new PagedResult<OrderListDto>
            {
                Items = orders.Items,
                TotalCount = orders.TotalCount,
                PageNumber = orders.PageNumber,
                PageSize = orders.PageSize
            };
        }

        public async Task<decimal> GetTotalSalesAsync()
        {
            return await _orderRepository.GetTotalSalesAsync();
        }

        public async Task<OrderDto> UpdateAsync(UpdateOrderDto updateOrder)
        {
            if (updateOrder == null)
                throw new ArgumentNullException(nameof(updateOrder));

            var existingOrder = await _orderRepository.GetByIdAsync(updateOrder.Id);
            if (existingOrder == null)
                throw new KeyNotFoundException($"ID {updateOrder.Id} ile Order bulunamadı.");

            _mapper.Map(updateOrder, existingOrder);
            await _orderRepository.UpdateAsync(existingOrder);
            return _mapper.Map<OrderDto>(existingOrder);
        }
    }
}
