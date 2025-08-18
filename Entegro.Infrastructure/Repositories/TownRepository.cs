using Entegro.Application.Interfaces.Repositories;
using Entegro.Domain.Entities;
using Entegro.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Entegro.Infrastructure.Repositories
{
    public class TownRepository : ITownRepository
    {
        private readonly EntegroContext _context;

        public TownRepository(EntegroContext context)
        {
            _context = context;
        }

        public async Task<Town> GetByIdAsync(int id)
        {
            return await _context.Towns
                .Include(t => t.Districts)
                .FirstOrDefaultAsync(t => t.Id == id);
        }

        public async Task<List<Town>> GetAllAsync()
        {
            return await _context.Towns
                .Include(t => t.Districts)
                .ToListAsync();
        }

        public async Task<List<Town>> GetByCityIdAsync(int cityId)
        {
            return await _context.Towns
                .Where(t => t.CityId == cityId)
                .Include(t => t.Districts)
                .ToListAsync();
        }

        public async Task AddAsync(Town town)
        {
            await _context.Towns.AddAsync(town);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Town town)
        {
            _context.Towns.Update(town);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var town = await _context.Towns.FindAsync(id);
            if (town != null)
            {
                _context.Towns.Remove(town);
                await _context.SaveChangesAsync();
            }
        }
    }
}
