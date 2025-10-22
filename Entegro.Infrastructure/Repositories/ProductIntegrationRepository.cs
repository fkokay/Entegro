using Entegro.Application.DTOs.Common;
using Entegro.Application.Interfaces.Repositories;
using Entegro.Domain.Entities.Catalog;
using Entegro.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Entegro.Infrastructure.Repositories
{
    public class ProductIntegrationRepository : IProductIntegrationRepository
    {
        private readonly EntegroDbContext _context;

        public ProductIntegrationRepository(EntegroDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(ProductIntegration productIntegration)
        {

            await _context.ProductIntegrations.AddAsync(productIntegration);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(ProductIntegration productIntegration)
        {
            var model = await _context.ProductIntegrations
                               .FirstOrDefaultAsync(x => x.Id == productIntegration.Id);

            if (model != null)
            {

                var orderItems = await _context.OrderItems
                                               .Where(oi => oi.ProductId == model.ProductId)
                                               .ToListAsync();

                foreach (var item in orderItems)
                {
                    item.ProductId = null;
                }

                _context.OrderItems.UpdateRange(orderItems);
                _context.ProductIntegrations.Remove(model);
                await _context.SaveChangesAsync();

            }
        }

        public async Task<List<ProductIntegration>> GetAllAsync()
        {
            return await _context.ProductIntegrations.AsNoTracking()
                .Include(m => m.IntegrationSystem).ThenInclude(m => m.IntegrationSystemParameters)
                .Include(m => m.Product).ThenInclude(m => m.Brand)
                .Include(m => m.Product.ProductCategories).ThenInclude(m => m.Category).ThenInclude(m => m.Parent)
                .Include(m => m.Product.ProductMediaFiles).ThenInclude(m => m.MediaFile).ThenInclude(m => m.Folder)
                .Include(m => m.Product.ProductVariantAttributes).ThenInclude(m => m.ProductAttribute)
                .Include(m => m.Product.ProductVariantAttributes).ThenInclude(m => m.ProductVariantAttributeValues)
                .Include(m => m.Product.ProductVariantAttributeCombinations).ToListAsync();
        }

        public async Task<List<ProductIntegration>> GetAllAsync(int productId)
        {
            return await _context.ProductIntegrations.Where(m => m.ProductId == productId).AsNoTracking()
                .Include(m => m.IntegrationSystem).ThenInclude(m => m.IntegrationSystemParameters)
                .Include(m => m.Product).ThenInclude(m => m.Brand)
                .Include(m => m.Product.ProductCategories).ThenInclude(m => m.Category).ThenInclude(m => m.Parent)
                .Include(m => m.Product.ProductMediaFiles).ThenInclude(m => m.MediaFile).ThenInclude(m => m.Folder)
                .Include(m => m.Product.ProductVariantAttributes).ThenInclude(m => m.ProductAttribute)
                .Include(m => m.Product.ProductVariantAttributes).ThenInclude(m => m.ProductVariantAttributeValues)
                .Include(m => m.Product.ProductVariantAttributeCombinations).ToListAsync();
        }

        public async Task<PagedResult<ProductIntegration>> GetAllAsync(int pageNumber, int pageSize)
        {
            var query = _context.ProductIntegrations.AsNoTracking().AsQueryable();

            var totalCount = await query.CountAsync();
            var orders = await query
                .OrderBy(b => b.Id)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResult<ProductIntegration>
            {
                Items = orders,
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }

        public async Task<ProductIntegration?> GetByIdAsync(int id)
        {
            var entity = await _context.ProductIntegrations
             .Include(pi => pi.IntegrationSystem)
                 .ThenInclude(isys => isys.IntegrationSystemParameters)
             .Include(pi => pi.IntegrationSystem)
                 .ThenInclude(isys => isys.IntegrationSystemLogs)
             .Include(pi => pi.Product)
                 .ThenInclude(p => p.Brand)
             .Include(pi => pi.Product)
                 .ThenInclude(p => p.ProductCategories)
                     .ThenInclude(pc => pc.Category)
                         .ThenInclude(c => c.Parent)
             .Include(pi => pi.Product)
                 .ThenInclude(p => p.ProductMediaFiles)
                     .ThenInclude(pm => pm.MediaFile)
             .ThenInclude(mf => mf.Folder) // sadece Folder ekledik
             .Include(pi => pi.Product)
                 .ThenInclude(p => p.ProductVariantAttributes)
                     .ThenInclude(pv => pv.ProductAttribute)
                         .ThenInclude(pa => pa.ProductAttributeValues)
             .Include(pi => pi.Product)
                 .ThenInclude(p => p.ProductVariantAttributeCombinations)
             .AsNoTracking()
             .FirstOrDefaultAsync(pi => pi.Id == id);

            return entity;
        }

        public async Task<ProductIntegration?> GetByIntegrationCodeAsync(string integrationCode)
        {
            return await _context.ProductIntegrations
                .Include(c => c.Product).AsNoTracking()
                .FirstOrDefaultAsync(t => t.IntegrationCode == integrationCode);
        }

        public async Task<ProductIntegration?> GetByIntegrationSystemIdandIntegrationCodeAsync(int integrationSystemId, string integrationCode)
        {
            var productIntegration = await _context.ProductIntegrations
               .Include(p => p.Product)
               .AsNoTracking()
               .FirstOrDefaultAsync(p =>
                   p.IntegrationSystemId == integrationSystemId &&
                   p.IntegrationCode == integrationCode);
            return productIntegration;
        }

        public async Task<ProductIntegration?> GetByProductIdandIntegrationSystemIdAsync(int productId, int integrationSystemId)
        {

            return await _context.ProductIntegrations.Include(c => c.Product).AsNoTracking()
                .FirstOrDefaultAsync(t => t.ProductId == productId && t.IntegrationSystemId == integrationSystemId);
        }

        public async Task UpdateAsync(ProductIntegration productIntegration)
        {
            _context.ProductIntegrations.Update(productIntegration);
            await _context.SaveChangesAsync();

        }
    }
}
