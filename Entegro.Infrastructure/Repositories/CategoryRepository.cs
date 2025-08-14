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

        public async Task UpdateAsync(Category category)
        {
            _context.Categories.Update(category);
            await _context.SaveChangesAsync();
        }
    }
}
