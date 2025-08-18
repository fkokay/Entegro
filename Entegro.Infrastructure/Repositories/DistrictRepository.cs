using Entegro.Application.Interfaces.Repositories;
using Entegro.Domain.Entities;
using Entegro.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Entegro.Infrastructure.Repositories
{
    public class DistrictRepository : IDistrictRepository
    {
        private readonly EntegroContext _context;

        public DistrictRepository(EntegroContext context)
        {
            _context = context;
        }

        public async Task<District> GetByIdAsync(int id)
        {
            return await _context.Districts.FindAsync(id);
        }

        public async Task<List<District>> GetAllAsync()
        {
            return await _context.Districts.ToListAsync();
        }

        public async Task<List<District>> GetByTownIdAsync(int townId)
        {
            return await _context.Districts
                .Where(d => d.TownId == townId)
                .ToListAsync();
        }

        public async Task AddAsync(District district)
        {
            await _context.Districts.AddAsync(district);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(District district)
        {
            _context.Districts.Update(district);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var district = await _context.Districts.FindAsync(id);
            if (district != null)
            {
                _context.Districts.Remove(district);
                await _context.SaveChangesAsync();
            }
        }
    }
}
