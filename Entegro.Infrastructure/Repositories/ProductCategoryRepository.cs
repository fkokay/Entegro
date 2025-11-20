using Entegro.Application.DTOs.Common;
using Entegro.Application.Interfaces.Repositories;
using Entegro.Domain.Entities.Catalog;
using Entegro.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq.Dynamic.Core;
namespace Entegro.Infrastructure.Repositories
{
    public class ProductCategoryRepository : IProductCategoryRepository
    {
        private readonly EntegroDbContext _context;

        public ProductCategoryRepository(EntegroDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(ProductCategory productCategoryMapping)
        {
            var mapping = new ProductCategory
            {
                ProductId = productCategoryMapping.ProductId,
                CategoryId = productCategoryMapping.CategoryId,
                DisplayOrder = productCategoryMapping.DisplayOrder
            };
            await _context.ProductCategories.AddAsync(mapping);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(ProductCategory productCategoryMapping)
        {
            _context.ProductCategories.Remove(productCategoryMapping);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteByProductIdAsync(int productId)
        {
            var categories = await _context.ProductCategories
                                           .Where(x => x.ProductId == productId)
                                           .ToListAsync();
            if (categories == null || categories.Count == 0)
                return;
            _context.ProductCategories.RemoveRange(categories);
            await _context.SaveChangesAsync();
        }

        public async Task<List<ProductCategory>> GetAllAsync()
        {
            return await _context.ProductCategories.Include(m => m.Product).Include(m => m.Category).AsNoTracking().ToListAsync();
        }

        public async Task<ProductCategory?> GetByIdAsync(int id)
        {
            return await _context.ProductCategories.AsNoTracking().FirstOrDefaultAsync(o => o.Id == id);
        }



        public async Task<List<ProductCategory>> GetByProductsWithCategoryAsync(IEnumerable<int> productIds)
        {
            var ids = productIds.Distinct().ToArray();

            return await _context.ProductCategories
                .AsNoTracking()
                .Where(m => ids.Contains(m.ProductId))
                .Include(m => m.Category)
                .OrderBy(m => m.ProductId)
                .ThenBy(m => m.DisplayOrder).ThenBy(m => m.CategoryId)
                .ToListAsync();
        }

        public async Task<List<ProductCategory>> GetByProductWithCategoryAsync(int productId)
        {
            return await _context.ProductCategories
             .AsNoTracking()
             .Where(m => m.ProductId == productId)
             .Include(m => m.Category)
             .OrderBy(m => m.DisplayOrder).ThenBy(m => m.CategoryId)
             .ToListAsync();
        }

        public async Task<Application.DTOs.Common.PagedResult<ProductCategory>> GetPagedAsync(GridCommand gridCommand, int productId)
        {

            var query = _context.ProductCategories
             .AsNoTracking()
             .Where(m => m.ProductId == productId)
             .Include(m => m.Category)
             .OrderBy(m => m.DisplayOrder).ThenBy(m => m.CategoryId).AsNoTracking().AsQueryable();

            if (gridCommand.Search != null)
            {
                if (!string.IsNullOrEmpty(gridCommand.Search.Value))
                {
                    query = query.Where(b => b.Category.Name.Contains(gridCommand.Search.Value)).AsQueryable();
                }
            }

            if (gridCommand.Order.Any())
            {
                foreach (var item in gridCommand.Order)
                {
                    query = query.OrderBy($"{gridCommand.Columns[item.Column].Data} {(item.Dir ?? "asc")}");
                }
            }
            else
            {
                query = query.OrderBy(b => b.Id);
            }

            var totalCount = await query.CountAsync();
            var productCategories = await query
            .Skip(gridCommand.Start)
            .Take(gridCommand.Length)
            .ToListAsync();

            return new Application.DTOs.Common.PagedResult<ProductCategory>
            {
                Items = productCategories,
                TotalCount = totalCount,
                PageNumber = gridCommand.Start + 1,
                PageSize = gridCommand.Length
            };
        }

        public async Task UpdateAsync(ProductCategory productCategoryMapping)
        {
            _context.ProductCategories.Update(productCategoryMapping);
            await _context.SaveChangesAsync();
        }
    }
}
