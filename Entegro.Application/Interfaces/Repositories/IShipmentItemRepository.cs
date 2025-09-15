using Entegro.Application.DTOs.Common;
using Entegro.Domain.Entities.Checkout;

namespace Entegro.Application.Interfaces.Repositories
{
    public interface IShipmentItemRepository
    {
        Task<ShipmentItem?> GetByIdAsync(int id);
        Task<bool> ExistsByIdAsync(int id);
        Task<ShipmentItem?> GetByShipmentIdAsync(int shipmentId);
        Task<bool> ExistsByShipmentIdAsync(int shipmentId);
        Task<List<ShipmentItem>> GetAllAsync();
        Task<PagedResult<ShipmentItem>> GetAllAsync(int pageNumber, int pageSize);
        Task<PagedResult<ShipmentItem>> GetPagedAsync(GridCommand gridCommand);
        Task AddAsync(ShipmentItem shipmentItem);
        Task UpdateAsync(ShipmentItem shipmentItem);
        Task DeleteAsync(ShipmentItem shipmentItem);
    }
}
