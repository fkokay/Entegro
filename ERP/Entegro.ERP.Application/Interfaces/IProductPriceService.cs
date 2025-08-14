using Entegro.ERP.Abstractions.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entegro.ERP.Application.Interfaces
{
    public interface IProductPriceService
    {
        Task<ErpResponse<ProductPriceDto>> GetProductPricesAsync(int page, int pageSize);
    }
}
