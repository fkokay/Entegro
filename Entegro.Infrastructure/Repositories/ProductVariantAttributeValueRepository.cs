using Entegro.Application.DTOs.Common;
using Entegro.Application.Interfaces.Repositories;
using Entegro.Domain.Entities.Catalog;
using Entegro.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq.Dynamic.Core;

namespace Entegro.Infrastructure.Repositories
{
    public class ProductVariantAttributeValueRepository : IProductVariantAttributeValueRepository
    {
        private readonly EntegroDbContext _context;

        public ProductVariantAttributeValueRepository(EntegroDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }
        public async Task AddAsync(ProductVariantAttributeValue data)
        {
            await _context.ProductVariantAttributeValues.AddAsync(data);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(ProductVariantAttributeValue data)
        {
            _context.ProductVariantAttributeValues.Remove(data);
            await _context.SaveChangesAsync();
        }

        public async Task<ProductVariantAttributeValue?> GetByIdAsync(int id)
        {
            return await _context.ProductVariantAttributeValues.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<ProductVariantAttributeValue?> GetByNameAsync(int productVariantAttributeId, string name)
        {
            return await _context.ProductVariantAttributeValues.FirstOrDefaultAsync(o => o.ProductVariantAttributeId == productVariantAttributeId && o.Name == name);
        }

        public async Task<ProductVariantAttributeValue?> GetByProductVariantAttributeIdAsync(int productVariantAttributeId)
        {
            return await _context.ProductVariantAttributeValues.FirstOrDefaultAsync(x => x.ProductVariantAttributeId == productVariantAttributeId);
        }

        public async Task<Application.DTOs.Common.PagedResult<ProductVariantAttributeValue>> GetPagedAsync(GridCommand gridCommand, int productVariantAttributeId)
        {
            var query = _context.ProductVariantAttributeValues.Where(x => x.ProductVariantAttributeId == productVariantAttributeId).Include(m=>m.ProductVariantAttribute).AsNoTracking();


            if (gridCommand.Columns != null)
            {
                foreach (var col in gridCommand.Columns)
                {
                    if (!string.IsNullOrEmpty(col.Search?.Value))
                    {
                        var searchVal = col.Search.Value.Trim('^', '$');

                        // ilgili property’nin tipini bul
                        var prop = typeof(ProductVariantAttributeValue).GetProperty(col.Data);
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

            var values = await query
                .Skip(gridCommand.Start)
                .Take(gridCommand.Length)
                .AsNoTracking()
                .AsSplitQuery()
                .ToListAsync();

            return new Application.DTOs.Common.PagedResult<ProductVariantAttributeValue>
            {
                Items = values,
                TotalCount = totalCount,
                PageNumber = gridCommand.Start / gridCommand.Length,
                PageSize = gridCommand.Length
            };
        }

        public async Task UpdateAsync(ProductVariantAttributeValue data)
        {
            _context.ProductVariantAttributeValues.Update(data);
            await _context.SaveChangesAsync();
        }
    }
}
