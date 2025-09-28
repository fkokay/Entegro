using Entegro.ERP.Abstractions.Interfaces;
using Entegro.ERP.Logo.Repositories;
using Entegro.ERP.Netsis.Repositories;
using Entegro.ERP.Opak.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entegro.ERP.Application.Factories
{
    public class ErpProductStockReaderFactory : IErpProductStockReaderFactory
    {
        public IErpProductStockReader Create(string erpType, string connectionString)
        {
            return erpType.ToLower() switch
            {
                "Logo" => new LogoProductStockReader(connectionString),
                "Netsis" => new NetsisProductStockReader(connectionString),
                "Opak" => new OpakProductStockReader(connectionString),
                _ => throw new ArgumentException($"ERP type not supported: {erpType}")
            };
        }
    }
}
