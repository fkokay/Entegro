using Entegro.Application.DTOs.Common;
using Entegro.Application.DTOs.OrderItem;
using Entegro.Application.DTOs.Product;
using Entegro.Application.Interfaces.Repositories;
using Entegro.Application.Interfaces.Services.Base;
using Entegro.Domain.Entities.Checkout;
using Entegro.Domain.Enums;
using MapsterMapper;

namespace Entegro.Application.Services.Base
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
        public async Task<OrderItemDto> AddAsync(CreateOrderItemDto orderItem)
        {
            var orderItemDto = _mapper.Map<OrderItem>(orderItem);
            await _orderItemRepository.AddAsync(orderItemDto);
            return _mapper.Map<OrderItemDto>(orderItemDto);

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

        public async Task<PagedResult<OrderItemDto>> GetPagedAsync(int pageNumber, int pageSize)
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

        public async Task<OrderItemDto> UpdateAsync(UpdateOrderItemDto orderItem)
        {
            if (orderItem == null)
                throw new ArgumentNullException(nameof(orderItem));

            var existingOrderItem = await _orderItemRepository.GetByIdAsync(orderItem.Id);
            if (existingOrderItem == null)
                throw new KeyNotFoundException($"ID {existingOrderItem.Id} ile OrderItem bulunamadı.");


            _mapper.Map(orderItem, existingOrderItem);

            await _orderItemRepository.UpdateAsync(existingOrderItem);

            return _mapper.Map<OrderItemDto>(existingOrderItem);
        }

        public async Task<List<OrderItemDto>> GetAllWithIntegrationSkuAsync(string integrationSku)
        {
            var orderItems = await _orderItemRepository.GetAllIntegrationSkuWithAsync(integrationSku);
            var orderItemDtos = _mapper.Map<IEnumerable<OrderItemDto>>(orderItems);
            return orderItemDtos.ToList();
        }

        public async Task<List<ProductSalesDto>> GetProductSalesByMarketplaceAsync(int groupByType)
        {
            var (startDate, endDate) = GetPeriodDateRange(groupByType);

            var orderItems = await _orderItemRepository.GetOrderItemsWithProductAndIntegrationAsync();

            var filteredItems = orderItems
                .Where(oi =>
                    oi.Order.OrderStatusId == (int)OrderStatus.Complete &&
                    oi.Order.OrderDateUtc >= startDate &&
                    oi.Order.OrderDateUtc < endDate &&
                    oi.Order.IntegrationSystem?.IntegrationSystemParameters != null
                )
                .SelectMany(oi =>
                    oi.Order.IntegrationSystem.IntegrationSystemParameters
                        .Where(p => p.Key == "CommerceType" || p.Key == "MarketplaceType")
                        .Select(p => new
                        {
                            ProductId = oi.Product.Id,
                            ProductName = oi.Product.Name,
                            IntegrationSystemName = oi.Order.IntegrationSystem.Name,
                            IntegrationKey = p.Key,
                            IntegrationValue = p.Value,
                            Quantity = oi.Quantity,
                            OrderDate = oi.Order.OrderDateUtc
                        })
                );

            var grouped = filteredItems
                .GroupBy(x => new
                {
                    x.ProductId,
                    x.ProductName,
                    x.IntegrationSystemName,
                    x.IntegrationKey,
                    x.IntegrationValue
                })
                .Select(g => new ProductSalesDto
                {
                    ProductId = g.Key.ProductId,
                    ProductName = g.Key.ProductName,
                    IntegrationSystemName = g.Key.IntegrationSystemName,
                    IntegrationKey = g.Key.IntegrationKey,
                    IntegrationValue = g.Key.IntegrationValue,
                    Period = GetPeriodName(groupByType), // Aylık, Yıllık vs.
                    TotalQuantitySold = g.Sum(x => x.Quantity)
                })
                .OrderByDescending(x => x.TotalQuantitySold)
                .ToList();

            return grouped;
        }


        private string GetPeriodName(int groupByType)
        {
            return groupByType switch
            {
                1 => "Haftalık",
                2 => "Aylık",
                3 => "Yıllık",
                _ => "Bilinmeyen"
            };
        }
        private (DateTime startDate, DateTime endDate) GetPeriodDateRange(int groupByType)
        {
            var today = DateTime.UtcNow.Date;

            return groupByType switch
            {
                1 => GetWeekRange(today),
                2 => (new DateTime(today.Year, today.Month, 1),
                      new DateTime(today.Year, today.Month, 1).AddMonths(1)),
                3 => (new DateTime(today.Year, 1, 1),
                      new DateTime(today.Year + 1, 1, 1)),
                _ => throw new ArgumentOutOfRangeException(nameof(groupByType), "Geçersiz grup türü")
            };
        }

        private (DateTime startDate, DateTime endDate) GetWeekRange(DateTime today)
        {
            var diff = (7 + (today.DayOfWeek - DayOfWeek.Monday)) % 7;
            var startOfWeek = today.AddDays(-1 * diff).Date;
            var endOfWeek = startOfWeek.AddDays(7);
            return (startOfWeek, endOfWeek);
        }



    }
}
