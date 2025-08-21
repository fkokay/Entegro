using Entegro.Application.DTOs.Common;
using Entegro.Application.Interfaces.Repositories;
using Entegro.Domain.Entities;
using Entegro.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Entegro.Infrastructure.Repositories
{
    public class IntegrationSystemRepository : IIntegrationSystemRepository
    {
        private readonly EntegroContext _context;

        public IntegrationSystemRepository(EntegroContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }
        public async Task AddAsync(IntegrationSystem integrationSystem)
        {
            await _context.IntegrationSystems.AddAsync(integrationSystem);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(IntegrationSystem integrationSystem)
        {
            _context.IntegrationSystems.Remove(integrationSystem);
            await _context.SaveChangesAsync();
        }

        public async Task<List<IntegrationSystem>> GetAllAsync()
        {
            return await _context.IntegrationSystems.Include(p => p.Parameters).ToListAsync();
        }

        public async Task<PagedResult<IntegrationSystem>> GetAllAsync(int pageNumber, int pageSize)
        {
            var query = _context.IntegrationSystems.AsQueryable();

            var totalCount = await query.CountAsync();
            var customers = await query
                .Skip(pageNumber * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResult<IntegrationSystem>
            {
                Items = customers,
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }

        public async Task<IntegrationSystem?> GetByIdAsync(int id)
        {
            return await _context.IntegrationSystems.Include(m => m.Parameters).FirstOrDefaultAsync(o => o.Id == id);
        }

        public async Task<IntegrationSystem?> GetByTypeIdAsync(int typeId)
        {
            return await _context.IntegrationSystems.Include(m => m.Parameters).FirstOrDefaultAsync(o => o.IntegrationSystemTypeId == typeId);
        }

        public async Task UpdateAsync(IntegrationSystem integrationSystem)
        {
            _context.IntegrationSystems.Update(integrationSystem);
            await _context.SaveChangesAsync();
        }
    }
}
