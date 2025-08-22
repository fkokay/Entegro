using Entegro.Application.DTOs.Common;
using Entegro.Application.Interfaces.Repositories;
using Entegro.Domain.Entities;
using Entegro.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Entegro.Infrastructure.Repositories
{
    public class ProductVariantAttributeCombinationRepository : IProductVariantAttributeCombinationRepository
    {
        private readonly EntegroContext _context;

        public ProductVariantAttributeCombinationRepository(EntegroContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }
        public async Task AddAsync(ProductVariantAttributeCombination productVariantAttributeCombination)
        {
            await _context.ProductVariantAttributeCombinations.AddAsync(productVariantAttributeCombination);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(ProductVariantAttributeCombination productVariantAttributeCombination)
        {
            _context.ProductVariantAttributeCombinations.Remove(productVariantAttributeCombination);
            await _context.SaveChangesAsync();
        }

        public async Task<List<ProductVariantAttributeCombination>> GetAllAsync()
        {
            return await _context.ProductVariantAttributeCombinations.ToListAsync();
        }

        public async Task<PagedResult<ProductVariantAttributeCombination>> GetAllAsync(int pageNumber, int pageSize)
        {
            var query = _context.ProductVariantAttributeCombinations.AsQueryable();

            var totalCount = await query.CountAsync();
            var customers = await query
                .Skip(pageNumber * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResult<ProductVariantAttributeCombination>
            {
                Items = customers,
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }

        public async Task<ProductVariantAttributeCombination?> GetByIdAsync(int id)
        {
            return await _context.ProductVariantAttributeCombinations.FirstOrDefaultAsync(o => o.Id == id);
        }

        public async Task UpdateAsync(ProductVariantAttributeCombination productVariantAttributeCombination)
        {
            _context.ProductVariantAttributeCombinations.Update(productVariantAttributeCombination);
            await _context.SaveChangesAsync();
        }
    }
}
