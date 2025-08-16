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

        public async Task UpdateAsync(ProductCategoryMapping productCategoryMapping)
        {
            _context.ProductCategories.Update(productCategoryMapping);
            await _context.SaveChangesAsync();
        }
    }
}
