using Entegro.Application.DTOs.Common;
using Entegro.Application.Interfaces.Repositories;
using Entegro.Infrastructure.Data;
using Entegro.Scheduling;
using Microsoft.EntityFrameworkCore;
using System.Linq.Dynamic.Core;
namespace Entegro.Infrastructure.Repositories
{
    public class TaskExecutionInfoRepository : ITaskExecutionInfoRepository
    {
        private readonly EntegroDbContext _context;

        public TaskExecutionInfoRepository(EntegroDbContext context)
        {
            _context = context;
        }

        public async Task DeleteAsync(TaskExecutionInfo info)
        {
            _context.TaskExecutionInfos.Remove(info);
            await _context.SaveChangesAsync();
        }

        public async Task<TaskExecutionInfo?> GetByIdAsync(int id)
        {
            return await _context.TaskExecutionInfos.FirstOrDefaultAsync(o => o.Id == id);
        }

        public async Task<Application.DTOs.Common.PagedResult<TaskExecutionInfo>> GetPagedAsync(GridCommand gridCommand, int taskDescriptorId)
        {
            var query = _context.TaskExecutionInfos.Where(cam => cam.TaskDescriptorId == taskDescriptorId).AsQueryable();

            if (gridCommand.Search != null)
            {
                if (!string.IsNullOrEmpty(gridCommand.Search.Value))
                {
                    query = query.Where(b => b.MachineName.Contains(gridCommand.Search.Value)).AsQueryable();
                }
            }

            IOrderedQueryable<TaskExecutionInfo> orderedQuery = null;
            if (gridCommand.Order.Any())
            {
                foreach (var item in gridCommand.Order)
                {
                    var field = string.IsNullOrEmpty(gridCommand.Columns[item.Column].Name)
                        ? gridCommand.Columns[item.Column].Data
                        : gridCommand.Columns[item.Column].Name;

                    if (orderedQuery == null)
                        orderedQuery = query.OrderBy($"{field} {(item.Dir ?? "asc")}");
                    else
                        orderedQuery = orderedQuery.ThenBy($"{field} {(item.Dir ?? "asc")}");
                }
                query = orderedQuery;
            }
            else
            {
                query = query.OrderBy(o => o.Id);
            }

            var totalCount = await query.CountAsync();
            var infos = await query
            .Skip(gridCommand.Start)
            .Take(gridCommand.Length)
            .ToListAsync();

            return new Application.DTOs.Common.PagedResult<TaskExecutionInfo>
            {
                Items = infos,
                TotalCount = totalCount,
                PageNumber = gridCommand.Start + 1,
                PageSize = gridCommand.Length
            };
        }
    }
}
