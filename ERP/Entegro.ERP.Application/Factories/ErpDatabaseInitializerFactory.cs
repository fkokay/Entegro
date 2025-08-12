using Entegro.ERP.Abstractions.Interfaces;
using Entegro.ERP.Logo.Install;
using Entegro.ERP.Netsis.Install;
using Entegro.ERP.Opak.Install;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entegro.ERP.Application.Factories
{
    public class ErpDatabaseInitializerFactory : IErpDatabaseInitializerFactory
    {

        public IErpDatabaseInitializer CreateDatabaseInitializer(string erpType, string connectionString)
        {
            return erpType.ToLower() switch
            {
                "logo" => new LogoDatabaseInitializer(connectionString),
                "netsis" => new NetsisDatabaseInitializer(connectionString),
                "opak" => new OpakDatabaseInitializer(connectionString),
                _ => throw new ArgumentException($"ERP tipi desteklenmiyor: {erpType}")
            };
        }
    }
}
