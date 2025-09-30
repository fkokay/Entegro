using Entegro.Application.DTOs.Common;
using Entegro.Application.Interfaces.Repositories;
using Entegro.Domain.Entities.Checkout;
using Entegro.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq.Dynamic.Core;
namespace Entegro.Infrastructure.Repositories
{
    public class InvoiceRepository : IInvoiceRepository
    {
        private readonly EntegroDbContext _context;

        public InvoiceRepository(EntegroDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Invoice invoice)
        {
            await _context.Invoices.AddAsync(invoice);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Invoice invoice)
        {
            var tracked = _context.Invoices.Local.FirstOrDefault(b => b.Id == invoice.Id);
            if (tracked != null)
            {
                _context.Invoices.Remove(tracked);
            }
            else
            {
                _context.Invoices.Attach(invoice);
                _context.Invoices.Remove(invoice);
            }

            await _context.SaveChangesAsync();
        }

        public async Task<bool> ExistsByIdAsync(int id)
        {
            return await _context.Invoices.AnyAsync(o => o.Id == id);
        }

        public async Task<bool> ExistsByInvoiceNumberAsync(string invoiceNumber)
        {
            return await _context.Invoices.AsNoTracking().AnyAsync(o => o.InvoiceNumber == invoiceNumber);
        }

        public async Task<bool> ExistsByPackageNoAsync(string packageNo)
        {
            return await _context.Invoices.AsNoTracking().AnyAsync(i => i.Order.Shipments.Any(s => s.PackageNo == packageNo));
        }

        public async Task<Invoice?> GetByIdAsync(int id)
        {
            return await _context.Invoices.Include(i => i.InvoiceItems).AsNoTracking().FirstOrDefaultAsync(o => o.Id == id);
        }

        public async Task<Invoice?> GetByInvoiceNumberAsync(string invoiceNumber)
        {
            return await _context.Invoices.Include(i => i.InvoiceItems).AsNoTracking().FirstOrDefaultAsync(o => o.InvoiceNumber == invoiceNumber);
        }

        public async Task<Invoice?> GetByPackageNoAsync(string packageNo)
        {
            return await _context.Invoices.AsNoTracking().FirstOrDefaultAsync(i => i.Order.Shipments.Any(s => s.PackageNo == packageNo));
        }

        public async Task<Application.DTOs.Common.PagedResult<Invoice>> GetPagedAsync(GridCommand gridCommand)
        {
            var query = _context.Invoices.Include(m => m.InvoiceItems).AsNoTracking();

            if (gridCommand.Search != null)
            {
                if (!string.IsNullOrEmpty(gridCommand.Search.Value))
                {
                    query = query.Where(b => b.InvoiceNumber.Contains(gridCommand.Search.Value) || b.CustomerName.Contains(gridCommand.Search.Value)).AsQueryable();
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
            var invoices = await query
            .Skip(gridCommand.Start)
            .Take(gridCommand.Length)
            .ToListAsync();

            return new Application.DTOs.Common.PagedResult<Invoice>
            {
                Items = invoices,
                TotalCount = totalCount,
                PageNumber = gridCommand.Start + 1,
                PageSize = gridCommand.Length
            };
        }

        public async Task UpdateAsync(Invoice invoice)
        {
            _context.Invoices.Update(invoice);
            await _context.SaveChangesAsync();
        }
    }
}
