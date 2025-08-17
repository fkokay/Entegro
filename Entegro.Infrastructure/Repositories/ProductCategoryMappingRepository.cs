using Entegro.Application.Interfaces.Repositories;
using Entegro.Domain.Entities;
using Entegro.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Entegro.Infrastructure.Repositories
{
    public class ProductCategoryMappingRepository : IProductCategoryMappingRepository
    {
        private readonly EntegroContext _context;

        public ProductCategoryMappingRepository(EntegroContext context)
        {
            _context = context;
        }

        public async Task AddAsync(ProductCategoryMapping productCategoryMapping)
        {
            await _context.ProductCategories.AddAsync(productCategoryMapping);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(ProductCategoryMapping productCategoryMapping)
        {
            _context.ProductCategories.Remove(productCategoryMapping);
            await _context.SaveChangesAsync();
        }

        public async Task<List<ProductCategoryMapping>> GetAllAsync()
        {
            return await _context.ProductCategories.Include(m => m.Product).Include(m => m.Category).ToListAsync();
        }

        public async Task<ProductCategoryMapping?> GetByIdAsync(int id)
        {
            return await _context.ProductCategories.FirstOrDefaultAsync(o => o.Id == id);
        }

        public async Task<List<ProductCategoryMapping>> GetByProductsWithCategoryAsync(IEnumerable<int> productIds, CancellationToken ct = default)
        {
            var ids = productIds.Distinct().ToArray();

            return await _context.ProductCategories
                .AsNoTracking()
                .Where(m => ids.Contains(m.ProductId))
                .Include(m => m.Category)
                .OrderBy(m => m.ProductId)
                .ThenBy(m => m.DisplayOrder).ThenBy(m => m.CategoryId)
                .ToListAsync(ct);
        }

        public async Task<List<ProductCategoryMapping>> GetByProductWithCategoryAsync(int productId, CancellationToken ct = default)
        {
            return await _context.ProductCategories
             .AsNoTracking()
             .Where(m => m.ProductId == productId)
             .Include(m => m.Category)
             .OrderBy(m => m.DisplayOrder).ThenBy(m => m.CategoryId)
             .ToListAsync(ct);
        }

        public async Task UpdateAsync(ProductCategoryMapping productCategoryMapping)
        {
            _context.ProductCategories.Update(productCategoryMapping);
            await _context.SaveChangesAsync();
        }
    }
}
