using Entegro.Application.DTOs.Common;
using Entegro.Application.Interfaces.Repositories;
using Entegro.Domain.Entities;
using Entegro.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Entegro.Infrastructure.Repositories
{
    public class BrandRepository : IBrandRepository
    {
        private readonly EntegroContext _context;

        public BrandRepository(EntegroContext context)
        {
            _context = context;
        }
        public async Task AddAsync(Brand brand)
        {
            brand.CreatedOn = DateTime.Now;
            brand.UpdatedOn = DateTime.Now;
            await _context.Brands.AddAsync(brand);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Brand brand)
        {
            _context.Brands.Remove(brand);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> ExistsByNameAsync(string name)
        {
            return await _context.Brands.AnyAsync(o => o.Name == name);
        }

        public async Task<List<Brand>> GetAllAsync()
        {
            return await _context.Brands.ToListAsync();
        }

        public async Task<PagedResult<Brand>> GetAllAsync(int pageNumber, int pageSize)
        {
            var query = _context.Brands.Include(m=>m.MediaFile).ThenInclude(m=>m.Folder).AsQueryable();

            var totalCount = await query.CountAsync();
            var brands = await query
                .Skip(pageNumber * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResult<Brand>
            {
                Items = brands,
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }

        public async Task<Brand?> GetByIdAsync(int id)
        {
            return await _context.Brands.FirstOrDefaultAsync(o => o.Id == id);
        }

        public async Task<Brand?> GetByIdWithMediaAsync(int id)
        {
            return await _context.Brands
             .Include(b => b.MediaFile)
             .ThenInclude(b=>b.Folder)
             .FirstOrDefaultAsync(b => b.Id == id);
        }

        public async Task<Brand?> GetByNameAsync(string name)
        {
            return await _context.Brands.FirstOrDefaultAsync(o => o.Name == name);
        }

        public async Task UpdateAsync(Brand brand)
        {
            brand.UpdatedOn = DateTime.Now;

            _context.Brands.Update(brand);
            await _context.SaveChangesAsync();
        }
    }
}
