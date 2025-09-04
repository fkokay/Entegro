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
            address.CreatedOn = DateTime.Now;
            address.UpdatedOn = DateTime.Now;
            await _context.Addresses.AddAsync(address);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Address address)
        {
            var modal = await _context.Addresses.FindAsync(address.Id);
            if (modal != null)
            {
                _context.Addresses.Remove(modal);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<List<Address>> GetAllAsync()
        {
            return await _context.Addresses.ToListAsync();
        }

        public async Task<PagedResult<Address>> GetAllAsync(int pageNumber, int pageSize)
        {
            var query = _context.Addresses.AsQueryable();

            var totalCount = await query.CountAsync();
            var brands = await query
                .OrderBy(b => b.Id)
                .Skip(pageNumber * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResult<Address>
            {
                Items = brands,
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }

        public async Task<Address?> GetByIdAsync(int id) => await _context.Addresses.FirstOrDefaultAsync(o => o.Id == id);

        public async Task UpdateAsync(Address address)
        {
            address.UpdatedOn = DateTime.Now;
            _context.Addresses.Update(address);
            await _context.SaveChangesAsync();
        }
    }
}
