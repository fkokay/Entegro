using Entegro.ERP.Abstractions.DTOs;
using Entegro.ERP.Abstractions.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entegro.ERP.Netsis.Repositories
{
    public class NetsisProductPriceReader : IErpProductPriceReader
    {
        private readonly string _connectionString;
        public NetsisProductPriceReader(string connectionString)
        {
            _connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
        }

        public Task<ErpResponse<ProductPriceDto>> GetProductPricesAsync(int page, int pageSize)
        {
            throw new NotImplementedException();
        }
    }
}
