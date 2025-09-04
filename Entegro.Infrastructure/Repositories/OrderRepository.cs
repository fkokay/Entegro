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

        public async Task<bool> ExistsByOrderNoAsync(string orderNo) => await _context.Orders.AnyAsync(p => p.OrderNo == orderNo);

        public async Task<List<Order>> GetAllAsync()
        {
            return await _context.Orders.AsNoTracking().ToListAsync();
        }

        public async Task<PagedResult<Order>> GetAllAsync(int pageNumber, int pageSize)
        {
            var query = _context.Orders.AsNoTracking().AsQueryable();

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
            }).OrderBy(m => m.Id)
                .Skip(pageNumber * pageSize)
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
            var order = await _context.Orders.Select(o => new Order
            {
                Id = o.Id,
                CustomerId = o.CustomerId,
                Deleted = o.Deleted,
                IsTransient = o.Deleted,
                OrderNo = o.OrderNo,
                Customer = new Customer
                {
                    Id = o.Customer.Id,
                    Name = o.Customer.Name,
                    Email = o.Customer.Email,
                    PhoneNumber = o.Customer.PhoneNumber
                },
                OrderDate = o.OrderDate,
                OrderSource = o.OrderSource,
                OrderSourceId = o.OrderSourceId,
                TotalAmount = o.TotalAmount,
                OrderItems = o.OrderItems.Select(x => new OrderItem
                {
                    Id = x.Id,
                    DiscountAmount = x.DiscountAmount,
                    OrderId = x.OrderId,
                    Price = x.Price,
                    ProductId = x.ProductId,
                    Quantity = x.Quantity,
                    TaxRate = x.TaxRate,
                    UnitPrice = x.UnitPrice,
                    Product = new Product
                    {
                        Id = x.Product.Id,
                        Name = x.Product.Name,
                        Code = x.Product.Code,
                        MainPictureId = x.Product.MainPictureId,
                        MainPicture = x.Product.MainPicture == null ? null : new MediaFile()
                        {
                            Alt = x.Product.MainPicture.Alt,
                            CreatedOn = x.Product.MainPicture.CreatedOn,
                            Deleted = x.Product.MainPicture.Deleted,
                            Extension = x.Product.MainPicture.Extension,
                            Folder = x.Product.MainPicture.Folder,
                            FolderId = x.Product.MainPicture.FolderId,
                            Height = x.Product.MainPicture.Height,
                            Hidden = x.Product.MainPicture.Hidden,
                            Id = x.Product.MainPicture.Id,
                            IsTransient = x.Product.MainPicture.IsTransient,
                            MediaType = x.Product.MainPicture.MediaType,
                            Metadata = x.Product.MainPicture.Metadata,
                            MimeType = x.Product.MainPicture.MimeType,
                            Name = x.Product.MainPicture.Name,
                            PixelSize = x.Product.MainPicture.PixelSize,
                            Size = x.Product.MainPicture.Size,
                            Title = x.Product.MainPicture.Title,
                            UpdatedOn = x.Product.MainPicture.UpdatedOn,
                            Version = x.Product.MainPicture.Version,
                            Width = x.Product.MainPicture.Width
                        },
                    }
                }).ToList()
            }).FirstOrDefaultAsync(o => o.Id == id);

            if (order is null)
                return new Order();


            return order;
        }

        public async Task UpdateAsync(Order order)
        {
            _context.Orders.Update(order);
            await _context.SaveChangesAsync();
        }
    }
}
