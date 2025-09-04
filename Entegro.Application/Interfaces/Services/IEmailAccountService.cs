using Entegro.Application.DTOs.EmailAccount;

namespace Entegro.Application.Interfaces.Services
{
    public interface IEmailAccountService
    {
        Task<EmailAccountDto> GetByIdAsync(int id);
        Task<List<EmailAccountDto>> GetAllAsync();
        Task<EmailAccountDto> AddAsync(CreateEmailAccountDto mail);
        Task<EmailAccountDto> UpdateAsync(UpdateEmailAccountDto mail);
        Task DeleteAsync(int id);
    }
}
