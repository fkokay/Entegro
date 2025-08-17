using Entegro.Application.DTOs.Common;
using Entegro.Application.Interfaces.Repositories;
using Entegro.Domain.Entities;
using Entegro.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Entegro.Infrastructure.Repositories
{
    public class IntegrationSystemLogRepository : IIntegrationSystemLogRepository
    {
        private readonly EntegroContext _context;

        public IntegrationSystemLogRepository(EntegroContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }
        public async Task AddAsync(IntegrationSystemLog integrationSystemLog)
        {
            await _context.IntegrationSystemLogs.AddAsync(integrationSystemLog);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(IntegrationSystemLog integrationSystemLog)
        {
            _context.IntegrationSystemLogs.Remove(integrationSystemLog);
            await _context.SaveChangesAsync();
        }

        public async Task<List<IntegrationSystemLog>> GetAllAsync()
        {
            return await _context.IntegrationSystemLogs.ToListAsync();
        }

        public async Task<PagedResult<IntegrationSystemLog>> GetAllAsync(int pageNumber, int pageSize)
        {
            var query = _context.IntegrationSystemLogs.AsQueryable();

            var totalCount = await query.CountAsync();
            var customers = await query
                .Skip(pageNumber * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResult<IntegrationSystemLog>
            {
                Items = customers,
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }

        public async Task<IntegrationSystemLog?> GetByIdAsync(int id)
        {
            return await _context.IntegrationSystemLogs.FirstOrDefaultAsync(o => o.Id == id);
        }

        public async Task UpdateAsync(IntegrationSystemLog integrationSystemLog)
        {
            _context.IntegrationSystemLogs.Update(integrationSystemLog);
            await _context.SaveChangesAsync();
        }
    }
}
