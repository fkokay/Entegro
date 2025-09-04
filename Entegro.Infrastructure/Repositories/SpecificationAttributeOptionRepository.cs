using Entegro.Application.DTOs.Common;
using Entegro.Application.Interfaces.Repositories;
using Entegro.Domain.Entities;
using Entegro.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Entegro.Infrastructure.Repositories
{
    public class SpecificationAttributeOptionRepository : ISpecificationAttributeOptionRepository
    {
        private readonly EntegroContext _context;

        public SpecificationAttributeOptionRepository(EntegroContext context)
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

        public async Task<PagedResult<SpecificationAttributeOption>> GetAllAsync(int pageNumber, int pageSize)
        {
            var query = _context.SpecificationAttributeOptions.AsNoTracking().AsQueryable();

            var totalCount = await query.CountAsync();
            var brands = await query
                .OrderBy(b => b.Id)
                .Skip(pageNumber * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResult<SpecificationAttributeOption>
            {
                Items = brands,
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }

        public async Task<SpecificationAttributeOption?> GetByIdAsync(int id) => await _context.SpecificationAttributeOptions.FirstOrDefaultAsync(o => o.Id == id);

        public async Task<SpecificationAttributeOption?> GetByNameAsync(string name) => await _context.SpecificationAttributeOptions.FirstOrDefaultAsync(o => o.Name == name);
        public async Task UpdateAsync(SpecificationAttributeOption specificationAttributeOption)
        {
            _context.SpecificationAttributeOptions.Update(specificationAttributeOption);
            await _context.SaveChangesAsync();
        }
    }
}
