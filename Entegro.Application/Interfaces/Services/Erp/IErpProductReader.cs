using Entegro.Application.DTOs.Erp;

namespace Entegro.Application.Interfaces.Services.Erp
{
    public interface IErpProductReader
    {
        Task<List<ErpProductDto>> GetProductsAsync(ErpApiContext context, int pageSize = 50);
    }
}
