using Entegro.Application.DTOs.Common;
using Entegro.Domain.Entities.Checkout;

namespace Entegro.Application.Interfaces.Repositories
{
    public interface IInvoiceRepository
    {
        Task<bool> ExistsByIdAsync(int id);
        Task<bool> ExistsByInvoiceNumberAsync(string invoiceNumber);
        Task<bool> ExistsByPackageNoAsync(string packageNo);
        Task<Invoice?> GetByIdAsync(int id);
        Task<Invoice?> GetByInvoiceNumberAsync(string invoiceNumber);
        Task<Invoice?> GetByPackageNoAsync(string packageNo);
        Task<PagedResult<Invoice>> GetPagedAsync(GridCommand gridCommand);
        Task AddAsync(Invoice invoice);
        Task UpdateAsync(Invoice invoice);
        Task DeleteAsync(Invoice invoice);
    }
}
