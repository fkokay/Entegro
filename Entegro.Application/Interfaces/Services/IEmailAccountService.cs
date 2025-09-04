using Entegro.Application.DTOs.Common;
using Entegro.Application.DTOs.EmailAccount;

namespace Entegro.Application.Interfaces.Services
{
    public interface IEmailAccountService
    {
        Task<EmailAccountDto> GetByIdAsync(int id);
        Task<List<EmailAccountDto>> GetAllAsync();
        Task<EmailAccountDto> AddAsync(CreateEmailAccountDto mail);
        Task<PagedResult<EmailAccountDto>> GetPagedAsync(int pageNumber = 1, int pageSize = 7);
        Task<EmailAccountDto> UpdateAsync(UpdateEmailAccountDto mail);
        Task DeleteAsync(int id);
    }
}
