using Entegro.Application.DTOs.Common;
using Entegro.Application.DTOs.IntegrationSystem;
using Entegro.Application.DTOs.IntegrationSystemParameter;
using Entegro.Application.DTOs.Order;
using Entegro.Application.DTOs.OrderItem;
using Entegro.Application.Interfaces.Repositories;
using Entegro.Domain.Entities.Catalog;
using Entegro.Domain.Entities.Checkout;
using Entegro.Domain.Entities.Common;
using Entegro.Domain.Entities.Content;
using Entegro.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq.Dynamic.Core;

namespace Entegro.Infrastructure.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly EntegroDbContext _context;
        private readonly IProductRepository _productRepository;
        public OrderRepository(EntegroDbContext context, IProductRepository productRepository)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _productRepository = productRepository ?? throw new ArgumentNullException(nameof(productRepository));
        }
        public async Task AddAsync(Order order)
        {
            await _context.Orders.AddAsync(order);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Order order)
        {
            _context.Orders.Remove(order);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> ExistsByOrderNoAsync(string orderNo) => await _context.Orders.AnyAsync(p => p.OrderNumber == orderNo);

        public async Task<List<Order>> GetAllAsync()
        {
            return await _context.Orders.AsNoTracking().ToListAsync();
        }

        public async Task<Application.DTOs.Common.PagedResult<Order>> GetAllAsync(int pageNumber, int pageSize)
        {
            var query = _context.Orders.Include(m => m.OrderItems).AsNoTracking().AsQueryable();

            var totalCount = await query.CountAsync();

            var orders = await query.OrderBy(m => m.Id)
                .Skip(pageNumber * pageSize)
                .Take(pageSize)
                .ToListAsync();


            return new Application.DTOs.Common.PagedResult<Order>
            {
                Items = orders,
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }

        public async Task<Order?> GetByIdAsync(int id)
        {
            var query = _context.Orders
                .Include(o => o.OrderItems)
                .Include(c => c.Customer)
                .Include(s => s.Shipments)
            .AsNoTracking();

            var order = await query.FirstOrDefaultAsync(o => o.Id == id);

            if (order is null)
                return new Order();


            return order;
        }

        public async Task<OrderListPageDto> GetOrderPageAsync()
        {
            OrderListPageDto orderListPage = new OrderListPageDto();
            orderListPage.ToBePackedQuantity = await _context.Orders.Where(o => o.OrderItems.Any(oi => oi.Quantity > oi.ShipmentItems.Sum(si => (int?)si.Quantity ?? 0))).CountAsync();
            orderListPage.ReadyToShipQuantity = await _context.Orders.Where(o => o.Shipments.Any() && !o.Shipments.Any(s => s.ShippedDateUtc != null)).CountAsync();
            orderListPage.ShippedQuantity = await _context.Orders.Where(o => o.Shipments.Any() && o.Shipments.Any(s => s.ShippedDateUtc != null)).CountAsync();
            orderListPage.DeliveredQuantity = await _context.Orders.Where(o => o.Shipments.Any() && o.Shipments.Any(s => s.DeliveryDateUtc != null)).CountAsync();
            orderListPage.UnDeliverdQuantity = await _context.Orders.Where(o => o.Shipments.Any() && o.Shipments.Any(s => s.DeliveryDateUtc == null)).CountAsync();
            orderListPage.PaymentAwaitingQuantity = await _context.Orders.Where(o => o.PaymentStatusId == (int)Domain.Enums.PaymentStatus.Pending).CountAsync();
            orderListPage.CancalledQuantity = await _context.Orders.Where(o => o.OrderStatusId == (int)Domain.Enums.OrderStatus.Cancelled).CountAsync();

            return orderListPage;
        }

        public async Task<Application.DTOs.Common.PagedResult<OrderListDto>> GetPagedAsync(GridCommand gridCommand, int orderStatus)
        {
            var query = _context.Orders
                .Include(o => o.OrderItems)
                .Include(o => o.Customer)
                .Include(o => o.Shipments)
                .AsNoTracking();

            if (!string.IsNullOrEmpty(gridCommand.Search?.Value))
            {
                query = query.Where(o => o.OrderNumber.Contains(gridCommand.Search.Value));
            }

            IOrderedQueryable<Order> orderedQuery = null;
            if (gridCommand.Order.Any())
            {
                foreach (var item in gridCommand.Order)
                {
                    var field = string.IsNullOrEmpty(gridCommand.Columns[item.Column].Name)
                        ? gridCommand.Columns[item.Column].Data
                        : gridCommand.Columns[item.Column].Name;

                    if (orderedQuery == null)
                        orderedQuery = query.OrderBy($"{field} {(item.Dir ?? "asc")}");
                    else
                        orderedQuery = orderedQuery.ThenBy($"{field} {(item.Dir ?? "asc")}");
                }
                query = orderedQuery;
            }
            else
            {
                query = query.OrderBy(o => o.Id);
            }

            switch (orderStatus)
            {
                case 1: // Paketlenecek
                    query = query.Where(o => o.OrderItems.Any(oi => oi.Quantity > oi.ShipmentItems.Sum(si => (int?)si.Quantity ?? 0)));
                    break;
                case 2: // Gönderime Hazır
                    query = query.Where(o => o.Shipments.Any() && !o.Shipments.Any(s => s.ShippedDateUtc != null));
                    break;
                case 3: // Kargoda
                    query = query.Where(o => o.Shipments.Any() && o.Shipments.Any(s => s.ShippedDateUtc != null));
                    break;
                case 4: // Teslim Edildi
                    query = query.Where(o => o.Shipments.Any() && o.Shipments.Any(s => s.DeliveryDateUtc != null));
                    break;
                case 5: // Teslim Edilemedi
                    query = query.Where(o => o.Shipments.Any() && o.Shipments.Any(s => s.DeliveryDateUtc == null));
                    break;
                case 6: // Ödemesi Bekleniyor
                    query = query.Where(o => o.PaymentStatusId == (int)Domain.Enums.PaymentStatus.Pending);
                    break;
                case 7: // İptal Edildi
                    query = query.Where(o => o.OrderStatusId == (int)Domain.Enums.OrderStatus.Cancelled);
                    break;
            }

            var totalCount = await query.CountAsync();

            var orders = await query
                .SelectMany(o => o.Shipments.DefaultIfEmpty(), (order, shipment) => new { order, shipment })
                .Select(x => new OrderListDto
                {
                    Id = x.order.Id,
                    PackageNo = x.shipment != null ? x.shipment.PackageNo : "",
                    IntegrationSystemId = x.order.IntegrationSystemId,
                    IntegrationSystem = x.order.IntegrationSystem != null
                        ? new IntegrationSystemDto
                        {
                            Id = x.order.IntegrationSystem.Id,
                            Name = x.order.IntegrationSystem.Name,
                            Description = x.order.IntegrationSystem.Description,
                            IntegrationSystemType = x.order.IntegrationSystem.IntegrationSystemType,
                            IntegrationSystemTypeId = x.order.IntegrationSystem.IntegrationSystemTypeId,
                            IntegrationSystemParameters = x.order.IntegrationSystem.IntegrationSystemParameters
                                .Select(p => new IntegrationSystemParameterDto
                                {
                                    Id = p.Id,
                                    IntegrationSystemId = p.IntegrationSystemId,
                                    Key = p.Key,
                                    Value = p.Value
                                }).ToList()
                        }
                        : null,
                    OrderNumber = x.order.OrderNumber,
                    OrderDate = x.order.OrderDateUtc,
                    DueDate = x.order.DueDateUtc,
                    CustomerName = x.order.Customer.Name,
                    CustomerOrderCounts = x.order.Customer.Orders.Count(),
                    ShipmentCarrier = x.shipment != null ? x.shipment.Carrier : "",
                    ShippingTrackingNumber = x.shipment != null ? x.shipment.TrackingNumber : "",
                    OrderSubTotal = x.shipment != null ? x.shipment.ShipmentItems.Sum(si => si.OrderItem.UnitPrice * si.Quantity) : x.order.OrderSubTotal,
                    OrderDiscount = 0,
                    OrderTotal = x.shipment != null ? x.shipment.ShipmentItems.Sum(si => si.OrderItem.UnitPrice * si.Quantity) : x.order.OrderTotal,
                    PaymentMethod = x.order.PaymentMethod,
                    PaymentStatus = x.order.PaymentStatusLabelHint,
                    OrderItems = (x.shipment != null
                          ? x.shipment.ShipmentItems.Select(si => new OrderItemListDto
                          {
                              Id = si.Id,
                              OrderId = si.OrderItem.OrderId,
                              UnitPrice = si.OrderItem.UnitPrice,
                              ProductId = si.OrderItem.ProductId,
                              ProductBarcode = si.OrderItem.Product.Barcode,
                              ProductCode = si.OrderItem.Product.Code,
                              ProductName = si.OrderItem.Product.Name,
                              ProductMainPictureId = si.OrderItem.Product.MainPictureId,
                              Quantity = si.Quantity,
                              IntegrationSku = si.OrderItem.IntegrationSku,
                              IntegrationProductName = si.OrderItem.IntegrationProductName
                          }).ToList()
                          : x.order.OrderItems.Select(oi => new OrderItemListDto
                          {
                              Id = oi.Id,
                              OrderId = oi.OrderId,
                              UnitPrice = oi.UnitPrice,
                              ProductId = oi.ProductId,
                              ProductBarcode = oi.Product != null ? oi.Product.Barcode : "",
                              ProductCode = oi.Product != null ? oi.Product.Code : "",
                              ProductName = oi.Product != null ? oi.Product.Name : "",
                              ProductMainPictureId = oi.Product != null ? oi.Product.MainPictureId : null,
                              Quantity = oi.Quantity,
                              IntegrationSku = oi.IntegrationSku,
                              IntegrationProductName = oi.IntegrationProductName
                          }).ToList())
                })
                .Skip(gridCommand.Start)
                .Take(gridCommand.Length)
                .ToListAsync();

            return new Application.DTOs.Common.PagedResult<OrderListDto>
            {
                Items = orders,
                TotalCount = totalCount,
                PageNumber = (gridCommand.Start / gridCommand.Length) + 1,
                PageSize = gridCommand.Length
            };
        }

        public async Task UpdateAsync(Order order)
        {
            _context.Orders.Update(order);
            await _context.SaveChangesAsync();
        }
    }
}
