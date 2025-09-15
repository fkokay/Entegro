using Entegro.Application.DTOs.Common;
using Entegro.Domain.Entities.Checkout;

namespace Entegro.Application.Interfaces.Repositories
{
    public interface IShipmentRepository
    {
        Task<bool> ExistsByIdAsync(int id);
        Task<bool> ExistsByOrderIdAsync(int orderId);
        Task<bool> ExistsByTrackingNumberAsync(string trackingNumber);
        Task<Shipment?> GetByIdAsync(int id);
        Task<Shipment?> GetByOrderIdAsync(int orderId);
        Task<Shipment?> GetByTrackingNumberAsync(string trackingNumber);
        Task<List<Shipment>> GetAllAsync();
        Task<PagedResult<Shipment>> GetAllAsync(int pageNumber, int pageSize);
        Task<PagedResult<Shipment>> GetPagedAsync(GridCommand gridCommand);
        Task AddAsync(Shipment shipment);
        Task UpdateAsync(Shipment shipment);
        Task UpdateByShippedDateAsync(int id);
        Task UpdateByDeliveryDateAsync(int id);
        Task DeleteAsync(Shipment shipment);
    }
}
