using Entegro.Application.DTOs.Order;

namespace Entegro.Application.DTOs.Common
{
    public class OrderListRequest
    {
        public GridCommand Grid { get; set; }
        public OrderListFilterDto? Filters { get; set; }
    }
}
