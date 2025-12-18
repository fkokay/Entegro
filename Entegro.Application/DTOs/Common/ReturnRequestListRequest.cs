using Entegro.Application.DTOs.Order;

namespace Entegro.Application.DTOs.Common
{
    public class ReturnRequestListRequest
    {
        public GridCommand Grid { get; set; }
        public ReturnRequestListFilterDto? Filters { get; set; }
    }
}
