using Entegro.Application.DTOs.Common;
using Entegro.Domain.Entities.Common;

namespace Entegro.Application.Interfaces.Repositories
{
    public interface INotificationRepository
    {
        Task<bool> ExistsByIdAsync(int id);
        Task<bool> ExistsByUserIdAsync(int userId);
        Task<bool> ExistsByTitleAsync(string title);
        Task<Notification?> GetByIdAsync(int id);
        Task<Notification?> GetByTitleAsync(string title);
        Task<Notification?> GetByUserIdAsync(int userId);
        Task<List<Notification>> GetAllAsync();
        Task<PagedResult<Notification>> GetAllAsync(int pageNumber, int pageSize);
        Task<PagedResult<Notification>> GetPagedAsync(GridCommand gridCommand);
        Task AddAsync(Notification notification);
        Task UpdateAsync(Notification notification);
        Task MarkAsRead(Notification notification);
        Task DeleteAsync(Notification notification);
        Task DeleteAllAsync(List<Notification> notification);
    }
}
