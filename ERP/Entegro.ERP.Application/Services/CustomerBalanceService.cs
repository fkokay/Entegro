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
    public class CustomerBalanceService : ICustomerBalanceService
    {
        private readonly IErpCustomerBalanceReader _erpCustomerBalanceReader;

        public CustomerBalanceService(IErpCustomerBalanceReader erpCustomerBalanceReader)
        {
            _erpCustomerBalanceReader = erpCustomerBalanceReader;
        }

        public async Task<ErpResponse<CustomerBalanceDto>> GetCustomerBalancesAsync(int page, int pageSize)
        {
            return await _erpCustomerBalanceReader.GetCustomerBalancesAsync(page, pageSize);
        }
    }
}
