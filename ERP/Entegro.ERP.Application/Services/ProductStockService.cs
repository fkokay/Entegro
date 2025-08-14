using Entegro.ERP.Abstractions.DTOs;
using Entegro.ERP.Abstractions.Interfaces;
using Entegro.ERP.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entegro.ERP.Application.Services
{
    public class ProductStockService : IProductStockService
    {
        private readonly IErpProductStockReader _erpProductStockReader;

        public ProductStockService(IErpProductStockReader erpProductStockReader)
        {
            _erpProductStockReader = erpProductStockReader;
        }
        public async Task<ErpResponse<ProductStockDto>> GetProductStocksAsync(int page, int pageSize)
        {
            return await _erpProductStockReader.GetProductStocksAsync(page, pageSize);
        }
    }
}
