using Entegro.Application.DTOs.Common;
using Entegro.Application.Interfaces.Repositories;
using Entegro.Domain.Entities.Catalog;
using Entegro.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Entegro.Infrastructure.Repositories
{
    public class ProductAttributeValueRepository : IProductAttributeValueRepository
    {
        private readonly EntegroContext _context;

        public ProductAttributeValueRepository(EntegroContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }
        public async Task AddAsync(ProductAttributeValue productAttributeValue)
        {
            await _context.ProductAttributeValues.AddAsync(productAttributeValue);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(ProductAttributeValue productAttributeValue)
        {
            _context.ProductAttributeValues.Remove(productAttributeValue);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(ProductAttributeValue productAttributeValue)
        {
            _context.ProductAttributeValues.Update(productAttributeValue);
            await _context.SaveChangesAsync();
        }

        public async Task<List<ProductAttributeValue>> GetAllAsync()
        {
            return await IncludeAllProperties(_context.ProductAttributeValues.AsNoTracking()).OrderBy(p => p.Id).ToListAsync();
        }

        public async Task<PagedResult<ProductAttributeValue>> GetAllAsync(int pageNumber, int pageSize)
        {
            var query = IncludeAllProperties(_context.ProductAttributeValues.AsNoTracking()).OrderBy(p => p.Id);


            var totalCount = await query.CountAsync();
            var productAttributeValues = await query
                .Skip(pageNumber * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResult<ProductAttributeValue>
            {
                Items = productAttributeValues,
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }

        public async Task<ProductAttributeValue?> GetByIdAsync(int id)
        {
            return await IncludeAllProperties(_context.ProductAttributeValues.AsNoTracking())
                    .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<ProductAttributeValue?> GetByNameAsync(string name)
        {
            return await IncludeAllProperties(_context.ProductAttributeValues.AsNoTracking())
              .FirstOrDefaultAsync(p => p.Name == name);
        }

        public async Task<ProductAttributeValue?> GetByNameOrAttributeIdAsync(string name, int attributeId)
        {
            return await IncludeAllProperties(_context.ProductAttributeValues.AsNoTracking())
              .FirstOrDefaultAsync(p => p.Name == name && p.ProductAttributeId == attributeId);
        }

        private IQueryable<ProductAttributeValue> IncludeAllProperties(IQueryable<ProductAttributeValue> query)
        {
            return query.Include(p => p.ProductAttribute).AsSplitQuery();
        }
    }
}
