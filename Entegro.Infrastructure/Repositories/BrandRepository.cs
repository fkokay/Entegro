using Entegro.Application.DTOs.Brand;
using Entegro.Application.DTOs.Common;
using Entegro.Application.Interfaces.Repositories;
using Entegro.Domain.Entities;
using Entegro.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Linq.Dynamic.Core;

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
            brand.CreatedOn = DateTime.UtcNow;
            brand.UpdatedOn = DateTime.UtcNow;
            await _context.Brands.AddAsync(brand);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Brand brand)
        {
            var tracked = _context.Brands.Local.FirstOrDefault(b => b.Id == brand.Id);
            if (tracked != null)
            {
                _context.Brands.Remove(tracked);
            }
            else
            {
                _context.Brands.Attach(brand);
                _context.Brands.Remove(brand);
            }

            await _context.SaveChangesAsync();
        }

        public async Task<bool> ExistsByIdAsync(int id)
        {
            return await _context.Brands.AnyAsync(o => o.Id == id);
        }

        public async Task<bool> ExistsByNameAsync(string name)
        {
            return await _context.Brands.AnyAsync(o => o.Name == name);
        }

        public async Task<List<Brand>> GetAllAsync()
        {
            return await _context.Brands.Include(m => m.MediaFile).ThenInclude(m => m.MediaFolder).AsNoTracking().OrderBy(b => b.Id).ToListAsync();
        }

        public async Task<Application.DTOs.Common.PagedResult<Brand>> GetAllAsync(int pageNumber, int pageSize)
        {
            var query = _context.Brands
                .Include(m => m.MediaFile).ThenInclude(m => m.MediaFolder)
                .AsNoTracking()
                .OrderBy(b => b.Id);

            var totalCount = await query.CountAsync();
            var brands = await query
                .Skip(pageNumber * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new Application.DTOs.Common.PagedResult<Brand>
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
             .ThenInclude(b => b.MediaFolder)
             .FirstOrDefaultAsync(b => b.Id == id);
        }

        public async Task<Brand?> GetByNameAsync(string name)
        {
            return await _context.Brands.FirstOrDefaultAsync(o => o.Name == name);
        }

        public async Task<Application.DTOs.Common.PagedResult<Brand>> GetPagedAsync(GridCommand gridCommand)
        {
            var query = _context.Brands.Include(m => m.MediaFile).ThenInclude(m => m.MediaFolder).AsNoTracking();

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
            var brands = await query
            .Skip(gridCommand.Start)
            .Take(gridCommand.Length)
            .ToListAsync();

            return new Application.DTOs.Common.PagedResult<Brand>
            {
                Items = brands,
                TotalCount = totalCount,
                PageNumber = gridCommand.Start + 1,
                PageSize = gridCommand.Length
            };
        }

        public async Task UpdateAsync(Brand brand)
        {
            brand.UpdatedOn = DateTime.UtcNow;
            _context.Brands.Update(brand);
            await _context.SaveChangesAsync();
        }
    }
}
