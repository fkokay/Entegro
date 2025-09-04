using Entegro.Application.DTOs.Common;
using Entegro.Application.Interfaces.Repositories;
using Entegro.Domain.Entities;
using Entegro.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Entegro.Infrastructure.Repositories
{
    public class SpecificationAttributeRepository : ISpecificationAttributeRepository
    {
        private readonly EntegroContext _context;

        public SpecificationAttributeRepository(EntegroContext context)
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

        public async Task<PagedResult<SpecificationAttribute>> GetAllAsync(int pageNumber, int pageSize)
        {
            var query = _context.SpecificationAttributes.AsNoTracking().AsQueryable();

            var totalCount = await query.CountAsync();
            var brands = await query
                .OrderBy(b => b.Id)
                .Skip(pageNumber * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResult<SpecificationAttribute>
            {
                Items = brands,
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }

        public async Task<SpecificationAttribute?> GetByIdAsync(int id) => await _context.SpecificationAttributes.FirstOrDefaultAsync(o => o.Id == id);

        public async Task<SpecificationAttribute?> GetByNameAsync(string name) => await _context.SpecificationAttributes.FirstOrDefaultAsync(o => o.Name == name);
        public async Task UpdateAsync(SpecificationAttribute specificationAttribute)
        {
            _context.SpecificationAttributes.Update(specificationAttribute);
            await _context.SaveChangesAsync();
        }
    }
}
