using Entegro.Application.DTOs.Common;
using Entegro.Application.Interfaces.Repositories;
using Entegro.Domain.Entities;
using Entegro.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Entegro.Infrastructure.Repositories
{
    public class CountryRepository : ICountryRepository
    {
        private readonly EntegroContext _context;

        public CountryRepository(EntegroContext context)
        {
            _context = context;
        }

        public async Task<Country> GetByIdAsync(int id)
        {
            return await _context.Countries
                .Include(c => c.Cities)
                .FirstOrDefaultAsync(t => t.Id == id);
        }

        public async Task<List<Country>> GetAllAsync()
        {
            return await _context.Countries
                .Include(t => t.Cities)
                .ToListAsync();
        }



        public async Task AddAsync(Country country)
        {
            await _context.Countries.AddAsync(country);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Country country)
        {
            _context.Countries.Update(country);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var country = await _context.Countries.FindAsync(id);
            if (country != null)
            {
                _context.Countries.Remove(country);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<PagedResult<Country>> GetAllAsync(int pageNumber, int pageSize)
        {
            var query = _context.Countries.AsQueryable();

            var totalCount = await query.CountAsync();
            var categories = await query
                .Skip(pageNumber * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResult<Country>
            {
                Items = categories,
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }
    }
}
