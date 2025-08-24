using Entegro.Application.DTOs.Common;
using Entegro.Application.Interfaces.Repositories;
using Entegro.Domain.Entities;
using Entegro.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Entegro.Infrastructure.Repositories
{
    public class OrderItemRepository : IOrderItemRepository
    {
        private readonly EntegroContext _context;
        public OrderItemRepository(EntegroContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task AddAsync(OrderItem orderItem)
        {
            await _context.OrderItems.AddAsync(orderItem);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(OrderItem orderItem)
        {
            _context.OrderItems.Remove(orderItem);
            await _context.SaveChangesAsync();
        }

        public async Task<List<OrderItem>> GetAllAsync()
        {
            return await _context.OrderItems.ToListAsync();
        }

        public async Task<PagedResult<OrderItem>> GetAllAsync(int pageNumber, int pageSize)
        {
            var query = _context.OrderItems.AsQueryable();

            var totalCount = await query.CountAsync();

            var orderItems = await query.Select(o => new OrderItem
            {
                Id = o.Id,
                DiscountAmount = o.DiscountAmount,
                OrderId = o.OrderId,
                Price = o.Price,
                ProductId = o.ProductId,
                Quantity = o.Quantity,
                TaxRate = o.TaxRate,
                UnitPrice = o.UnitPrice,
                Product = new Product
                {
                    Id = o.ProductId,
                    MetaDescription = o.Product.MetaDescription,
                    Name = o.Product.Name,
                    Barcode = o.Product.Barcode,
                    Brand = o.Product.Brand,
                    BrandId = o.Product.BrandId,
                    Code = o.Product.Code,
                    CreatedOn = o.Product.CreatedOn,
                    Currency = o.Product.Currency,
                    Deleted = o.Product.Deleted,
                    Description = o.Product.Description,
                    Gtin = o.Product.Gtin,
                    Height = o.Product.Height,
                    Length = o.Product.Length,
                    MainPicture = o.Product.MainPicture,
                    MainPictureId = o.Product.MainPictureId,
                    ManufacturerPartNumber = o.Product.ManufacturerPartNumber,
                    MetaKeywords = o.Product.MetaKeywords,
                    MetaTitle = o.Product.MetaTitle,
                    OldPrice = o.Product.OldPrice,
                    Price = o.Product.Price,
                    Published = o.Product.Published,
                    Width = o.Product.Width,
                    SpecialPrice = o.Product.SpecialPrice,
                    Unit = o.Product.Unit,
                    VatInc = o.Product.VatInc,
                    VatRate = o.Product.VatRate,
                    UpdatedOn = o.Product.UpdatedOn,
                    Weight = o.Product.Weight

                },
                Order = o.Order
            }).Skip(pageNumber * pageSize)
                .Take(pageSize)
                .ToListAsync();


            return new PagedResult<OrderItem>
            {
                Items = orderItems,
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }

        public async Task<OrderItem?> GetByIdAsync(int id)
        {
            return await _context.OrderItems.FirstOrDefaultAsync(o => o.Id == id);
        }

        public async Task<List<OrderItem>> GetByOrderIdAsync(int orderId)
        {
            var orderItems = await _context.OrderItems.Where(o => o.OrderId == orderId).ToListAsync();

            return orderItems.Select(x => new OrderItem
            {
                Id = x.Id,
                DiscountAmount = x.DiscountAmount,
                UnitPrice = x.UnitPrice,
                OrderId = x.OrderId,
                Price = x.Price,
                ProductId = x.ProductId,
                Order = x.Order,
                Product = x.Product,
                Quantity = x.Quantity,
                TaxRate = x.TaxRate,
            }).ToList();
        }

        public async Task UpdateAsync(OrderItem orderItem)
        {
            _context.OrderItems.Update(orderItem);
            await _context.SaveChangesAsync();
        }
    }
}
