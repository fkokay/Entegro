using Entegro.Domain.Entities;

namespace Entegro.Application.Interfaces.Repositories
{
    public interface IEmailAccountRepository
    {
        Task<EmailAccount> GetByIdAsync(int id);
        Task<List<EmailAccount>> GetAllAsync();
        Task AddAsync(EmailAccount email);
        Task UpdateAsync(EmailAccount email);
        Task DeleteAsync(EmailAccount email);
    }
}
