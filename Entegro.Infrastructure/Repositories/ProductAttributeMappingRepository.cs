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
        public async Task AddAsync(ProductAttributeMapping productAttributeMapping)
        {
            await _context.ProductAttributeMappings.AddAsync(productAttributeMapping);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(ProductAttributeMapping productAttributeMapping)
        {
            _context.ProductAttributeMappings.Remove(productAttributeMapping);
            await _context.SaveChangesAsync();
        }

        public async Task<List<ProductAttributeMapping>> GetAllAsync()
        {
            return await _context.ProductAttributeMappings.ToListAsync();
        }

        public async Task<PagedResult<ProductAttributeMapping>> GetAllAsync(int pageNumber, int pageSize)
        {
            var query = _context.ProductAttributeMappings.AsQueryable();

            var totalCount = await query.CountAsync();
            var customers = await query
                .Skip(pageNumber * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResult<ProductAttributeMapping>
            {
                Items = customers,
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }

        public async Task<ProductAttributeMapping?> GetByAttributeIdAsync(int id)
        {
            return await _context.ProductAttributeMappings.FirstOrDefaultAsync(o => o.ProductAttributeId == id);
        }

        public async Task<ProductAttributeMapping?> GetByIdAsync(int id)
        {
            return await _context.ProductAttributeMappings.FirstOrDefaultAsync(o => o.Id == id);
        }

        public async Task UpdateAsync(ProductAttributeMapping productAttributeMapping)
        {
            _context.ProductAttributeMappings.Update(productAttributeMapping);
            await _context.SaveChangesAsync();
        }
    }
}
