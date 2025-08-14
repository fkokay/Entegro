using Entegro.ERP.Abstractions.DTOs;
using Entegro.ERP.Abstractions.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entegro.ERP.Netsis.Repositories
{
    public class NetsisCustomerReader : IErpCustomerReader
    {
        private readonly string _connectionString;
        public NetsisCustomerReader(string connectionString)
        {
            _connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
        }

        public Task<ErpResponse<CustomerDto>> GetCustomersAsync(int page, int pageSize)
        {
            throw new NotImplementedException();
        }
    }
}
