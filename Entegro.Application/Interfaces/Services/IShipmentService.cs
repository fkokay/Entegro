using Entegro.Application.DTOs.Common;
using Entegro.Application.DTOs.Shipment;

namespace Entegro.Application.Interfaces.Services
{
    public interface IShipmentService
    {
        Task<bool> ExistsByIdAsync(int id);
        Task<bool> ExistsByOrderIdAsync(int orderId);
        Task<bool> ExistsByTrackingNumberAsync(string trackingNumber);
        Task<ShipmentDto?> GetByIdAsync(int id);
        Task<ShipmentDto?> GetByOrderIdAsync(int orderId);
        Task<ShipmentDto?> GetByTrackingNumberAsync(string trackingNumber);
        Task<List<ShipmentDto>> GetAllAsync();
        Task<PagedResult<ShipmentDto>> GetAllAsync(int pageNumber, int pageSize);
        Task<PagedResult<ShipmentDto>> GetPagedAsync(GridCommand gridCommand);
        Task<ShipmentDto> AddAsync(CreateShipmentDto shipment);
        Task<ShipmentDto> UpdateAsync(UpdateShipmentDto shipment);
        Task<ShipmentDto> UpdateByShippedDateAsync(int id);
        Task<ShipmentDto> UpdateByDeliveryDateAsync(int id);
        Task DeleteAsync(int id);
    }
}
