using Entegro.Application.DTOs.Common;
using Entegro.Application.DTOs.Notification;

namespace Entegro.Application.Interfaces.Services
{
    public interface INotificationService
    {
        Task<bool> ExistsByIdAsync(int id);
        Task<bool> ExistsByUserIdAsync(int userId);
        Task<bool> ExistsByTitleAsync(string title);
        Task<NotificationDto?> GetByIdAsync(int id);
        Task<NotificationDto?> GetByTitleAsync(string title);
        Task<NotificationDto?> GetByUserIdAsync(int userId);
        Task<IEnumerable<NotificationDto>> GetAllAsync();
        Task<PagedResult<NotificationDto>> GetPagedAsync(int pageNumber = 1, int pageSize = 7);
        Task<PagedResult<NotificationDto>> GetPagedAsync(GridCommand gridCommand);
        Task<NotificationDto> AddAsync(CreateNotificationDto model);
        Task<NotificationDto> UpdateAsync(UpdateNotificationDto model);
        Task DeleteAsync(int id);
    }
}
