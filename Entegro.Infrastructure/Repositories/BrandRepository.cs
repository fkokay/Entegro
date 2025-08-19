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
            var query = _context.Brands.AsQueryable();

            var totalCount = await query.CountAsync();
            var categories = await query
                .Skip(pageNumber * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResult<Brand>
            {
                Items = categories,
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
            //var query = _context.Brands.AsQueryable();

            //var totalCount = await query.CountAsync();
            //var brands = await query
            //    .Skip((pageNumber - 1) * pageSize)
            //    .Take(pageSize)
            //    .ToListAsync();

            //return new PagedResult<Brand>
            //{
            //    Items = brands,
            //    TotalCount = totalCount,
            //    PageNumber = pageNumber,
            //    PageSize = pageSize
            //};
        }

        public async Task<Brand?> GetByIdAsync(int id)
        {
            return await _context.Brands.FirstOrDefaultAsync(o => o.Id == id);
        }

        public async Task<Brand?> GetByIdWithMediaAsync(int id)
        {
            return await _context.Brands
             .Include(b => b.MediaFile)
             .FirstOrDefaultAsync(b => b.Id == id);
        }

        public async Task<Brand?> GetByNameAsync(string name)
        {
            return await _context.Brands.FirstOrDefaultAsync(o => o.Name == name);
        }

        public async Task UpdateAsync(Brand brand)
        {
            var existingBrand = await _context.Brands.FindAsync(brand.Id);
            if (existingBrand == null)
            {
                throw new KeyNotFoundException($"Brand with ID {brand.Id} not found.");
            }

            existingBrand.Name = brand.Name;
            existingBrand.Description = brand.Description;
            existingBrand.MetaDescription = brand.MetaDescription;
            existingBrand.MetaTitle = brand.MetaTitle;
            existingBrand.DisplayOrder = brand.DisplayOrder;
            existingBrand.MetaKeywords = brand.MetaKeywords;
            existingBrand.CreatedOn = brand.CreatedOn;
            existingBrand.UpdatedOn = DateTime.Now;

            _context.Brands.Update(existingBrand);
            await _context.SaveChangesAsync();
        }
    }
}
