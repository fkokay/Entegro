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

        public async Task<Application.DTOs.Common.PagedResult<OrderModel>> GetPagedAsync(GridCommand gridCommand, int orderStatus)
        {
            var query = _context.Orders
                .Include(o => o.OrderItems)
                .Include(c => c.Customer)
                .Include(c => c.Shipments)
            .AsNoTracking();

            if (gridCommand.Search != null)
            {
                if (!string.IsNullOrEmpty(gridCommand.Search.Value))
                {
                    query = query.Where(b => b.OrderNumber.Contains(gridCommand.Search.Value)).AsQueryable();
                }
            }

            if (gridCommand.Order.Any())
            {
                foreach (var item in gridCommand.Order)
                {
                    string field = "";
                    if (string.IsNullOrEmpty(gridCommand.Columns[item.Column].Name))
                    {
                        field = gridCommand.Columns[item.Column].Data;
                    }
                    else
                    {
                        field = gridCommand.Columns[item.Column].Name;
                    }


                    query = query.OrderBy($"{field} {(item.Dir ?? "asc")}");
                }
            }
            else
            {
                query = query.OrderBy(b => b.Id);
            }

            var totalCount = await query.CountAsync();

            //Paketlenecek Siparişler
            if (orderStatus == 1)
            {
                var orders = await query
                .Where(o => o.OrderItems.Any(oi => oi.Quantity > oi.ShipmentItems.Sum(si => si.Quantity)))
                .Select(m => new OrderModel()
                {
                    Id = m.Id,
                    PackageNo = "",
                    IntegrationSystemId = m.IntegrationSystemId,
                    IntegrationSystem = m.IntegrationSystem == null ? null : new IntegrationSystemDto()
                    {
                        Id = m.IntegrationSystem.Id,
                        Description = m.IntegrationSystem.Description,
                        Name = m.IntegrationSystem.Name,
                        IntegrationSystemType = m.IntegrationSystem.IntegrationSystemType,
                        IntegrationSystemTypeId = m.IntegrationSystem.IntegrationSystemTypeId,
                        IntegrationSystemParameters = m.IntegrationSystem.IntegrationSystemParameters.Select(x => new IntegrationSystemParameterDto()
                        {
                            Id = x.Id,
                            IntegrationSystemId = x.IntegrationSystemId,
                            Key = x.Key,
                            Value = x.Value
                        }).ToList(),
                    },
                    OrderNumber = m.OrderNumber,
                    OrderDate = m.OrderDateUtc,
                    DueDate = m.DueDateUtc,
                    CustomerName = m.Customer.Name,
                    CustomerOrderCounts = m.Customer.Orders.Count(),
                    ShipmentCarrier = "",
                    ShippingTrackingNumber = "",
                    OrderSubTotal = m.OrderSubTotal,
                    OrderDiscount = m.OrderDiscount,
                    OrderTotal = m.OrderTotal,
                    PaymentMethod = m.PaymentMethod,
                    PaymentStatus = m.PaymentStatusLabelHint,
                    OrderItems = m.OrderItems.Select(x => new OrderItemModel()
                    {
                        Id = x.Id,
                        OrderId = x.OrderId,
                        UnitPrice = x.UnitPrice,
                        AttributeDescription = "",
                        ProductId = x.ProductId,
                        ProductBarcode = x.Product == null ? "" : x.Product.Barcode,
                        ProductCode = x.Product == null ? "" : x.Product.Code,
                        ProductName = x.Product == null ? "" : x.Product.Name,
                        ProductMainPicture = "",
                        ProductMainPictureId = x.Product == null ? null : x.Product.MainPictureId,
                        IntegrationSku = x.IntegrationSku,
                        IntegrationProductName = x.IntegrationProductName,
                        Quantity = x.Quantity - x.ShipmentItems.Sum(si => si.Quantity)
                    }).ToList()
                })
                .Skip(gridCommand.Start)
                .Take(gridCommand.Length)
                .ToListAsync();

                return new Application.DTOs.Common.PagedResult<OrderModel>
                {
                    Items = orders,
                    TotalCount = totalCount,
                    PageNumber = gridCommand.Start + 1,
                    PageSize = gridCommand.Length
                };
            }
            else
            {
                //Gönderime Hazır
                if (orderStatus == 2)
                {
                    query = query.Where(m => m.Shipments.Any() && !m.Shipments.Where(m => m.ShippedDateUtc != null).Any());
                }
                //Kargoda
                else if (orderStatus == 3)
                {
                    query = query.Where(m => m.Shipments.Any() && m.Shipments.Where(m => m.ShippedDateUtc != null).Any());
                }
                //Teslim Edildi
                else if (orderStatus == 4)
                {
                    query = query.Where(m => m.Shipments.Any() && m.Shipments.Where(m => m.DeliveryDateUtc != null).Any());
                }
                //Teslim Edilemedi
                else if (orderStatus == 5)
                {
                    query = query.Where(m => m.Shipments.Any() && m.Shipments.Where(m => m.DeliveryDateUtc != null).Any());
                }
                //Ödemesi Bekleniyor
                else if (orderStatus == 6)
                {
                    query = query.Where(m => m.PaymentStatus == Domain.Enums.PaymentStatus.Pending);
                }
                //İptal Edildi
                else if (orderStatus == 7)
                {
                    query = query.Where(m => m.OrderStatus == Domain.Enums.OrderStatus.Cancelled);
                }



                var orders = await query
                .SelectMany(order => order.Shipments, (order, shipment) => new { order, shipment })
                .Select(x => new OrderModel
                {
                    Id = x.order.Id,
                    PackageNo = x.shipment.PackageNo,
                    IntegrationSystemId = x.order.IntegrationSystemId,
                    IntegrationSystem = x.order.IntegrationSystem == null ? null : new IntegrationSystemDto()
                    {
                        Id = x.order.IntegrationSystem.Id,
                        Description = x.order.IntegrationSystem.Description,
                        Name = x.order.IntegrationSystem.Name,
                        IntegrationSystemType = x.order.IntegrationSystem.IntegrationSystemType,
                        IntegrationSystemTypeId = x.order.IntegrationSystem.IntegrationSystemTypeId,
                        IntegrationSystemParameters = x.order.IntegrationSystem.IntegrationSystemParameters.Select(m => new IntegrationSystemParameterDto()
                        {
                            Id = m.Id,
                            IntegrationSystemId = m.IntegrationSystemId,
                            Key = m.Key,
                            Value = m.Value
                        }).ToList(),
                    },
                    OrderNumber = x.order.OrderNumber,
                    OrderDate = x.order.OrderDateUtc,
                    DueDate = x.order.DueDateUtc,
                    CustomerName = x.order.Customer.Name,
                    CustomerOrderCounts = x.order.Customer.Orders.Count(),
                    ShipmentCarrier = x.shipment.Carrier,
                    ShippingTrackingNumber = x.shipment.TrackingNumber,
                    OrderSubTotal = x.shipment.ShipmentItems.Sum(si => si.OrderItem.UnitPrice * si.Quantity),
                    OrderDiscount = 0,
                    OrderTotal = x.shipment.ShipmentItems.Sum(si => si.OrderItem.UnitPrice * si.Quantity),
                    PaymentMethod = x.order.PaymentMethod,
                    PaymentStatus = x.order.PaymentStatusLabelHint,
                    OrderItems = x.shipment.ShipmentItems.Select(si => new OrderItemModel
                    {
                        Id = si.Id,
                        OrderId = si.OrderItem.OrderId,
                        UnitPrice = si.OrderItem.UnitPrice,
                        AttributeDescription = "",
                        ProductBarcode = si.OrderItem.Product.Barcode,
                        ProductCode = si.OrderItem.Product.Code,
                        ProductName = si.OrderItem.Product.Name,
                        ProductMainPicture = "",
                        ProductMainPictureId = si.OrderItem.Product.MainPictureId,
                        Quantity = si.Quantity,
                        ProductId = si.OrderItem.ProductId,
                        IntegrationSku = si.OrderItem.IntegrationSku,
                        IntegrationProductName = si.OrderItem.IntegrationProductName,
                    }).ToList()
                })
                .Skip(gridCommand.Start)
                .Take(gridCommand.Length)
                .ToListAsync();

                return new Application.DTOs.Common.PagedResult<OrderModel>
                {
                    Items = orders,
                    TotalCount = totalCount,
                    PageNumber = gridCommand.Start + 1,
                    PageSize = gridCommand.Length
                };
            }
        }

        public async Task UpdateAsync(Order order)
        {
            _context.Orders.Update(order);
            await _context.SaveChangesAsync();
        }
    }
}
