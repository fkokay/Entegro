using Entegro.Application.DTOs.Common;
using Entegro.Application.Interfaces.Repositories;
using Entegro.Domain.Entities;
using Entegro.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Entegro.Infrastructure.Repositories
{
    public class IntegrationSystemParameterRepository : IIntegrationSystemParameterRepository
    {
        private readonly EntegroContext _context;

        public IntegrationSystemParameterRepository(EntegroContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }
        public async Task AddAsync(IntegrationSystemParameter integrationSystemParameter)
        {
            await _context.IntegrationSystemParameters.AddAsync(integrationSystemParameter);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(IntegrationSystemParameter integrationSystemParameter)
        {
            _context.IntegrationSystemParameters.Remove(integrationSystemParameter);
            await _context.SaveChangesAsync();
        }

        public async Task<List<IntegrationSystemParameter>> GetAllAsync()
        {
            return await _context.IntegrationSystemParameters.ToListAsync();
        }

        public async Task<PagedResult<IntegrationSystemParameter>> GetAllAsync(int pageNumber, int pageSize)
        {
            var query = _context.IntegrationSystemParameters.AsQueryable();

            var totalCount = await query.CountAsync();
            var customers = await query
                .Skip(pageNumber * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResult<IntegrationSystemParameter>
            {
                Items = customers,
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }

        public async Task<IntegrationSystemParameter?> GetByIdAsync(int id)
        {
            return await _context.IntegrationSystemParameters.AsNoTracking().FirstOrDefaultAsync(o => o.Id == id);
        }

        public async Task<IntegrationSystemParameter?> GetByKeyAsync(string key)
        {
            return await _context.IntegrationSystemParameters.AsNoTracking().FirstOrDefaultAsync(o => o.Key == key);
        }

        public async Task<IntegrationSystemParameter?> GetByKeyAsync(string key, int integrationSystemId)
        {
            return await _context.IntegrationSystemParameters.AsNoTracking().FirstOrDefaultAsync(o => o.Key == key && o.IntegrationSystemId == integrationSystemId);
        }

        public async Task UpdateAsync(IntegrationSystemParameter integrationSystemParameter)
        {
            _context.IntegrationSystemParameters.Update(integrationSystemParameter);
            await _context.SaveChangesAsync();
        }
    }
}
