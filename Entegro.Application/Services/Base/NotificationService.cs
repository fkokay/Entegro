using Entegro.Application.DTOs.Common;
using Entegro.Application.DTOs.Notification;
using Entegro.Application.Interfaces.Repositories;
using Entegro.Application.Interfaces.Services.Base;
using Entegro.Domain.Entities.Common;
using Entegro.Domain.Enums;
using MapsterMapper;
using Microsoft.AspNetCore.SignalR.Client;

namespace Entegro.Application.Services.Base
{
    public class NotificationService : INotificationService
    {
        private readonly INotificationRepository _notificationRepository;
        private readonly ISettingService _settingService;
        private readonly IMapper _mapper;
        public NotificationService(INotificationRepository notificationRepository, ISettingService settingService, IMapper mapper)
        {
            _notificationRepository = notificationRepository ?? throw new ArgumentNullException(nameof(notificationRepository));
            _settingService = settingService;
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<NotificationDto> AddAsync(CreateNotificationDto model)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            var notification = _mapper.Map<Notification>(model);
            await _notificationRepository.AddAsync(notification);

            return _mapper.Map<NotificationDto>(notification);
        }

        public async Task DeleteAllAsync(List<int> idList)
        {
            if (idList == null || !idList.Any())
                throw new ArgumentException("Silinecek ID listesi boş olamaz.", nameof(idList));

            var notificationsToDelete = new List<Notification>();

            foreach (var id in idList)
            {
                if (id <= 0)
                    continue;

                var notification = await _notificationRepository.GetByIdAsync(id);
                if (notification != null)
                {
                    notificationsToDelete.Add(notification);
                }
            }

            if (!notificationsToDelete.Any())
                throw new KeyNotFoundException("Hiçbir geçerli Notification bulunamadı.");
            await _notificationRepository.DeleteAllAsync(notificationsToDelete);
        }


        public async Task DeleteAsync(int id)
        {
            if (id <= 0)
                throw new ArgumentOutOfRangeException(nameof(id));

            var notification = await _notificationRepository.GetByIdAsync(id);
            if (notification == null)
                throw new KeyNotFoundException($"ID {id} ile Notification bulunamadı.");

            await _notificationRepository.DeleteAsync(notification);
        }

        public async Task<bool> ExistsByIdAsync(int id)
        {
            if (id <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(id));
            }

            return await _notificationRepository.ExistsByIdAsync(id);
        }

        public async Task<bool> ExistsByTitleAsync(string title)
        {
            if (string.IsNullOrWhiteSpace(title))
                throw new ArgumentException("Title boş olamaz.", nameof(title));

            return await _notificationRepository.ExistsByTitleAsync(title);
        }

        public async Task<bool> ExistsByUserIdAsync(int userId)
        {
            if (userId <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(userId));
            }

            return await _notificationRepository.ExistsByUserIdAsync(userId);
        }

        public async Task<IEnumerable<NotificationDto>> GetAllAsync()
        {
            var notifications = await _notificationRepository.GetAllAsync();
            var notificationDtos = _mapper.Map<IEnumerable<NotificationDto>>(notifications);
            return notificationDtos;
        }

        public async Task<NotificationDto?> GetByIdAsync(int id)
        {
            if (id <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(id));
            }

            var notification = await _notificationRepository.GetByIdAsync(id);
            if (notification == null)
            {
                return null;
            }
            var notificationDto = _mapper.Map<NotificationDto>(notification);

            return notificationDto;
        }

        public async Task<NotificationDto?> GetByTitleAsync(string title)
        {
            if (string.IsNullOrEmpty(title))
            {
                throw new ArgumentException("Title cannot be null or empty.", nameof(title));
            }

            var notification = await _notificationRepository.GetByTitleAsync(title);
            if (notification == null)
            {
                return null;
            }
            var notificationDto = _mapper.Map<NotificationDto>(notification);
            return notificationDto;
        }

        public async Task<NotificationDto?> GetByUserIdAsync(int userId)
        {
            if (userId <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(userId));
            }

            var notification = await _notificationRepository.GetByUserIdAsync(userId);
            if (notification == null)
            {
                return null;
            }
            var notificationDto = _mapper.Map<NotificationDto>(notification);

            return notificationDto;
        }

        public async Task<PagedResult<NotificationDto>> GetPagedAsync(int pageNumber = 1, int pageSize = 7)
        {
            if (pageNumber < 0)
                throw new ArgumentOutOfRangeException(nameof(pageNumber));
            if (pageSize <= 0)
                throw new ArgumentOutOfRangeException(nameof(pageSize));


            var notifications = await _notificationRepository.GetAllAsync(pageNumber, pageSize);
            return new PagedResult<NotificationDto>
            {
                Items = _mapper.Map<IEnumerable<NotificationDto>>(notifications.Items),
                TotalCount = notifications.TotalCount,
                PageNumber = notifications.PageNumber,
                PageSize = notifications.PageSize
            };
        }

        public async Task<PagedResult<NotificationDto>> GetPagedAsync(GridCommand gridCommand)
        {
            var notifications = await _notificationRepository.GetPagedAsync(gridCommand);
            return new PagedResult<NotificationDto>
            {
                Items = _mapper.Map<IEnumerable<NotificationDto>>(notifications.Items),
                TotalCount = notifications.TotalCount,
                PageNumber = notifications.PageNumber,
                PageSize = notifications.PageSize
            };
        }

        public async Task<NotificationDto> MarkAsRead(int id)
        {
            if (id <= 0)
                throw new ArgumentOutOfRangeException(nameof(id), "Geçerli bir ID girilmelidir.");

            var existingNotification = await _notificationRepository.GetByIdAsync(id);
            if (existingNotification == null)
                throw new KeyNotFoundException($"ID {id} ile Notification bulunamadı.");

            // Zaten okunduysa tekrar güncelleme yapma
            if (!existingNotification.IsRead)
            {
                existingNotification.IsRead = true;
                await _notificationRepository.UpdateAsync(existingNotification);
            }
            return _mapper.Map<NotificationDto>(existingNotification);
        }



        public async Task SendNotification(NotificationType notificationType, string title, string message)
        {
            var systemUrl = await _settingService.GetByKeyAsync("SystemUrl");

            var connection = new HubConnectionBuilder()
              .WithUrl(systemUrl.Value + "/notificationHub")
              .Build();

            await connection.StartAsync();

            await connection.InvokeAsync("SendNotification", notificationType, title, message);

            await connection.StopAsync();
        }

        public async Task<NotificationDto> UpdateAsync(UpdateNotificationDto model)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            var existingNotification = await _notificationRepository.GetByIdAsync(model.Id);
            if (existingNotification == null)
                throw new KeyNotFoundException($"ID {model.Id} ile Notification bulunamadı.");

            _mapper.Map(model, existingNotification);
            await _notificationRepository.UpdateAsync(existingNotification);

            return _mapper.Map<NotificationDto>(existingNotification);
        }
    }
}
