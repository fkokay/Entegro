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
    public class ErpOrderReaderFactory : IErpOrderReaderFactory
    {
        public IErpOrderReader Create(string erpType, string connectionString)
        {
            return erpType.ToLower() switch
            {
                "logo" => new LogoOrderReader(connectionString),
                "netsis" => new NetsisOrderReader(connectionString),
                "opak" => new OpakOrderReader(connectionString),
                _ => throw new ArgumentException($"ERP tipi desteklenmiyor: {erpType}")
            };
        }
    }
}
