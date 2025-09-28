using Entegro.ERP.Abstractions.DTOs;
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
    public class ErpCustomerReaderFactory : IErpCustomerReaderFactory
    {
        public IErpCustomerReader Create(string erpType, string connectionString)
        {
            return erpType.ToLower() switch
            {
                "Logo" => new LogoCustomerReader(connectionString),
                "Netsis" => new NetsisCustomerReader(connectionString),
                "Opak" => new OpakCustomerReader(connectionString),
                _ => throw new ArgumentException($"ERP type not supported: {erpType}")
            };
        }
    }
}
