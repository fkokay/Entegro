using Entegro.Application.DTOs.Common;
using Entegro.Application.Interfaces.Repositories;
using Entegro.Domain.Entities.Catalog;
using Entegro.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq.Dynamic.Core;
namespace Entegro.Infrastructure.Repositories
{
    public class ProductSpecificationAttributeMappingRepository : IProductSpecificationAttributeMappingRepository
    {
        private readonly EntegroDbContext _context;

        public ProductSpecificationAttributeMappingRepository(EntegroDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(ProductSpecificationAttribute productSpecificationAttribute)
        {
            var mapping = new ProductSpecificationAttribute
            {
                ProductId = productSpecificationAttribute.ProductId,
                SpecificationAttributeOptionId = productSpecificationAttribute.SpecificationAttributeOptionId,
                DisplayOrder = productSpecificationAttribute.DisplayOrder
            };
            await _context.ProductSpecificationAttributes.AddAsync(mapping);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(ProductSpecificationAttribute productSpecificationAttribute)
        {
            _context.ProductSpecificationAttributes.Remove(productSpecificationAttribute);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> ExistsByIdAsync(int specificationAttributeOptionId, int productId)
        {
            return await _context.ProductSpecificationAttributes.AnyAsync(o => o.SpecificationAttributeOptionId == specificationAttributeOptionId && o.ProductId == productId);
        }

        public async Task<List<ProductSpecificationAttribute>> GetAllAsync()
        {
            return await _context.ProductSpecificationAttributes.Include(m => m.Product).Include(m => m.SpecificationAttributeOption).AsNoTracking().ToListAsync();
        }

        public async Task<ProductSpecificationAttribute?> GetByIdAsync(int id)
        {
            return await _context.ProductSpecificationAttributes.AsNoTracking().FirstOrDefaultAsync(o => o.Id == id);
        }



        public async Task<Application.DTOs.Common.PagedResult<ProductSpecificationAttribute>> GetPagedAsync(GridCommand gridCommand, int productId)
        {
            var query = _context.ProductSpecificationAttributes
                .Include(psa => psa.Product)
                .Include(psa => psa.SpecificationAttributeOption)
                .ThenInclude(psa => psa.SpecificationAttribute).OrderBy(b => b.Id)
                .Where(b => b.ProductId == productId).AsNoTracking();


            if (gridCommand.Columns != null)
            {
                foreach (var col in gridCommand.Columns)
                {
                    if (!string.IsNullOrEmpty(col.Search?.Value))
                    {
                        var searchVal = col.Search.Value.Trim('^', '$');

                        // ilgili property’nin tipini bul
                        var propName = col.Data.Contains(".") ? col.Data.Split('.').Last().Trim() : col.Data.Trim();
                        var prop = typeof(SpecificationAttributeOption).GetProperty(propName);
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
                    b.SpecificationAttributeOption.Name.Contains(gridCommand.Search.Value) ||
                    b.SpecificationAttributeOption.SpecificationAttribute.Name.Contains(gridCommand.Search.Value)).AsQueryable();
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

            var productSpecificationAttributes = await query
                .Skip(gridCommand.Start)
                .Take(gridCommand.Length)
                .AsNoTracking()
                .AsSplitQuery()
                .ToListAsync();

            return new Application.DTOs.Common.PagedResult<ProductSpecificationAttribute>
            {
                Items = productSpecificationAttributes,
                TotalCount = totalCount,
                PageNumber = gridCommand.Start / gridCommand.Length,
                PageSize = gridCommand.Length
            };

        }

        public async Task<List<ProductSpecificationAttribute>> GetSpecificationAttributeByProductId(int productId)
        {
            return await _context.ProductSpecificationAttributes.Include(m => m.Product).Include(m => m.SpecificationAttributeOption).AsNoTracking().Where(x => x.ProductId == productId).ToListAsync();
        }

        public async Task UpdateAsync(ProductSpecificationAttribute productSpecificationAttribute)
        {
            _context.ProductSpecificationAttributes.Update(productSpecificationAttribute);
            await _context.SaveChangesAsync();
        }
    }
}
