using Entegro.Application.DTOs.Common;
using Entegro.Application.Interfaces.Repositories;
using Entegro.Domain.Entities.Catalog;
using Entegro.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Entegro.Infrastructure.Repositories
{
    public class ProductVariantAttributeRepository : IProductVariantAttributeRepository
    {
        private readonly EntegroContext _context;

        public ProductVariantAttributeRepository(EntegroContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }
        public async Task AddAsync(ProductVariantAttribute productAttributeMapping)
        {
            await _context.ProductVariantAttributes.AddAsync(productAttributeMapping);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(ProductVariantAttribute productAttributeMapping)
        {
            _context.ProductVariantAttributes.Remove(productAttributeMapping);
            await _context.SaveChangesAsync();
        }

        public async Task<List<ProductVariantAttribute>> GetAllAsync()
        {
            return await _context.ProductVariantAttributes.ToListAsync();
        }

        public async Task<PagedResult<ProductVariantAttribute>> GetAllAsync(int pageNumber, int pageSize)
        {
            var query = _context.ProductVariantAttributes.AsQueryable();

            var totalCount = await query.CountAsync();
            var customers = await query
                .OrderBy(b => b.Id)
                .Skip(pageNumber * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResult<ProductVariantAttribute>
            {
                Items = customers,
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }

        public async Task<ProductVariantAttribute?> GetByAttributeIdAsync(int id)
        {
            return await _context.ProductVariantAttributes.FirstOrDefaultAsync(o => o.ProductAttributeId == id);
        }

        public async Task<ProductVariantAttribute?> GetByAttributeIdAsync(int productId, int attributeId)
        {
            return await _context.ProductVariantAttributes.FirstOrDefaultAsync(o => o.ProductId == productId && o.ProductAttributeId == attributeId);
        }

        public async Task<ProductVariantAttribute?> GetByIdAsync(int id)
        {
            return await _context.ProductVariantAttributes.FirstOrDefaultAsync(o => o.Id == id);
        }

        public async Task UpdateAsync(ProductVariantAttribute productAttributeMapping)
        {
            _context.ProductVariantAttributes.Update(productAttributeMapping);
            await _context.SaveChangesAsync();
        }
    }
}
