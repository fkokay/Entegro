using Entegro.Application.DTOs.Common;
using Entegro.Application.Interfaces.Repositories;
using Entegro.Domain.Entities;
using Entegro.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Entegro.Infrastructure.Repositories
{
    public class ProductImageMappingRepository : IProductImageMappingRepository
    {
        private readonly EntegroContext _context;
        public ProductImageMappingRepository(EntegroContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task AddAsync(ProductMediaFile productImage)
        {
            await _context.ProductImages.AddAsync(productImage);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(ProductMediaFile productImage)
        {
            _context.ProductImages.Remove(productImage);
            await _context.SaveChangesAsync();
        }

        public async Task<List<ProductMediaFile>> GetAllAsync()
        {
            return await _context.ProductImages.ToListAsync();
        }

        public async Task<PagedResult<ProductMediaFile>> GetAllAsync(int pageNumber, int pageSize)
        {
            var query = _context.ProductImages.AsQueryable();

            var totalCount = await query.CountAsync();
            var categories = await query
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

        public async Task<ProductMediaFile?> GetByIdAsync(int id)
        {
            return await _context.ProductImages.AsNoTracking().FirstOrDefaultAsync(o => o.Id == id);
        }

        public async Task<ProductMediaFile> GetByPictureIdProductIdAsync(int pictureId, int productId)
        {
            var result = await _context.ProductImages.AsNoTracking()
                 .FirstOrDefaultAsync(o => o.Id == pictureId && o.ProductId == productId);
            return result ?? throw new KeyNotFoundException($"ProductImage with PictureId {pictureId} and ProductId {productId} not found.");
        }

        public async Task UpdateAsync(ProductMediaFile productImage)
        {
            _context.ProductImages.Update(productImage);
            await _context.SaveChangesAsync();
        }
    }
}


