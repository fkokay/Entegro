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
            var query = _context.SpecificationAttributes
             .AsNoTracking();

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
                    string field = "";
                    if (string.IsNullOrEmpty(gridCommand.Columns[item.Column].Name))
                    {
                        field = gridCommand.Columns[item.Column].Data;
                    }
                    else
                    {
                        field = gridCommand.Columns[item.Column].Name;
                    }


                    query = query.OrderBy($"{field} {(item.Dir ?? "asc")}");
                }
            }
            else
            {
                query = query.OrderBy(b => b.Id);
            }

            var totalCount = await query.CountAsync();
            var orders = await query
            .Skip(gridCommand.Start)
            .Take(gridCommand.Length)
            .ToListAsync();

            return new Application.DTOs.Common.PagedResult<SpecificationAttribute>
            {
                Items = orders,
                TotalCount = totalCount,
                PageNumber = gridCommand.Start + 1,
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
