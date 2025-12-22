using Entegro.Application.DTOs.Marketplace.Pazarama;
using Entegro.Application.Interfaces.Repositories;
using Entegro.Domain.Entities.Checkout;
using Entegro.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entegro.Infrastructure.Repositories
{
    public class ReturnRequestItemRepository : IReturnRequestItemRepository
    {
        private readonly EntegroDbContext _context;
        public ReturnRequestItemRepository(EntegroDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<List<ReturnRequestItem>> GetAllIntegrationSkuWithAsync(string integrationSku)
        {
            return await _context.ReturnRequestItems.Where(m => m.Barcode == integrationSku).AsNoTracking().ToListAsync();
        }

        public async Task<ReturnRequestItem?> GetByIdAsync(int id)
        {
            return await _context.ReturnRequestItems.FirstOrDefaultAsync(o => o.Id == id);
        }

        public async Task UpdateAsync(ReturnRequestItem returnRequestItem)
        {
            _context.ReturnRequestItems.Update(returnRequestItem);
            await _context.SaveChangesAsync();
        }
    }
}
