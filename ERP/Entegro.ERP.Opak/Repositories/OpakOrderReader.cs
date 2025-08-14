using Entegro.ERP.Abstractions.DTOs;
using Entegro.ERP.Abstractions.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entegro.ERP.Opak.Repositories
{
    public class OpakOrderReader : IErpOrderReader
    {
        private readonly string _connectionString;
        public OpakOrderReader(string connectionString)
        {
            _connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
        }

        public Task<ErpResponse<OrderDto>> GetOrdersAsync(int page, int pageSize)
        {
            throw new NotImplementedException();
        }
    }
}
