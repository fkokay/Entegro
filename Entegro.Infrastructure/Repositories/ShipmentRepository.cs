using Entegro.Application.DTOs.Common;
using Entegro.Application.Interfaces.Repositories;
using Entegro.Domain.Entities.Checkout;
using Entegro.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq.Dynamic.Core;
namespace Entegro.Infrastructure.Repositories
{
    public class ShipmentRepository : IShipmentRepository
    {
        private readonly EntegroDbContext _context;

        public ShipmentRepository(EntegroDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Shipment shipment)
        {
            shipment.CreatedOnUtc = DateTime.UtcNow;
            await _context.Shipments.AddAsync(shipment);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Shipment shipment)
        {
            var tracked = _context.Shipments.Local.FirstOrDefault(b => b.Id == shipment.Id);
            if (tracked != null)
            {
                _context.Shipments.Remove(tracked);
            }
            else
            {
                _context.Shipments.Attach(shipment);
                _context.Shipments.Remove(shipment);
            }

            await _context.SaveChangesAsync();
        }


        public async Task<bool> ExistsByIdAsync(int id)
        {
            return await _context.Shipments.AnyAsync(o => o.Id == id);
        }

        public async Task<bool> ExistsByOrderIdAsync(int orderId)
        {
            return await _context.Shipments.AnyAsync(o => o.OrderId == orderId);
        }

        public async Task<bool> ExistsByTrackingNumberAsync(string trackingNumber)
        {
            return await _context.Shipments.AnyAsync(o => o.TrackingNumber == trackingNumber);
        }

        public async Task<List<Shipment>> GetAllAsync()
        {
            return await _context.Shipments.Include(m => m.ShipmentItems).Include(m => m.Order).AsNoTracking().OrderBy(b => b.Id).ToListAsync();
        }

        public async Task<Application.DTOs.Common.PagedResult<Shipment>> GetAllAsync(int pageNumber, int pageSize)
        {
            var query = _context.Shipments
                  .Include(m => m.ShipmentItems)
                  .Include(m => m.Order)
                  .AsNoTracking()
                  .OrderBy(b => b.Id);

            var totalCount = await query.CountAsync();
            var brands = await query
                .Skip(pageNumber * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new Application.DTOs.Common.PagedResult<Shipment>
            {
                Items = brands,
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }

        public async Task<Shipment?> GetByIdAsync(int id)
        {
            return await _context.Shipments.FirstOrDefaultAsync(o => o.Id == id);
        }

        public async Task<Shipment?> GetByOrderIdAsync(int orderId)
        {
            return await _context.Shipments.FirstOrDefaultAsync(o => o.OrderId == orderId);
        }

        public async Task<Shipment?> GetByTrackingNumberAsync(string trackingNumber)
        {
            return await _context.Shipments.FirstOrDefaultAsync(o => o.TrackingNumber == trackingNumber);
        }

        public async Task<Application.DTOs.Common.PagedResult<Shipment>> GetPagedAsync(GridCommand gridCommand)
        {
            var query = _context.Shipments.Include(m => m.ShipmentItems).Include(m => m.Order).AsNoTracking();

            if (gridCommand.Search != null)
            {
                if (!string.IsNullOrEmpty(gridCommand.Search.Value))
                {
                    query = query.Where(b => b.TrackingNumber.Contains(gridCommand.Search.Value)).AsQueryable();
                }
            }
            if (gridCommand.Search != null)
            {
                if (!string.IsNullOrEmpty(gridCommand.Search.Value))
                {
                    query = query.Where(b => b.OrderId.ToString().Contains(gridCommand.Search.Value)).AsQueryable();
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

            return new Application.DTOs.Common.PagedResult<Shipment>
            {
                Items = brands,
                TotalCount = totalCount,
                PageNumber = gridCommand.Start + 1,
                PageSize = gridCommand.Length
            };
        }

        public async Task UpdateAsync(Shipment shipment)
        {
            _context.Shipments.Update(shipment);
            await _context.SaveChangesAsync();
        }

        public Task UpdateByDeliveryDateAsync(int id)
        {
            var shipment = _context.Shipments.FirstOrDefault(s => s.Id == id);
            shipment.DeliveryDateUtc = DateTime.UtcNow;
            _context.Shipments.Update(shipment);
            return _context.SaveChangesAsync();
        }

        public Task UpdateByShippedDateAsync(int id)
        {
            var shipment = _context.Shipments.FirstOrDefault(s => s.Id == id);
            shipment.ShippedDateUtc = DateTime.UtcNow;
            _context.Shipments.Update(shipment);
            return _context.SaveChangesAsync();
        }
    }
}
