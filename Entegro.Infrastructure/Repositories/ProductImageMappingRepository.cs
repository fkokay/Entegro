using Entegro.Application.DTOs.Common;
using Entegro.Application.Interfaces.Repositories;
using Entegro.Domain.Entities;
using Entegro.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Entegro.Infrastructure.Repositories
{
    public class ProductImageMappingRepository : IProductImageMappingRepository
    {
        private readonly EntegroContext _context;
        public ProductImageMappingRepository(EntegroContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task AddAsync(ProductImageMapping productImage)
        {
            await _context.ProductImages.AddAsync(productImage);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(ProductImageMapping productImage)
        {
            _context.ProductImages.Remove(productImage);
            await _context.SaveChangesAsync();
        }

        public async Task<List<ProductImageMapping>> GetAllAsync()
        {
            return await _context.ProductImages.ToListAsync();
        }

        public async Task<PagedResult<ProductImageMapping>> GetAllAsync(int pageNumber, int pageSize)
        {
            var query = _context.ProductImages.AsQueryable();

            var totalCount = await query.CountAsync();
            var categories = await query
                .Skip(pageNumber * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResult<ProductImageMapping>
            {
                Items = categories,
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }

        public async Task<ProductImageMapping?> GetByIdAsync(int id)
        {
            return await _context.ProductImages.FirstOrDefaultAsync(o => o.Id == id);
        }

        public async Task UpdateAsync(ProductImageMapping productImage)
        {
            _context.ProductImages.Update(productImage);
            await _context.SaveChangesAsync();
        }
    }
}


