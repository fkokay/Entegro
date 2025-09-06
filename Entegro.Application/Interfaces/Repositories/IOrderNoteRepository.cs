using Entegro.Application.DTOs.Common;
using Entegro.Domain.Entities.Checkout;
using System.Linq.Expressions;

namespace Entegro.Application.Interfaces.Repositories
{
    public interface IOrderNoteRepository
    {
        Task<OrderNote?> GetByAsync(Expression<Func<OrderNote, bool>> predicate);
        Task<PagedResult<OrderNote>> GetAllAsync(string term, int pageNumber, int pageSize);
        Task AddAsync(OrderNote orderNote);
        Task UpdateAsync(OrderNote orderNote);
        Task DeleteAsync(OrderNote orderNote);
    }
}
