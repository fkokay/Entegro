using Entegro.Application.DTOs.Common;
using Entegro.Application.Interfaces.Repositories;
using Entegro.Domain.Entities;
using Entegro.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Entegro.Infrastructure.Repositories
{
    public class AddressRepository : IAddressRepository
    {
        private readonly EntegroContext _context;

        public AddressRepository(EntegroContext context)
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

        public async Task<PagedResult<Address>> GetAllAsync(int pageNumber, int pageSize)
        {
            var query = _context.Addresses.AsNoTracking().OrderBy(a => a.Id);

            var totalCount = await query.CountAsync();
            var items = await query
                .Skip(pageNumber * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResult<Address>
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

        public async Task UpdateAsync(Address address)
        {
            address.UpdatedOnUtc = DateTime.UtcNow;
            _context.Addresses.Update(address);
            await _context.SaveChangesAsync();
        }
    }
}
