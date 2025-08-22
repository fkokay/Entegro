using Entegro.Application.DTOs.Common;
using Entegro.Application.Interfaces.Repositories;
using Entegro.Domain.Entities;
using Entegro.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Entegro.Infrastructure.Repositories
{
    public class ProductAttributeMappingRepository : IProductAttributeMappingRepository
    {
        private readonly EntegroContext _context;

        public ProductAttributeMappingRepository(EntegroContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }
        public async Task AddAsync(ProductVariantAttribute productAttributeMapping)
        {
            await _context.ProductAttributeMappings.AddAsync(productAttributeMapping);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(ProductVariantAttribute productAttributeMapping)
        {
            _context.ProductAttributeMappings.Remove(productAttributeMapping);
            await _context.SaveChangesAsync();
        }

        public async Task<List<ProductVariantAttribute>> GetAllAsync()
        {
            return await _context.ProductAttributeMappings.ToListAsync();
        }

        public async Task<PagedResult<ProductVariantAttribute>> GetAllAsync(int pageNumber, int pageSize)
        {
            var query = _context.ProductAttributeMappings.AsQueryable();

            var totalCount = await query.CountAsync();
            var customers = await query
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
            return await _context.ProductAttributeMappings.FirstOrDefaultAsync(o => o.ProductAttributeId == id);
        }

        public async Task<ProductVariantAttribute?> GetByIdAsync(int id)
        {
            return await _context.ProductAttributeMappings.FirstOrDefaultAsync(o => o.Id == id);
        }

        public async Task UpdateAsync(ProductVariantAttribute productAttributeMapping)
        {
            _context.ProductAttributeMappings.Update(productAttributeMapping);
            await _context.SaveChangesAsync();
        }
    }
}
