using Entegro.Application.DTOs.Common;
using Entegro.Application.Interfaces.Repositories;
using Entegro.Domain.Entities.Platform.Messaging;
using Entegro.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq.Dynamic.Core;
namespace Entegro.Infrastructure.Repositories
{
    public class EmailAccountRepository : IEmailAccountRepository
    {
        private readonly EntegroDbContext _context;

        public EmailAccountRepository(EntegroDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(EmailAccount email)
        {
            await _context.EmailAccounts.AddAsync(email);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(EmailAccount email)
        {
            var model = await _context.EmailAccounts.FindAsync(email.Id);
            if (model != null)
            {
                _context.EmailAccounts.Remove(model);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<List<EmailAccount>> GetAllAsync()
        {
            return await _context.EmailAccounts.ToListAsync();
        }

        public async Task<Application.DTOs.Common.PagedResult<EmailAccount>> GetAllAsync(int pageNumber, int pageSize)
        {
            var query = _context.EmailAccounts.AsQueryable();

            var totalCount = await query.CountAsync();
            var brands = await query
                .Skip(pageNumber * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new Application.DTOs.Common.PagedResult<EmailAccount>
            {
                Items = brands,
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }

        public async Task<EmailAccount?> GetByIdAsync(int id)
        {
            return await _context.EmailAccounts.FirstOrDefaultAsync(o => o.Id == id);
        }

        public async Task<Application.DTOs.Common.PagedResult<EmailAccount>> GetPagedAsync(GridCommand gridCommand)
        {
            var query = _context.EmailAccounts.AsNoTracking();

            if (gridCommand.Search != null)
            {
                if (!string.IsNullOrEmpty(gridCommand.Search.Value))
                {
                    query = query.Where(b => b.Email.Contains(gridCommand.Search.Value)).AsQueryable();
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
            var accounts = await query
            .Skip(gridCommand.Start)
            .Take(gridCommand.Length)
            .ToListAsync();

            return new Application.DTOs.Common.PagedResult<EmailAccount>
            {
                Items = accounts,
                TotalCount = totalCount,
                PageNumber = gridCommand.Start + 1,
                PageSize = gridCommand.Length
            };
        }

        public async Task UpdateAsync(EmailAccount email)
        {
            _context.EmailAccounts.Update(email);
            await _context.SaveChangesAsync();
        }
    }
}
