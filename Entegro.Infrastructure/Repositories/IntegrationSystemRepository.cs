using Entegro.Application.DTOs.Common;
using Entegro.Application.Interfaces.Repositories;
using Entegro.Domain.Entities.Integration;
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
            var integrationSystems = await _context.IntegrationSystems
            .Include(x => x.IntegrationSystemParameters)
            .AsNoTracking()
            .ToListAsync();

            return integrationSystems;
        }

        public async Task<List<IntegrationSystem>> GetAllAsync(int? integrationSystemTypeId)
        {
            var query = _context.IntegrationSystems
            .Include(x => x.IntegrationSystemParameters)
            .AsNoTracking();

            if (integrationSystemTypeId.HasValue)
            {
                query = query.Where(x => x.IntegrationSystemTypeId == integrationSystemTypeId.Value);
            }

            var integrationSystems = await query.ToListAsync();

            return integrationSystems;
        }

        public async Task<PagedResult<IntegrationSystem>> GetAllAsync(int pageNumber, int pageSize)
        {
            var query = _context.IntegrationSystems.AsNoTracking().AsQueryable();

            var totalCount = await query.CountAsync();
            var customers = await query
                .OrderBy(b => b.Id)
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
            return await _context.IntegrationSystems.AsNoTracking().Include(m => m.IntegrationSystemParameters).Select(m => new IntegrationSystem()
            {
                Id = m.Id,
                Description = m.Description,
                IntegrationSystemParameters = m.IntegrationSystemParameters,
                Name = m.Name,
                IntegrationSystemTypeId = m.IntegrationSystemTypeId,
                IntegrationSystemType = m.IntegrationSystemType
            }).FirstOrDefaultAsync(o => o.Id == id);
        }

        public async Task<IntegrationSystem?> GetByTypeIdAsync(int typeId)
        {
            return await _context.IntegrationSystems.AsNoTracking().Include(m => m.IntegrationSystemParameters).Select(m => new IntegrationSystem()
            {
                Id = m.Id,
                Description = m.Description,
                IntegrationSystemParameters = m.IntegrationSystemParameters,
                Name = m.Name,
                IntegrationSystemTypeId = m.IntegrationSystemTypeId,
                IntegrationSystemType = m.IntegrationSystemType,
            }).FirstOrDefaultAsync(o => o.IntegrationSystemTypeId == typeId);
        }

        public async Task UpdateAsync(IntegrationSystem integrationSystem)
        {
            _context.IntegrationSystems.Update(integrationSystem);
            await _context.SaveChangesAsync();
        }
    }
}
