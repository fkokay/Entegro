
using Entegro.Application.DTOs.Common;
using Entegro.Application.Interfaces.Repositories;
using Entegro.Domain.Entities.Catalog;
using Entegro.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq.Dynamic.Core;

namespace Entegro.Infrastructure.Repositories
{
    public class BrandRepository : IBrandRepository
    {
        private readonly EntegroDbContext _context;

        public BrandRepository(EntegroDbContext context)
        {
            _context = context;
        }
        public async Task AddAsync(Brand brand)
        {
            brand.CreatedOnUtc = DateTime.UtcNow;
            brand.UpdatedOnUtc = DateTime.UtcNow;
            await _context.Brands.AddAsync(brand);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Brand brand)
        {
            var tracked = _context.Brands.Local.FirstOrDefault(b => b.Id == brand.Id);
            if (tracked != null)
            {
                _context.Brands.Remove(tracked);
            }
            else
            {
                _context.Brands.Attach(brand);
                _context.Brands.Remove(brand);
            }

            await _context.SaveChangesAsync();
        }

        public async Task<bool> ExistsByIdAsync(int id)
        {
            return await _context.Brands.AnyAsync(o => o.Id == id);
        }

        public async Task<bool> ExistsByNameAsync(string name)
        {
            return await _context.Brands.AsNoTracking().AnyAsync(o => o.Name == name);
        }

        public async Task<Application.DTOs.Common.PagedResult<Brand>> GetAllAsync(int page, string term)
        {
            var query = _context.Brands.Include(m => m.MediaFile).ThenInclude(m => m.Folder).AsNoTracking();
            if (!string.IsNullOrEmpty(term))
            {
                query = query.Where(b => b.Name.Contains(term)).AsQueryable();
            }

            var totalCount = await query.CountAsync();
            var items = await query.Skip((page * 7) - 7)
                .Take(7).ToListAsync();

            return new Application.DTOs.Common.PagedResult<Brand>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = page,
                PageSize = 7
            };
        }

        public async Task<List<Brand>> GetAllAsync()
        {
            return await _context.Brands.Include(m => m.MediaFile).ThenInclude(m => m.Folder).AsNoTracking().ToListAsync();
        }

        public async Task<Brand?> GetByIdAsync(int id)
        {
            return await _context.Brands
                .Include(b => b.MediaFile)
                .ThenInclude(b => b.Folder)
                .AsNoTracking().FirstOrDefaultAsync(o => o.Id == id);
        }

        public async Task<Brand?> GetByIdWithMediaAsync(int id)
        {
            return await _context.Brands
             .Include(b => b.MediaFile)
             .ThenInclude(b => b.Folder)
             .FirstOrDefaultAsync(b => b.Id == id);
        }

        public async Task<Brand?> GetByNameAsync(string name)
        {
            return await _context.Brands.AsNoTracking().FirstOrDefaultAsync(o => o.Name == name);
        }

        public async Task<Application.DTOs.Common.PagedResult<Brand>> GetPagedAsync(GridCommand gridCommand)
        {
            var query = _context.Brands.Include(m => m.MediaFile).ThenInclude(m => m.Folder).OrderBy(b => b.Id).AsNoTracking();


            if (gridCommand.Columns != null)
            {
                foreach (var col in gridCommand.Columns)
                {
                    if (!string.IsNullOrEmpty(col.Search?.Value))
                    {
                        var searchVal = col.Search.Value.Trim('^', '$');

                        // ilgili property’nin tipini bul
                        var prop = typeof(Brand).GetProperty(col.Data);
                        if (prop == null) continue;

                        if (prop.PropertyType == typeof(string))
                        {
                            query = query.Where($"{col.Data}.Contains(@0)", searchVal);
                        }
                        else if (prop.PropertyType == typeof(int) || prop.PropertyType == typeof(int?))
                        {
                            if (int.TryParse(searchVal, out var intVal))
                                query = query.Where($"{col.Data} == @0", intVal);
                        }
                        else if (prop.PropertyType == typeof(bool) || prop.PropertyType == typeof(bool?))
                        {
                            if (bool.TryParse(searchVal, out var boolVal))
                                query = query.Where($"{col.Data} == @0", boolVal);
                        }
                        else if (prop.PropertyType == typeof(DateTime) || prop.PropertyType == typeof(DateTime?))
                        {
                            if (DateTime.TryParse(searchVal, out var dt))
                                query = query.Where($"{col.Data}.Date == @0", dt.Date);
                        }
                    }
                }
            }

            if (gridCommand.Search != null)
            {
                if (!string.IsNullOrEmpty(gridCommand.Search.Value))
                {
                    query = query.Where(b =>
                    b.Name.Contains(gridCommand.Search.Value)).AsQueryable();
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
                .AsNoTracking()
                .AsSplitQuery()
                .ToListAsync();

            return new Application.DTOs.Common.PagedResult<Brand>
            {
                Items = brands,
                TotalCount = totalCount,
                PageNumber = gridCommand.Start / gridCommand.Length,
                PageSize = gridCommand.Length
            };
        }

        public async Task UpdateAsync(Brand brand)
        {
            brand.UpdatedOnUtc = DateTime.UtcNow;
            _context.Brands.Update(brand);
            await _context.SaveChangesAsync();
        }

    }
}
