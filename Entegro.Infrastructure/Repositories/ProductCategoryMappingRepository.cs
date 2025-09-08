using Entegro.Application.Interfaces.Repositories;
using Entegro.Domain.Entities.Catalog;
using Entegro.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Entegro.Infrastructure.Repositories
{
    public class ProductCategoryMappingRepository : IProductCategoryMappingRepository
    {
        private readonly EntegroDbContext _context;

        public ProductCategoryMappingRepository(EntegroDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(ProductCategory productCategoryMapping)
        {
            var mapping = new ProductCategory
            {
                ProductId = productCategoryMapping.ProductId,
                CategoryId = productCategoryMapping.CategoryId,
                DisplayOrder = productCategoryMapping.DisplayOrder
            };
            await _context.ProductCategories.AddAsync(mapping);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(ProductCategory productCategoryMapping)
        {
            _context.ProductCategories.Remove(productCategoryMapping);
            await _context.SaveChangesAsync();
        }

        public async Task<List<ProductCategory>> GetAllAsync()
        {
            return await _context.ProductCategories.Include(m => m.Product).Include(m => m.Category).AsNoTracking().ToListAsync();
        }

        public async Task<ProductCategory?> GetByIdAsync(int id)
        {
            return await _context.ProductCategories.AsNoTracking().FirstOrDefaultAsync(o => o.Id == id);
        }



        public async Task<List<ProductCategory>> GetByProductsWithCategoryAsync(IEnumerable<int> productIds)
        {
            var ids = productIds.Distinct().ToArray();

            return await _context.ProductCategories
                .AsNoTracking()
                .Where(m => ids.Contains(m.ProductId))
                .Include(m => m.Category)
                .OrderBy(m => m.ProductId)
                .ThenBy(m => m.DisplayOrder).ThenBy(m => m.CategoryId)
                .ToListAsync();
        }

        public async Task<List<ProductCategory>> GetByProductWithCategoryAsync(int productId)
        {
            return await _context.ProductCategories
             .AsNoTracking()
             .Where(m => m.ProductId == productId)
             .Include(m => m.Category)
             .OrderBy(m => m.DisplayOrder).ThenBy(m => m.CategoryId)
             .ToListAsync();
        }

        public async Task UpdateAsync(ProductCategory productCategoryMapping)
        {
            _context.ProductCategories.Update(productCategoryMapping);
            await _context.SaveChangesAsync();
        }
    }
}
