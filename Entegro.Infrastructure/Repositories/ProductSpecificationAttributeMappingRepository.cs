using Entegro.Application.Interfaces.Repositories;
using Entegro.Domain.Entities.Catalog;
using Entegro.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Entegro.Infrastructure.Repositories
{
    public class ProductSpecificationAttributeMappingRepository : IProductSpecificationAttributeMappingRepository
    {
        private readonly EntegroDbContext _context;

        public ProductSpecificationAttributeMappingRepository(EntegroDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(ProductSpecificationAttribute productSpecificationAttribute)
        {
            var mapping = new ProductSpecificationAttribute
            {
                ProductId = productSpecificationAttribute.ProductId,
                SpecificationAttributeOptionId = productSpecificationAttribute.SpecificationAttributeOptionId,
                DisplayOrder = productSpecificationAttribute.DisplayOrder
            };
            await _context.ProductSpecificationAttributes.AddAsync(mapping);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(ProductSpecificationAttribute productSpecificationAttribute)
        {
            _context.ProductSpecificationAttributes.Remove(productSpecificationAttribute);
            await _context.SaveChangesAsync();
        }

        public async Task<List<ProductSpecificationAttribute>> GetAllAsync()
        {
            return await _context.ProductSpecificationAttributes.Include(m => m.Product).Include(m => m.SpecificationAttributeOption).AsNoTracking().ToListAsync();
        }

        public async Task<ProductSpecificationAttribute?> GetByIdAsync(int id)
        {
            return await _context.ProductSpecificationAttributes.Include(m => m.Product).Include(m => m.SpecificationAttributeOption).AsNoTracking().FirstOrDefaultAsync(o => o.Id == id);
        }

        public async Task<List<ProductSpecificationAttribute>> GetSpecificationAttributeByProductId(int productId)
        {
            return await _context.ProductSpecificationAttributes.Include(m => m.Product).Include(m => m.SpecificationAttributeOption).AsNoTracking().Where(x => x.ProductId == productId).ToListAsync();
        }

        public async Task UpdateAsync(ProductSpecificationAttribute productSpecificationAttribute)
        {
            _context.ProductSpecificationAttributes.Update(productSpecificationAttribute);
            await _context.SaveChangesAsync();
        }
    }
}
