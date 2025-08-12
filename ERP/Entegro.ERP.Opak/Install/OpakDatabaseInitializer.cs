using Entegro.ERP.Abstractions.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entegro.ERP.Opak.Install
{
    public class OpakDatabaseInitializer : IErpDatabaseInitializer
    {
        private readonly string _connectionString;
        public OpakDatabaseInitializer(string connectionString)
        {
            _connectionString = connectionString;
        }
        public Task EnsureViewsAsync()
        {
            throw new NotImplementedException();
        }
    }
}
