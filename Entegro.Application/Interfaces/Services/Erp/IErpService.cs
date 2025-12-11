using Entegro.Application.DTOs.Erp;

namespace Entegro.Application.Interfaces.Services.Erp
{
    public interface IErpService : IErpProductReader
    {
        Task<IEnumerable<ErpOrderDto>> GetOrdersAsync(ErpApiContext context, int pageSize = 50);
    }
}
