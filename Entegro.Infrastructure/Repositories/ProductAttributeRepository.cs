using Entegro.Application.DTOs.Common;
using Entegro.Application.Interfaces.Repositories;
using Entegro.Domain.Entities.Catalog;
using Entegro.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq.Dynamic.Core;

namespace Entegro.Infrastructure.Repositories
{
    public class ProductAttributeRepository : IProductAttributeRepository
    {
        private readonly EntegroDbContext _context;

        public ProductAttributeRepository(EntegroDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }
        public async Task AddAsync(ProductAttribute productAttribute)
        {
            await _context.ProductAttributes.AddAsync(productAttribute);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(ProductAttribute productAttribute)
        {
            _context.ProductAttributes.Remove(productAttribute);
            await _context.SaveChangesAsync();
        }

        public async Task<List<ProductAttribute>> GetAllAsync()
        {
            return await _context.ProductAttributes.AsNoTracking().ToListAsync();
        }

        public async Task<Application.DTOs.Common.PagedResult<ProductAttribute>> GetAllAsync(int pageNumber, int pageSize)
        {
            var query = _context.ProductAttributes.AsNoTracking().AsQueryable();

            var totalCount = await query.CountAsync();
            var customers = await query
                .OrderBy(b => b.Id)
                .Skip(pageNumber * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new Application.DTOs.Common.PagedResult<ProductAttribute>
            {
                Items = customers,
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }

        public async Task<Application.DTOs.Common.PagedResult<ProductAttribute>> GetAllAsync(int page, string term)
        {
            var query = _context.ProductAttributes.AsNoTracking();
            if (!string.IsNullOrEmpty(term))
            {
                query = query.Where(b => b.Name.Contains(term)).AsQueryable();
            }

            var totalCount = await query.CountAsync();
            var items = await query.Skip((page * 7) - 7)
                .Take(7).ToListAsync();

            return new Application.DTOs.Common.PagedResult<ProductAttribute>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = page,
                PageSize = 7
            };
        }

        public async Task<ProductAttribute?> GetByIdAsync(int id)
        {
            return await _context.ProductAttributes.AsNoTracking().FirstOrDefaultAsync(o => o.Id == id);
        }

        public async Task<ProductAttribute?> GetByNameAsync(string name)
        {
            return await _context.ProductAttributes.AsNoTracking().FirstOrDefaultAsync(o => o.Name == name);
        }

        public async Task<Application.DTOs.Common.PagedResult<ProductAttribute>> GetPagedAsync(GridCommand gridCommand)
        {
            var query = _context.ProductAttributes.OrderBy(b => b.Id).AsNoTracking();

            if (gridCommand.Columns != null)
            {
                foreach (var col in gridCommand.Columns)
                {
                    if (!string.IsNullOrEmpty(col.Search?.Value))
                    {
                        var searchVal = col.Search.Value.Trim('^', '$');

                        // ilgili property’nin tipini bul
                        var prop = typeof(ProductAttribute).GetProperty(col.Data);
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
                    b.Name.Contains(gridCommand.Search.Value)).AsQueryable();
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

            var attributes = await query
                .Skip(gridCommand.Start)
                .Take(gridCommand.Length)
                .AsNoTracking()
                .AsSplitQuery()
                .ToListAsync();

            return new Application.DTOs.Common.PagedResult<ProductAttribute>
            {
                Items = attributes,
                TotalCount = totalCount,
                PageNumber = gridCommand.Start / gridCommand.Length,
                PageSize = gridCommand.Length
            };
        }

        public async Task UpdateAsync(ProductAttribute productAttribute)
        {
            _context.ProductAttributes.Update(productAttribute);
            await _context.SaveChangesAsync();
        }
    }
}
