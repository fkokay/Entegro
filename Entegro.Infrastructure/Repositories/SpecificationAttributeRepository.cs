using Entegro.Application.DTOs.Common;
using Entegro.Application.Interfaces.Repositories;
using Entegro.Domain.Entities.Catalog;
using Entegro.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq.Dynamic.Core;
namespace Entegro.Infrastructure.Repositories
{
    public class SpecificationAttributeRepository : ISpecificationAttributeRepository
    {
        private readonly EntegroDbContext _context;

        public SpecificationAttributeRepository(EntegroDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(SpecificationAttribute specificationAttribute)
        {
            await _context.SpecificationAttributes.AddAsync(specificationAttribute);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(SpecificationAttribute specificationAttribute)
        {
            _context.SpecificationAttributes.Remove(specificationAttribute);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> ExistsByIdAsync(int id) => await _context.SpecificationAttributes.AnyAsync(o => o.Id == id);

        public async Task<bool> ExistsByNameAsync(string name) => await _context.SpecificationAttributes.AnyAsync(o => o.Name == name);

        public async Task<List<SpecificationAttribute>> GetAllAsync() => await _context.SpecificationAttributes.AsNoTracking().ToListAsync();

        public async Task<Application.DTOs.Common.PagedResult<SpecificationAttribute>> GetAllAsync(int pageNumber, int pageSize)
        {
            var query = _context.SpecificationAttributes.AsNoTracking().AsQueryable();

            var totalCount = await query.CountAsync();
            var brands = await query
                .OrderBy(b => b.Id)
                .Skip(pageNumber * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new Application.DTOs.Common.PagedResult<SpecificationAttribute>
            {
                Items = brands,
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }

        public async Task<Application.DTOs.Common.PagedResult<SpecificationAttribute>> GetAllAsync(int page, string term)
        {
            {
                var query = _context.SpecificationAttributes
                    .Include(x => x.SpecificationAttributeOptions).AsNoTracking();
                if (!string.IsNullOrEmpty(term))
                {
                    query = query.Where(b =>
                    b.Name.Contains(term)).AsQueryable();
                }

                var totalCount = await query.CountAsync();
                var products = await query.Skip((page * 7) - 7)
                    .Take(7).ToListAsync();

                return new Application.DTOs.Common.PagedResult<SpecificationAttribute>
                {
                    Items = products,
                    TotalCount = totalCount,
                    PageNumber = page,
                    PageSize = 7
                };
            }
        }

        public async Task<SpecificationAttribute?> GetByIdAsync(int id)
        {
            return await _context.SpecificationAttributes
            .Include(o => o.SpecificationAttributeOptions)
            .AsNoTracking()
            .FirstOrDefaultAsync(o => o.Id == id);
        }

        public async Task<SpecificationAttribute?> GetByNameAsync(string name) => await _context.SpecificationAttributes.FirstOrDefaultAsync(o => o.Name == name);

        public async Task<Application.DTOs.Common.PagedResult<SpecificationAttribute>> GetPagedAsync(GridCommand gridCommand)
        {
            var query = _context.SpecificationAttributes.AsNoTracking();

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

            return new Application.DTOs.Common.PagedResult<SpecificationAttribute>
            {
                Items = attributes,
                TotalCount = totalCount,
                PageNumber = gridCommand.Start / gridCommand.Length,
                PageSize = gridCommand.Length
            };
        }

        public async Task UpdateAsync(SpecificationAttribute specificationAttribute)
        {
            _context.SpecificationAttributes.Update(specificationAttribute);
            await _context.SaveChangesAsync();
        }
    }
}
