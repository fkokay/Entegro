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
            _context.Entry(product).Collection(p => p.ProductMediaFiles).IsModified = false;
            _context.Entry(product).Collection(p => p.ProductCategories).IsModified = false;
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

        public async Task<Application.DTOs.Common.PagedResult<Product>> GetAllAsync(int page, string term)
        {
            var query = IncludeAllProperties(_context.Products.AsNoTracking());
            if (!string.IsNullOrEmpty(term))
            {
                query = query.Where(b =>
                b.Name.Contains(term) ||
                b.Code.Contains(term) ||
                b.Barcode.Contains(term)).AsQueryable();
            }

            var totalCount = await query.CountAsync();
            var products = await query.Skip((page * 7) - 7)
                .Take(7).ToListAsync();

            return new Application.DTOs.Common.PagedResult<Product>
            {
                Items = products,
                TotalCount = totalCount,
                PageNumber = page,
                PageSize = 7
            };
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

            if (gridCommand.Columns != null)
            {
                foreach (var col in gridCommand.Columns)
                {
                    if (!string.IsNullOrEmpty(col.Search?.Value))
                    {
                        var searchVal = col.Search.Value.Trim('^', '$');

                        // ilgili property’nin tipini bul
                        var prop = typeof(Product).GetProperty(col.Data);
                        if (prop == null) continue;

                        if (prop.PropertyType == typeof(string))
                        {
                            query = query.Where($"{col.Data}.Contains(@0)", searchVal);
                        }
                        else if (prop.PropertyType == typeof(int) || prop.PropertyType == typeof(int?))
                        {
                            if (int.TryParse(searchVal, out var intVal))
                                query = query.Where($"{col.Data} == @0", intVal);
                        }
                        else if (prop.PropertyType == typeof(bool) || prop.PropertyType == typeof(bool?))
                        {
                            if (bool.TryParse(searchVal, out var boolVal))
                                query = query.Where($"{col.Data} == @0", boolVal);
                        }
                        else if (prop.PropertyType == typeof(DateTime) || prop.PropertyType == typeof(DateTime?))
                        {
                            if (DateTime.TryParse(searchVal, out var dt))
                                query = query.Where($"{col.Data}.Date == @0", dt.Date);
                        }
                    }
                }
            }

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
                .AsSplitQuery()
                .ToListAsync();

            return new Application.DTOs.Common.PagedResult<Product>
            {
                Items = products,
                TotalCount = totalCount,
                PageNumber = gridCommand.Start / gridCommand.Length,
                PageSize = gridCommand.Length
            };
        }

        public async Task<int> GetProductCountAsync()
        {
            return await _context.Products.CountAsync(x => x.Published);
        }
    }
}
