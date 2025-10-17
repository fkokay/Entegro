using Entegro.Application.DTOs.Common;
using Entegro.Application.Interfaces.Repositories;
using Entegro.Domain.Entities.Checkout;
using Entegro.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq.Dynamic.Core;

namespace Entegro.Infrastructure.Repositories
{
    public class ShipmentItemRepository : IShipmentItemRepository
    {
        private readonly EntegroDbContext _context;

        public ShipmentItemRepository(EntegroDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(ShipmentItem shipmentItem)
        {
            await _context.ShipmentItems.AddAsync(shipmentItem);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(ShipmentItem shipmentItem)
        {
            var tracked = _context.ShipmentItems.Local.FirstOrDefault(b => b.Id == shipmentItem.Id);
            if (tracked != null)
            {
                _context.ShipmentItems.Remove(tracked);
            }
            else
            {
                _context.ShipmentItems.Attach(shipmentItem);
                _context.ShipmentItems.Remove(shipmentItem);
            }

            await _context.SaveChangesAsync();
        }

        public async Task<bool> ExistsByIdAsync(int id)
        {
            return await _context.ShipmentItems.AnyAsync(o => o.Id == id);
        }

        public async Task<bool> ExistsByShipmentIdAsync(int shipmentId)
        {
            return await _context.ShipmentItems.AnyAsync(o => o.ShipmentId == shipmentId);
        }

        public async Task<List<ShipmentItem>> GetAllAsync()
        {
            return await _context.ShipmentItems.AsNoTracking().OrderBy(b => b.Id).ToListAsync();
        }

        public async Task<Application.DTOs.Common.PagedResult<ShipmentItem>> GetAllAsync(int pageNumber, int pageSize)
        {
            var query = _context.ShipmentItems
                 .AsNoTracking()
                 .OrderBy(b => b.Id);

            var totalCount = await query.CountAsync();
            var brands = await query
                .Skip(pageNumber * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new Application.DTOs.Common.PagedResult<ShipmentItem>
            {
                Items = brands,
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }

        public async Task<ShipmentItem?> GetByIdAsync(int id)
        {
            return await _context.ShipmentItems.AsNoTracking().FirstOrDefaultAsync(o => o.Id == id);
        }

        public async Task<ShipmentItem?> GetByShipmentIdAsync(int shipmentId)
        {
            return await _context.ShipmentItems.AsNoTracking().FirstOrDefaultAsync(o => o.ShipmentId == shipmentId);
        }

        public async Task<Application.DTOs.Common.PagedResult<ShipmentItem>> GetPagedAsync(GridCommand gridCommand)
        {
            var query = _context.ShipmentItems.AsNoTracking();

            if (gridCommand.Search != null)
            {
                if (!string.IsNullOrEmpty(gridCommand.Search.Value))
                {
                    query = query.Where(b => b.ShipmentId.ToString().Contains(gridCommand.Search.Value)).AsQueryable();
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
            var brands = await query
            .Skip(gridCommand.Start)
            .Take(gridCommand.Length)
            .ToListAsync();

            return new Application.DTOs.Common.PagedResult<ShipmentItem>
            {
                Items = brands,
                TotalCount = totalCount,
                PageNumber = gridCommand.Start + 1,
                PageSize = gridCommand.Length
            };
        }

        public async Task UpdateAsync(ShipmentItem shipmentItem)
        {
            _context.ShipmentItems.Update(shipmentItem);
            await _context.SaveChangesAsync();
        }
    }
}
