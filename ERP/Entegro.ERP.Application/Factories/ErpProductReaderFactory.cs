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
    public class ErpProductReaderFactory : IErpProductReaderFactory
    {
        public IErpProductReader Create(string erpType, string connectionString)
        {
            return erpType.ToLower() switch
            {
                "logo" => new LogoProductReader(connectionString),
                "netsis" => new NetsisProductReader(connectionString),
                "opak" => new OpakProductReader(connectionString),
                _ => throw new ArgumentException($"ERP tipi desteklenmiyor: {erpType}")
            };
        }
    }
}
