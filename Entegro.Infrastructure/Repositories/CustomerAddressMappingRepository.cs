using Entegro.Application.Interfaces.Repositories;
using Entegro.Domain.Entities.Checkout;
using Entegro.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Entegro.Infrastructure.Repositories
{
    public class CustomerAddressMappingRepository : ICustomerAddressMappingRepository
    {
        private readonly EntegroDbContext _context;

        public CustomerAddressMappingRepository(EntegroDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(CustomerAddressMapping mapping)
        {
            await _context.CustomerAddressMappings.AddAsync(mapping);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(CustomerAddressMapping mapping)
        {
            _context.CustomerAddressMappings.Remove(mapping);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> ExistsAsync(int customerId, int addressId)
        {
            return await _context.CustomerAddressMappings.AsNoTracking().AnyAsync(cam => cam.CustomerId == customerId && cam.AddressId == addressId);
        }

        public async Task<CustomerAddressMapping?> GetAsync(int customerId, int addressId)
        {
            return await _context.CustomerAddressMappings
            .Include(cam => cam.Customer)
            .Include(cam => cam.Address).AsNoTracking()
            .FirstOrDefaultAsync(cam => cam.CustomerId == customerId && cam.AddressId == addressId);
        }

        public async Task<IEnumerable<CustomerAddressMapping>> GetByCustomerIdAsync(int customerId)
        {
            return await _context.CustomerAddressMappings
           .Include(cam => cam.Address)
           .Where(cam => cam.CustomerId == customerId).AsNoTracking()
           .ToListAsync();
        }

        public async Task UpdateAsync(CustomerAddressMapping mapping)
        {
            _context.CustomerAddressMappings.Update(mapping);
            await _context.SaveChangesAsync();
        }
    }
}
