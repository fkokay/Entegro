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
        public OrderRepository(EntegroContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
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
            return await _context.Orders.FirstOrDefaultAsync(o => o.Id == id);
        }

        public async Task UpdateAsync(Order order)
        {
            _context.Orders.Update(order);
            await _context.SaveChangesAsync();
        }
    }
}
