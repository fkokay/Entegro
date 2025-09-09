using Entegro.Application.DTOs.Common;
using Entegro.Application.Interfaces.Repositories;
using Entegro.Domain.Entities.Catalog;
using Entegro.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq.Dynamic.Core;
using System.Linq.Expressions;

namespace Entegro.Infrastructure.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly EntegroDbContext _context;

        public ProductRepository(EntegroDbContext context)
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

            _context.Entry(product).State = EntityState.Modified;
            _context.Entry(product).Collection(p => p.ProductIntegrations).IsModified = false;
            _context.Entry(product).Collection(p => p.ProductVariantAttributeCombinations).IsModified = false;
            _context.Entry(product).Collection(p => p.ProductVariantAttributes).IsModified = false;
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
                .Include(x => x.Brand).AsNoTracking()
                .Include(x => x.ProductMediaFiles).ThenInclude(pm => pm.MediaFile).ThenInclude(x => x.Folder)
                .Include(x => x.ProductCategories).ThenInclude(pc => pc.Category)
                .Include(x => x.ProductVariantAttributes).ThenInclude(pi => pi.ProductVariantAttributeValues)
                .Include(x => x.ProductVariantAttributeCombinations)
                .Include(x => x.ProductIntegrations).ThenInclude(pi => pi.IntegrationSystem).ThenInclude(isys => isys.IntegrationSystemParameters).AsNoTracking();
        }

        public async Task<Application.DTOs.Common.PagedResult<Product>> GetPagedAsync(GridCommand gridCommand)
        {
            var query = IncludeAllProperties(_context.Products.AsNoTracking());

            if (gridCommand.Search != null)
            {
                if (!string.IsNullOrEmpty(gridCommand.Search.Value))
                {
                    query = query.Where(b =>
                    b.Name.Contains(gridCommand.Search.Value) ||
                    b.Code.Contains(gridCommand.Search.Value) ||
                    b.Barcode.Contains(gridCommand.Search.Value)).AsQueryable();
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

            var products = await query
                .Skip(gridCommand.Start)
                .Take(gridCommand.Length)
                .AsNoTracking()
                .ToListAsync();

            return new Application.DTOs.Common.PagedResult<Product>
            {
                Items = products,
                TotalCount = totalCount,
                PageNumber = gridCommand.Start / gridCommand.Length,
                PageSize = gridCommand.Length
            };
        }
    }
}
