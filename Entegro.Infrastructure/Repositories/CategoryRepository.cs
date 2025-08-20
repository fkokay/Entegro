using Entegro.Application.DTOs.Common;
using Entegro.Application.Interfaces.Repositories;
using Entegro.Domain.Entities;
using Entegro.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Entegro.Infrastructure.Repositories
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly EntegroContext _context;

        public CategoryRepository(EntegroContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Category category)
        {
            category.CreatedOn = DateTime.UtcNow;
            category.UpdatedOn = DateTime.UtcNow;
            category.TreePath = $"/{category.Id}/";

            await _context.Categories.AddAsync(category);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Category category)
        {
            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();
        }

        public async Task<List<Category>> GetAllAsync()
        {
            return await _context.Categories.ToListAsync();
        }

        public async Task<PagedResult<Category>> GetAllAsync(int pageNumber, int pageSize)
        {
            var query = _context.Categories.AsQueryable();

            var totalCount = await query.CountAsync();
            var categories = await query
                .Skip(pageNumber * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResult<Category>
            {
                Items = categories,
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }

        public async Task<Category?> GetByIdAsync(int id)
        {
            return await _context.Categories.FirstOrDefaultAsync(o => o.Id == id);
        }

        public async Task<Category?> GetByIdWithMediaAsync(int id)
        {
            return await _context.Categories
             .Include(b => b.MediaFile)
             .ThenInclude(b=>b.Folder)
             .FirstOrDefaultAsync(b => b.Id == id);
        }

        public async Task<List<Category>> GetByParentIdAsync(int parentCategoryId)
        {
            return await _context.Categories.Where(c => c.ParentCategoryId == parentCategoryId).ToListAsync();
        }

        public async Task<Dictionary<int, string>> GetNamesByIdsAsync(IEnumerable<int> ids, CancellationToken ct = default)
        {

            var set = ids.Distinct().ToArray();
            return await _context.Categories
                .AsNoTracking()
                .Where(c => set.Contains(c.Id))
                .Select(c => new { c.Id, c.Name })
                .ToDictionaryAsync(k => k.Id, v => v.Name, ct);


            //var distinctIds = ids.Distinct().ToList();
            //if (distinctIds.Count == 0) return new Dictionary<int, string>();

            //return await _context.Categories.AsNoTracking()
            //    .Where(c => distinctIds.Contains(c.Id))
            //    .Select(c => new { c.Id, c.Name })
            //    .ToDictionaryAsync(x => x.Id, x => x.Name, ct);
        }

        public async Task<PagedResult2<CategorySlim>> SearchPagedAsync(string? term, int page, int pageSize, CancellationToken ct = default)
        {
            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = 20;
            if (pageSize > 100) pageSize = 100;

            var q = _context.Categories.AsNoTracking();

            if (!string.IsNullOrWhiteSpace(term))
            {
                term = term.Trim();
                q = q.Where(c => EF.Functions.Collate(c.Name, "Turkish_CI_AI").Contains(term));
                if (int.TryParse(term, out var idVal))
                {
                    q = q.Union(_context.Categories.AsNoTracking().Where(c => c.Id == idVal));
                }
            }

            q = q.OrderBy(c => c.DisplayOrder).ThenBy(c => c.Name).ThenBy(c => c.Id);

            var skip = (page - 1) * pageSize;

            var rows = await q.Select(c => new CategorySlim { Id = c.Id, Name = c.Name, TreePath = c.TreePath })
                              .Skip(skip)
                              .Take(pageSize + 1) // +1 => hasMore
                              .ToListAsync(ct);

            var hasMore = rows.Count > pageSize;
            if (hasMore) rows.RemoveAt(pageSize);

            return new PagedResult2<CategorySlim>
            {
                Items = rows,
                HasMore = hasMore
            };
        }

        public async Task UpdateAsync(Category category)
        {
            category.UpdatedOn = DateTime.UtcNow;

            _context.Categories.Update(category);
            await _context.SaveChangesAsync();
        }
    }
}


