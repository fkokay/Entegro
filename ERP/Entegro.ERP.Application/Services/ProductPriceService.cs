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
    public class ProductPriceService : IProductPriceService
    {
        private readonly IErpProductPriceReader _erpProductPriceReader;

        public ProductPriceService(IErpProductPriceReader erpProductPriceReader)
        {
            _erpProductPriceReader = erpProductPriceReader;
        }
        public async Task<ErpResponse<ProductPriceDto>> GetProductPricesAsync(int page, int pageSize)
        {
           return await _erpProductPriceReader.GetProductPricesAsync(page, pageSize);
        }
    }
}
