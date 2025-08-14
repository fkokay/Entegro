using Entegro.ERP.Abstractions.DTOs;
using Entegro.ERP.Abstractions.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entegro.ERP.Logo.Repositories
{
    public class LogoCustomerBalanceReader : IErpCustomerBalanceReader
    {
        private readonly string _connectionString;

        public LogoCustomerBalanceReader(string connectionString)
        {
            _connectionString = connectionString;
        }

        public Task<ErpResponse<CustomerBalanceDto>> GetCustomerBalancesAsync(int page, int pageSize)
        {
            throw new NotImplementedException();
        }
    }
}
