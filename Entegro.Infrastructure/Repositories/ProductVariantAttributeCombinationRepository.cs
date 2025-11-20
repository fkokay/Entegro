using Entegro.Application.DTOs.Common;
using Entegro.Application.Interfaces.Repositories;
using Entegro.Domain.Entities.Catalog;
using Entegro.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Entegro.Infrastructure.Repositories
{
    public class ProductVariantAttributeCombinationRepository : IProductVariantAttributeCombinationRepository
    {
        private readonly EntegroDbContext _context;

        public ProductVariantAttributeCombinationRepository(EntegroDbContext context)
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

        public async Task DeleteByProductIdAsync(int productId)
        {
            var combinations = await _context.ProductVariantAttributeCombinations
                                            .Where(x => x.ProductId == productId)
                                            .ToListAsync();

            if (combinations.Count == 0)
                return;

            _context.ProductVariantAttributeCombinations.RemoveRange(combinations);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> ExistsAsync(int productId, string gtin)
        {
            return await _context.ProductVariantAttributeCombinations.AnyAsync(o => o.ProductId == productId && o.Gtin == gtin);
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
                .OrderBy(b => b.Id)
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
            return await _context.ProductVariantAttributeCombinations.AsNoTracking().FirstOrDefaultAsync(o => o.Id == id);
        }

        public async Task<List<ProductVariantAttributeCombination>> GetByProductIdAsync(int productId)
        {
            return await _context.ProductVariantAttributeCombinations.Where(o => o.ProductId == productId).ToListAsync();
        }

        public async Task UpdateAsync(ProductVariantAttributeCombination productVariantAttributeCombination)
        {
            _context.ProductVariantAttributeCombinations.Update(productVariantAttributeCombination);
            await _context.SaveChangesAsync();
        }
    }
}
