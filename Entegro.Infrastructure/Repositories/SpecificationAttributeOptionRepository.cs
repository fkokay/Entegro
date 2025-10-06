using Entegro.Application.DTOs.Common;
using Entegro.Application.Interfaces.Repositories;
using Entegro.Domain.Entities.Catalog;
using Entegro.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq.Dynamic.Core;

namespace Entegro.Infrastructure.Repositories
{
    public class SpecificationAttributeOptionRepository : ISpecificationAttributeOptionRepository
    {
        private readonly EntegroDbContext _context;

        public SpecificationAttributeOptionRepository(EntegroDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(SpecificationAttributeOption specificationAttributeOption)
        {
            await _context.SpecificationAttributeOptions.AddAsync(specificationAttributeOption);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(SpecificationAttributeOption specificationAttributeOption)
        {
            _context.SpecificationAttributeOptions.Remove(specificationAttributeOption);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> ExistsByIdAsync(int id) => await _context.SpecificationAttributeOptions.AnyAsync(o => o.Id == id);

        public async Task<bool> ExistsByNameAsync(string name) => await _context.SpecificationAttributeOptions.AnyAsync(o => o.Name == name);

        public async Task<List<SpecificationAttributeOption>> GetAllAsync() => await _context.SpecificationAttributeOptions.AsNoTracking().ToListAsync();

        public async Task<Application.DTOs.Common.PagedResult<SpecificationAttributeOption>> GetAllAsync(int pageNumber, int pageSize)
        {
            var query = _context.SpecificationAttributeOptions.AsNoTracking().AsQueryable();

            var totalCount = await query.CountAsync();
            var brands = await query
                .OrderBy(b => b.Id)
                .Skip(pageNumber * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new Application.DTOs.Common.PagedResult<SpecificationAttributeOption>
            {
                Items = brands,
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }

        public async Task<SpecificationAttributeOption?> GetByIdAsync(int id) => await _context.SpecificationAttributeOptions.FirstOrDefaultAsync(o => o.Id == id);

        public async Task<SpecificationAttributeOption?> GetByNameAsync(string name) => await _context.SpecificationAttributeOptions.FirstOrDefaultAsync(o => o.Name == name);

        public async Task<Application.DTOs.Common.PagedResult<SpecificationAttributeOption>> GetPagedAsync(GridCommand gridCommand, int specificationAttributeId)
        {
            var query = _context.SpecificationAttributeOptions.Where(m => m.SpecificationAttributeId == specificationAttributeId).AsQueryable();

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

            var totalCount = query.Count();
            var options = query
            .Skip(gridCommand.Start)
            .Take(gridCommand.Length)
            .ToList();

            return new Application.DTOs.Common.PagedResult<SpecificationAttributeOption>
            {
                Items = options,
                TotalCount = totalCount,
                PageNumber = gridCommand.Start + 1,
                PageSize = gridCommand.Length
            };
        }

        public async Task UpdateAsync(SpecificationAttributeOption specificationAttributeOption)
        {
            _context.SpecificationAttributeOptions.Update(specificationAttributeOption);
            await _context.SaveChangesAsync();
        }
    }
}
