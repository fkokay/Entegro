using Entegro.Application.DTOs.Erp;
using Entegro.Application.DTOs.Marketplace.Trendyol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entegro.Application.Interfaces.Services.Erp
{
    public interface IErpService
    {
        Task<List<ErpProductDto>> GetProductsAsync(string erpType,int pageSize = 50);
    }
}
