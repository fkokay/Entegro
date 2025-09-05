using Entegro.Application.DTOs.Common;
using Entegro.Application.Interfaces.Repositories;
using Entegro.Domain.Entities;
using Entegro.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Entegro.Infrastructure.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly EntegroContext _context;

        public ProductRepository(EntegroContext context)
        {
            _context = context;
        }
        public async Task AddAsync(Product product)
        {
            product.CreatedOnUtc = DateTime.UtcNow;
            product.UpdatedOnUtc = DateTime.UtcNow;

            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Product product)
        {
            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Product product)
        {
            product.UpdatedOnUtc = DateTime.UtcNow;
            _context.Products.Update(product);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> ExistsAsync(Expression<Func<Product, bool>> predicate)
        {
            return await _context.Products.AsNoTracking().AnyAsync(predicate);
        }

        public async Task<Product?> GetByAsync(Expression<Func<Product, bool>> predicate)
        {
            var product = await IncludeAllProperties(_context.Products.AsNoTracking())
            .FirstOrDefaultAsync(predicate);

            return product;
        }

        public async Task<List<Product>> GetAllAsync()
        {
            var query = IncludeAllProperties(_context.Products.AsNoTracking());

            var products = await query.ToListAsync();

            return products;
        }

        public async Task<PagedResult<Product>> GetAllAsync(int pageNumber, int pageSize)
        {
            var query = IncludeAllProperties(_context.Products.AsNoTracking());

            var totalCount = await query.CountAsync();

            var products = await query
                .OrderBy(p => p.Id)
                .Skip(pageNumber * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResult<Product>
            {
                Items = products,
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }

        public async Task UpdateMainPictureIdAsync(int productId, int mainPictureId)
        {
            var product = new Product { Id = productId, MainPictureId = mainPictureId, UpdatedOnUtc = DateTime.UtcNow };
            _context.Products.Attach(product);
            _context.Entry(product).Property(p => p.MainPictureId).IsModified = true;
            _context.Entry(product).Property(p => p.UpdatedOnUtc).IsModified = true;
            await _context.SaveChangesAsync();
        }

        private IQueryable<Product> IncludeAllProperties(IQueryable<Product> query)
        {
            return query
                .Include(x => x.Brand)
                .Include(x => x.ProductMediaFiles).ThenInclude(pm => pm.MediaFile)
                .Include(x => x.ProductCategories).ThenInclude(pc => pc.Category)
                .Include(x => x.ProductIntegrations).ThenInclude(pi => pi.IntegrationSystem).ThenInclude(isys => isys.IntegrationSystemParameters);
        }
    }
}
