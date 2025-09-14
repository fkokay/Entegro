using Entegro.Application.DTOs.Erp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entegro.Application.Interfaces.Services.Erp
{
    public interface IErpProductReader
    {
        Task<List<ErpProductDto>> GetProductsAsync(string erpType, int pageSize = 50);
    }
}
