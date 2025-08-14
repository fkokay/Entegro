using Entegro.ERP.Abstractions.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entegro.ERP.Abstractions.Interfaces
{
    public interface IErpCustomerReader
    {
        Task<ErpResponse<CustomerDto>> GetCustomersAsync(int page, int pageSize);
    }
}
