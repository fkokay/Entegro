using Entegro.Application.DTOs.Common;
using Entegro.Application.Interfaces.Repositories;
using Entegro.Domain.Entities.Import;
using Entegro.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq.Dynamic.Core;
namespace Entegro.Infrastructure.Repositories
{
    public class ImportProfileRepository : IImportProfileRepository
    {
        private readonly EntegroDbContext _context;

        public ImportProfileRepository(EntegroDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(ImportProfile importProfile)
        {
            importProfile.CreatedOnUtc = DateTime.UtcNow;
            await _context.ImportProfiles.AddAsync(importProfile);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(ImportProfile importProfile)
        {
            var tracked = _context.ImportProfiles.Local.FirstOrDefault(b => b.Id == importProfile.Id);
            if (tracked != null)
            {
                _context.ImportProfiles.Remove(tracked);
            }
            else
            {
                _context.ImportProfiles.Attach(importProfile);
                _context.ImportProfiles.Remove(importProfile);
            }

            await _context.SaveChangesAsync();
        }

        public async Task<bool> ExistsByNameAsync(string name)
        {
            return await _context.ImportProfiles.AsNoTracking().AnyAsync(o => o.ProfileName == name);
        }

        public async Task<List<ImportProfile>> GetAllAsync()
        {
            return await _context.ImportProfiles.AsNoTracking().OrderBy(b => b.Id).ToListAsync();
        }

        public async Task<Application.DTOs.Common.PagedResult<ImportProfile>> GetAllAsync(int pageNumber, int pageSize)
        {
            var query = _context.ImportProfiles
                .AsNoTracking()
                .OrderBy(b => b.Id);

            var totalCount = await query.CountAsync();
            var brands = await query
                .Skip(pageNumber * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new Application.DTOs.Common.PagedResult<ImportProfile>
            {
                Items = brands,
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }

        public async Task<ImportProfile?> GetByIdAsync(int id)
        {
            return await _context.ImportProfiles.FirstOrDefaultAsync(o => o.Id == id);
        }

        public async Task<Application.DTOs.Common.PagedResult<ImportProfile>> GetPagedAsync(GridCommand gridCommand)
        {
            var query = _context.ImportProfiles.AsNoTracking();

            if (gridCommand.Search != null)
            {
                if (!string.IsNullOrEmpty(gridCommand.Search.Value))
                {
                    query = query.Where(b => b.ProfileName.Contains(gridCommand.Search.Value)).AsQueryable();
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
            var brands = await query
            .Skip(gridCommand.Start)
            .Take(gridCommand.Length)
            .ToListAsync();

            return new Application.DTOs.Common.PagedResult<ImportProfile>
            {
                Items = brands,
                TotalCount = totalCount,
                PageNumber = gridCommand.Start + 1,
                PageSize = gridCommand.Length
            };
        }

        public async Task UpdateAsync(ImportProfile importProfile)
        {
            _context.ImportProfiles.Update(importProfile);
            await _context.SaveChangesAsync();
        }
    }
}
