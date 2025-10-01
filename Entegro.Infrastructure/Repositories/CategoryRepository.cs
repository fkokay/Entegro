using Entegro.Application.DTOs.Common;
using Entegro.Application.Interfaces.Repositories;
using Entegro.Domain.Entities.Catalog;
using Entegro.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq.Dynamic.Core;
using System.Linq.Expressions;
namespace Entegro.Infrastructure.Repositories
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly EntegroDbContext _context;

        public CategoryRepository(EntegroDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Category category)
        {
            category.CreatedOnUtc = DateTime.UtcNow;
            category.UpdatedOnUtc = DateTime.UtcNow;

            await _context.Categories.AddAsync(category);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Category category)
        {
            category.UpdatedOnUtc = DateTime.UtcNow;

            _context.Categories.Update(category);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Category category)
        {
            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();
        }

        public async Task<List<Category>> GetAllAsync(bool includeHidden = false)
        {
            var query = _context.Categories.Where(m => m.Deleted == includeHidden)
                .Include(c => c.Parent)
                .Include(m => m.MediaFile)
                .ThenInclude(m => m.Folder).AsNoTracking();

            query = includeHidden ? query : query.Where(c => c.Published);
            query = query.OrderBy(x => x.ParentId)
                .ThenBy(x => x.DisplayOrder)
                .ThenBy(x => x.Name);

            return await query.ToListAsync();
        }

        public async Task<Application.DTOs.Common.PagedResult<Category>> GetAllAsync(string term, int pageNumber, int pageSize)
        {
            var query = _context.Categories.Where(m => m.Deleted == false)
                .Include(c => c.Parent)
                .Include(m => m.MediaFile)
                .ThenInclude(m => m.Folder)
                .OrderBy(b => b.Id)
                .AsNoTracking();


            if (!string.IsNullOrEmpty(term))
            {
                query = query.Where(m => m.Name.Contains(term));
            }


            var totalCount = await query.CountAsync();
            var items = await query
                .Skip(pageNumber * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new Application.DTOs.Common.PagedResult<Category>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }

        public async Task<bool> ExistsAsync(Expression<Func<Category, bool>> predicate)
        {
            return await _context.Categories.AnyAsync(predicate);
        }

        public async Task<Category?> GetByAsync(Expression<Func<Category, bool>> predicate)
        {
            return await _context.Categories
            .Include(b => b.Parent)
            .Include(b => b.MediaFile)
            .ThenInclude(b => b.Folder)
            .FirstOrDefaultAsync(predicate);
        }

        public async Task<List<Category>> GetManyAsync(IEnumerable<int> ids, bool tracked = false)
        {
            var query = _context.Categories
                .Where(c => ids.Contains(c.Id));
            if (tracked)
                query = query.AsTracking();
            else
                query = query.AsNoTracking();

            return await query.ToListAsync();

        }

        public async Task<Application.DTOs.Common.PagedResult<Category>> GetPagedAsync(GridCommand gridCommand)
        {
            var query = _context.Categories.Where(m => m.Deleted == false)
                .Include(c => c.Parent)
                .Include(m => m.MediaFile)
                .ThenInclude(m => m.Folder)
                .OrderBy(b => b.Id)
                .AsNoTracking();

            if (gridCommand.Columns != null)
            {
                foreach (var col in gridCommand.Columns)
                {
                    if (!string.IsNullOrEmpty(col.Search?.Value))
                    {
                        var searchVal = col.Search.Value.Trim('^', '$');

                        // ilgili property’nin tipini bul
                        var prop = typeof(Category).GetProperty(col.Data);
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

            var categories = await query
                .Skip(gridCommand.Start)
                .Take(gridCommand.Length)
                .AsNoTracking()
                .AsSplitQuery()
                .ToListAsync();

            return new Application.DTOs.Common.PagedResult<Category>
            {
                Items = categories,
                TotalCount = totalCount,
                PageNumber = gridCommand.Start / gridCommand.Length,
                PageSize = gridCommand.Length
            };
        }
    }
}


