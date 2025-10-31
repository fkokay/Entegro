using Entegro.Application.DTOs.Common;
using Entegro.Application.Interfaces.Repositories;
using Entegro.Domain.Entities.Catalog;
using Entegro.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq.Dynamic.Core;

namespace Entegro.Infrastructure.Repositories
{
    public class ProductVariantAttributeRepository : IProductVariantAttributeRepository
    {
        private readonly EntegroDbContext _context;

        public ProductVariantAttributeRepository(EntegroDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }
        public async Task AddAsync(ProductVariantAttribute productAttributeMapping)
        {
            await _context.ProductVariantAttributes.AddAsync(productAttributeMapping);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(ProductVariantAttribute productAttributeMapping)
        {
            _context.ProductVariantAttributes.Remove(productAttributeMapping);
            await _context.SaveChangesAsync();
        }

        public async Task<List<ProductVariantAttribute>> GetAllAsync()
        {
            return await _context.ProductVariantAttributes.ToListAsync();
        }

        public async Task<List<ProductVariantAttribute>> GetAllAsync(int productId)
        {
            return await _context.ProductVariantAttributes.Where(m => m.ProductId == productId).ToListAsync();
        }

        public async Task<Application.DTOs.Common.PagedResult<ProductVariantAttribute>> GetAllAsync(int pageNumber, int pageSize)
        {
            var query = _context.ProductVariantAttributes.AsQueryable();

            var totalCount = await query.CountAsync();
            var customers = await query
                .OrderBy(b => b.Id)
                .Skip(pageNumber * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new Application.DTOs.Common.PagedResult<ProductVariantAttribute>
            {
                Items = customers,
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }

        public async Task<ProductVariantAttribute?> GetByAttributeIdAsync(int id)
        {
            return await _context.ProductVariantAttributes.FirstOrDefaultAsync(o => o.ProductAttributeId == id);
        }

        public async Task<ProductVariantAttribute?> GetByAttributeIdAsync(int productId, int attributeId)
        {
            return await _context.ProductVariantAttributes.FirstOrDefaultAsync(o => o.ProductId == productId && o.ProductAttributeId == attributeId);
        }

        public async Task<ProductVariantAttribute?> GetByIdAsync(int id)
        {
            return await _context.ProductVariantAttributes.Include(p => p.ProductAttribute).FirstOrDefaultAsync(o => o.Id == id);
        }

        public async Task<Application.DTOs.Common.PagedResult<ProductVariantAttribute>> GetPagedAsync(GridCommand gridCommand, int productId)
        {
            var query = _context.ProductVariantAttributes.Include(p => p.Product).Include(p => p.ProductAttribute).Include(p => p.ProductVariantAttributeValues).Where(x => x.ProductId == productId).OrderBy(b => b.Id).AsNoTracking();


            if (gridCommand.Columns != null)
            {
                foreach (var col in gridCommand.Columns)
                {
                    if (!string.IsNullOrEmpty(col.Search?.Value))
                    {
                        var searchVal = col.Search.Value.Trim('^', '$');

                        // ilgili property’nin tipini bul
                        var prop = typeof(ProductVariantAttribute).GetProperty(col.Data);
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
                    b.ProductAttribute.Name.Contains(gridCommand.Search.Value)).AsQueryable();
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
            var brands = await query
                .Skip(gridCommand.Start)
                .Take(gridCommand.Length)
                .AsNoTracking()
                .AsSplitQuery()
                .ToListAsync();

            return new Application.DTOs.Common.PagedResult<ProductVariantAttribute>
            {
                Items = brands,
                TotalCount = totalCount,
                PageNumber = gridCommand.Start / gridCommand.Length,
                PageSize = gridCommand.Length
            };
        }

        public async Task UpdateAsync(ProductVariantAttribute productAttributeMapping)
        {
            _context.ProductVariantAttributes.Update(productAttributeMapping);
            await _context.SaveChangesAsync();
        }
    }
}
