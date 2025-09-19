
using Entegro.Application.DTOs.Common;
using Entegro.Application.Interfaces.Repositories;
using Entegro.Domain.Entities.Common;
using Entegro.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq.Dynamic.Core;
namespace Entegro.Infrastructure.Repositories
{
    public class NotificationRepository : INotificationRepository
    {
        private readonly EntegroDbContext _context;

        public NotificationRepository(EntegroDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Notification notification)
        {
            await _context.Notifications.AddAsync(notification);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Notification notification)
        {
            var tracked = _context.Notifications.Local.FirstOrDefault(b => b.Id == notification.Id);
            if (tracked != null)
            {
                _context.Notifications.Remove(tracked);
            }
            else
            {
                _context.Notifications.Attach(notification);
                _context.Notifications.Remove(notification);
            }

            await _context.SaveChangesAsync();
        }

        public async Task<bool> ExistsByIdAsync(int id)
        {
            return await _context.Notifications.AnyAsync(o => o.Id == id);
        }

        public async Task<bool> ExistsByTitleAsync(string title)
        {
            return await _context.Notifications.AnyAsync(o => o.Title == title);
        }

        public async Task<bool> ExistsByUserIdAsync(int userId)
        {
            return await _context.Notifications.AnyAsync(o => o.UserId == userId);
        }

        public async Task<List<Notification>> GetAllAsync()
        {
            return await _context.Notifications.Include(m => m.User).AsNoTracking().OrderBy(b => b.Id).ToListAsync();
        }

        public async Task<Application.DTOs.Common.PagedResult<Notification>> GetAllAsync(int pageNumber, int pageSize)
        {
            var query = _context.Notifications
                .Include(m => m.User)
                .AsNoTracking()
                .OrderBy(b => b.Id);

            var totalCount = await query.CountAsync();
            var notifications = await query
                .Skip(pageNumber * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new Application.DTOs.Common.PagedResult<Notification>
            {
                Items = notifications,
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }

        public async Task<Notification?> GetByIdAsync(int id)
        {
            return await _context.Notifications.FirstOrDefaultAsync(o => o.Id == id);
        }

        public async Task<Notification?> GetByTitleAsync(string title)
        {
            return await _context.Notifications.FirstOrDefaultAsync(o => o.Title == title);
        }

        public async Task<Notification?> GetByUserIdAsync(int userId)
        {
            return await _context.Notifications.FirstOrDefaultAsync(o => o.UserId == userId);
        }

        public async Task<Application.DTOs.Common.PagedResult<Notification>> GetPagedAsync(GridCommand gridCommand)
        {
            var query = _context.Notifications.Include(m => m.User).AsNoTracking();

            if (gridCommand.Search != null)
            {
                if (!string.IsNullOrEmpty(gridCommand.Search.Value))
                {
                    query = query.Where(b => b.Title.Contains(gridCommand.Search.Value)).AsQueryable();
                }
            }

            if (gridCommand.Order.Any())
            {
                foreach (var item in gridCommand.Order)
                {
                    query = query.OrderBy($"{gridCommand.Columns[item.Column].Data} {(item.Dir ?? "asc")}");
                }
            }
            else
            {
                query = query.OrderBy(b => b.Id);
            }

            var totalCount = await query.CountAsync();
            var notifcations = await query
            .Skip(gridCommand.Start)
            .Take(gridCommand.Length)
            .ToListAsync();

            return new Application.DTOs.Common.PagedResult<Notification>
            {
                Items = notifcations,
                TotalCount = totalCount,
                PageNumber = gridCommand.Start + 1,
                PageSize = gridCommand.Length
            };
        }

        public async Task UpdateAsync(Notification notification)
        {
            _context.Notifications.Update(notification);
            await _context.SaveChangesAsync();
        }
    }
}
