using Entegro.Application.DTOs.Common;
using Entegro.Application.Interfaces.Repositories;
using Entegro.Domain.Entities.Checkout;
using Entegro.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Entegro.Infrastructure.Repositories
{
    public class OrderNoteRepository : IOrderNoteRepository
    {
        private readonly EntegroContext _context;

        public OrderNoteRepository(EntegroContext context)
        {
            _context = context;
        }

        public async Task AddAsync(OrderNote orderNote)
        {
            orderNote.CreatedOnUtc = DateTime.UtcNow;
            await _context.OrderNote.AddAsync(orderNote);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(OrderNote orderNote)
        {
            _context.OrderNote.Remove(orderNote);
            await _context.SaveChangesAsync();
        }


        public async Task<PagedResult<OrderNote>> GetAllAsync(string term, int pageNumber, int pageSize)
        {
            var query = _context.OrderNote
               .OrderBy(b => b.Id)
               .AsNoTracking();

            var totalCount = await query.CountAsync();
            var items = await query
                .Skip(pageNumber * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResult<OrderNote>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }

        public async Task<OrderNote?> GetByAsync(Expression<Func<OrderNote, bool>> predicate)
        {
            return await _context.OrderNote
             .FirstOrDefaultAsync(predicate);
        }

        public async Task UpdateAsync(OrderNote orderNote)
        {
            _context.OrderNote.Update(orderNote);
            await _context.SaveChangesAsync();
        }
    }
}
