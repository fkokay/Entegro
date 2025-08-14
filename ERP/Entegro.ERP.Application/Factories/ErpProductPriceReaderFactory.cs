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
    public class ErpProductPriceReaderFactory : IErpProductPriceReaderFactory
    {
        public IErpProductPriceReader Create(string erpType, string connectionString)
        {
            return erpType.ToLower() switch
            {
                "logo" => new LogoProductPriceReader(connectionString),
                "netsis" => new NetsisProductPriceReader(connectionString),
                "opak" => new OpakProductPriceReader(connectionString),
                _ => throw new ArgumentException($"ERP type not supported: {erpType}")
            };
        }
    }
}
