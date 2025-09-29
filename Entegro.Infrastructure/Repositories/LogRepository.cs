using Entegro.Application.DTOs.Common;
using Entegro.Application.Interfaces.Repositories;
using Entegro.Domain.Entities.Platform.Logging;
using Entegro.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq.Dynamic.Core;
namespace Entegro.Infrastructure.Repositories
{
    public class LogRepository : ILogRepository
    {
        private readonly EntegroDbContext _context;

        public LogRepository(EntegroDbContext context)
        {
            _context = context;
        }

        public Task AddAsync(Log log)
        {
            throw new NotImplementedException();
        }

        public async Task DeleteAllAsync()
        {
            var allLogs = await _context.Logs.ToListAsync();
            _context.Logs.RemoveRange(allLogs);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Log log)
        {
            var tracked = _context.Logs.Local.FirstOrDefault(b => b.Id == log.Id);
            if (tracked != null)
            {
                _context.Logs.Remove(tracked);
            }
            else
            {
                _context.Logs.Attach(log);
                _context.Logs.Remove(log);
            }

            await _context.SaveChangesAsync();
        }

        public Task<List<Log>> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        public Task<Application.DTOs.Common.PagedResult<Log>> GetAllAsync(int page, string term)
        {
            throw new NotImplementedException();
        }

        public async Task<Log?> GetByIdAsync(int id)
        {
            return await _context.Logs.AsNoTracking().FirstOrDefaultAsync(o => o.Id == id);
        }

        public async Task<Application.DTOs.Common.PagedResult<Log>> GetPagedAsync(GridCommand gridCommand)
        {
            var query = _context.Logs.AsNoTracking();

            if (gridCommand.Search != null)
            {
                if (!string.IsNullOrEmpty(gridCommand.Search.Value))
                {
                    query = query.Where(b => b.Message.Contains(gridCommand.Search.Value) ||
                    b.MessageTemplate.Contains(gridCommand.Search.Value)).AsQueryable();
                }
            }

            if (gridCommand.Order.Any())
            {
                foreach (var item in gridCommand.Order)
                {
                    query = query.OrderBy($"{gridCommand.Columns[item.Column].Data} {(item.Dir ?? "asc")}");
                }
            }
            else
            {
                query = query.OrderBy(b => b.Id);
            }

            var totalCount = await query.CountAsync();
            var logs = await query
            .Skip(gridCommand.Start)
            .Take(gridCommand.Length)
            .ToListAsync();

            return new Application.DTOs.Common.PagedResult<Log>
            {
                Items = logs,
                TotalCount = totalCount,
                PageNumber = gridCommand.Start + 1,
                PageSize = gridCommand.Length
            };
        }

        public Task UpdateAsync(Log log)
        {
            throw new NotImplementedException();
        }
    }
}
