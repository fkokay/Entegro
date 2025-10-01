using Entegro.Application.DTOs.EInvoice.EFinans;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entegro.Application.Interfaces.Services.EInvoice
{
    public interface IEFinansService
    {
        Task<EFinansResponse<string>> LoginAsync(string username, string password);
        void SetCookie(string cookie);
        Task<EFinansResponse<bool>> IsEInvoiceUserAsync(string taxId);
        Task<EFinansResponse<string?>> GenerateInvoiceNumberAsync(InvoiceNumberRequest request);
        Task<EFinansResponse<byte[]?>> PreviewAsync(PreviewRequest request);
        Task<EFinansResponse<string?>> SendDocumentAsync(SendDocumentRequest request);
        Task<EFinansResponse<string?>> CreateDraftAsync(DraftRequest request);
    }
}
