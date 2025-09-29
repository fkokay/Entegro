using Entegro.Application.DTOs.Common;
using Entegro.Application.Interfaces.Repositories;
using Entegro.Infrastructure.Data;
using Entegro.Scheduling;
using Microsoft.EntityFrameworkCore;
using System.Linq.Dynamic.Core;

namespace Entegro.Infrastructure.Repositories
{
    public class TaskDescriptorRepository : ITaskDescriptorRepository
    {
        private readonly EntegroDbContext _context;
        public TaskDescriptorRepository(EntegroDbContext context)
        {
            _context = context;
        }

        public async Task<TaskDescriptor?> GetByIdAsync(int id)
        {
            return await _context.TaskDescriptors.Include(b => b.ExecutionHistory).AsNoTracking().FirstOrDefaultAsync(o => o.Id == id);
        }

        public async Task<Application.DTOs.Common.PagedResult<TaskDescriptor>> GetPagedAsync(GridCommand gridCommand)
        {
            var query = _context.TaskDescriptors.AsNoTracking();

            if (gridCommand.Search != null)
            {
                if (!string.IsNullOrEmpty(gridCommand.Search.Value))
                {
                    query = query.Where(b => b.Name.Contains(gridCommand.Search.Value)).AsQueryable();
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
            var items = await query
            .Skip(gridCommand.Start)
            .Take(gridCommand.Length)
            .ToListAsync();

            return new Application.DTOs.Common.PagedResult<TaskDescriptor>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = gridCommand.Start + 1,
                PageSize = gridCommand.Length
            };
        }

        public async Task UpdateAsync(TaskDescriptor taskDescriptor)
        {
            _context.TaskDescriptors.Update(taskDescriptor);
            await _context.SaveChangesAsync();
        }
    }
}
