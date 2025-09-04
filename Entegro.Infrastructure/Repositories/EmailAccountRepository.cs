using Entegro.Application.DTOs.Common;
using Entegro.Application.Interfaces.Repositories;
using Entegro.Domain.Entities;
using Entegro.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Entegro.Infrastructure.Repositories
{
    public class EmailAccountRepository : IEmailAccountRepository
    {
        private readonly EntegroContext _context;

        public EmailAccountRepository(EntegroContext context)
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

        public async Task<PagedResult<EmailAccount>> GetAllAsync(int pageNumber, int pageSize)
        {
            var query = _context.EmailAccounts.AsQueryable();

            var totalCount = await query.CountAsync();
            var brands = await query
                .Skip(pageNumber * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResult<EmailAccount>
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

        public async Task UpdateAsync(EmailAccount email)
        {
            _context.EmailAccounts.Update(email);
            await _context.SaveChangesAsync();
        }
    }
}
