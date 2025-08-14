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
    public class CustomerService : ICustomerService
    {
        private readonly IErpCustomerReader _erpCustomerReader;

        public CustomerService(IErpCustomerReader erpCustomerReader)
        {
            _erpCustomerReader = erpCustomerReader;
        }

        public async Task<ErpResponse<CustomerDto>> GetCustomersAsync(int page, int pageSize)
        {
            return await _erpCustomerReader.GetCustomersAsync(page, pageSize);
        }
    }
}
