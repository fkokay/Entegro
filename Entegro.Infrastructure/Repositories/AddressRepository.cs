using Entegro.Application.DTOs.Common;
using Entegro.Application.Interfaces.Repositories;
using Entegro.Domain.Entities.Common;
using Entegro.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq.Dynamic.Core;

namespace Entegro.Infrastructure.Repositories
{
    public class AddressRepository : IAddressRepository
    {
        private readonly EntegroDbContext _context;

        public AddressRepository(EntegroDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Address address)
        {
            address.CreatedOnUtc = DateTime.UtcNow;
            address.UpdatedOnUtc = DateTime.UtcNow;
            await _context.Addresses.AddAsync(address);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Address address)
        {
            var entity = new Address { Id = address.Id };
            _context.Addresses.Attach(entity);
            _context.Addresses.Remove(entity);
            await _context.SaveChangesAsync();
        }

        public async Task<List<Address>> GetAllAsync()
        {
            return await _context.Addresses.AsNoTracking().OrderBy(a => a.Id).ToListAsync();
        }

        public async Task<Application.DTOs.Common.PagedResult<Address>> GetAllAsync(int pageNumber, int pageSize)
        {
            var query = _context.Addresses.AsNoTracking().OrderBy(a => a.Id);

            var totalCount = await query.CountAsync();
            var items = await query
                .Skip(pageNumber * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new Application.DTOs.Common.PagedResult<Address>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }

        public async Task<Address?> GetByIdAsync(int id)
        {
            return await _context.Addresses.FirstOrDefaultAsync(o => o.Id == id);
        }

        public async Task<Application.DTOs.Common.PagedResult<Address>> GetPagedAsync(GridCommand gridCommand, int customerId)
        {
            var query = _context.CustomerAddressMappings.Where(cam => cam.CustomerId == customerId).Select(cam => cam.Address).AsNoTracking().AsQueryable();

            if (gridCommand.Search != null)
            {
                if (!string.IsNullOrEmpty(gridCommand.Search.Value))
                {
                    query = query.Where(b => b.Title.Contains(gridCommand.Search.Value)).AsQueryable();
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
            var addresses = await query
            .Skip(gridCommand.Start)
            .Take(gridCommand.Length)
            .ToListAsync();

            return new Application.DTOs.Common.PagedResult<Address>
            {
                Items = addresses,
                TotalCount = totalCount,
                PageNumber = gridCommand.Start + 1,
                PageSize = gridCommand.Length
            };
        }

        public async Task UpdateAsync(Address address)
        {
            address.UpdatedOnUtc = DateTime.UtcNow;
            _context.Addresses.Update(address);
            await _context.SaveChangesAsync();
        }
    }
}
