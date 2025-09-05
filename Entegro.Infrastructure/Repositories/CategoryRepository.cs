using Entegro.Application.DTOs.Common;
using Entegro.Application.Interfaces.Repositories;
using Entegro.Domain.Entities;
using Entegro.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

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
            await _context.Categories.AddAsync(category);
            await _context.SaveChangesAsync();

            category.TreePath = $"/{category.Id}/";
            _context.Categories.Update(category);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Category category)
        {
            category.UpdatedOn = DateTime.UtcNow;

            _context.Categories.Update(category);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Category category)
        {
            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();
        }

        public async Task<List<Category>> GetAllAsync()
        {
            return await _context.Categories
                .Include(c => c.ParentCategory)
                .Include(m => m.MediaFile)
                .ThenInclude(m => m.MediaFolder).AsNoTracking().ToListAsync();
        }

        public async Task<PagedResult<Category>> GetAllAsync(string term,int pageNumber, int pageSize)
        {
            var query = _context.Categories
                .Include(c => c.ParentCategory)
                .Include(m => m.MediaFile)
                .ThenInclude(m => m.MediaFolder)
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

            return new PagedResult<Category>
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
            .Include(b => b.MediaFile)
            .ThenInclude(b => b.MediaFolder)
            .FirstOrDefaultAsync(predicate);
        }
    }
}


