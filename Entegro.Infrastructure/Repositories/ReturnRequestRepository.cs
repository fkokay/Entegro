using Entegro.Application.DTOs.Common;
using Entegro.Application.Interfaces.Repositories;
using Entegro.Domain.Entities.Checkout;
using Entegro.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq.Dynamic.Core;

namespace Entegro.Infrastructure.Repositories
{
    public class ReturnRequestRepository : IReturnRequestRepository
    {
        private readonly EntegroDbContext _context;

        public ReturnRequestRepository(EntegroDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(ReturnRequest returnRequest)
        {
            await _context.ReturnRequests.AddAsync(returnRequest);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(ReturnRequest returnRequest)
        {
            var tracked = _context.ReturnRequests.Local.FirstOrDefault(b => b.Id == returnRequest.Id);
            if (tracked != null)
            {
                _context.ReturnRequests.Remove(tracked);
            }
            else
            {
                _context.ReturnRequests.Attach(returnRequest);
                _context.ReturnRequests.Remove(returnRequest);
            }

            await _context.SaveChangesAsync();
        }

        public async Task<bool> ExistsByCustomerNameAsync(string customerName)
        {
            return await _context.ReturnRequests.Include(x => x.Items).AsNoTracking().AnyAsync(rr => rr.CustomerFirstName == customerName);
        }

        public async Task<bool> ExistsByIdAsync(int id)
        {
            return await _context.ReturnRequests.AsNoTracking().AnyAsync(o => o.Id == id);
        }

        public async Task<bool> ExistsByOrderNumberAsync(string orderNumber)
        {
            return await _context.ReturnRequests
                .AsNoTracking()
                .AnyAsync(o => o.OrderNumber == orderNumber);
        }

        public async Task<bool> ExistsByReturnRequestStatusAsync(int requestStatus)
        {
            return await _context.ReturnRequests.AsNoTracking().AnyAsync(rr => rr.Items.Any(i => i.ReturnRequestStatusId == requestStatus));
        }

        public async Task<ReturnRequest?> GetByCustomerNameAsync(string name)
        {
            return await _context.ReturnRequests.Include(rr => rr.Items).FirstOrDefaultAsync(rr => rr.CustomerFirstName == name);
        }

        public async Task<ReturnRequest?> GetByIdAsync(int id)
        {
            return await _context.ReturnRequests.Include(rr => rr.Items).AsNoTracking().FirstOrDefaultAsync(rr => rr.Id == id);
        }

        public async Task<ReturnRequest?> GetByOrderNumberAsync(string orderNumber)
        {
            return await _context.ReturnRequests.Include(rr => rr.Items
            ).AsNoTracking().FirstOrDefaultAsync(o => o.OrderNumber == orderNumber);
        }

        public async Task<ReturnRequest?> GetByReturnRequestStatusAsync(int requestStatus)
        {
            return await _context.ReturnRequests.Where(rr => rr.Items.Any(i => i.ReturnRequestStatusId == requestStatus))
            .Include(rr => rr.Items
                .Where(i => i.ReturnRequestStatusId == requestStatus))
            .AsNoTracking()
            .FirstOrDefaultAsync();
        }

        public async Task<Application.DTOs.Common.PagedResult<ReturnRequest>> GetPagedAsync(GridCommand gridCommand)
        {
            var query = _context.ReturnRequests.Include(rr => rr.Items).OrderBy(b => b.Id).AsNoTracking();


            if (gridCommand.Columns != null)
            {
                foreach (var col in gridCommand.Columns)
                {
                    if (!string.IsNullOrEmpty(col.Search?.Value))
                    {
                        var searchVal = col.Search.Value.Trim('^', '$');

                        // ilgili property’nin tipini bul
                        var prop = typeof(ReturnRequest).GetProperty(col.Data);
                        if (prop == null) continue;

                        if (prop.PropertyType == typeof(string))
                        {
                            query = query.Where($"{col.Data}.Contains(@0)", searchVal);
                        }
                        else if (prop.PropertyType == typeof(int) || prop.PropertyType == typeof(int?))
                        {
                            if (int.TryParse(searchVal, out var intVal))
                                query = query.Where($"{col.Data} == @0", intVal);
                        }
                        else if (prop.PropertyType == typeof(bool) || prop.PropertyType == typeof(bool?))
                        {
                            if (bool.TryParse(searchVal, out var boolVal))
                                query = query.Where($"{col.Data} == @0", boolVal);
                        }
                        else if (prop.PropertyType == typeof(DateTime) || prop.PropertyType == typeof(DateTime?))
                        {
                            if (DateTime.TryParse(searchVal, out var dt))
                                query = query.Where($"{col.Data}.Date == @0", dt.Date);
                        }
                    }
                }
            }

            if (gridCommand.Search != null)
            {
                if (!string.IsNullOrEmpty(gridCommand.Search.Value))
                {
                    query = query.Where(b =>
                    b.CustomerFirstName.Contains(gridCommand.Search.Value)).AsQueryable();
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

            var request = await query
                .Skip(gridCommand.Start)
                .Take(gridCommand.Length)
                .AsNoTracking()
                .AsSplitQuery()
                .ToListAsync();

            return new Application.DTOs.Common.PagedResult<ReturnRequest>
            {
                Items = request,
                TotalCount = totalCount,
                PageNumber = gridCommand.Start / gridCommand.Length,
                PageSize = gridCommand.Length
            };
        }

        public async Task UpdateAsync(ReturnRequest returnRequest)
        {
            returnRequest.UpdatedOnUtc = DateTime.UtcNow;
            _context.ReturnRequests.Update(returnRequest);
            await _context.SaveChangesAsync();
        }
    }
}
