using Entegro.Application.DTOs.Common;
using Entegro.Application.Interfaces.Repositories;
using Entegro.Domain.Entities.Checkout;
using Entegro.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq.Dynamic.Core;
namespace Entegro.Infrastructure.Repositories
{
    public class OrderItemRepository : IOrderItemRepository
    {
        private readonly EntegroDbContext _context;
        public OrderItemRepository(EntegroDbContext context)
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

        public async Task<Application.DTOs.Common.PagedResult<OrderItem>> GetAllAsync(int pageNumber, int pageSize)
        {
            var query = _context.OrderItems.Include(m => m.Product).AsQueryable();

            var totalCount = await query.CountAsync();

            var orderItems = await query.OrderBy(o => o.Id)
                .Skip(pageNumber * pageSize)
                .Take(pageSize)
                .ToListAsync();


            return new Application.DTOs.Common.PagedResult<OrderItem>
            {
                Items = orderItems,
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }

        public async Task<List<OrderItem>> GetAllIntegrationSkuWithAsync(string integrationSku)
        {
            return await _context.OrderItems.AsNoTracking().Where(m => m.IntegrationSku == integrationSku).ToListAsync();
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

        public async Task<Application.DTOs.Common.PagedResult<OrderItem>> GetOrderItemsWithProductAndIntegrationAsync(GridCommand gridCommand)
        {
            var query = _context.OrderItems
                .Include(oi => oi.Product)
                .Include(oi => oi.Order)
                    .ThenInclude(o => o.IntegrationSystem)
                        .ThenInclude(isys => isys.IntegrationSystemParameters)
                .Where(oi => oi.Order.OrderStatusId == 30).AsNoTracking().AsQueryable();

            if (gridCommand.Search != null)
            {
                if (!string.IsNullOrEmpty(gridCommand.Search.Value))
                {
                    query = query = query.Where(b => b.Order.IntegrationSystem.IntegrationSystemParameters.Any(x => x.Value.ToLower().Contains(gridCommand.Search.Value))).AsQueryable();
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

            var totalCount = await query.CountAsync();
            var orderItems = await query
            .Skip(gridCommand.Start)
            .Take(gridCommand.Length)
            .ToListAsync();

            return new Application.DTOs.Common.PagedResult<OrderItem>
            {
                Items = orderItems,
                TotalCount = totalCount,
                PageNumber = gridCommand.Start + 1,
                PageSize = gridCommand.Length
            };

        }

        public async Task UpdateAsync(OrderItem orderItem)
        {
            _context.Entry(orderItem).State = EntityState.Modified;
            _context.OrderItems.Update(orderItem);
            await _context.SaveChangesAsync();
        }
    }
}
