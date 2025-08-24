using Entegro.Application.DTOs.Common;
using Entegro.Application.Interfaces.Repositories;
using Entegro.Domain.Entities;
using Entegro.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Entegro.Infrastructure.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly EntegroContext _context;
        private readonly IProductRepository _productRepository;
        public OrderRepository(EntegroContext context, IProductRepository productRepository)
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

        public async Task<bool> ExistsByOrderNoAsync(string orderNo)
        {
            return await _context.Orders.AnyAsync(p => p.OrderNo == orderNo);
        }

        public async Task<List<Order>> GetAllAsync()
        {
            return await _context.Orders.ToListAsync();
        }

        public async Task<PagedResult<Order>> GetAllAsync(int pageNumber, int pageSize)
        {
            //var query = _context.Orders.Include(m => m.Customer).Include(m => m.OrderItems).AsQueryable();
            var query = _context.Orders.AsQueryable();

            var totalCount = await query.CountAsync();

            var orders = await query.Select(o => new Order
            {
                Id = o.Id,
                OrderNo = o.OrderNo,
                OrderDate = o.OrderDate,
                CustomerId = o.CustomerId,
                TotalAmount = o.TotalAmount,
                Deleted = o.Deleted,
                IsTransient = o.IsTransient,
                OrderSource = o.OrderSource,
                OrderSourceId = o.OrderSourceId,
                Customer = o.Customer,
                OrderItems = o.OrderItems.Select(i => new OrderItem
                {
                    Id = i.Id,
                    DiscountAmount = i.DiscountAmount,
                    OrderId = i.OrderId,
                    Price = i.Price,
                    ProductId = i.ProductId,
                    Quantity = i.Quantity,
                    TaxRate = i.TaxRate,
                    UnitPrice = i.UnitPrice
                }).ToList()
            }).Skip(pageNumber * pageSize)
                .Take(pageSize)
                .ToListAsync();


            return new PagedResult<Order>
            {
                Items = orders,
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }

        public async Task<Order?> GetByIdAsync(int id)
        {
            var order = await _context.Orders.FirstOrDefaultAsync(o => o.Id == id);
            var orderItems = await _context.OrderItems.Where(oi => oi.OrderId == id).ToListAsync();
            var products = await _productRepository.GetAllAsync(orderItems.Select(oi => oi.ProductId).Distinct().ToList());
            var customer = await _context.Customers.FirstOrDefaultAsync(c => c.Id == order.CustomerId);


            if (order is null)
                return new Order();

            var orders = new Order
            {
                Id = order.Id,
                CustomerId = order.CustomerId,
                Deleted = order.Deleted,
                IsTransient = order.Deleted,
                OrderNo = order.OrderNo,
                Customer = new Customer
                {
                    Id = customer.Id,
                    Name = customer.Name,
                    Email = customer.Email,
                    PhoneNumber = customer.PhoneNumber
                },
                OrderDate = order.OrderDate,
                OrderSource = order.OrderSource,
                OrderSourceId = order.OrderSourceId,
                TotalAmount = order.TotalAmount,
                OrderItems = orderItems.Select(x => new OrderItem
                {
                    Id = x.Id,
                    DiscountAmount = x.DiscountAmount,
                    OrderId = x.OrderId,
                    Price = x.Price,
                    ProductId = x.ProductId,
                    Quantity = x.Quantity,
                    TaxRate = x.TaxRate,
                    UnitPrice = x.UnitPrice,
                    Product = products.Where(p => p.Id == x.ProductId).Select(p => new Product
                    {
                        Id = p.Id,
                        Name = p.Name,
                        MainPictureId = p.MainPictureId,
                        MainPicture = p.MainPicture == null ? null : new MediaFile()
                        {
                            Alt = p.MainPicture.Alt,
                            CreatedOn = p.MainPicture.CreatedOn,
                            Deleted = p.MainPicture.Deleted,
                            Extension = p.MainPicture.Extension,
                            Folder = p.MainPicture.Folder,
                            FolderId = p.MainPicture.FolderId,
                            Height = p.MainPicture.Height,
                            Hidden = p.MainPicture.Hidden,
                            Id = p.MainPicture.Id,
                            IsTransient = p.MainPicture.IsTransient,
                            MediaType = p.MainPicture.MediaType,
                            Metadata = p.MainPicture.Metadata,
                            MimeType = p.MainPicture.MimeType,
                            Name = p.MainPicture.Name,
                            PixelSize = p.MainPicture.PixelSize,
                            Size = p.MainPicture.Size,
                            Title = p.MainPicture.Title,
                            UpdatedOn = p.MainPicture.UpdatedOn,
                            Version = p.MainPicture.Version,
                            Width = p.MainPicture.Width
                        },
                    }).FirstOrDefault()
                }).ToList()
            };
            return orders;
        }

        public async Task UpdateAsync(Order order)
        {
            _context.Orders.Update(order);
            await _context.SaveChangesAsync();
        }
    }
}
