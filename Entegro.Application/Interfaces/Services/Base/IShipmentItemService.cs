using Entegro.Application.DTOs.Common;
using Entegro.Application.DTOs.ShipmentItem;
using Entegro.Domain.Entities.Checkout;

namespace Entegro.Application.Interfaces.Services.Base
{
    public interface IShipmentItemService
    {

        Task<ShipmentItemDto?> GetByIdAsync(int id);
        Task<bool> ExistsByIdAsync(int id);
        Task<bool> ExistsByShipmentIdAsync(int shipmentId);
        Task<ShipmentItemDto?> GetByShipmentIdAsync(int shipmentId);
        Task<List<ShipmentItemDto>> GetAllAsync();
        Task<PagedResult<ShipmentItemDto>> GetAllAsync(int pageNumber, int pageSize);
        Task<PagedResult<ShipmentItem>> GetPagedAsync(GridCommand gridCommand);
        Task<ShipmentItemDto> AddAsync(CreateShipmentItemDto shipmentItem);
        Task<ShipmentItemDto> UpdateAsync(UpdateShipmentItemDto shipmentItem);
        Task DeleteAsync(int id);
    }
}
