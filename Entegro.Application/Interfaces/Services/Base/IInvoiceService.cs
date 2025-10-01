using Entegro.Application.DTOs.Common;
using Entegro.Application.DTOs.Invoice;

namespace Entegro.Application.Interfaces.Services.Base
{
    public interface IInvoiceService
    {
        Task<bool> ExistsByIdAsync(int id);
        Task<bool> ExistsByInvoiceNumberAsync(string invoiceNumber);
        Task<bool> ExistsByPackageNoAsync(string packageNo);
        Task<InvoiceDto?> GetByIdAsync(int id);
        Task<InvoiceDto?> GetByInvoiceNumberAsync(string invoiceNumber);
        Task<InvoiceDto?> GetByPackageNoAsync(string packageNo);
        Task<PagedResult<InvoiceDto>> GetPagedAsync(GridCommand gridCommand);
        Task<InvoiceDto> AddAsync(CreateInvoiceDto model);
        Task<InvoiceDto> UpdateAsync(UpdateInvoiceDto model);
        Task DeleteAsync(int id);
    }
}
