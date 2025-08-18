using Entegro.Application.DTOs.Common;
using Entegro.Application.Interfaces.Repositories;
using Entegro.Domain.Entities;
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

        public async Task<List<ProductAttributeValue>> GetAllAsync()
        {
            return await _context.ProductAttributeValues.ToListAsync();
        }

        public async Task<PagedResult<ProductAttributeValue>> GetAllAsync(int pageNumber, int pageSize)
        {
            var query = _context.ProductAttributeValues
          .AsNoTracking()
          .Select(x => new ProductAttributeValue
          {
              Id = x.Id,
              ProductAttributeId = x.ProductAttributeId,
              Name = x.Name,
              DisplayOrder = x.DisplayOrder,
              ProductAttribute = new ProductAttribute
              {
                  Id = x.ProductAttribute.Id,
                  Name = x.ProductAttribute.Name,
                  Description = x.ProductAttribute.Description,
                  DisplayOrder = x.ProductAttribute.DisplayOrder
              }
          });

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
            return await _context.ProductAttributeValues.FirstOrDefaultAsync(o => o.Id == id);
        }

        public async Task UpdateAsync(ProductAttributeValue productAttributeValue)
        {
            _context.ProductAttributeValues.Update(productAttributeValue);
            await _context.SaveChangesAsync();
        }
    }
}
