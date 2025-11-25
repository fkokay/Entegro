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
            return await _context.Shipments.AsNoTracking().AnyAsync(o => o.Id == id);
        }

        public async Task<bool> ExistsByOrderIdAsync(int orderId)
        {
            return await _context.Shipments.AsNoTracking().AnyAsync(o => o.OrderId == orderId);
        }

        public async Task<bool> ExistsByTrackingNumberAsync(string trackingNumber)
        {
            return await _context.Shipments.AsNoTracking().AnyAsync(o => o.TrackingNumber == trackingNumber);
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
            return await _context.Shipments.AsNoTracking().FirstOrDefaultAsync(o => o.Id == id);
        }

        public async Task<Shipment?> GetByOrderIdAsync(int orderId)
        {
            return await _context.Shipments.AsNoTracking().FirstOrDefaultAsync(o => o.OrderId == orderId);
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



        public async Task<Application.DTOs.Common.PagedResult<Shipment>> GetShipmentsByIntegrationIdAsync(GridCommand gridCommand)
        {
            var query = _context.Shipments
                .Include(m => m.ShipmentItems)
                .Include(m => m.Order)
                .Where(m => string.IsNullOrEmpty(m.PackageNo))
                .Where(m => m.ShippingIntegrationId != null)
                .AsNoTracking();
            if (gridCommand.Columns != null)
            {
                foreach (var col in gridCommand.Columns)
                {
                    if (!string.IsNullOrEmpty(col.Search?.Value))
                    {
                        var searchVal = col.Search.Value.Trim('^', '$');

                        // ilgili property’nin tipini bul
                        var prop = typeof(Shipment).GetProperty(col.Data);
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
                    b.Carrier.Contains(gridCommand.Search.Value) ||
                    b.OrderId.ToString().Contains(gridCommand.Search.Value)).AsQueryable();
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
            var shipments = await query
            .Skip(gridCommand.Start)
            .Take(gridCommand.Length)
            .ToListAsync();

            return new Application.DTOs.Common.PagedResult<Shipment>
            {
                Items = shipments,
                TotalCount = totalCount,
                PageNumber = gridCommand.Start + 1,
                PageSize = gridCommand.Length
            };
        }

        public async Task UpdateAsync(Shipment shipment)
        {
            _context.Entry(shipment).State = EntityState.Modified;
            _context.Entry(shipment).Collection(p => p.ShipmentItems).IsModified = false;
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
