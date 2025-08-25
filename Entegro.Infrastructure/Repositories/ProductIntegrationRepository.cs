using Entegro.Application.DTOs.Common;
using Entegro.Application.Interfaces.Repositories;
using Entegro.Domain.Entities;
using Entegro.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Entegro.Infrastructure.Repositories
{
    public class ProductIntegrationRepository : IProductIntegrationRepository
    {
        private readonly EntegroContext _context;

        public ProductIntegrationRepository(EntegroContext context)
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
            var model = await _context.ProductIntegrations.FindAsync(productIntegration.Id);
            if (model != null)
            {
                _context.ProductIntegrations.Remove(model);
                await _context.SaveChangesAsync();

            }

        }

        public async Task<List<ProductIntegration>> GetAllAsync()
        {
            return await _context.ProductIntegrations.AsNoTracking().Include(m => m.Product).ThenInclude(m => m.Brand).Include(m => m.Product.ProductCategories).ThenInclude(m => m.Category).ToListAsync();
        }

        public async Task<PagedResult<ProductIntegration>> GetAllAsync(int pageNumber, int pageSize)
        {
            var query = _context.ProductIntegrations.AsNoTracking().AsQueryable();

            var totalCount = await query.CountAsync();
            var orders = await query
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
            return await _context.ProductIntegrations
                .Include(c => c.Product).AsNoTracking()
                .FirstOrDefaultAsync(t => t.Id == id);
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
