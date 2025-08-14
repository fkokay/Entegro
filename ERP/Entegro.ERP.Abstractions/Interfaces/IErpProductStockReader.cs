using Entegro.ERP.Abstractions.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entegro.ERP.Abstractions.Interfaces
{
    public interface IErpProductStockReader
    {
        Task<ErpResponse<ProductStockDto>> GetProductStocksAsync(int page, int pageSize);
    }
}
