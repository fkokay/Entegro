using Entegro.Application.DTOs.Common;
using Entegro.Application.Interfaces.Repositories;
using Entegro.Domain.Entities.Setttings;
using Entegro.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq.Dynamic.Core;
namespace Entegro.Infrastructure.Repositories
{
    public class SettingRepository : ISettingRepository
    {
        private readonly EntegroDbContext _context;

        public SettingRepository(EntegroDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Setting model)
        {
            await _context.Settings.AddAsync(model);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Setting model)
        {
            var tracked = _context.Settings.Local.FirstOrDefault(b => b.Id == model.Id);
            if (tracked != null)
            {
                _context.Settings.Remove(tracked);
            }
            else
            {
                _context.Settings.Attach(model);
                _context.Settings.Remove(model);
            }

            await _context.SaveChangesAsync();
        }

        public async Task<bool> ExistsByIdAsync(int id)
        {
            return await _context.Settings.AnyAsync(o => o.Id == id);
        }

        public async Task<bool> ExistsByKeyAsync(string key)
        {
            return await _context.Settings.AsNoTracking().AnyAsync(o => o.Key == key);
        }

        public async Task<bool> ExistsByValueAsync(string value)
        {
            return await _context.Settings.AsNoTracking().AnyAsync(o => o.Value == value);
        }

        public async Task<List<Setting>> GetAllAsync()
        {
            return await _context.Settings.AsNoTracking().OrderBy(b => b.Id).ToListAsync();
        }

        public async Task<Application.DTOs.Common.PagedResult<Setting>> GetAllAsync(int pageNumber, int pageSize)
        {
            var query = _context.Settings
               .AsNoTracking()
               .OrderBy(b => b.Id);

            var totalCount = await query.CountAsync();
            var settings = await query
                .Skip(pageNumber * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new Application.DTOs.Common.PagedResult<Setting>
            {
                Items = settings,
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }

        public async Task<Setting?> GetByIdAsync(int id)
        {
            return await _context.Settings.FirstOrDefaultAsync(o => o.Id == id);
        }

        public async Task<Setting?> GetByKeyAsync(string key)
        {
            return await _context.Settings.AsNoTracking().FirstOrDefaultAsync(o => o.Key == key);
        }

        public async Task<Setting?> GetByValueAsync(string value)
        {
            return await _context.Settings.AsNoTracking().FirstOrDefaultAsync(o => o.Value == value);
        }

        public async Task<Application.DTOs.Common.PagedResult<Setting>> GetPagedAsync(GridCommand gridCommand)
        {
            var query = _context.Settings.AsNoTracking();

            if (gridCommand.Search != null)
            {
                if (!string.IsNullOrEmpty(gridCommand.Search.Value))
                {
                    query = query.Where(b => b.Key.Contains(gridCommand.Search.Value)).AsQueryable();
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
            var settings = await query
            .Skip(gridCommand.Start)
            .Take(gridCommand.Length)
            .ToListAsync();

            return new Application.DTOs.Common.PagedResult<Setting>
            {
                Items = settings,
                TotalCount = totalCount,
                PageNumber = gridCommand.Start + 1,
                PageSize = gridCommand.Length
            };
        }

        public async Task UpdateAsync(Setting model)
        {
            _context.Settings.Update(model);
            await _context.SaveChangesAsync();
        }
    }
}
