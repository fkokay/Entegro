using Entegro.Application.DTOs.Common;
using Entegro.Application.DTOs.OrderItem;
using Entegro.Application.DTOs.Product;

namespace Entegro.Application.Interfaces.Services.Base
{
    public interface IOrderItemService
    {
        Task<List<OrderItemDto>> GetByOrderIdAsync(int orderId);
        Task<OrderItemDto?> GetByIdAsync(int id);
        Task<List<OrderItemDto>> GetAllWithIntegrationSkuAsync(string integrationSku);
        Task<List<OrderItemDto>> GetAllAsync();
        Task<PagedResult<OrderItemDto>> GetPagedAsync(int pageNumber, int pageSize);
        Task<OrderItemDto> AddAsync(CreateOrderItemDto orderItem);
        Task<OrderItemDto> UpdateAsync(UpdateOrderItemDto orderItem);
        Task DeleteAsync(int id);
        Task<List<ProductSalesDto>> GetProductSalesByMarketplaceAsync(int groupByType);
    }
}
