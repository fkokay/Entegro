using Entegro.ERP.Abstractions.DTOs;
using Entegro.ERP.Abstractions.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entegro.ERP.Opak.Repositories
{
    public class OpakProductReader : IErpProductReader
    {
        private readonly string _connectionString;

        public OpakProductReader(string connectionString)
        {
            _connectionString = connectionString;
        }
        public Task<ErpResponse<ProductDto>> GetProductsAsync(int page, int pageSize)
        {
            throw new NotImplementedException();
        }
    }
}
