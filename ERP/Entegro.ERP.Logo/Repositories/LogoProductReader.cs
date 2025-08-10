using Entegro.ERP.Abstractions.DTOs;
using Entegro.ERP.Abstractions.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entegro.ERP.Logo.Repositories
{
    public class LogoProductReader : IErpProductReader
    {
        private readonly string _connectionString;

        public LogoProductReader(string connectionString)
        {
            _connectionString = connectionString;
        }

        public Task<IEnumerable<ProductDto>> GetProductsAsync(int page, int pageSize)
        {
            throw new NotImplementedException();
        }
    }
}
