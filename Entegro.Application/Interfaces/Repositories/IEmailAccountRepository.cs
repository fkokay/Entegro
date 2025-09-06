using Entegro.Application.DTOs.Common;
using Entegro.Domain.Entities.Platform.Messaging;

namespace Entegro.Application.Interfaces.Repositories
{
    public interface IEmailAccountRepository
    {
        Task<EmailAccount?> GetByIdAsync(int id);
        Task<List<EmailAccount>> GetAllAsync();
        Task AddAsync(EmailAccount email);
        Task<PagedResult<EmailAccount>> GetAllAsync(int pageNumber, int pageSize);
        Task UpdateAsync(EmailAccount email);
        Task DeleteAsync(EmailAccount email);
    }
}
