using Entegro.Application.DTOs.Common;
using Entegro.Application.Interfaces.Repositories;
using Entegro.Domain.Entities.Catalog;
using Entegro.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Entegro.Infrastructure.Repositories
{
    public class ProductImageMappingRepository : IProductImageMappingRepository
    {
        private readonly EntegroDbContext _context;
        public ProductImageMappingRepository(EntegroDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task AddAsync(ProductMediaFile productImage)
        {
            await _context.ProductMediaFiles.AddAsync(productImage);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(ProductMediaFile productImage)
        {
            _context.ProductMediaFiles.Remove(productImage);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteByProductIdAsync(int productId)
        {
            var images = await _context.ProductMediaFiles
                              .Where(x => x.ProductId == productId)
                              .ToListAsync();

            if (images == null || images.Count == 0)
                return;

            _context.ProductMediaFiles.RemoveRange(images);
            await _context.SaveChangesAsync();
        }

        public async Task<List<ProductMediaFile>> GetAllAsync()
        {
            return await _context.ProductMediaFiles.ToListAsync();
        }

        public async Task<PagedResult<ProductMediaFile>> GetAllAsync(int pageNumber, int pageSize)
        {
            var query = _context.ProductMediaFiles.AsQueryable();

            var totalCount = await query.CountAsync();
            var categories = await query
                .OrderBy(b => b.Id)
                .Skip(pageNumber * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResult<ProductMediaFile>
            {
                Items = categories,
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }

        public async Task<List<ProductMediaFile>> GetAllAsync(int productId)
        {
            return await _context.ProductMediaFiles.Where(m => m.ProductId == productId).AsNoTracking().ToListAsync();
        }

        public async Task<ProductMediaFile?> GetByIdAsync(int id)
        {
            return await _context.ProductMediaFiles.AsNoTracking().FirstOrDefaultAsync(o => o.Id == id);
        }

        public async Task<ProductMediaFile?> GetByPictureIdProductIdAsync(int pictureId, int productId)
        {
            var result = await _context.ProductMediaFiles.AsNoTracking()
                 .FirstOrDefaultAsync(o => o.MediaFileId == pictureId && o.ProductId == productId);
            return result ?? null;
        }

        public async Task<ProductMediaFile?> GetByPictureIdSortAsync(int pictureId, int productId)
        {
            var result = await _context.ProductMediaFiles.AsNoTracking()
                 .FirstOrDefaultAsync(o => o.Id == pictureId && o.ProductId == productId);
            return result ?? null;
        }

        public async Task UpdateAsync(ProductMediaFile productImage)
        {
            _context.ProductMediaFiles.Update(productImage);
            await _context.SaveChangesAsync();
        }
    }
}


