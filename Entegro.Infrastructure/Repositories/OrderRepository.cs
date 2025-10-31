using Entegro.Application.DTOs.Common;
using Entegro.Application.DTOs.IntegrationSystem;
using Entegro.Application.DTOs.IntegrationSystemParameter;
using Entegro.Application.DTOs.Order;
using Entegro.Application.DTOs.OrderItem;
using Entegro.Application.Interfaces.Repositories;
using Entegro.Domain.Entities.Checkout;
using Entegro.Domain.Enums;
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

        public async Task<int> CompleteOrderStatusCount()
        {
            return await _context.Orders.CountAsync(x => x.OrderStatusId == (int)Domain.Enums.OrderStatus.Complete);
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
                .Include(s => s.Shipments);

            var order = await query.AsNoTracking().FirstOrDefaultAsync(o => o.Id == id);

            if (order is null)
                return new Order();


            return order;
        }

        public async Task<Order?> GetByOrderNoAsync(string orderNo)
        {
            var query = _context.Orders
                           .Include(o => o.OrderItems)
                           .Include(c => c.Customer)
                           .Include(s => s.Shipments);

            var order = await query.AsNoTracking().FirstOrDefaultAsync(o => o.OrderNumber == orderNo);

            if (order is null)
                return new Order();

            return order;
        }

        public async Task<List<Order>> GetLast10OrdersWithItemsAsync()
        {
            return await _context.Orders
              .Include(o => o.OrderItems)
              .OrderByDescending(o => o.OrderDateUtc)
              .Take(10)
              .ToListAsync();
        }

        public async Task<List<(int Month, decimal TotalAmount)>> GetMonthlySalesByYearAsync(int year)
        {
            var result = await _context.Orders
                .Where(o => o.OrderStatusId == 30 && o.OrderDateUtc.Year == year)
                .GroupBy(o => o.OrderDateUtc.Month)
                .Select(g => new { Month = g.Key, Total = g.Sum(x => x.OrderTotal) })
                .ToListAsync();

            var list = Enumerable.Range(1, 12)
                .Select(m => (Month: m,
                              TotalAmount: result.FirstOrDefault(x => x.Month == m)?.Total ?? 0m))
                .ToList();

            return list;
        }

        public async Task<List<Order>> GetOrderByIntegrationIdAsync(int integrationId)
        {
            var query = _context.Orders
                           .Include(o => o.OrderItems)
                           .Include(c => c.Customer)
                           .Include(s => s.Shipments);

            var orders = await query.AsNoTracking().Where(o => o.IntegrationSystemId == integrationId).ToListAsync();

            if (orders is null)
                return new List<Order>();

            return orders;
        }

        public async Task<OrderListPageDto> GetOrderPageAsync()
        {
            OrderListPageDto orderListPage = new OrderListPageDto();
            orderListPage.ToBePackedQuantity = await _context.Orders.Where(o => o.OrderItems.Any(oi => oi.Quantity > oi.ShipmentItems.Sum(si => (int?)si.Quantity ?? 0))).CountAsync();
            orderListPage.ReadyToShipQuantity = await _context.Orders.Where(o => o.Shipments.Any() && !o.Shipments.Any(s => s.ShippedDateUtc != null) && o.OrderStatusId == (int)OrderStatus.Processing).CountAsync();
            orderListPage.ShippedQuantity = await _context.Orders.Where(o => o.Shipments.Any() && o.Shipments.Any(s => s.ShippedDateUtc != null) && o.ShippingStatusId == (int)ShippingStatus.Shipped).CountAsync();
            orderListPage.DeliveredQuantity = await _context.Orders.Where(o => o.Shipments.Any() && o.Shipments.Any(s => s.DeliveryDateUtc != null) && o.ShippingStatusId == (int)ShippingStatus.Delivered).CountAsync();
            orderListPage.UnDeliverdQuantity = await _context.Orders.Where(o => o.Shipments.Any() && o.Shipments.Any(s => s.DeliveryDateUtc == null)).CountAsync();
            orderListPage.PaymentAwaitingQuantity = await _context.Orders.Where(o => o.PaymentStatusId == (int)Domain.Enums.PaymentStatus.Pending).CountAsync();
            orderListPage.CancalledQuantity = await _context.Orders.Where(o => o.OrderStatusId == (int)Domain.Enums.OrderStatus.Cancelled).CountAsync();

            return orderListPage;
        }

        public async Task<Application.DTOs.Common.PagedResult<OrderListDto>> GetPagedAsync(GridCommand gridCommand, OrderListFilterDto filters, int orderStatus)
        {
            var query = _context.Orders
                .Include(o => o.OrderItems)
                .Include(o => o.Customer)
                .Include(o => o.Shipments)
                .AsNoTracking();

            if (filters != null)
            {
                if (!string.IsNullOrEmpty(filters.CustomerName))
                    query = query.Where(o => o.Customer.Name.Contains(filters.CustomerName));

                if (!string.IsNullOrEmpty(filters.OrderNo))
                    query = query.Where(o => o.OrderNumber.Contains(filters.OrderNo));

                if (!string.IsNullOrEmpty(filters.PackageNo))
                    query = query.Where(o => o.Shipments.Any(s => s.PackageNo.Contains(filters.PackageNo)));

                if (!string.IsNullOrEmpty(filters.Barcode))
                    query = query.Where(o => o.OrderItems.Any(oi => oi.Product.Barcode.Contains(filters.Barcode)));

                if (!string.IsNullOrEmpty(filters.CargoCode))
                    query = query.Where(o => o.Shipments.Any(s => s.TrackingNumber.Contains(filters.CargoCode)));

                if (!string.IsNullOrEmpty(filters.ProductName))
                    query = query.Where(o => o.OrderItems.Any(oi => oi.Product.Name.Contains(filters.ProductName)
                                                                 || oi.Product.Code.Contains(filters.ProductName)));

                if (filters.StartDate.HasValue)
                    query = query.Where(o => o.OrderDateUtc >= filters.StartDate.Value);

                if (filters.EndDate.HasValue)
                    query = query.Where(o => o.OrderDateUtc <= filters.EndDate.Value);
            }


            if (gridCommand.Columns != null)
            {
                foreach (var col in gridCommand.Columns)
                {
                    if (!string.IsNullOrEmpty(col.Search?.Value))
                    {
                        var searchVal = col.Search.Value.Trim('^', '$');

                        // ilgili property’nin tipini bul
                        var propName = col.Data.Contains(".")
                        ? col.Data.Replace(".", "")
                        : col.Data;

                        var prop = typeof(OrderListDto).GetProperty(propName);
                        if (prop == null) continue;

                        if (prop.PropertyType == typeof(string))
                        {
                            query = query.Where($"{col.Data}.Contains(@0)", searchVal);
                        }
                        else if (prop.PropertyType == typeof(int) || prop.PropertyType == typeof(int?))
                        {
                            if (int.TryParse(searchVal, out var intVal))
                                query = query.Where($"{col.Data} == @0", intVal);
                        }
                        else if (prop.PropertyType == typeof(bool) || prop.PropertyType == typeof(bool?))
                        {
                            if (bool.TryParse(searchVal, out var boolVal))
                                query = query.Where($"{col.Data} == @0", boolVal);
                        }
                        else if (prop.PropertyType == typeof(DateTime) || prop.PropertyType == typeof(DateTime?))
                        {
                            if (DateTime.TryParse(searchVal, out var dt))
                                query = query.Where($"{col.Data}.Date == @0", dt.Date);
                        }
                    }
                }
            }

            if (gridCommand.Search != null)
            {
                if (!string.IsNullOrEmpty(gridCommand.Search.Value))
                {
                    query = query.Where(b =>
                    b.Customer.Name.Contains(gridCommand.Search.Value)).AsQueryable();
                }
            }

            if (gridCommand.Order.Any())
            {
                foreach (var item in gridCommand.Order)
                {
                    query = query.OrderBy($"{gridCommand.Columns[item.Column].Data} {(item.Dir ?? "asc")}");
                }
            }
            else
            {
                query = query.OrderBy(b => b.Id);
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
                    query = query.Where(o => o.Shipments.Any() && !o.Shipments.Any(s => s.ShippedDateUtc != null) && o.OrderStatusId == (int)OrderStatus.Processing);
                    break;
                case 3: // Kargoda
                    query = query.Where(o => o.Shipments.Any() && o.Shipments.Any(s => s.ShippedDateUtc != null) && o.ShippingStatusId == (int)ShippingStatus.Shipped);
                    break;
                case 4: // Teslim Edildi
                    query = query.Where(o => o.Shipments.Any() && o.Shipments.Any(s => s.DeliveryDateUtc != null) && o.ShippingStatusId == (int)ShippingStatus.Delivered);
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
                    CustomerId = x.order.CustomerId,
                    CustomerName = x.order.Customer.Name,
                    CustomerOrderCounts = x.order.Customer.Orders.Count(),
                    ShipmentCarrier = x.shipment != null ? x.shipment.Carrier : "",
                    ShippingTrackingNumber = x.shipment != null ? x.shipment.TrackingNumber : "",
                    TrackingUrl = x.shipment != null ? x.shipment.TrackingUrl : "",
                    ShippedDateUtc = x.shipment != null && x.shipment.ShippedDateUtc != null ? x.shipment.ShippedDateUtc.Value : DateTime.MinValue,
                    DeliveryDateUtc = x.shipment != null && x.shipment.DeliveryDateUtc != null ? x.shipment.DeliveryDateUtc.Value : DateTime.MinValue,
                    OrderStatusId = x.order.OrderStatusId,
                    ShippingStatusId = x.order.ShippingStatusId,
                    OrderSubTotal = x.shipment != null ? x.shipment.ShipmentItems.Sum(si => si.OrderItem.UnitPrice * si.Quantity) : x.order.OrderSubTotal,
                    OrderDiscount = x.order.OrderDiscount,
                    OrderTotal = x.shipment != null ? x.shipment.ShipmentItems.Sum(si => si.OrderItem.UnitPrice * si.Quantity) - x.order.OrderDiscount : x.order.OrderTotal,
                    PaymentMethod = x.order.PaymentMethod,
                    PaymentStatus = x.order.PaymentStatusLabelHint,
                    InvoiceLink = x.order.InvoiceLink,
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
                              IntegrationProductName = si.OrderItem.IntegrationProductName,
                              IntegrationProductImageUrl = si.OrderItem.IntegrationProductImageUrl,
                              AttributeDescription = si.OrderItem.AttributesDescription
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
                              IntegrationProductName = oi.IntegrationProductName,
                              IntegrationProductImageUrl = oi.IntegrationProductImageUrl,
                              AttributeDescription = oi.AttributesDescription
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

        public async Task<List<StoreProductSalesDto>> GetStoreProductSalesAsync()
        {
            var query = await _context.Orders
                .Where(o => o.OrderStatusId != 40)
                .Join(_context.OrderItems,
                    o => o.Id,
                    oi => oi.OrderId,
                    (o, oi) => new { o, oi })
                .Join(_context.Products,
                    x => x.oi.ProductId,
                    p => p.Id,
                    (x, p) => new { x.o, x.oi, p })
                .Join(_context.IntegrationSystems,
                    x => x.o.IntegrationSystemId,
                    ins => ins.Id,
                    (x, ins) => new { x.o, x.oi, x.p, ins })
                .Join(_context.IntegrationSystemParameters,
                    x => x.ins.Id,
                    isp => isp.IntegrationSystemId,
                    (x, isp) => new { x.o, x.oi, x.p, isp })
                .Where(x => x.isp.Key == "CommerceType" || x.isp.Key == "MarketplaceType")
                .GroupBy(g => new
                {
                    g.isp.Value,
                    g.oi.IntegrationSku
                })
                .OrderBy(g => g.Key.Value)
                .ThenByDescending(g => g.Sum(x => x.oi.Quantity))
                .Select(g => new StoreProductSalesDto
                {
                    StoreName = g.Key.Value,
                    IntegrationSku = g.Key.IntegrationSku,
                    IntegrationProductName = g.Max(x => x.oi.IntegrationProductName),
                    ProductId = g.Max(x => x.p.Id),
                    ProductName = g.Max(x => x.p.Name),
                    Barcode = g.Max(x => x.p.Barcode),
                    TotalQuantity = g.Sum(x => x.oi.Quantity)
                })
                .ToListAsync();

            return query;
        }


        public async Task<decimal> GetTotalSalesAsync()
        {
            return await _context.Orders.Where(x => x.OrderStatusId == (int)Domain.Enums.OrderStatus.Complete).SumAsync(x => x.OrderTotal);
        }

        public async Task UpdateAsync(Order order)
        {
            _context.Entry(order).State = EntityState.Modified;
            _context.Entry(order).Collection(p => p.OrderItems).IsModified = false;
            _context.Orders.Update(order);
            await _context.SaveChangesAsync();
        }
    }
}
