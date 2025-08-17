using Entegro.Application.DTOs.Common;
using Entegro.Application.Interfaces.Repositories;
using Entegro.Domain.Entities;
using Entegro.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Entegro.Infrastructure.Repositories
{
    public class ProductAttributeRepository : IProductAttributeRepository
    {
        private readonly EntegroContext _context;

        public ProductAttributeRepository(EntegroContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }
        public async Task AddAsync(ProductAttribute productAttribute)
        {
            await _context.ProductAttributes.AddAsync(productAttribute);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(ProductAttribute productAttribute)
        {
            _context.ProductAttributes.Remove(productAttribute);
            await _context.SaveChangesAsync();
        }

        public async Task<List<ProductAttribute>> GetAllAsync()
        {
            return await _context.ProductAttributes.ToListAsync();
        }

        public async Task<PagedResult<ProductAttribute>> GetAllAsync(int pageNumber, int pageSize)
        {
            var query = _context.ProductAttributes.AsQueryable();

            var totalCount = await query.CountAsync();
            var customers = await query
                .Skip(pageNumber * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResult<ProductAttribute>
            {
                Items = customers,
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }

        public async Task<ProductAttribute?> GetByIdAsync(int id)
        {
            return await _context.ProductAttributes.FirstOrDefaultAsync(o => o.Id == id);
        }

        public async Task UpdateAsync(ProductAttribute productAttribute)
        {
            _context.ProductAttributes.Update(productAttribute);
            await _context.SaveChangesAsync();
        }
    }
}
