using Entegro.Application.DTOs.Common;
using Entegro.Application.Interfaces.Repositories;
using Entegro.Domain.Entities.Catalog;
using Entegro.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq.Dynamic.Core;

namespace Entegro.Infrastructure.Repositories
{
    public class ProductAttributeValueRepository : IProductAttributeValueRepository
    {
        private readonly EntegroDbContext _context;

        public ProductAttributeValueRepository(EntegroDbContext context)
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

        public async Task UpdateAsync(ProductAttributeValue productAttributeValue)
        {
            _context.ProductAttributeValues.Update(productAttributeValue);
            await _context.SaveChangesAsync();
        }

        public async Task<List<ProductAttributeValue>> GetAllAsync()
        {
            return await IncludeAllProperties(_context.ProductAttributeValues.AsNoTracking()).OrderBy(p => p.Id).ToListAsync();
        }

        public async Task<Application.DTOs.Common.PagedResult<ProductAttributeValue>> GetAllAsync(int pageNumber, int pageSize)
        {
            var query = IncludeAllProperties(_context.ProductAttributeValues.AsNoTracking()).OrderBy(p => p.Id);


            var totalCount = await query.CountAsync();
            var productAttributeValues = await query
                .Skip(pageNumber * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new Application.DTOs.Common.PagedResult<ProductAttributeValue>
            {
                Items = productAttributeValues,
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }

        public async Task<ProductAttributeValue?> GetByIdAsync(int id)
        {
            return await IncludeAllProperties(_context.ProductAttributeValues.AsNoTracking())
                    .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<ProductAttributeValue?> GetByNameAsync(string name)
        {
            return await IncludeAllProperties(_context.ProductAttributeValues.AsNoTracking())
              .FirstOrDefaultAsync(p => p.Name == name);
        }

        public async Task<ProductAttributeValue?> GetByNameOrAttributeIdAsync(string name, int attributeId)
        {
            return await IncludeAllProperties(_context.ProductAttributeValues.AsNoTracking())
              .FirstOrDefaultAsync(p => p.Name == name && p.ProductAttributeId == attributeId);
        }

        private IQueryable<ProductAttributeValue> IncludeAllProperties(IQueryable<ProductAttributeValue> query)
        {
            return query.Include(p => p.ProductAttribute).AsSplitQuery();
        }

        public async Task<Application.DTOs.Common.PagedResult<ProductAttributeValue>> GetPagedAsync(GridCommand gridCommand)
        {
            var query = IncludeAllProperties(_context.ProductAttributeValues.AsNoTracking());



            if (gridCommand.Columns != null)
            {
                foreach (var col in gridCommand.Columns)
                {
                    if (!string.IsNullOrEmpty(col.Search?.Value))
                    {
                        var searchVal = col.Search.Value.Trim('^', '$');

                        // ilgili property’nin tipini bul
                        var prop = typeof(ProductAttributeValue).GetProperty(col.Data);
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

            var attributeValues = await query
                .Skip(gridCommand.Start)
                .Take(gridCommand.Length)
                .AsNoTracking()
                .AsSplitQuery()
                .ToListAsync();

            return new Application.DTOs.Common.PagedResult<ProductAttributeValue>
            {
                Items = attributeValues,
                TotalCount = totalCount,
                PageNumber = gridCommand.Start / gridCommand.Length,
                PageSize = gridCommand.Length
            };
        }
    }
}
