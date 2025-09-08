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
            var query = _context.ProductAttributes.AsNoTracking().AsQueryable();

            if (gridCommand.Search != null)
            {
                if (!string.IsNullOrEmpty(gridCommand.Search.Value))
                {
                    query = query.Where(b => b.Name.Contains(gridCommand.Search.Value)).AsQueryable();
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
            var items = await query
            .Skip(gridCommand.Start)
            .Take(gridCommand.Length)
            .ToListAsync();

            return new Application.DTOs.Common.PagedResult<ProductAttribute>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = gridCommand.Start + 1,
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
